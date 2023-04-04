using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPFClient.Utilities;
using WPFClient.Model;
using WPFClient.View;
using static WPFClient.Controller.Login_controller;
using System.IO;
using System.Security.Principal;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        public ObservableCollection<ProductListGridRow> gridRows { get; set; }
        public ObservableCollection<ProjectListGridRow> gridRows2 { get; set; }

        // A.3
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
                        Availibility = 0
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

        internal Task Button_Click_list(Technician_list_projects technician_list_projects)
        {
            throw new NotImplementedException();
        }

        // A.4
        public async Task AssignItems(Technician_AssignItems_view view)
        {
            var product = (StockItem_model)view.productComboBox.SelectedItem;
            var currentProject= (Project_model)view.projectsComboBox.SelectedItem;
            int currentProjectId = currentProject.Id;
            int count = Convert.ToInt32(view.quantityTextBox.Text);
            int sumcount = await GetSumCountForProduct(product.Id);
            if (count > sumcount)
            {
                MessageBox.Show("Not enough avaliable pieces!");
            }
            else
            {
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
                    // If we already have this product reserved, update the count and date
                    if (stockAccount != null)
                    {
                        var modifiedStockAccount = new
                        {
                            Id = stockAccount.Id,
                            StockAccountType = "Reservation",
                            Pieces = stockAccount.Pieces + count,
                            AccountTime = DateTime.Now,
                            ProjectId = stockAccount.ProjectId,
                            StockItemId = stockAccount.StockItemId,
                            UserId = userid
                        };
                        var content = new StringContent(JsonConvert.SerializeObject(modifiedStockAccount), Encoding.UTF8, "application/json");
                        response = await client.PutAsync("api/StockAccount?id=" + stockAccount.Id, content);
                    }
                    // Else create new StockAccount
                    else
                    {
                        var newStockAccount = new
                        {
                            StockAccountType = "Reservation",
                            Pieces = count,
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
                    else
                    {
                        MessageBox.Show("New reservation to the project is done in the StockAccounts table.");
                    }

                    // Step 2: increase reserved pieces in Stock table
                    // Iterate through all Stocks containing the current product
                    var productstock = stocks.Where(i => i.StockItemId == product.Id);
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
                            MessageBox.Show("The Stock table is updated.");
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
                        MessageBox.Show("StockItem not found in Stock table.");
                    }

                    // Step 4: update project status if necessary
                    string projectstatus = "Draft";
                    var responseProject = await client.GetAsync("api/Project/" + currentProjectId);
                    var contentProject = await responseProject.Content.ReadAsStringAsync();
                    var project = JsonConvert.DeserializeObject<Project_model>(contentProject);
                    ProjectStatus projectStatusEnum;
                    Enum.TryParse<ProjectStatus>(projectstatus, true, out projectStatusEnum);

                    // Make changes to project status only if we actually need to change it
                    if (project.ProjectType == "New")
                    {
                        //create a new entry in the ProjectAccounts table with "Draft" status
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
                            MessageBox.Show("Error creating new ProjectAccount.");
                        }
                        else
                        {
                            MessageBox.Show("A new entry in the ProjectAccounts table is created with Draft status.");
                        }

                        //update the status to "Draft" in the Projects table
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
                            MessageBox.Show("Error while updating Project.");
                        }
                        else
                        {
                            MessageBox.Show("Project status is updated in the Projects table.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Project status was not updated in Projects and ProjectAccounts tables.");
                    }

                    //check if there is a prereservation based on projectId and itemId
                    var preReservations = await getPreReservationsByProjectIdAndItemId(currentProjectId, product.Id);
                    int assignedQuantity = Convert.ToInt32(view.quantityTextBox.Text);
                    if (preReservations.Count > 0)
                    {
                        //create a list for the IDs to delete from the StockAccounts table
                        List<int> idsToDelete = new List<int>();
                        foreach (var row in preReservations)
                        {
                            idsToDelete.Add(row.Id);
                        }

                        //delete all the prereserved lines for the project and item combination
                        foreach (var ID in idsToDelete)
                        {
                            var deleteResponse = await client.DeleteAsync("https://localhost:7243/api/StockAccount/" + ID);
                            string deleteStatus = deleteResponse.StatusCode.ToString();
                            if (deleteResponse.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Id " + ID + " was deleted from ProjectAccounts tables.");
                            }
                            else
                            {
                                MessageBox.Show("Delete for id " + ID + " was not successful from ProjectAccounts table: " + deleteStatus);
                            }
                        }

                        //get the total amount of prereserved items for a project+item combination
                        var preReservationsGrouped = preReservations
                            .Where(i => i.ProjectId == currentProjectId && i.Type == StockAccountType.PreReservation && i.StockItemId == product.Id)
                            .GroupBy(i => i.StockItemId)
                            .Select(g => new StockAccount_model
                            {
                                ProjectId = currentProjectId,
                                StockItemId = g.Key,
                                Type = StockAccountType.PreReservation,
                                Pieces = g.Sum(i => i.Pieces)
                            })
                            .ToList();
                        int totalPreReservedItemsCount = preReservationsGrouped[0].Pieces;

                        
                        //if the assigned quantity is more then the prereserved quantity
                        if (assignedQuantity < preReservationsGrouped[0].Pieces)
                        {
                            //make a prereservation with the "prereserved-assigned" quantity
                            int newQuantityToPreReserve = totalPreReservedItemsCount - assignedQuantity;
                            var new_StockAccounts_row = new
                            {
                                stockAccountType = "PreReservation",
                                pieces = newQuantityToPreReserve,
                                accountTime = DateTime.Now,
                                projectId = currentProjectId,
                                stockItemId = product.Id,
                                userId = userid
                            };

                            var json = JsonConvert.SerializeObject(new_StockAccounts_row);
                            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                            var putResponse = await client.PostAsync("https://localhost:7243/api/StockAccount", httpContent);
                            var status = putResponse.StatusCode;
                            if (putResponse.IsSuccessStatusCode)
                            {
                                MessageBox.Show("New row created in the StockAccounts table. " + newQuantityToPreReserve + " pieces are prereserved.");
                            }
                            else
                            {
                                MessageBox.Show("Error: status update denied: " + status.ToString());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("There is no prereservation for this project and item in the StockAccounts table.");
                    }

                }
            }
        }

        // make a pre-reservation for button click
        public async Task PrereserveItems_controller(Technician_prereservation_view view)
        {
            //get the data from the comboboxes (project and product)
            var stockAccounts_Pieces = view.quantityTextBox_prereservation.Text;
            var stockAccounts_ProjectId = (view.projectsComboBox_prereservation.SelectedItem as Project_model).Id;
            var stockAccounts_StockItemId = (view.productComboBox_prereservation.SelectedItem as StockItem_model).Id;

            var new_StockAccounts_row = new
            {
                stockAccountType = "PreReservation",
                pieces = stockAccounts_Pieces,
                accountTime = DateTime.Now,
                projectId = stockAccounts_ProjectId,
                stockItemId = stockAccounts_StockItemId,
                userId = userid
            };

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new_StockAccounts_row);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7243/api/StockAccount", httpContent);
                var status = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("New row created in the StockAccounts table.");
                }
                else
                {
                    MessageBox.Show("Error: status update denied: " + status.ToString());
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
                var response = await client.PutAsync("api/Stock?id=" + stock.Id, content);
            }
        }

        // Calculates available pieces for a StockItem
        internal async Task GetAvailableCount(Technician_AssignItems_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                int availibility = 0;
                var product = (StockItem_model)view.productComboBox.SelectedItem;

                var response = await client.GetAsync("api/Stock");
                var content = await response.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(content);
                stocks = stocks.FindAll(i => i.StockItemId == product.Id);
                foreach (var stock in stocks)
                {
                    availibility += (stock.AvailablePieces - stock.ReservedPieces);
                }
                view.availableTextBox.Text = availibility >= 0 ? availibility.ToString() : "0";
            }
        }

        // Fetches StockItems for products ComboBox
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

        // Fetches items for products ComboBox for pre-reservation
        internal async Task<ObservableCollection<StockItem_model>> GetProductCollection_prereservation()
        {
            using (var client = RestHelper.GetRestClient())
            {
                //get the content of the StockItems table
                var stockItems_Response = await client.GetAsync("https://localhost:7243/api/StockItem");
                var stockItems_Content = await stockItems_Response.Content.ReadAsStringAsync();
                var stockItems = JsonConvert.DeserializeObject<List<StockItem_model>>(stockItems_Content);

                //get the content of the Stocks table
                var stock_Response = await client.GetAsync("https://localhost:7243/api/Stock");
                
                if (!stock_Response.IsSuccessStatusCode)
                {
                    return new ObservableCollection<StockItem_model>();
                }
                
                //filter those items in the Stock table where the the sum(AvailablePieces)-sum(ReservedPieces) = 0
                var stock_Content = await stock_Response.Content.ReadAsStringAsync();
                var stock_items = JsonConvert.DeserializeObject<List<Stock_model>>(stock_Content);
                var groupedStock_items = stock_items.GroupBy(x => x.StockItemId).Select(g => new
                                            {
                                                StockItemId = g.Key,
                                                AvailablePiecesSum = g.Sum(x => x.AvailablePieces),
                                                ReservedPiecesSum = g.Sum(x => x.ReservedPieces)
                                            }).ToList();
                var filteredStock_items = groupedStock_items.Where(x => x.AvailablePiecesSum - x.ReservedPiecesSum == 0).ToList();

                //extend the filteredStock_items with those items, that does not have a line in the Stock table
                var missingStockItem_Ids = stockItems.Select(x => x.Id)
                                      .Except(groupedStock_items.Select(x => x.StockItemId))
                                      .ToList();

                foreach (var missingStockItemId in missingStockItem_Ids)
                {
                    var stockItem = stockItems.FirstOrDefault(x => x.Id == missingStockItemId);
                    if (stockItem != null)
                    {
                        filteredStock_items.Add(new
                        {
                            StockItemId = stockItem.Id,
                            AvailablePiecesSum = 0,
                            ReservedPiecesSum = 0
                        });
                    }
                }



                // Replace the StockItemId with the corresponding item name
                var stockItems_WithNames = new List<StockItem_model>();
                foreach (var iterator_stock in filteredStock_items)
                {
                    var stockItem = stockItems.FirstOrDefault(x => x.Id == iterator_stock.StockItemId);
                    if (stockItem != null)
                    {
                        stockItems_WithNames.Add(new StockItem_model
                        {
                            Id = stockItem.Id,
                            ItemPrice = stockItem.ItemPrice,
                            ItemType = stockItem.ItemType,
                            MaxItem = stockItem.MaxItem
                        });
                    }
                }   

                var sortedItems = stockItems_WithNames.OrderBy(x => x.ItemType).ToList();
                return new ObservableCollection<StockItem_model>(sortedItems);
            }
        }

        // Fetches projects for ComboBox
        internal async Task<ObservableCollection<Project_model>> GetProjectCollection()
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (!response.IsSuccessStatusCode)
                {
                    return new ObservableCollection<Project_model>();
                }
                var content = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                return new ObservableCollection<Project_model>(projects.FindAll(i => i.UserId == userid && (i.ProjectType == "New" || i.ProjectType == "Draft" || i.ProjectType == "Wait")));
            }
        }

        internal async Task<ObservableCollection<Project_model>> GetProjectCollection_prereservation()
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (!response.IsSuccessStatusCode)
                {
                    return new ObservableCollection<Project_model>();
                }
                var content = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                return new ObservableCollection<Project_model>(projects.FindAll(i => i.UserId == userid && i.ProjectType == "Wait"));
            }
        }

        //create new orderer A.0
        public async Task Button_Click_Create_orderer_controller(Techinican_create_orderer obj)
        {
            string orderer_name = obj.New_orderer_textbox.Text;
            string description = obj.Description_textbox.Text;


            //both textbox are filled
            if (orderer_name != "" && description != "")
            {
                // orderer doesn't exist
                var existOrderer = await existOrdererName(orderer_name);
                if (!existOrderer)
                {
                    using (var client = RestHelper.GetRestClient())
                    {
                        var newOrderer = new
                        {
                            ordererName = orderer_name,
                            description = description,
                            userId = userid,
                            projectId = 0
                        };
                        var json = JsonConvert.SerializeObject(newOrderer);
                        var response = await client.PostAsync("api/Orderer", new StringContent(json, Encoding.UTF8, "application/json"));
                        string status = response.StatusCode.ToString();
                        if (response.IsSuccessStatusCode)
                        {
                            // Orderer created successfully
                            MessageBox.Show("Orderer created successfully!");
                            obj.Description_textbox.Text = "";
                            obj.New_orderer_textbox.Text = "";

                        }
                        else
                        {
                            MessageBox.Show("Orderer creation failed!");

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Orderer already exists!");
                }

            }
            else
            {
                MessageBox.Show("Fill all the textbox!");
            }
        }
        //create new project A.1
        public async Task Button_Click_new_Project(Technician_create_new_project obj)
        {
            string _project_type = obj.Project_type_combobox.Text;
            string _description = obj.Description_textbox.Text;
            string _place = obj.Place_textbox.Text;
            string _orderer = obj.Orderer_select.Text;
            DateTime accountDateTime = DateTime.Now;

            //if there is not an empty field
            if (_project_type != "" && _description != "" && _place != "" && _orderer != "")
            {
                //gets the orderer id
                var _ordererId = await GetOrdererId(_orderer);
                //check whether orderer already has a project connected
                var _ordererId_project = await GetOrdererProjectId(_orderer);
                if (_ordererId_project == 0)
                {
                    using (var client = RestHelper.GetRestClient())
                    {

                        var newProjectRecord = new
                        {
                            projectType = _project_type,
                            projectDescription = _description,
                            place = _place,
                            ordererId = _ordererId,
                            userId = userid
                        };
                        var json = JsonConvert.SerializeObject(newProjectRecord);
                        var response = await client.PostAsync("api/Project", new StringContent(json, Encoding.UTF8, "application/json"));
                        string status = response.StatusCode.ToString();
                        if (response.IsSuccessStatusCode)
                        {
                            // Item created successfully
                            MessageBox.Show("New Project record created successfully!");
                            obj.Project_type_combobox.SelectedValue = 0;
                            obj.Description_textbox.Text = "";
                            obj.Place_textbox.Text = "";
                            obj.Orderer_select.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Project record creation failed: " + status);
                            obj.Project_type_combobox.SelectedValue = 0;
                            obj.Description_textbox.Text = "";
                            obj.Place_textbox.Text = "";
                            obj.Orderer_select.Text = "";
                            return;
                        }
                    }
                    //get the projectid
                    var projectid = await GetProjectId(_project_type, _description, _place, _ordererId);
                    using (var client = RestHelper.GetRestClient())
                    {

                        var newProjectAccountRecord = new
                        {
                            projectAccounType = _project_type,
                            createdDate = accountDateTime,
                            projectId = projectid,
                        };
                        var json = JsonConvert.SerializeObject(newProjectAccountRecord);
                        var response = await client.PostAsync("api/ProjectAccount", new StringContent(json, Encoding.UTF8, "application/json"));
                        string status = response.StatusCode.ToString();
                        if (response.IsSuccessStatusCode)
                        {
                            // Item created successfully
                            MessageBox.Show("New ProjectAccount record created successfully!");
                            obj.Project_type_combobox.SelectedValue = 0;
                            obj.Description_textbox.Text = "";
                            obj.Place_textbox.Text = "";
                            obj.Orderer_select.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("ProjectAccount record creation failed: " + status);
                            obj.Project_type_combobox.SelectedValue = 0;
                            obj.Description_textbox.Text = "";
                            obj.Place_textbox.Text = "";
                            obj.Orderer_select.Text = "";
                            return;
                        }
                    }
                    var _ordererDescription = await GetOrdererDescription(_orderer);
                    var putOrderer = new
                    {
                        id = _ordererId,
                        ordererName = _orderer,
                        description = _ordererDescription,
                        userId = userid,
                        projectId = projectid,

                    };
                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/Orderer?id=" + _ordererId);
                        var content = new StringContent(JsonConvert.SerializeObject(putOrderer), Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        if (status.ToString() == "NoContent")
                        {
                            MessageBox.Show("Orderer project_id updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Error: project_id update denied: " + status.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Project already exist in connection with the Orderer!");
                }
            }
            else
            {
                MessageBox.Show("Fill all the boxes!");
            }
        }

        //setting the project as Scheduled
        public async Task set_as_scheduled_Click_controller(Technician_AssignItems_view view)
        {
            //get the project description from the projectsComboBox
            string selectedProjectPlace = view.projectsComboBox.Text;
            int SelectedProjectId = await GetProjectIdByPlace(selectedProjectPlace);
            var project = await GetProjectById(SelectedProjectId);

            // check if the project has a PreReservation line in the StockAccounts table
            var stockAccounts_PreReservation = new List<StockAccount_model>();
            using (var client = RestHelper.GetRestClient())
            {
                var stockAccounts_response = await client.GetAsync("api/StockAccount");
                var stockAccounts_content = await stockAccounts_response.Content.ReadAsStringAsync();
                var stockAccounts_lines = JsonConvert.DeserializeObject<List<StockAccount_model>>(stockAccounts_content);
                // Filter for Reservation type and the current project
                stockAccounts_PreReservation = stockAccounts_lines.FindAll(i => i.Type == StockAccountType.PreReservation);
                stockAccounts_PreReservation = stockAccounts_PreReservation.FindAll(i => i.ProjectId == project.Id);
                
            }
            if (stockAccounts_PreReservation.Count > 0)
            {
                MessageBox.Show("There is at least one pre-reservation, project cannot be set as Scheduled!");
            }
            else
            {
                if (project != null)
                {
                    //1: update the ProjectType in the Projects table
                    using (var client = RestHelper.GetRestClient())
                    {


                        {
                            var putObject = new
                            {
                                id = project.Id,
                                projectType = "Scheduled",
                                projectDescription = project.ProjectDescription,
                                place = project.Place,
                                ordererId = project.OrdererId,
                                userId = project.UserId,
                            };
                            var project_Request = new HttpRequestMessage(HttpMethod.Put, "api/Project?id=" + project.Id);
                            var putContent = new StringContent(JsonConvert.SerializeObject(putObject), Encoding.UTF8, "application/json");
                            project_Request.Content = putContent;
                            var response2 = await client.SendAsync(project_Request);
                            var status = response2.StatusCode;
                            if (status.ToString() == "NoContent")
                            {
                                MessageBox.Show("Project status is modified to Scheduled: project id = " + project.Id + ".");
                            }
                            else
                            {
                                MessageBox.Show("Error: status update denied: " + status.ToString());
                            }
                        }
                    }

                    //2: creating a new row in the ProjectAccounts table
                    // 2/1: create a new projectAccount object with the Scheduled status
                    var projectAccount = new ProjectAccount_model
                    {
                        Type = ProjectAccountStatus.Scheduled,
                        CreatedDate = DateTime.Now,
                        //gathering the ProjectId based on the description
                        ProjectId = project.Id

                    };
                    // 2/2: creating a json object from the projectAccount object
                    var json = JsonConvert.SerializeObject(projectAccount);
                    using (var client = RestHelper.GetRestClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("api/ProjectAccount", content);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("New row created in the ProjectAccounts table.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to create new row in the ProjectAccounts table!");
                        }
                    }

                    //3: price calculation for the Scheduled project, updating the Projects table
                    // 3/1: price calculation
                    int totalPrice = 0;
                    var stockAccounts = await GetStockAccountsByProjectId(project.Id);
                    foreach (var account in stockAccounts)
                    {
                        var stockItem = await GetStockItemById(account.StockItemId);
                        totalPrice += stockItem.ItemPrice * account.Pieces;
                    }
                    MessageBox.Show("Total price for project " + project.Id.ToString() + " is " + totalPrice + ".");

                    // 3/2: updating the Projects table
                    var projectRow = new
                    {
                        id = project.Id,
                        projectType = "Scheduled",
                        projectDescription = project.ProjectDescription + ", price: " + totalPrice.ToString() + " HUF",
                        place = project.Place,
                        ordererId = project.OrdererId,
                        userId = project.UserId,
                    };

                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/Project?id=" + project.Id);
                        var content = new StringContent(JsonConvert.SerializeObject(projectRow), Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        if (status.ToString() == "NoContent")
                        {
                            MessageBox.Show("Project description is updated with the price. Project id = " + project.Id + ".");
                        }
                        else
                        {
                            MessageBox.Show("Error: status update denied: " + status.ToString());
                        }
                    }
                }
            }
        }

        //setting a project as Wait
        public async Task set_as_wait_Click_controller(Technician_AssignItems_view view)
        {
            //get the project description from the projectsComboBox
            string selectedProjectPlace = view.projectsComboBox.Text;
            int SelectedProjectId = await GetProjectIdByPlace(selectedProjectPlace);
            var project = await GetProjectById(SelectedProjectId);
            if (project.ProjectType == "Wait")
            {
                MessageBox.Show("This project is already in Wait status.");
            }
            else
            {
                if (project != null)
                {
                    //1: updating the ProjectType in the Projects table
                    var putObject = new
                    {
                        id = project.Id,
                        projectType = "Wait",
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
                            MessageBox.Show("Project status is modified to Wait: project id = " + project.Id + ".");
                        }
                        else
                        {
                            MessageBox.Show("Error: status update denied: " + status.ToString());
                        }
                    }

                    //2: creating a new row in the ProjectAccounts table
                    //  2/1: create a new projectAccount object with the Scheduled status
                    var projectAccount = new ProjectAccount_model
                    {
                        Type = ProjectAccountStatus.Wait,
                        CreatedDate = DateTime.Now,
                        //gathering the ProjectId based on the description
                        ProjectId = project.Id

                    };
                    // 2/2: creating a json object from the projectAccount object
                    var json = JsonConvert.SerializeObject(projectAccount);
                    using (var client = RestHelper.GetRestClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("api/ProjectAccount", content);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("New row created in the ProjectAccounts table.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to create new row in the projectAccoun table!");
                        }
                    }
                }
            }
        }






        ////LISTENERS-------------------------------------------------------------------------------------------
        //loads the content of the combobox orderers for new project
        public async Task ListBoxLoaded_orderer_controller(Technician_create_new_project obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Orderer");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<Orderer_model>>(content);
                    var sortedItems = items.OrderBy(x => x.OrdererName).ToList(); // sort the Orderers by Name in ascending order
                    obj.Orderer_select.ItemsSource = sortedItems.Select(x => x.OrdererName);
                }
            }
        }
        public async Task<int> GetOrdererId(string orderer_name)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Orderer");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Orderer_model>>(responseContent);
                var selectedItemType = items.Find(item => item.OrdererName == orderer_name);
                return selectedItemType.Id;
            }
        }
        public async Task<int> GetOrdererProjectId(string orderer_name)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Orderer");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Orderer_model>>(responseContent);
                var selectedItemType = items.Find(item => item.OrdererName == orderer_name);
                return selectedItemType.ProjectId;
            }
        }
        public async Task<string> GetOrdererDescription(string orderer_name)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Orderer");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Orderer_model>>(responseContent);
                var selectedItemType = items.Find(item => item.OrdererName == orderer_name);
                return selectedItemType.Description;
            }
        }
        //whether orderer name already exist
        public async Task<bool> existOrdererName(string orderer_name)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Orderer");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Orderer_model>>(responseContent);
                var selectedItemType = items.Any(item => item.OrdererName == orderer_name);
                return selectedItemType;
            }
        }
        public async Task<string> GetProjectType(int projectId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Find(item => item.Id == projectId);
                return selectedItemType.ProjectType;
            }
        }
        public async Task<string> GetProjectDescription(int projectId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Find(item => item.Id == projectId);
                return selectedItemType.ProjectDescription;
            }
        }
        public async Task<string> GetProjectPlace(int projectId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Find(item => item.Id == projectId);
                return selectedItemType.Place;
            }
        }
        public async Task<int> GetProjectOrdererId(int projectId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Find(item => item.Id == projectId);
                return selectedItemType.OrdererId;
            }
        }
        public async Task<int> GetProjectUserId(int projectId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Find(item => item.Id == projectId);
                return selectedItemType.UserId;
            }
        }
        public async Task<int> GetProjectId(string _project_type, string _description, string _place, int _ordererId)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Any(item => item.ProjectDescription == _description
                    && item.Place == _place && item.ProjectType == _project_type && item.OrdererId == _ordererId);
                if (selectedItemType)
                {
                    var project = items.Find(item => item.ProjectDescription == _description && item.Place == _place && item.ProjectType == _project_type && item.OrdererId == _ordererId);
                    return project.Id;
                }
                else
                {
                    return 0;
                }

            }
        }

        //gathering the ProjectId based on the description
        public async Task<int> GetProjectIdByPlace(string _place)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Project");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Project_model>>(responseContent);
                var selectedItemType = items.Any(item => item.Place.ToString() == _place);
                if (selectedItemType)
                {
                    var project = items.Find(item => item.Place.ToString() == _place);
                    return project.Id;
                }
                else
                {
                    return 0;
                }

            }
        }
        //get GetSumCountForProduct

        public async Task<int> GetSumCountForProduct(int stockItemtId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7243/api/Stock");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve stock: {response.ReasonPhrase}");
                }
                var content = await response.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(content);
                var selectedStocks = stocks.FindAll(s => s.StockItemId == stockItemtId).ToList();
                var sum = 0;
                foreach (var stock in selectedStocks )
                {
                    sum += (stock.AvailablePieces - stock.ReservedPieces);
                }
                return sum;
            }
        }
        //get project by ID
        public async Task<Project_model> GetProjectById(int projectId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7243/api/Project/" + projectId);
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

        //gets a list of ProjectAccouts object that belongs to a projectId
        public async Task<List<StockAccount_model>> GetStockAccountsByProjectId(int projectId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7243/api/StockAccount");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve stock accounts: {response.ReasonPhrase}");
                }
                var content = await response.Content.ReadAsStringAsync();
                var stockAccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(content);
                return stockAccounts.Where(sa => sa.ProjectId == projectId).ToList();
            }
        }

        //gets a StockItem object by the StockItemId
        public async Task<StockItem_model> GetStockItemById(int stockItemId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7243/api/StockItem/" + stockItemId);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve stock item with Id {stockItemId}: {response.ReasonPhrase}");
                }
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<StockItem_model>(content);
            }
        }

        // Loads data into ComboBoxes in AssignItem view
        public async Task LoadAssignItemsData(Technician_AssignItems_view view)
        {
            view.Projects = await GetProjectCollection();
            var sortedProjects = view.Projects.OrderBy(p => p.Place);
            view.projectsComboBox.ItemsSource = sortedProjects;
            view.projectsComboBox.SelectedIndex = 0;

            view.Products = await GetProductCollection();
            var sortedProducts = view.Products.OrderBy(p => p.ItemType);
            view.productComboBox.ItemsSource = sortedProducts;
            view.productComboBox.SelectedIndex = 0;
        }

        // Loads data into ComboBoxes in Technician_prereservation_view
        public async Task LoadPrereservationData_controller(Technician_prereservation_view view)
        {
            view.Projects = await GetProjectCollection_prereservation();
            var sortedProjects = view.Projects.OrderBy(p => p.Place);
            view.projectsComboBox_prereservation.ItemsSource = sortedProjects;
            view.projectsComboBox_prereservation.SelectedIndex = 0;

            view.Products = await GetProductCollection_prereservation();
            var sortedProducts = view.Products.OrderBy(p => p.ItemType);
            view.productComboBox_prereservation.ItemsSource = sortedProducts;
            view.productComboBox_prereservation.SelectedIndex = 0;
        }

        //calculates the cost of working hours
        public async Task Button_Click_Calculate_workhours(Technician_calculate_workcost obj)
        {
            int estimated_hours, price_per_hour, disembarkation_cost;
            string estimated_hours_string = obj.Estimated_hours_textbox.Text;
            string price_per_hour_string = obj.Price_per_hour_textbox.Text;
            string disembarkation_cost_string = obj.Disembarkation_cost_textbox.Text;
            var selectedStockId = obj.ProjectId_combobox.SelectedItem.ToString();
            var selectedProjectkId = Convert.ToInt32(obj.ProjectId_combobox.SelectedItem);
            var _projectType = await GetProjectType(selectedProjectkId);
            var _projectDescription = await GetProjectDescription(selectedProjectkId);
            var _projectPlace = await GetProjectPlace(selectedProjectkId);
            var _projectOrdererId = await GetProjectOrdererId(selectedProjectkId);
            var _projectUserId = await GetProjectUserId(selectedProjectkId);




            //correct time
            if (int.TryParse(estimated_hours_string, out estimated_hours))
            {
                if (estimated_hours > 0)
                {
                    //correct time and price

                    if (int.TryParse(price_per_hour_string, out price_per_hour))
                    {
                        if (price_per_hour > 0)
                        {
                            //correct time and price and one-time-cost
                            {
                                if (int.TryParse(disembarkation_cost_string, out disembarkation_cost))
                                {
                                    if (disembarkation_cost > 0)
                                    {
                                        int calculated_cost = (estimated_hours * price_per_hour) + disembarkation_cost;
                                        var _projectDescription_2 = _projectDescription + "" +
                " Wage Cost: " + calculated_cost.ToString();
                                        var putProject = new
                                        {
                                            Id = selectedProjectkId,
                                            ProjectType = _projectType,
                                            Projectdescription = _projectDescription_2,
                                            Place = _projectPlace,
                                            OrdererId = _projectOrdererId,
                                            UserId = _projectUserId
                                        };
                                        using (var client = RestHelper.GetRestClient())
                                        {
                                            var request = new HttpRequestMessage(HttpMethod.Put, "api/Project?id=" + selectedProjectkId);
                                            var content = new StringContent(JsonConvert.SerializeObject(putProject), Encoding.UTF8, "application/json");
                                            request.Content = content;
                                            var response = await client.SendAsync(request);
                                            var status = response.StatusCode;
                                            if (status.ToString() == "NoContent")
                                            {
                                                MessageBox.Show("Project description updated successfully!");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Error: project description update denied: " + status.ToString());
                                            }
                                        }
                                        obj.Wage_cost_textbox.Text = calculated_cost.ToString();

                                        // Get the text from the textbox
                                        //string textboxText = obj.Estimated_hours_textbox.Text;

                                        // Get the selected item from the ComboBox
                                        //int selectedItem = Convert.ToInt32(obj.ProjectId_combobox.SelectedItem.ToString());

                                        // Create an object to hold the data
                                        //var workhours = new { textboxText, selectedItem };

                                        // Serialize the data to JSON
                                        //string jsonString = JsonConvert.SerializeObject(workhours);

                                        // Write the JSON to a file
                                        //File.WriteAllText("workhours.json", jsonString);
                                    }
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Price must be greater than 0!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Price must be an integer!");
                    }
                }
                else
                {
                    MessageBox.Show("Number of hours must be greater than 0!");
                }
            }
            else
            {
                MessageBox.Show("Number of hours must be an integer!");
            }
        }



        public async Task ListBoxLoad_controller2(Technician_calculate_workcost obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                    var sortedProjects = projects.OrderBy(x => x.Id).ToList();
                    obj.ProjectId_combobox.ItemsSource = sortedProjects.Select(x => x.Id);
                }
            }
        }

        // Load the already assigned items into the grid
        internal async Task<ObservableCollection<ProductListGridRow>> GetAssignedItems(Project_model project)
        {
            ObservableCollection<ProductListGridRow> productlist = new ObservableCollection<ProductListGridRow>();
            using (var client = RestHelper.GetRestClient())
            {
                var responseStockItem = await client.GetAsync("api/StockItem");
                var contentStockItem = await responseStockItem.Content.ReadAsStringAsync();
                var stockitems = JsonConvert.DeserializeObject<List<StockItem_model>>(contentStockItem);

                var response = await client.GetAsync("api/StockAccount");
                var content = await response.Content.ReadAsStringAsync();
                var stockaccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(content);

                // Filter for Reservation type and the current project
                stockaccounts = stockaccounts.FindAll(i => i.Type == StockAccountType.Reservation);
                stockaccounts = stockaccounts.FindAll(i => i.ProjectId == project.Id);
                foreach (var stockaccount in stockaccounts)
                {
                    productlist.Add(new ProductListGridRow()
                    {
                        Name = stockitems.Where(i => i.Id == stockaccount.StockItemId).FirstOrDefault().ItemType,
                        Count = stockaccount.Pieces
                    });
                }
                productlist = new ObservableCollection<ProductListGridRow>(productlist.OrderBy(p => p.Name));
                return productlist;
            }
        }

        // Load the already pre-reserved items into the grid
        internal async Task<ObservableCollection<ProductListGridRow>> GetPrereservedItems_controller(Project_model project)
        {
            ObservableCollection<ProductListGridRow> productlist = new ObservableCollection<ProductListGridRow>();
            using (var client = RestHelper.GetRestClient())
            {
                var responseStockItem = await client.GetAsync("api/StockItem");
                var contentStockItem = await responseStockItem.Content.ReadAsStringAsync();
                var stockitems = JsonConvert.DeserializeObject<List<StockItem_model>>(contentStockItem);

                var response = await client.GetAsync("api/StockAccount");
                var content = await response.Content.ReadAsStringAsync();
                var stockaccounts = JsonConvert.DeserializeObject<List<StockAccount_model>>(content);

                // Filter for Prereservation type and the current project
                stockaccounts = stockaccounts.FindAll(i => i.Type == StockAccountType.PreReservation);
                stockaccounts = stockaccounts.FindAll(i => i.ProjectId == project.Id);

                //group the prereserved items
                var groupedStockAccounts = stockaccounts.GroupBy(i => i.StockItemId).Select(g => new { StockItemId = g.Key, Pieces = g.Sum(i => i.Pieces) });

                foreach (var stockaccount in groupedStockAccounts)
                {
                    productlist.Add(new ProductListGridRow()
                    {
                        Name = stockitems.Where(i => i.Id == stockaccount.StockItemId).FirstOrDefault().ItemType,
                        Count = stockaccount.Pieces
                    });
                }
                productlist = new ObservableCollection<ProductListGridRow>(productlist.OrderBy(p => p.Name));
                return productlist;
            }
        }


        public async Task GetProjectList(Technician_list_projects view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
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

        public async Task<List<StockAccount_model>> getPreReservationsByProjectIdAndItemId(int projectId, int itemId)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockAccount");
                var contentStockAccount = await response.Content.ReadAsStringAsync();
                // Filter out any StockAccounts with different projectId and type
                var preReservations = JsonConvert.DeserializeObject<List<StockAccount_model>>(contentStockAccount);
                preReservations = preReservations.FindAll(i => i.ProjectId == projectId && i.Type == StockAccountType.PreReservation && i.StockItemId == itemId);
                //preReservations = preReservations.FindAll(i => i.Type == StockAccountType.PreReservation);
                return preReservations;
            }
        }
    }
}
