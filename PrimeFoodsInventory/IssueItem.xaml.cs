using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory;

public partial class IssueItem : ContentPage
{
    decimal ProductAmount = 0;
	public IssueItem()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        List<string> CategoryList = new List<string>(); // a list to hold product names
        CategoryList.Clear();
        string retrieveQuery = "Select ItemCategory from ItemStock GROUP BY ItemCategory";

        using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand(retrieveQuery, connection))
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                // Read each row asynchronously
                while (await reader.ReadAsync())
                {
                    string productName = reader.GetString(0); // first column
                    CategoryList.Add(productName);
                }
            }
        }


        string[] productArray = CategoryList.ToArray();
        CategoryPicker.Items.Clear();
        foreach (var product in productArray)
        {
            CategoryPicker.Items.Add(product);
        }
    }

    private async void GivenBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(GivenToEntry.Text?.ToString().Trim()))
            {
                await DisplayAlert("ERROR", "Please enter the Person Given", "OK");
                return;
            }

            if (String.IsNullOrEmpty(GivenByEntry.Text?.ToString().Trim()))
            {
                await DisplayAlert("ERROR", "Please enter the Person Giving", "OK");
                return;
            }

            if (!String.IsNullOrEmpty(quatityEntry.Text?.ToString().Trim()) && decimal.TryParse(quatityEntry.Text?.ToString().Trim(), out decimal number))
            {

            }
            else
            {
                await DisplayAlert("ERROR", "Please ensure the quantity field is NUMBERS only\nand not Empty", "OK");
                return;

            }

            string ProductCategory = CategoryPicker.SelectedItem?.ToString();
            string ProductName = ProductPicker.SelectedItem?.ToString();
            decimal Quantity = Convert.ToDecimal(quatityEntry.Text?.ToString().Trim());
            string GivenTo = GivenToEntry.Text?.ToString().Trim();
            string GivenBy = GivenByEntry.Text?.ToString().Trim();
            string UnitOfMeasurement = UnitOfMeasurementLbl.Text?.ToString();

            if (ProductAmount < Quantity)
            {
                await DisplayAlert("Error","The quantity entered\nis greater than the one in the store","ok");
                return;
            }
            string insertQuery = "Insert into InventoryOutGoing(ItemName,ItemCategory,Quantity,UnitOfMeasurement,GivenTo,GivenBy,DateRecorded) values(@ItemName,@ItemCategory,@Quantity,@UnitOfMeasurement,@GivenTo,@GivenBy,GETDATE());" +
                "\nupdate ItemStock Set Quantity=Quantity-@Quantity where ItemName=@ItemName and ItemCategory=@ItemCategory;";

            using (var conn = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@ItemName", ProductName);
                cmd.Parameters.AddWithValue("@ItemCategory", ProductCategory);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@UnitOfMeasurement", UnitOfMeasurement);
                cmd.Parameters.AddWithValue("@GivenTo", GivenTo);
                cmd.Parameters.AddWithValue("@GivenBy", GivenBy);

                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();

            }

            await DisplayAlert("Success", "Submited Successfully", "OK");
        }
        catch (Exception ex)
        {
          await  DisplayAlert("Error", ex.Message, "OK");
        }

    }

    private async void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<string> productList = new List<string>(); // a list to hold product names


        try
        {
            string ItemCategory = CategoryPicker.SelectedItem?.ToString();

            if (ItemCategory != null)
            {

            }
            else
            {
                return;
            }


            ProductPicker.Items.Clear();
            productList.Clear();
            string retrieveQuery = "Select ItemName from ItemStock WHERE ItemCategory=@ItemCategory;";

            using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(retrieveQuery, connection))
                {
                    command.Parameters.AddWithValue("@ItemCategory", ItemCategory);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // Read each row asynchronously
                        while (await reader.ReadAsync())
                        {
                            string productName = reader.GetString(0); // first column
                            productList.Add(productName);
                        }

                    }
                }

                await connection.CloseAsync();

            }

            string[] productArray = productList.ToArray();

            foreach (var product in productArray)
            {

                ProductPicker.Items.Add(product);
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void ProductPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string ProductName = ProductPicker.SelectedItem?.ToString();
        string ItemCategory = ProductPicker.SelectedItem?.ToString();
        string retrieveQuery = "Select UnitOfMeasurement,Quantity from ItemStock WHERE ItemName=@ProductName;";

        if (ProductName != null && ItemCategory != null)
        {
        }
        else
        {
            return;
        }

        using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand(retrieveQuery, connection))
            {
                command.Parameters.AddWithValue("@ProductName", ProductName);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // Read each row asynchronously
                    while (await reader.ReadAsync())
                    {
                        string productName = reader.GetString(0); // first column
                        ProductAmount=reader.GetDecimal(1);
                        UnitOfMeasurementLbl.Text = productName;
                    }
                    await connection.CloseAsync();
                }
            }
        }
        GivenBtn.IsEnabled=true;
    }
}