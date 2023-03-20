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
    /// Interaction logic for Technician_create_new_project.xaml
    /// </summary>
    public partial class Technician_create_new_project : Window
    {
        public Technician_create_new_project()
        {
            InitializeComponent();
        }


        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        private async void ListBoxLoaded_orderer(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.ListBoxLoaded_orderer_controller(this);
        }

        private async void Button_Click_new_Project(object sender, RoutedEventArgs e)
        {
            Technician_controller classObj = new Technician_controller();
            await classObj.Button_Click_new_Project(this);

        }

        private void back_to_menu_Click(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }
    }

    ////LISTENERS-------------------------------------------------------------------------------------------
    //loads the content of the combobox in the price modification site
    
}
