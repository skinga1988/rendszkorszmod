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
    /// Interaction logic for Technician_calculate_workcost.xaml
    /// </summary>
    public partial class Technician_calculate_workcost : Window
    {
        public Technician_calculate_workcost()
        {
            InitializeComponent();
        }


        //Back button: back to the technician main menu
        private void Button_Click_Back3(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back3(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        private async void Button_Click_calculate_workcost(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.Button_Click_Calculate_workhours(this);
        }




        private async void ListBoxLoad2(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.ListBoxLoad_controller2(this);
        }

        private void ProjectId_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}