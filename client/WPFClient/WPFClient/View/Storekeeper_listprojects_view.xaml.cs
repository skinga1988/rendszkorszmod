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
using WPFClient.Controller;
using WPFClient.Model;

namespace WPFClient.View
{
    /// <summary>
    /// Interaction logic for Storekeeper_listprojects_view.xaml
    /// </summary>
    public partial class Storekeeper_listprojects_view : Window
    {
        public Storekeeper_listprojects_view()
        {
            InitializeComponent();
        }

        private async void Button_Click_list(object sender, RoutedEventArgs e)
        {
            Storekeeper_controller controller = new Storekeeper_controller();
            await controller.GetProjectList(this);
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Storekeeper_view window = new Storekeeper_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            System.Environment.Exit(0);
        }


        private async void ListBoxLoad(object sender, RoutedEventArgs e)
        {
            Storekeeper_controller classObj = new Storekeeper_controller();
            await classObj.ListBoxLoad_listprojects(this);
        }


        private void ProjectId_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Storekeeper_controller classObj = new Storekeeper_controller();
            await classObj.SetInProgressProject(this);

            int projectId = Convert.ToInt32(ProjectID_combobox.SelectedItem);
            Storekeeper_listitems_view window = new Storekeeper_listitems_view(projectId);
            this.Close();
            window.Show();
        }
    }
}
