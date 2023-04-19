using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Manager_ListMissingPartItems_view.xaml
    /// </summary>
    public partial class Manager_ListMissingPartItems_view : Window
    {
        public ObservableCollection<StockItem_model> Products { get; set; }
        public ObservableCollection<Project_model> Projects { get; set; }
        public ObservableCollection<ProductListGridRow> PrereservedProducts { get; set; }


        public Manager_ListMissingPartItems_view()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager_view window = new Manager_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Manager_controller controller = new Manager_controller();
            await controller.GetMissingProducts(this);
        }

        /* private async void Button_Click_2(object sender, RoutedEventArgs e)
         {
             Manager_controller controller = new Manager_controller();
             await controller.GetMissingProductsPreReserved(this);

             datagrid_prereservation.ItemsSource = PrereservedProducts;
         }*/
    }
}
