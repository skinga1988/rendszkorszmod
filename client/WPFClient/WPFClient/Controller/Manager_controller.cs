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

namespace WPFClient.Controller
{
    internal class Manager_controller
    {
        ////BUTTONS--------------------------------------------------------------------------------------------
        //modifies the price of the item
        public async Task Button_Click_modify_price_controller(Manager_modify_price_view obj)
        {
            int new_price;
            if (int.TryParse(obj.New_price_textbox.Text, out new_price))
            {
                if (new_price > 0)
                {
                    // get the ItemId, ItemType and MaxItem
                    var selectedItemType = obj.Part_Item_combobox.SelectedItem.ToString();
                    var selectedItemId = await GetIdForSelectedItem(selectedItemType);
                    var selectedItemMaxItem = await GetMaxQuantityForSelectedItem(selectedItemType);
                    
                    var putObject = new
                    {
                        id = selectedItemId,
                        itemPrice = new_price,
                        itemType = selectedItemType,
                        maxItem = selectedItemMaxItem
                    };

                    //modify an item price based on the Id
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7243");
                        var request = new HttpRequestMessage(HttpMethod.Put, "/api/StockItem?id=" + selectedItemId);
                        var content = new StringContent(JsonConvert.SerializeObject(putObject), Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        if (status.ToString() == "NoContent")
                        {
                            MessageBox.Show("Part item price update successfully finished.");
                        }
                        else
                        {
                            MessageBox.Show("Error: price update denied: " + status.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Price must be greater than 0!");
                }
            }
            else
            {
                MessageBox.Show("Price must be an integer!");
            }
        }

        //creates a new item in the StockItem table
        public async Task Button_Click_Create_controller(Manager_create_new_part_item_view obj)
        {
            int item_price, max_quantity;
            string item_type = obj.New_item_type_textbox.Text;
            string item_price_string = obj.Item_price_textbox.Text;
            string max_quantity_string = obj.Max_quantity_textbox.Text;

            //if item name is not an empty string
            if (item_type != "")
            {
                //correct item type
                if (int.TryParse(item_price_string, out item_price))
                {
                    if (item_price > 0)
                    {
                        //correct item name and price

                        if (int.TryParse(max_quantity_string, out max_quantity))
                        {
                            if (max_quantity > 0)
                            {
                                //correct item name and price and quantity
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri("https://localhost:7243");
                                    var newItem = new
                                    {
                                        itemType = item_type,
                                        itemPrice = item_price,
                                        maxItem = max_quantity
                                    };
                                    var json = JsonConvert.SerializeObject(newItem);
                                    var response = await client.PostAsync("/api/StockItem", new StringContent(json, Encoding.UTF8, "application/json"));
                                    string status = response.StatusCode.ToString();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        // Item created successfully
                                        MessageBox.Show("Item created successfully!");
                                    }
                                    else
                                    {
                                        // Item creation failed
                                        if (status == "422")
                                        {
                                            MessageBox.Show("Item creation failed: existing item type!");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Item creation failed: " + status);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Max quantity must be greater than 0!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Max quantity must be an integer!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Price must be greater than 0!");
                    }
                }
                else
                {
                    MessageBox.Show("Price must be an integer!");
                }
            }
            else
            {
                MessageBox.Show("Item type is invalid or empty!");
            }
        }

        
        ////LISTENERS-------------------------------------------------------------------------------------------
        //loads the content of the combobox in the price modification site
        public async Task ListBoxLoad_controller(Manager_modify_price_view obj)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7243/");
                var response = await client.GetAsync("api/StockItem");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                    var sortedItems = items.OrderBy(x => x.ItemType).ToList(); // sort the items by ItemType in ascending order
                    obj.Part_Item_combobox.ItemsSource = sortedItems.Select(x => x.ItemType);
                }
            }
        }

        //listener for combobox selection changes
        public async Task Part_Item_combobox_SelectionChanged(Manager_modify_price_view obj)
        {
            var selectedItem = obj.Part_Item_combobox.SelectedItem.ToString();
            var itemPrice = await GetPriceForSelectedItem(selectedItem);
            obj.Current_price_textbox.Text = itemPrice.ToString();
        }

        ////FUNCTIONS--------------------------------------------------------------------------------------------
        //gets the price of the selected item in the combobox list
        public async Task<int> GetPriceForSelectedItem(string selectedItem)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7243/api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.ItemPrice;
            }
        }

        //gets the id of the selected item in the combobox list
        public async Task<int> GetIdForSelectedItem(string selectedItem)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7243/api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.Id;
            }
        }

        //gets the maximum quantity of the selected item in the combobox list
        public async Task<int> GetMaxQuantityForSelectedItem(string selectedItem)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7243/api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.MaxItem;
            }
        }

        

    }
}
