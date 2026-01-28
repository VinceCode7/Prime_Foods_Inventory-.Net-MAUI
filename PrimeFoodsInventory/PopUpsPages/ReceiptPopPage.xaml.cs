
using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.PopUpsPages;

public partial class ReceiptPopPage : ContentPage
{
    byte[] _imageByte;

    public ReceiptPopPage(byte[] imageByte)
	{
		InitializeComponent();
        _imageByte = imageByte;
        RetrieveRecords();

    }

    private async Task RetrieveRecords()
    {
        try
        {
            SpecifidReceiptImage.Source = ImageSource.FromStream(() => new MemoryStream(_imageByte));

            List<Items> ItemsInventoryList = new List<Items>();
            string retrieveQuery3 = "SELECT ItemName,ItemCategory,Quantity,UnitOfMeasurement,SupplierName,ItemPrice,DateRecorded FROM InventoryIncoming WHERE ReceiptData=@ReceiptData;";

            using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(retrieveQuery3, connection))
                {
                    command.Parameters.AddWithValue("@ReceiptData", _imageByte);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ItemsInventoryList.Add(new Items
                            {
                                ItemName = reader.GetString(0),
                                ItemCategory = reader.GetString(1),
                                Quantity = reader.GetDecimal(2),
                                UnitOfMeasurement = reader.GetString(3),
                                SupplierName = reader.GetString(4),
                                ItemPrice = reader.GetDecimal(5),
                                date = reader.GetDateTime(6),
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
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string SupplierName { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime date { get; set; }
    }

   
}