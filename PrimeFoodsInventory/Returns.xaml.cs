using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory;

public partial class Returns : ContentPage
{
	public Returns()
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

    private async void SubmitBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(ReturnedByEntry.Text?.ToString().Trim()))
            {
                await DisplayAlert("ERROR", "Please enter the Person Returning the Products", "OK");
                return;
            }

            if (String.IsNullOrEmpty(ReturnedToEntry.Text?.ToString().Trim()))
            {
                await DisplayAlert("ERROR", "Please enter the Person Receiving the Product", "OK");
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
            string ReturnedBy = ReturnedByEntry.Text?.ToString().Trim();
            string ReturnedTo = ReturnedToEntry.Text?.ToString().Trim();
            string UnitOfMeasurement = UnitOfMeasurementLbl.Text?.ToString();
            string ReturnType = "";
            string insertQuery = "";

            if (RejectRDB.IsChecked){

                ReturnType = "REJECT";
                insertQuery = "Insert into ReturnedProducts(ItemName,ItemCategory,Quantity,UnitOfMeasurement,ReturnedBy,ReturnedTo,ReturnType,DateRecorded) values(@ItemName,@ItemCategory,@Quantity,@UnitOfMeasurement,@ReturnedBy,@ReturnedTo,@ReturnType,GETDATE());";
            }
            if (NotRejectRDB.IsChecked) {
                    ReturnType = "NOT REJECT";
                    insertQuery = "Insert into ReturnedProducts(ItemName,ItemCategory,Quantity,UnitOfMeasurement,ReturnedBy,ReturnedTo,ReturnType,DateRecorded) values(@ItemName,@ItemCategory,@Quantity,@UnitOfMeasurement,@ReturnedBy,@ReturnedTo,@ReturnType,GETDATE());" +
                    "\nupdate ItemStock Set Quantity=Quantity+@Quantity where ItemName=@ItemName and ItemCategory=@ItemCategory;";
            }

            using (var conn = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@ItemName", ProductName);
                cmd.Parameters.AddWithValue("@ItemCategory", ProductCategory);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@UnitOfMeasurement", UnitOfMeasurement);
                cmd.Parameters.AddWithValue("@ReturnedBy", ReturnedBy);
                cmd.Parameters.AddWithValue("@ReturnedTo", ReturnedTo);
                cmd.Parameters.AddWithValue("@ReturnType", ReturnType);

                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();

            }

            await DisplayAlert("Success", "Submited Successfully", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
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
        string retrieveQuery = "Select UnitOfMeasurement from ItemStock WHERE ItemName=@ProductName;";

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
                        UnitOfMeasurementLbl.Text = productName;
                    }
                    await connection.CloseAsync();
                }
            }
        }
        SubmitBtn.IsEnabled = true;

    }
}