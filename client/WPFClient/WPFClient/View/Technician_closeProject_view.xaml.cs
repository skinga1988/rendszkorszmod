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

namespace WPFClient.View
{
    /// <summary>
    /// Interaction logic for Technician_closeProject_view.xaml
    /// </summary>
    public partial class Technician_closeProject_view : Window
    {
        public Technician_closeProject_view()
        {
            InitializeComponent();
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.GetProjectListForCompletion(this);
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            System.Environment.Exit(0);
        }


        private async void ListBoxLoad(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.ListBoxLoad_listprojectsForCompletion(this);
        }


        private void ProjectId_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void Button_Click_completed(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.SetProject_completed(this);
        }


        private async void Button_Click_failed(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.SetProject_failed(this);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.Modify_description(this);
        }

        private async void Button_Click_list(object sender, RoutedEventArgs e)
        {

            Technician_controller controller = new Technician_controller();
            await controller.GetProjectListForCompletion(this);
        }
    }

    
}
