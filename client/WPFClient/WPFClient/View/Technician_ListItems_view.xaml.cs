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
    /// Interaction logic for Technician_ListItems_view.xaml
    /// </summary>
    public partial class Technician_ListItems_view : Window
    {
        public Technician_ListItems_view()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.GetProductList(this);
        }

        private void grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var grid = sender as DataGrid;
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;

                    if (bindingPath == "Count")
                    {
                        var el = e.EditingElement as TextBox;
                        try
                        {
                            int value = Convert.ToInt32(el.Text);
                            // Check checkbox automatically
                            //(e.Row.Item as ProductListGridRow).IsSelected = true;
                            //grid.Dispatcher.BeginInvoke(
                            //    new Action(() => grid.Items.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
                        }
                        catch
                        {
                            MessageBox.Show("Only numbers are allowed");
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
    }
}
