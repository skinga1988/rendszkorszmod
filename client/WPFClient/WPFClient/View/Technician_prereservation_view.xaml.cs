using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Technician_prereservation_view.xaml
    /// </summary>
    public partial class Technician_prereservation_view : Window
    {
        public ObservableCollection<StockItem_model> Products { get; set; }
        public ObservableCollection<Project_model> Projects { get; set; }
        public ObservableCollection<ProductListGridRow> PrereservedProducts { get; set; }
        public Technician_prereservation_view()
        {
            InitializeComponent();
            DataContext = this;  
        }

        //loading the content of the page
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.LoadPrereservationData_controller(this);
        }

        //back to the Assign item page
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Technician_AssignItems_view view = new Technician_AssignItems_view();
            view.Show();
            Close();
        }

        //pre-reserve button click
        private async void prereserve_Button_Click(object sender, RoutedEventArgs e)
        {
            if(quantityTextBox_prereservation.Text == "")
            {
                MessageBox.Show("Please enter the quantity");
                return;
            }
            Technician_controller controller = new Technician_controller();
            await controller.PrereserveItems_controller(this);
            quantityTextBox_prereservation.Text = "";

            // Redisplay pre-reserved items
            PrereservedProducts = await controller.GetPrereservedItems_controller((Project_model)projectsComboBox_prereservation.SelectedItem);
            datagrid_prereservation.ItemsSource = PrereservedProducts;
        }

        private void PreviewTextInput_event(object sender, TextCompositionEventArgs e)
        {
            // Filter every non-numerical character
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void projectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            PrereservedProducts = await controller.GetPrereservedItems_controller((Project_model)projectsComboBox_prereservation.SelectedItem);
            datagrid_prereservation.ItemsSource = PrereservedProducts;
        }
    }
}
