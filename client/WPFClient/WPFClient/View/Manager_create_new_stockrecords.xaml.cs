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
    /// Interaction logic for Manager_create_new_stockrecords.xaml
    /// </summary>
    public partial class Manager_create_new_stockrecords : Window
    {
        public Manager_create_new_stockrecords()
        {
            InitializeComponent();

        }

        ////BUTTONS--------------------------------------------------------------------------------------------
        //create new stockaccount and stock record
        private async void Button_Click_new_StockAccount(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Button_Click_new_StockAccount_controller(this);

        }

        //takes us back to the manager main menu
        private void back_to_menu_Click(object sender, RoutedEventArgs e)
        {
            Manager_view window = new Manager_view();
            window.Show();
            this.Close();
        }

        //closing properly
        private void Button_ContextMenuClosing_Back(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        ////LISTENERS-------------------------------------------------------------------------------------------
        //loads the content of the combobox in the stocaccount and stock modification site
        private async void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.ListBoxLoad_controller(this);
        }

        //listener for combobox selection changes in the stocaccount and stock modification site
        private async void Stock_item_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manager_controller classObj = new Manager_controller();
            await classObj.Part_Item_combobox_SelectionChanged(this);
        }
    }
}
