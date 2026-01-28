//using Java.Lang;
using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory;

public partial class Inventory : ContentPage
{
	public Inventory()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
       await ItemsInventoryAsync();
    }
    private async Task ItemsInventoryAsync()
    {
        try
        {
            List<Items> ItemsInventoryList = new List<Items>();
            string retrieveQuery3 = "Select * from ItemStock;";

            using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(retrieveQuery3, connection))
                {

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ItemsInventoryList.Add(new Items
                            {
                                ItemId = reader.GetInt32(0),
                                ItemName = reader.GetString(1),
                                ItemCategory = reader.GetString(2),
                                UnitOfMeasurement= reader.GetString(3),
                                Quantity = reader.GetDecimal(4),
                                LastPurchaseDate = reader.GetDateTime(5),
                                ItemDescription = reader.GetString(6),
                            });
                        }
                        await connection.CloseAsync();
                        ItemsInventory.ItemsSource = ItemsInventoryList;

                    }
                }
            }


        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    public class Items
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
        public DateTime LastPurchaseDate { get; set; }
        public string ItemDescription { get; set; }
    }
}