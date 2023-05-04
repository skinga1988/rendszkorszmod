using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Model;
using WPFClient.Utilities;
using WPFClient.View;
using static WPFClient.Controller.Login_controller;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WPFClient.Controller
{
    internal class Storekeeper_controller
    {
        enum Direction { Up, Down }
        public ObservableCollection<ItemListGridRow> gridItems { get; set; }

        public ObservableCollection<ProjectListGridRow> gridRows2 { get; set; }

        public async Task GetProjectList(Storekeeper_listprojects_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                    projects = projects.FindAll(i => i.ProjectType == "Scheduled");
                    gridRows2 = new ObservableCollection<ProjectListGridRow>();
                    foreach (var project in projects)
                    {
                        gridRows2.Add(new ProjectListGridRow()
                        {
                            Id = project.Id,
                            ProjectType = project.ProjectType,
                            ProjectDescription = project.ProjectDescription,
                            Place = project.Place,
                            OrdererId = project.OrdererId,
                            UserId = project.UserId
                        });
                    }
                }
                view.ProjectsDataGrid.DataContext = gridRows2;
            }
        }

        public async Task ListBoxLoad_listprojects(Storekeeper_listprojects_view obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                    var sortedProjects = projects.OrderBy(x => x.Id).ToList();
                    sortedProjects = sortedProjects.FindAll(i => i.ProjectType == "Scheduled");
                    obj.ProjectID_combobox.ItemsSource = sortedProjects.Select(x => x.Id);
                }
            }
        }

        public async Task<Project_model> GetProjectById(int projectId)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project/" + projectId);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var project = JsonConvert.DeserializeObject<Project_model>(json);
                    return project;
                }
                else
                {
                    return null;
                }
            }
        }


        public async Task SetInProgressProject(Storekeeper_listprojects_view view)
        {
            //get the project description from the projectsComboBox
            var SelectedProjectId = Convert.ToInt32(view.ProjectID_combobox.SelectedItem);
            var project = await GetProjectById(SelectedProjectId);
            if (project.ProjectType == "InProgress")
            {
                MessageBox.Show("This project is already in InProgress status.");
            }
            else
            {
                if (project != null)
                {
                    //1: updating the ProjectType in the Projects table
                    var putObject = new
                    {
                        id = project.Id,
                        projectType = "InProgress",
                        projectDescription = project.ProjectDescription,
                        place = project.Place,
                        ordererId = project.OrdererId,
                        userId = project.UserId,
                    };

                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/Project?id=" + project.Id);
                        var content = new StringContent(JsonConvert.SerializeObject(putObject), Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        if (status.ToString() == "NoContent")
                        {
                            MessageBox.Show("Project status is modified to InProgress: project id = " + project.Id + ".");
                        }
                        else
                        {
                            MessageBox.Show("Error: status update denied: " + status.ToString());
                        }
                        //2: add a new record to ProjectAccount table
                        var projectAccount = new
                        {
                            projectAccounType = "InProgress",
                            createdDate = DateTime.Now,
                            projectId = project.Id
                        };
                        var contentProjectAccount = new StringContent(JsonConvert.SerializeObject(projectAccount), Encoding.UTF8, "application/json");
                        var responseProjectAccount = await client.PostAsync("api/ProjectAccount", contentProjectAccount);
                    }
                }
            }
        }

        public async Task<List<ItemListGridRow>> GetItemList(Storekeeper_listitems_view view, int projectId)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Stock");
                if (!response.IsSuccessStatusCode)
                {
                    return new List<ItemListGridRow>();
                }
                var content = await response.Content.ReadAsStringAsync();
                var stock = JsonConvert.DeserializeObject<List<Stock_model>>(content);
                gridItems = new ObservableCollection<ItemListGridRow>();

                var responsestockitem = await client.GetAsync("api/StockItem");
                if (!responsestockitem.IsSuccessStatusCode)
                {
                    return new List<ItemListGridRow>();
                }
                var contentitem = await responsestockitem.Content.ReadAsStringAsync();
                var stockItems = JsonConvert.DeserializeObject<List<StockItem_model>>(contentitem);

                var responseStockAccount = await client.GetAsync("api/StockAccount");
                if (!responseStockAccount.IsSuccessStatusCode)
                {
                    return new List<ItemListGridRow>();
                }
                var contentStockAccount = await responseStockAccount.Content.ReadAsStringAsync();
                var stockAccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(contentStockAccount);

                var projectStockAccounts = stockAccounts.Where(x => x.ProjectId == projectId).ToList();
                
                // Collect every item assigned to the project
                var itemList = new List<ItemListGridRow>();
                foreach (var account in projectStockAccounts)
                {
                    int count = 0;
                    var stocks = stock.Where(x => x.StockItemId == account.StockItemId);
                    // Search all stocks with current item
                    foreach(var currentStock in stocks)
                    {
                        int piecesInCurrentStock;
                        // Found enough, next item in project
                        if(count == account.Pieces)
                        {
                            break;
                        }
                        // No items in this stock
                        if(currentStock.AvailablePieces == 0)
                        {
                            continue;
                        }
                        // Get the necessary number or all
                        if (currentStock.AvailablePieces <= account.Pieces)
                        {
                            count += currentStock.AvailablePieces;
                            piecesInCurrentStock = currentStock.AvailablePieces;

                        } else
                        {
                            count += account.Pieces;
                            piecesInCurrentStock = account.Pieces;
                        }
                        itemList.Add(new ItemListGridRow()
                        {
                            ItemId = account.StockItemId,
                            Name = stockItems.Where(x => x.Id == account.StockItemId).FirstOrDefault().ItemType,
                            RowId = currentStock.RowId,
                            ColumnId = currentStock.ColumnId,
                            BoxId = currentStock.BoxId,
                            Count = piecesInCurrentStock,
                            Id = currentStock.Id,
                            AccountId = account.Id
                        });
                    }
                }

                // "Optimize" path
                var rowSorted = itemList.OrderBy(x => x.RowId).ToList();
                var direction = Direction.Up;
                while (rowSorted.Any())
                {
                    int currentAlley = RowToAlley(rowSorted[0].RowId);
                    var itemsInAlley = rowSorted.Where(x => RowToAlley(x.RowId) == currentAlley).ToList();
                    if(direction == Direction.Up) {
                        itemsInAlley = itemsInAlley.OrderBy(x => x.ColumnId).ToList();
                    } else
                    {
                        itemsInAlley = itemsInAlley.OrderByDescending(x => x.ColumnId).ToList();
                    }  
                    foreach(var item in itemsInAlley)
                    {
                        rowSorted.Remove(item);
                        gridItems.Add(item);

                        switch(direction)
                        {
                            case Direction.Up: direction = Direction.Down; break;
                            case Direction.Down: direction = Direction.Up; break;
                        }
                    }
                }
                
                view.grid.DataContext = gridItems;
            }
            return gridItems.ToList();
        }

        private int RowToAlley(int row)
        {
            /* 1   -> 0 
               2;3 -> 1 
               4;5 -> 2
               6;7 -> 3
               8;9 -> 4
               10  -> 5 */
            return row/2;
        }

        public async Task RemoveItemsFromStore(List<ItemListGridRow> items, int projectId)
        {
            using (var client = RestHelper.GetRestClient())
            {
                foreach (var item in items)
                {
                    // 1. update Stock (reduce availablePieces and reservedPieces)
                    var responseStock = await client.GetAsync("api/Stock/" + item.Id);
                    if (!responseStock.IsSuccessStatusCode)
                    {
                        return;
                    }
                    var json = await responseStock.Content.ReadAsStringAsync();
                    var stock = JsonConvert.DeserializeObject<Stock_model>(json);
                    int available = stock.AvailablePieces - item.Count;
                    int reserved = stock.ReservedPieces - item.Count;
                    var putObject = new
                    {
                        id = stock.Id,
                        rowId = stock.RowId,
                        columnId = stock.ColumnId,
                        boxId = stock.BoxId,
                        availablePieces = available,
                        reservedPieces = reserved,
                        stockItemId = stock.StockItemId
                    };
                    var requestStock = new HttpRequestMessage(HttpMethod.Put, "api/Stock?id=" + stock.Id);
                    var contentStock = new StringContent(JsonConvert.SerializeObject(putObject), Encoding.UTF8, "application/json");
                    requestStock.Content = contentStock;
                    var putResponse = await client.SendAsync(requestStock);
                    var status = putResponse.StatusCode;
                    // 2. StockAccount: add records for outbound items
                    var newStockAccount = new
                    {
                        StockAccountType = "Outcome",
                        Pieces = item.Count,
                        AccountTime = DateTime.Now,
                        ProjectId = projectId,
                        StockItemId = item.ItemId,
                        UserId = userid
                    };
                    var contentStockAcc = new StringContent(JsonConvert.SerializeObject(newStockAccount), Encoding.UTF8, "application/json");
                    var pushResponse = await client.PostAsync("api/StockAccount", contentStockAcc);
                }
            }
        }
    }
}
