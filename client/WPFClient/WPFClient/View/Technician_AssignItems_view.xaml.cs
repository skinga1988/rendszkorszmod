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
    /// Interaction logic for Technician_AssignItems_view.xaml
    /// </summary>
    public partial class Technician_AssignItems_view : Window
    {
        private ObservableCollection<ProductListGridRow> allItems;
        public Technician_AssignItems_view(ItemCollection griddata)
        {
            InitializeComponent();
            allItems = new ObservableCollection<ProductListGridRow>();
            foreach (var data in griddata.SourceCollection)
            {
                var row = data as ProductListGridRow;
                if(row.IsSelected && row.Count > 0) 
                { 
                    allItems.Add(row); 
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.LoadAssignItemsData(this, allItems);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Technician_view view = new Technician_view();
            view.Show();
            Close();
        }
    }
}
