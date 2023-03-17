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
using WPFClient.Model;
using System.Security.RightsManagement;
using WPFClient.Utilities;

namespace WPFClient.Controller
{
    internal class Manager_controller
    {
        public async Task Button_Click_modify_price_controller(Manager_modify_price_view obj)
        {
            int new_price;
            if (int.TryParse(obj.New_price_textbox.Text, out new_price))
            {
                if (new_price > 0)
                {
                    // get the ItemId
                    var selectedItem = obj.Part_Item_combobox.SelectedItem.ToString();
                    var itemId = await GetIdForSelectedItem(selectedItem);

                    //modify an item based on the Id
                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/StockItem/" + itemId);
                        var content = new StringContent
                        (
                            "{\"itemPrice\": " + new_price.ToString() + "}",
                            Encoding.UTF8,
                            "application/json"
                        );
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        MessageBox.Show(status.ToString());
                        var responseContent = await response.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Price cannot be negative!");
                }
            }
            else
            {
                MessageBox.Show("Please provide an integer as price!");
            }
        }

        public async Task ListBoxLoad_controller(Manager_modify_price_view obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                    obj.Part_Item_combobox.ItemsSource = items.Select(x => x.ItemType);
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
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.ItemPrice;
            }
        }

        public async Task<int> GetIdForSelectedItem(string selectedItem)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.Id;
            }
        }
    }
}
