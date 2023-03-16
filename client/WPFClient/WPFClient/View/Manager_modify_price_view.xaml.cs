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
    /// Interaction logic for Manager_modify_price_view.xaml
    /// </summary>
    public partial class Manager_modify_price_view : Window
    {
        public Manager_modify_price_view()
        {
            InitializeComponent();
        }

        private async void Button_Click_modify_price(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Button_Click_modify_price_controller(this);
        }

        private async void ListBoxLoad(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.ListBoxLoad_controller(this);
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Manager_view window = new Manager_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        private async void Part_Item_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Part_Item_combobox_SelectionChanged(this);

            
        }
    }
}
