using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory;

public partial class AddNewItem : ContentPage
{
	public AddNewItem()
	{
		InitializeComponent();
	}

    private async void AddNewItemBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ItemNameEntry.Text?.Trim()))
            {
  await DisplayAlert("ERROR", "Please enter the product name", "OK");
                return; 
            }
            else {           
            }

            if (string.IsNullOrEmpty(ItemCategoryEntry.Text?.Trim()))
            {  await DisplayAlert("ERROR", "Please enter the product Category", "OK");
                return;
            }
            else
            {             
            }

            if (string.IsNullOrEmpty(ItemUnitMeasurementEntry.Text?.Trim()))
            { await DisplayAlert("ERROR", "Please enter the Unit of measurement", "OK");
                return;
            }
            else
            {              
            }

            if (string.IsNullOrEmpty(ItemDescriptionEntry.Text?.Trim()))
            {   await DisplayAlert("ERROR", "Please enter the Product Description", "OK");
                return;
            }
            else
            {
            }

            string ItemName = ItemNameEntry.Text?.Trim();
            string ItemCategory = ItemCategoryEntry.Text?.Trim();
            string UnitOfMeasurement = ItemUnitMeasurementEntry.Text?.Trim();
            string ItemDescription = ItemDescriptionEntry.Text?.Trim();

            string insertQuery = "Insert into ItemStock(ItemName,ItemCategory,UnitOfMeasurement,ItemDescription) values(@ItemName,@ItemCategory,@UnitOfMeasurement,@ItemDescription)";
            using (var conn = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@ItemName", ItemName);
                cmd.Parameters.AddWithValue("@ItemCategory", ItemCategory);
                cmd.Parameters.AddWithValue("@UnitOfMeasurement", UnitOfMeasurement);
                cmd.Parameters.AddWithValue("@ItemDescription", ItemDescription);

                await cmd.ExecuteNonQueryAsync();
                await DisplayAlert("Success", "Recorded Successfully", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}