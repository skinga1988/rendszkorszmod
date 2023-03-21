using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient.Model;
using WPFClient.Utilities;

namespace WPFClient.View
{
    /// <summary>
    /// Interaction logic for Technician_list_projects.xaml
    /// </summary>
    public partial class Technician_list_projects : Window
    {
        public Technician_list_projects()
        {
            InitializeComponent();
        }


        private async Task refreshdataAsync()
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/Project");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<Project_model>>(content);
                    ProjectsDataGrid.ItemsSource = projects;
                }
            }
        }

        ////BUTTONS----------------------------------------------------------------------------------------------------
        //Back button: back to the technician main menu
        private void Button_Click_Back4(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back4(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        //listing projects
        private async void Button_Click_List2(object sender, RoutedEventArgs e)
        {
            await refreshdataAsync();

        }



    }
}