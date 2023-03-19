using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Security.Cryptography;
using WPFClient.View;
using WPFClient.Controller;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Manager_view : Window
    {
        public Manager_view()
        {
            InitializeComponent();
        }

        ////BUTTONS------------------------------------------------------------------------------------------------
        //takes us back to the login window
        private void Button_Click_Logout(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        //opens the part item creation window
        private void Button_Click_1create_new_part_item(object sender, RoutedEventArgs e)
        {
            Manager_create_new_part_item_view window = new Manager_create_new_part_item_view();
            window.Show();
            this.Close();
        }

        //opens the part item price modification window
        private void Button_Click_2modify_price(object sender, RoutedEventArgs e)
        {
            Manager_modify_price_view window = new Manager_modify_price_view();
            window.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager_create_new_stockrecords window = new Manager_create_new_stockrecords();
            window.Show();
            this.Close();
        }
    }
}
