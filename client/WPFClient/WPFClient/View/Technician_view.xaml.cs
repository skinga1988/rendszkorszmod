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

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Technician_view : Window
    {
        public Technician_view()
        {
            InitializeComponent();
        }

        ////BUTTONS---------------------------------------------------------------------------------------------
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

        private void Button_Click_new_orderer(object sender, RoutedEventArgs e)
        {
            Techinican_create_orderer window = new Techinican_create_orderer();
            window.Show();
            this.Close();

        }

        private void Button_Click_create_new_project(object sender, RoutedEventArgs e)
        {
            Technician_create_new_project window = new Technician_create_new_project();
            window.Show();
            this.Close();
        }

        //opens the listing projects window
        private void Button_Click_2list_projects(object sender, RoutedEventArgs e)
        {
            Technician_list_projects list = new Technician_list_projects();
            list.Show();
            this.Close();
        }

        //opens the calculating work cost window
        private void Button_Click_5calculate_workcost(object sender, RoutedEventArgs e)
        {
            Technician_calculate_workcost calculate = new Technician_calculate_workcost();
            calculate.Show();
            this.Close();
        }
    }
}
