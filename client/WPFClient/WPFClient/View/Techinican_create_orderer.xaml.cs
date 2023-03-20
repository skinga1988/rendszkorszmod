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
    /// Interaction logic for Techinican_create_orderer.xaml
    /// </summary>
    public partial class Techinican_create_orderer : Window
    {
        public Techinican_create_orderer()
        {
            InitializeComponent();
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

        private async void Button_Click_Create_orderer(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.Button_Click_Create_orderer_controller(this);
        }
    }
}
