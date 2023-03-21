using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        
        //create new orderer
        public async Task Button_Click_Create_orderer_controller(Techinican_create_orderer obj)
        {
            string orderer_name = obj.New_orderer_textbox.Text;
            string description = obj.Description_textbox.Text;


            //both textbox are filled
            if (orderer_name != "" && description != "")
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
                    }
                    else
                    {
                        MessageBox.Show("Orderer creation failed!");

                    }
                }
            }
            else
            {
                MessageBox.Show("Fill both textbox!");
            }
        }
        //create new project
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
                var projectid = await GetProjectId(_project_type,_description, _place, _ordererId);
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
