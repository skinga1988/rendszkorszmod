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
                foreach ( var item in sortedItems )
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
                    }
                }
                view.grid.DataContext = gridRows;
            }
        }

        public async Task LoadAssignItemsData(Technician_AssignItems_view view, ObservableCollection<ProductListGridRow> allItems)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if(!response.IsSuccessStatusCode)
                {
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                // TODO filter projects by userID
                view.projectsComboBox.ItemsSource = projects.Select(i => i.Id);
                view.projectsComboBox.SelectedIndex = 0;

                view.datagrid.DataContext = allItems;
            }
        }

        public async Task AssignItems(Technician_AssignItems_view view)
        {
            // Tasks:
            // 1. for every selected item check if the product is available in the desired quantity
            // 2. for every product make new StockAccounts or update them
            // 3. for every product increase the reserved in the Stock table
            // 4. ?set the project status? wait or draft
            ObservableCollection<ProductListGridRow> productToReserve = (ObservableCollection<ProductListGridRow>)view.datagrid.Items.SourceCollection;
            int currentProjectId = (int)view.projectsComboBox.SelectedItem;
            bool allItemsInStock = true;
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockAccount");
                var contentStockAccount = await response.Content.ReadAsStringAsync();
                // Filter out any StockAccounts with different projectId and type
                var stockAccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(contentStockAccount);
                stockAccounts = stockAccounts.FindAll(i => i.ProjectId == currentProjectId);
                stockAccounts = stockAccounts.FindAll(i => i.Type == StockAccountType.Reservation);
                response = await client.GetAsync("api/Stock");
                var contentStock = await response.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(contentStock);
                foreach (var product in productToReserve)
                {
                    // Step 1: check if product is available in the desired quantity
                    if (product.Count > product.Availibility)
                    {
                        allItemsInStock = false;
                    }
                    // Step 2: Update or create new StockAccount for a product
                    var stockAccount = stockAccounts.Where(i => i.StockItemId == product.Id).FirstOrDefault();
                    // We already have this product reserved, update the count and date
                    if (stockAccount != null)
                    {
                        var modifiedStockAccount = new
                        {
                            Id = stockAccount.Id,
                            StockAccountType = "Reservation",
                            Pieces = stockAccount.Pieces+product.Count,
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
                            Pieces = product.Count,
                            AccountTime = DateTime.Now,
                            ProjectId = currentProjectId,
                            StockItemId = product.Id,
                            UserId = userid
                        };
                        var content = new StringContent(JsonConvert.SerializeObject(newStockAccount), Encoding.UTF8, "application/json");
                        response = await client.PostAsync("api/StockAccount", content);
                    }
                    if(!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to update StockAccount table");
                    }
                    // Step 3: increase reserved pieces in Stock table
                    // Iterate through all Stocks containing the current product
                    var productstock = stocks.Where(i => i.StockItemId == product.Id);
                    int count = product.Count;
                    if (productstock.Count() > 0)
                    {
                        foreach(Stock_model stock in productstock)
                        {
                            // If we need more pieces than available in the stock
                            if (count > (stock.AvailablePieces-stock.ReservedPieces))
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
                        if(count > 0)
                        {
                            productstock.First().ReservedPieces += count;
                            UpdateStock(productstock.First());
                        }
                    } else
                    {
                        MessageBox.Show("StockItem not found in Stock table");
                    }
                }
                // Step 4: update project status
                StringContent contentProjectAccount;
                if (allItemsInStock)
                {
                    var projectAccount = new
                    {
                        projectAccounType = "Wait",
                        createdDate = DateTime.Now,
                        projectId = currentProjectId
                    };
                    contentProjectAccount = new StringContent(JsonConvert.SerializeObject(projectAccount), Encoding.UTF8, "application/json");
                }
                else
                {
                    var projectAccount = new
                    {
                        projectAccounType = "Draft",
                        createdDate = DateTime.Now,
                        projectId = currentProjectId
                    };
                    contentProjectAccount = new StringContent(JsonConvert.SerializeObject(projectAccount), Encoding.UTF8, "application/json");
                }
                var reponse = await client.PostAsync("api/ProjectAccount", contentProjectAccount);
                if (reponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("All items have been assigned to the project");
                }
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
                var response = await client.PutAsync("api/Stock?id="+stock.Id, content);
            }
        }
    }
}
