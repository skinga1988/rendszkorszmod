using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static WPFClient.Controller.Login_controller;
using System.Windows;
using WPFClient.View;

namespace WPFClient.Controller
{
    internal class Manager_controller
    {
        public async void Button_Click_2modify_price_controller(Manager_modify_price_view obj)
        {
            Manager_view manager_view = new Manager_view();
            obj.Close();
            manager_view.Show();
        }

        public async void ListBoxLoad_controller(Manager_modify_price_view obj)
        {
            List<string> items = new List<string>() { "Item 1", "Item 2", "Item 3" };
            obj.Part_Item_listbox.ItemsSource = items;
        }
    }
}
