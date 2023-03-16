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
using System.Windows.Controls;

namespace WPFClient.Controller
{
    internal class Manager_controller
    {
        public void Button_Click_2modify_price_controller(Manager_modify_price_view obj)
        {
            Manager_view manager_view = new Manager_view();
            obj.Close();
            manager_view.Show();
        }

        public async Task ListBoxLoad_controller(Manager_modify_price_view obj)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7243/");
                var response = await client.GetAsync("api/StockItem");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<Item>>(content);
                    obj.Part_Item_combobox.ItemsSource = items.Select(x => x.itemType);
                }
            }
        }

        public async Task Part_Item_combobox_SelectionChanged(Manager_modify_price_view obj)
        {
            var selectedItem = obj.Part_Item_combobox.SelectedItem.ToString();
            var itemPrice = await GetPriceForSelectedItem(selectedItem);
            obj.Current_price_textbox.Text = itemPrice.ToString();
        }

        public async Task<int> GetPriceForSelectedItem(string selectedItem)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7243/api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Item>>(responseContent);
                var selectedItemType = items.Find(item => item.itemType == selectedItem);
                return selectedItemType.itemPrice;
            }
        }

        public class Item
        {
            public int id { get; set; }
            public int itemPrice { get; set; }
            public string itemType { get; set; }
        }
    }
}
