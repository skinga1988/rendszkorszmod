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
    /// Interaction logic for Manager_modify_max_item.xaml
    /// </summary>
    public partial class Manager_modify_max_item : Window
    {
        public Manager_modify_max_item()
        {
            InitializeComponent();
        }


        //takes us back to the manager main menu
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
        ////LISTENERS-------------------------------------------------------------------------------------------
        //loads the content of the combobox in the price modification site
        private async void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.ListBoxLoad_controller(this);
        }

        //listener for combobox selection changes
        private async void Part_Item_combobox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Part_Item_combobox_SelectionChanged_2(this);
        }


        private async void Button_Click_modify_maxnum(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Button_Click_modify_maxnum(this);
        }
    }
}
