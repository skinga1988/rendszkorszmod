using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;
using WPFClient.Utilities;
using WPFClient.View;

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        public ObservableCollection<ProductListGridRow> gridRows { get; set; }

        public async Task GetProductList(Technician_ListItems_view grid)
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
                foreach ( var item in sortedItems )
                {
                    gridRows.Add(new ProductListGridRow()
                    {
                        Id = item.Id,
                        Name = item.ItemType,
                        Price = item.ItemPrice,
                        Availibility = 0,
                        IsSelected = false,
                        Count = 0
                    });
                }
                // Get availibility
                var responseStock = await RestHelper.GetRestClient().GetAsync("api/Stock");
                var contentStock = await responseStock.Content.ReadAsStringAsync();
                if (!responseStock.IsSuccessStatusCode)
                {
                    return;
                }
                var stocks = JsonConvert.DeserializeObject<List<Stock_model>>(contentStock);
                foreach (var stock in stocks)
                {
                    var item = gridRows.Where(i => i.Id == stock.StockItemId).FirstOrDefault();
                    if (item != null)
                    {
                        item.Availibility += stock.AvailablePieces;
                        item.Availibility -= stock.ReservedPieces;
                    }
                }
                grid.DataContext = gridRows;
            }
        }
    }
}
