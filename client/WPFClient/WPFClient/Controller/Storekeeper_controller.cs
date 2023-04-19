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

namespace WPFClient.Controller
{
    internal class Storekeeper_controller
    {
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
                    obj.ProjectID_combobox.ItemsSource = sortedProjects.Select(x => x.Id);
                }
            }
        }

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
                    }

                }
            }
        }

        public async Task GetItemList(Storekeeper_listitems_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Stock");
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(content);
                var sortedItems = items.OrderBy(x => x.Id).ToList();
                gridItems = new ObservableCollection<ItemListGridRow>();
                foreach (var item in sortedItems)
                {
                    gridItems.Add(new ItemListGridRow()
                    {
                        Id = item.Id,
                        RowId = item.RowId,
                        ColumnId = item.ColumnId,
                        BoxId = item.BoxId,
                        Name = item.StockItem.ItemType

                    });
                }
                view.grid.DataContext = gridItems;
            }
        }
    }
}
