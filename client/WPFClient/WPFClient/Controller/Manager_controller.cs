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
using Xceed.Wpf.Toolkit.Primitives;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WPFClient.Controller
{
    internal class Manager_controller
    {

        public ObservableCollection<ProductListGridRow> gridRows { get; set; }

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
                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/StockItem?id=" + selectedItemId);
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

        //modifies the maximum number of the item
        public async Task Button_Click_modify_maxnum(Manager_modify_max_item obj)
        {
            int new_maxnum;
            if (int.TryParse(obj.New_maxnum_textbox.Text, out new_maxnum))
            {
                if (new_maxnum > 0)
                {
                    // get the ItemId, ItemType and MaxItem
                    var selectedItemType = obj.Part_Item_combobox_2.SelectedItem.ToString();
                    var selectedItemId = await GetIdForSelectedItem(selectedItemType);
                    var selectedTtemPrice = await GetPriceForSelectedItem(selectedItemType);

                    var putObject = new
                    {
                        id = selectedItemId,
                        itemPrice = selectedTtemPrice,
                        itemType = selectedItemType,
                        maxItem = new_maxnum
                    };

                    //modify an item maximum number based on the Id
                    using (var client = RestHelper.GetRestClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Put, "api/StockItem?id=" + selectedItemId);
                        var content = new StringContent(JsonConvert.SerializeObject(putObject), Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var status = response.StatusCode;
                        if (status.ToString() == "NoContent")
                        {
                            MessageBox.Show("Part item maximum number update successfully finished.");
                            obj.New_maxnum_textbox.Text = "";
                            obj.Current_maximum_number_textbox.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Error: maximum number update denied: " + status.ToString());
                        }
                    }
                    //Kell egy lista azokból a stock sorokból ahol a változtatott termék id-ja van
                    List<Stock_model> stocks = await getTheListWhereIdTheSame(selectedItemId);
                    //egy loopban módosítani kell a maxid-kat
                    foreach (var Stock in stocks)
                    {
                        //put metódus kell 
                        using (var client = RestHelper.GetRestClient())
                        {
                            var selectedItemId_ = Stock.Id;
                            var row_id_ = Stock.RowId;
                            var column_id_ = Stock.ColumnId;
                            var box_id_ = Stock.ColumnId;
                            var available_pieces_ = Stock.AvailablePieces;
                            var reserved_pieces_ = Stock.ReservedPieces;
                            var stockitem_id_ = Stock.StockItemId;
                            var putStock = new
                            {
                                Id = selectedItemId_,
                                RowId = row_id_,
                                ColumnID = column_id_,
                                BoxId = box_id_,
                                MaxPieces = new_maxnum,
                                AvailablePieces = available_pieces_,
                                ReservedPieces = reserved_pieces_,
                                StockItemId = stockitem_id_
                            };                           
                            var request = new HttpRequestMessage(HttpMethod.Put, "api/Stock?id=" + selectedItemId_);
                            var content = new StringContent(JsonConvert.SerializeObject(putStock), Encoding.UTF8, "application/json");
                            request.Content = content;
                            var response = await client.SendAsync(request);
                            var status = response.StatusCode;
                            if (status.ToString() == "NoContent")
                            {
                                MessageBox.Show("Stock Id: " + selectedItemId_ + " successfully updated.");                               
                            }
                            else
                            {
                                MessageBox.Show("Error: stock record update denied: " + status.ToString());
                            }
                        }
                    }
                    
                }
                else
            {
                MessageBox.Show("Maximum number must be greater than 0!");
            }
        }
            else
            {
                MessageBox.Show("Maximum number must be an integer!");
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
                                using (var client = RestHelper.GetRestClient())
                                {
                                    var newItem = new
                                    {
                                        itemType = item_type,
                                        itemPrice = item_price,
                                        maxItem = max_quantity
                                    };
                                    var json = JsonConvert.SerializeObject(newItem);
                                    var response = await client.PostAsync("api/StockItem", new StringContent(json, Encoding.UTF8, "application/json"));
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

        //creates a new record in the StockAccount and Stock table
        public async Task Button_Click_new_StockAccount_controller(Manager_create_new_stockrecords obj)
        {
            int pieces, row_id, column_id, box_id;
            string stock_account_type = obj.StockAccount_type_combobox.Text;
            string _pieces = obj.Pieces_editable_textbox.Text;
            string _stock_item_type = obj.Stock_item_select.Text;
            string _row_id = obj.Row_n.Text;
            string _column_id = obj.column_n.Text;
            string _box_id = obj.box_n.Text;
            DateTime accountDateTime = DateTime.Now;

            //if account type is not an empty string
            if (stock_account_type != "")
            {
                //correct row and column and box id
                if (int.TryParse(_row_id, out row_id) && int.TryParse(_column_id, out column_id) && int.TryParse(_box_id, out box_id))
                {
                    if (row_id > 0 && column_id > 0 && box_id > 0)
                    {
                        //check whether row+column+box combo exist in database
                        var existingRCBid = await GetIdIfExistInDatabase(row_id, column_id, box_id);
                        if (existingRCBid != 0)
                        //the row+column+box combo is already exist
                        {
                            var stockitemId = await GetStockItemIdInStock(existingRCBid);
                            var selectedItemType = obj.Stock_item_select.SelectedItem.ToString();
                            var selectedItemId = await GetIdForSelectedItem(selectedItemType);
                            //check whether the stockitem id is the same
                            if (stockitemId == selectedItemId)
                            {
                                var selectedMaxItem = await GetMaxQuantityForSelectedItem(selectedItemType); //max amount can be 
                                var avaliable_pieces = await GetStockItemAvailableInStock(existingRCBid); //available amount
                                //in case the existing RCB contains the same itemtype check the max_db
                                int.TryParse(_pieces, out pieces);
                                var new_available_pieces = avaliable_pieces + pieces;
                                if (new_available_pieces <= selectedMaxItem)
                                {
                                    //if the new available_pieces is not bigger than the maxpieces create the new record in stock and stockaccount 
                                    //correct item name and price and quantity
                                    var sel_reserved_pieces = await GetStockItemReservedInStock(existingRCBid);
                                    using (var client = RestHelper.GetRestClient())
                                    {

                                        var newStockAccountRecord = new
                                        {
                                            StockAccountType = stock_account_type,
                                            Pieces = pieces,
                                            AccountTime = accountDateTime,
                                            ProjectId = 0,
                                            StockItemId = stockitemId,
                                            UserId = userid
                                        };
                                        var json = JsonConvert.SerializeObject(newStockAccountRecord);
                                        var response = await client.PostAsync("api/StockAccount", new StringContent(json, Encoding.UTF8, "application/json"));
                                        string status = response.StatusCode.ToString();
                                        if (response.IsSuccessStatusCode)
                                        {
                                            // Item created successfully
                                            MessageBox.Show("New StockAccount record created successfully!");
                                        }
                                        else
                                        {
                                            {
                                                MessageBox.Show("StockAccount record creation failed: " + status);
                                                obj.StockAccount_type_combobox.SelectedValue = 0;
                                                obj.box_n.SelectedValue = 0;
                                                obj.Row_n.SelectedValue = 0;
                                                obj.column_n.SelectedValue = 0;
                                                obj.Pieces_editable_textbox.Text = "";
                                                return;
                                            }
                                        }
                                    }
                                    var putStockRecord = new
                                    {
                                        Id = existingRCBid,
                                        RowId = row_id,
                                        ColumnID = column_id,
                                        BoxId = box_id,
                                        MaxPieces = selectedMaxItem,
                                        AvailablePieces = new_available_pieces,
                                        ReservedPieces = sel_reserved_pieces,
                                        StockItemId = stockitemId
                                    };
                                    using (var client = RestHelper.GetRestClient())
                                    {
                                        var request = new HttpRequestMessage(HttpMethod.Put, "api/Stock?id=" + existingRCBid);
                                        var content = new StringContent(JsonConvert.SerializeObject(putStockRecord), Encoding.UTF8, "application/json");
                                        request.Content = content;
                                        var response = await client.SendAsync(request);
                                        var status = response.StatusCode;
                                        if (status.ToString() == "NoContent")
                                        {
                                            MessageBox.Show("Update Stock record successfully finished.");
                                            obj.StockAccount_type_combobox.SelectedValue = 0;
                                            obj.box_n.SelectedValue = 0;
                                            obj.Row_n.SelectedValue = 0;
                                            obj.column_n.SelectedValue = 0;
                                            obj.Pieces_editable_textbox.Text = "";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Update Stock record denied: " + status.ToString());
                                            obj.StockAccount_type_combobox.SelectedValue = 0;
                                            obj.box_n.SelectedValue = 0;
                                            obj.Row_n.SelectedValue = 0;
                                            obj.column_n.SelectedValue = 0;
                                            obj.Pieces_editable_textbox.Text = "";
                                        }
                                    }

                                }
                                else
                                {
                                    var piecesCanBeplaced = selectedMaxItem - avaliable_pieces;
                                    MessageBox.Show("Only " + piecesCanBeplaced.ToString() + " item can be placed in the box!");
                                }

                            }
                            else
                            {
                                var itemid = await GetStockItemIdInStock(existingRCBid);
                                var itemtype = await GetItemType(itemid);
                                MessageBox.Show("The box only available for item: " + itemtype);
                            }

                        }
                        else
                        {

                            var selectedItemType = obj.Stock_item_select.SelectedItem.ToString();
                            var stockitemId = await GetIdForSelectedItem(selectedItemType);
                            var selectedMaxItem = await GetMaxQuantityForSelectedItem(selectedItemType);
                            var avaliable_pieces = 0; //available amount  new record                                                                                                   
                            int.TryParse(_pieces, out pieces);
                            var new_available_pieces = avaliable_pieces + pieces;
                            if (new_available_pieces <= selectedMaxItem)
                            {
                                using (var client = RestHelper.GetRestClient())
                                {
                                    var newStockAccountRecord = new
                                    {
                                        stockAccountType = stock_account_type,
                                        pieces = pieces,
                                        accountTime = accountDateTime,
                                        projectId = 0,
                                        stockItemId = stockitemId,
                                        userId = userid
                                    };
                                    var json = JsonConvert.SerializeObject(newStockAccountRecord);
                                    var response = await client.PostAsync("api/StockAccount", new StringContent(json, Encoding.UTF8, "application/json"));
                                    string status = response.StatusCode.ToString();
                                    int.TryParse(_pieces, out pieces);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        MessageBox.Show("New StockAccount record created successfully!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("StockAccount record creation failed: " + status);
                                        obj.StockAccount_type_combobox.SelectedValue = 0;
                                        obj.box_n.SelectedValue = 0;
                                        obj.Row_n.SelectedValue = 0;
                                        obj.column_n.SelectedValue = 0;
                                        obj.Pieces_editable_textbox.Text = "";
                                        return;
                                    }
                                }
                                using (var client = RestHelper.GetRestClient())
                                {
                                    int.TryParse(_pieces, out pieces);
                                    var newStockRecord = new
                                    {
                                        RowId = row_id,
                                        ColumnID = column_id,
                                        BoxId = box_id,
                                        MaxPieces = selectedMaxItem,
                                        AvailablePieces = pieces,
                                        ReservedPieces = 0,
                                        StockItemId = stockitemId
                                    };
                                    var json = JsonConvert.SerializeObject(newStockRecord);
                                    var response = await client.PostAsync("api/Stock", new StringContent(json, Encoding.UTF8, "application/json"));
                                    string status = response.StatusCode.ToString();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        MessageBox.Show("New Stock record created successfully!");
                                        obj.StockAccount_type_combobox.SelectedValue = 0;
                                        obj.box_n.SelectedValue = 0;
                                        obj.Row_n.SelectedValue = 0;
                                        obj.column_n.SelectedValue = 0;
                                        obj.Pieces_editable_textbox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("StockAccount record creation failed: " + status);
                                        obj.StockAccount_type_combobox.SelectedValue = 0;
                                        obj.box_n.SelectedValue = 0;
                                        obj.Row_n.SelectedValue = 0;
                                        obj.column_n.SelectedValue = 0;
                                        obj.Pieces_editable_textbox.Text = "";
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Max item can be placed: " + selectedMaxItem.ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Fill the StockAccountType!");
            }
        }


        ////LISTENERS-------------------------------------------------------------------------------------------
        //loads the content of the combobox in the price modification site
        public async Task ListBoxLoad_controller(Manager_modify_price_view obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
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
        public async Task ListBoxLoad_controller(Manager_create_new_stockrecords obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                    var sortedItems = items.OrderBy(x => x.ItemType).ToList(); // sort the items by ItemType in ascending order
                    obj.Stock_item_select.ItemsSource = sortedItems.Select(x => x.ItemType);
                }
            }
        }

        public async Task ListBoxLoad_controller(Manager_modify_max_item obj)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                    var sortedItems = items.OrderBy(x => x.ItemType).ToList(); // sort the items by ItemType in ascending order
                    obj.Part_Item_combobox_2.ItemsSource = sortedItems.Select(x => x.ItemType);
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
        public async Task Stock_item_select_SelectionChanged(Manager_create_new_stockrecords obj)
        {
            var selectedItem = obj.Stock_item_select.SelectedItem.ToString();
           
        }

        public async Task Part_Item_combobox_SelectionChanged_2(Manager_modify_max_item obj)
        {
            var selectedItem = obj.Part_Item_combobox_2.SelectedItem.ToString();
            var itemMax = await GetMaxQuantityForSelectedItem(selectedItem);
            obj.Current_maximum_number_textbox.Text = itemMax.ToString();
        }

        ////FUNCTIONS--------------------------------------------------------------------------------------------
        //gets the price of the selected item in the combobox list
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

        //gets the id of the selected item in the combobox list
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
        //find existing row+column+box Id in database
        public async Task<int> GetIdIfExistInDatabase(int r, int c, int b)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Stock");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(responseContent);
                var selecteditem = items.Any(item => item.RowId == r && item.ColumnId == c && item.BoxId == b);
                if (selecteditem)
                {
                    var selecteditem_ = items.Find(item => item.RowId == r && item.ColumnId == c && item.BoxId == b);
                    return selecteditem_.Id;
                }
                else
                    return 0;
            }
        }
        // find the stockitemId in stock
        public async Task<int> GetStockItemIdInStock(int stock_id)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Stock");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(responseContent);
                var selecteditem = items.Find(item => item.Id == stock_id);
                return selecteditem.StockItemId;
            }
        }
        //gets the maximum quantity of the selected item in the stock 
        public async Task<int> GetStockItemAvailableInStock(int stock_id)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Stock");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(responseContent);
                var selecteditem = items.Find(item => item.Id == stock_id);
                return selecteditem.AvailablePieces;
            }
        }
        //gets the reserved quantity of the selected item in the stock
        public async Task<int> GetStockItemReservedInStock(int stock_id)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Stock");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(responseContent);
                var selecteditem = items.Find(item => item.Id == stock_id);
                return selecteditem.ReservedPieces;
            }
        }
        //get the itemtype from stockitem
        public async Task<string> GetItemType(int stock_id)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selecteditem = items.Find(item => item.Id == stock_id);
                return selecteditem.ItemType;
            }
        }


        //gets the maximum quantity of the selected item in the combobox list
        public async Task<int> GetMaxQuantityForSelectedItem(string selectedItem)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/StockItem");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(responseContent);
                var selectedItemType = items.Find(item => item.ItemType == selectedItem);
                return selectedItemType.MaxItem;
            }
        }

        //gets the list of stock based on id
        public async Task<List<Stock_model>> getTheListWhereIdTheSame(int stockitemid)
        {
            using (var httpClient = RestHelper.GetRestClient())
            {
                var response = await httpClient.GetAsync("api/Stock");
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Stock_model>>(responseContent);
                List<Stock_model> selectedStockList = items.FindAll(item => item.StockItemId == stockitemid);
                return selectedStockList;
            }

        }

        public async Task GetMissingProducts(Manager_ListMissingPartItems_view view)
        {
            using (var client = RestHelper.GetRestClient())
            {
                var response = await client.GetAsync("api/StockItem");
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<StockItem_model>>(content);
                var sortedItems = items.OrderBy(x => x.ItemType).ToList();
                gridRows = new ObservableCollection<ProductListGridRow>();
                foreach (var item in sortedItems)
                {
                    gridRows.Add(new ProductListGridRow()
                    {
                        Id = item.Id,
                        Name = item.ItemType,
                        Price = item.ItemPrice,
                        Availibility = 0
                    });
                }
                // Get availibility
                var responseStock = await RestHelper.GetRestClient().GetAsync("api/Stock");
                if (!responseStock.IsSuccessStatusCode)
                {
                    return;
                }
                var contentStock = await responseStock.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(contentStock);
                foreach (var stock in stocks)
                {
                    var item = gridRows.Where(i => i.Id == stock.StockItemId).FirstOrDefault();
                    if (item != null)
                    {
                        item.Availibility += stock.AvailablePieces;
                        item.Availibility -= stock.ReservedPieces;
                        if (item.Availibility < 0)
                        {
                            item.Availibility = 0;
                        }
                    }
                }
                var missing = gridRows.Where(i => i.Availibility == 0).ToList();
                view.grid.DataContext = missing;
            }
        }
    }
}
