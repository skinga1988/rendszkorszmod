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
    /// Interaction logic for Storekeeper_listitems_view.xaml
    /// </summary>
    public partial class Storekeeper_listitems_view : Window
    {
        public Storekeeper_listitems_view()
        {
            InitializeComponent();
        }


        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storekeeper_controller controller = new Storekeeper_controller();
            await controller.GetItemList(this);
        }



    }
}
