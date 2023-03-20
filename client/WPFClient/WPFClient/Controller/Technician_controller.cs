using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFClient.Model;
using WPFClient.Utilities;
using WPFClient.View;
using static WPFClient.Controller.Login_controller;

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        public ObservableCollection<ProductListGridRow> gridRows { get; set; }

        public async Task GetProductList(Technician_ListItems_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                var sortedItems = items.OrderBy(x => x.ItemType).ToList();
                gridRows = new ObservableCollection<ProductListGridRow>();
                foreach (var item in sortedItems)
                {
                    gridRows.Add(new ProductListGridRow()
                    {
                        Id = item.Id,
                        Name = item.ItemType,
                        Price = item.ItemPrice,
                        Availibility = 0,
                        IsSelected = false,
                        Count = 0
                    });
                }
                // Get availibility
                var responseStock = await RestHelper.GetRestClient().GetAsync("api/Stock");
                if (!responseStock.IsSuccessStatusCode)
                {
                    return;
                }
                var contentStock = await responseStock.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(contentStock);
                foreach (var stock in stocks)
                {
                    var item = gridRows.Where(i => i.Id == stock.StockItemId).FirstOrDefault();
                    if (item != null)
                    {
                        item.Availibility += stock.AvailablePieces;
                        item.Availibility -= stock.ReservedPieces;
                        if (item.Availibility < 0)
                        {
                            item.Availibility = 0;
                        }
                    }
                }
                view.grid.DataContext = gridRows;
            }
        }

        public async Task LoadAssignItemsData(Technician_AssignItems_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                projects = projects.FindAll(i => i.UserId == userid);
                view.projectsComboBox.ItemsSource = projects.Select(i => i.Id);
                view.projectsComboBox.SelectedIndex = 0;
            }
            view.Products = await GetProductCollection();
            view.productComboBox.ItemsSource = view.Products;
        }

        public async Task AssignItems(Technician_AssignItems_view view)
        {
            // Tasks:
            // 1. make new StockAccount or update it
            // 2. increase the reserved in the Stock table
            // 3. ?set the project status? draft

            var product = (StockItem_model)view.productComboBox.SelectedItem;
            int currentProjectId = (int)view.projectsComboBox.SelectedItem;
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockAccount");
                var contentStockAccount = await response.Content.ReadAsStringAsync();
                // Filter out any StockAccounts with different projectId and type
                var stockAccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(contentStockAccount);
                stockAccounts = stockAccounts.FindAll(i => i.ProjectId == currentProjectId);
                stockAccounts = stockAccounts.FindAll(i => i.Type == StockAccountType.Reservation);

                // Get Stock
                response = await client.GetAsync("api/Stock");
                var contentStock = await response.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(contentStock);
                // Step 1: Update or create new StockAccount for a product
                var stockAccount = stockAccounts.Where(i => i.StockItemId == product.Id).FirstOrDefault();
                // We already have this product reserved, update the count and date
                if (stockAccount != null)
                {
                    var modifiedStockAccount = new
                    {
                        Id = stockAccount.Id,
                        StockAccountType = "Reservation",
                        Pieces = stockAccount.Pieces + Convert.ToInt32(view.quantityTextBox.Text),
                        AccountTime = DateTime.Now,
                        ProjectId = stockAccount.ProjectId,
                        StockItemId = stockAccount.StockItemId,
                        UserId = userid
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(modifiedStockAccount), Encoding.UTF8, "application/json");
                    response = await client.PutAsync("api/StockAccount?id=" + stockAccount.Id, content);
                }
                // Create new StockAccount
                else
                {
                    var newStockAccount = new
                    {
                        StockAccountType = "Reservation",
                        Pieces = Convert.ToInt32(view.quantityTextBox.Text),
                        AccountTime = DateTime.Now,
                        ProjectId = currentProjectId,
                        StockItemId = product.Id,
                        UserId = userid
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(newStockAccount), Encoding.UTF8, "application/json");
                    response = await client.PostAsync("api/StockAccount", content);
                }
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to update StockAccount table");
                }
                // Step 2: increase reserved pieces in Stock table
                // Iterate through all Stocks containing the current product
                var productstock = stocks.Where(i => i.StockItemId == product.Id);
                int count = Convert.ToInt32(view.quantityTextBox.Text);
                if (productstock.Count() > 0)
                {
                    foreach (Stock_model stock in productstock)
                    {
                        // If we need more pieces than available in the stock
                        if (count > (stock.AvailablePieces - stock.ReservedPieces))
                        {
                            count -= (stock.AvailablePieces - stock.ReservedPieces);
                            stock.ReservedPieces = stock.AvailablePieces;
                            UpdateStock(stock);
                        }
                        else
                        // If we have enough pieces in a stock
                        {
                            stock.ReservedPieces += count;
                            count = 0;
                            UpdateStock(stock);
                            break;
                        }
                    }
                    // If we reserved all available pieces and we still need more
                    // add the remaining count to the reservedPieces value of the first Stock in the list
                    if (count > 0)
                    {
                        productstock.First().ReservedPieces += count;
                        UpdateStock(productstock.First());
                    }
                }
                else
                {
                    MessageBox.Show("StockItem not found in Stock table");
                }

                // Step 4: update project status
                string projectstatus = "Draft";
                var responseProject = await client.GetAsync("api/Project/" + currentProjectId);
                var contentProject = await responseProject.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<Project_model>(contentProject);
                ProjectStatus projectStatusEnum;
                Enum.TryParse<ProjectStatus>(projectstatus, true, out projectStatusEnum);

                // Make changes to project status only if we actually need to change it
                if (project.Type != projectStatusEnum)
                {
                    var projectAccount = new
                    {
                        projectAccounType = projectstatus,
                        createdDate = DateTime.Now,
                        projectId = currentProjectId
                    };
                    var contentProjectAccount = new StringContent(JsonConvert.SerializeObject(projectAccount), Encoding.UTF8, "application/json");
                    var responseProjectAccount = await client.PostAsync("api/ProjectAccount", contentProjectAccount);
                    if (!responseProjectAccount.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error creating new ProjectAccount");
                    }

                    var updatedProject = new
                    {
                        id = project.Id,
                        projectType = projectstatus,
                        projectDescription = project.ProjectDescription,
                        place = project.Place,
                        ordererId = project.OrdererId,
                        userid = project.UserId,
                    };
                    var requestProject = new StringContent(JsonConvert.SerializeObject(updatedProject), Encoding.UTF8, "application/json");
                    var responseProjectUpdate = await client.PutAsync("api/Project?id=" + currentProjectId, requestProject);
                    if (!responseProjectUpdate.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error while updating Project");
                    }
                }
                MessageBox.Show("All items have been assigned to the project");
            }

        }

        private async void UpdateStock(Stock_model stock)
        {
            var putStockRecord = new
            {
                Id = stock.Id,
                RowId = stock.RowId,
                ColumnID = stock.ColumnId,
                BoxId = stock.BoxId,
                MaxPieces = stock.MaxPieces,
                AvailablePieces = stock.AvailablePieces,
                ReservedPieces = stock.ReservedPieces,
                StockItemId = stock.StockItemId
            };
            using (var client = RestHelper.GetRestClient())
            {
                var json = JsonConvert.SerializeObject(putStockRecord);
                var content = new StringContent(JsonConvert.SerializeObject(putStockRecord), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/Stock?id=" + stock.Id, content);
            }
        }

        internal async Task<ObservableCollection<StockItem_model>> GetProductCollection()
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (!response.IsSuccessStatusCode)
                {
                    return new ObservableCollection<StockItem_model>();
                }
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                var sortedItems = items.OrderBy(x => x.ItemType).ToList();
                return new ObservableCollection<StockItem_model>(sortedItems);
            }
        }
    }
}
