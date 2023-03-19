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
    }
}
