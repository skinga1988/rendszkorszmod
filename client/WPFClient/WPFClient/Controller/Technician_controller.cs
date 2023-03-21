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

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        public ObservableCollection<ProductListGridRow> gridRows { get; set; }

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

        // A.4
        public async Task AssignItems(Technician_AssignItems_view view)
        {
            // Tasks:
            // 1. make new StockAccount or update it
            // 2. increase the reserved in the Stock table
            // 3. set the project status to Draft

            var product = (StockItem_model)view.productComboBox.SelectedItem;
            var currentProject= (Project_model)view.projectsComboBox.SelectedItem;
            int currentProjectId = currentProject.Id;
            int count = Convert.ToInt32(view.quantityTextBox.Text);
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
                        Pieces = stockAccount.Pieces + count,
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
                if (project.ProjectType != projectstatus)
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
                MessageBox.Show("The item has been assigned to the project");
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
                foreach(var stock in stocks)
                {
                    availibility += (stock.AvailablePieces - stock.ReservedPieces);
                }
                view.availableTextBox.Text = availibility>=0?availibility.ToString():"0";
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
                return new ObservableCollection<Project_model>(projects.FindAll(i => i.UserId == userid));
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

        // Loads data into ComboBoxes in AssignItem view
        public async Task LoadAssignItemsData(Technician_AssignItems_view view)
        {
            view.Projects = await GetProjectCollection();
            view.projectsComboBox.ItemsSource = view.Projects;
            view.projectsComboBox.SelectedIndex = 0;

            view.Products = await GetProductCollection();
            view.productComboBox.ItemsSource = view.Products;
            view.productComboBox.SelectedIndex = 0;
        }

        //calculates the cost of working hours
        public async Task Button_Click_Calculate_workhours(Technician_calculate_workcost obj)
        {
            int estimated_hours, price_per_hour, disembarkation_cost;
            string estimated_hours_string = obj.Estimated_hours_textbox.Text;
            string price_per_hour_string = obj.Price_per_hour_textbox.Text;
            string disembarkation_cost_string = obj.Disembarkation_cost_textbox.Text;
            var selectedStockId = obj.ProjectId_combobox.SelectedItem.ToString();



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

                                        {


                                            int calculated_cost = (estimated_hours * price_per_hour) + disembarkation_cost;

                                            MessageBox.Show("The calculated price is: " + calculated_cost);

                                            // Get the text from the textbox
                                            string textboxText = obj.Estimated_hours_textbox.Text;

                                            // Get the selected item from the ComboBox
                                            int selectedItem = Convert.ToInt32(obj.ProjectId_combobox.SelectedItem.ToString());

                                            // Create an object to hold the data
                                            var workhours = new { textboxText, selectedItem };

                                            // Serialize the data to JSON
                                            string jsonString = JsonConvert.SerializeObject(workhours);

                                            // Write the JSON to a file
                                            File.WriteAllText("workhours.json", jsonString);
                                        }
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

    }

}
