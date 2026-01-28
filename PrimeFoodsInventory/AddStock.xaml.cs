using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
//using static Android.Renderscripts.ScriptGroup;

namespace PrimeFoodsInventory;

public partial class AddStock : ContentPage
{
    byte[] fileBytes;


    public AddStock()
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


    private async void AddStockBtn_Clicked(object sender, EventArgs e)
    {

        try
        {
          

            if (String.IsNullOrEmpty(supplierEntry.Text?.ToString().Trim()))
            {
                await DisplayAlert("ERROR", "Please fill the Supplier Name", "OK");
                return;
            }

            if (!String.IsNullOrEmpty(quatityEntry.Text?.ToString().Trim()) &&decimal.TryParse(quatityEntry.Text?.ToString().Trim(), out decimal  number))
            {
               
            }
            else
            {
                await DisplayAlert("ERROR", "Please ensure the quantity field is NUMBERS only", "OK");
                return;

            }

            if (!String.IsNullOrEmpty(priceEntry.Text?.ToString()) && decimal.TryParse(priceEntry.Text?.ToString(), out decimal number2))
            {

            }
            else
            {
                await DisplayAlert("ERROR", "Please ensure the Price field is NUMBERS only", "OK");
                return;

            }

                if (fileBytes != null)
                {
                await DisplayAlert("ERROR", "Please Upload the receipt", "OK");
                return;
            }
            string ProductCategory = CategoryPicker.SelectedItem?.ToString();
            string ProductName = ProductPicker.SelectedItem?.ToString();
            decimal Quantity = Convert.ToDecimal(quatityEntry.Text?.ToString().Trim());
            string SupplierName = supplierEntry.Text?.ToString().Trim();
            decimal ProductPrice = Convert.ToDecimal(priceEntry.Text?.ToString());
            string UnitOfMeasurement= UnitOfMeasurementLbl.Text?.ToString();
           
          

            string insertQuery = "Insert into InventoryIncoming(ItemName,ItemCategory,Quantity,UnitOfMeasurement,SupplierName,ItemPrice,DateRecorded,ReceiptData) values(@ItemName,@ItemCategory,@Quantity,@UnitOfMeasurement,@SupplierName,@ItemPrice,GETDATE(),@ReceiptData);" +
                "\nupdate ItemStock Set Quantity=Quantity+@Quantity,LastPurchaseDate=GetDate() where ItemName=@ItemName and ItemCategory=@ItemCategory;";


            using (var conn = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@ItemName", ProductName);
                cmd.Parameters.AddWithValue("@ItemCategory", ProductCategory);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@UnitOfMeasurement", UnitOfMeasurement);
                cmd.Parameters.AddWithValue("@SupplierName", SupplierName);
                cmd.Parameters.AddWithValue("@ItemPrice", ProductPrice);
                cmd.Parameters.AddWithValue("@ReceiptData", fileBytes);

                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();

            }

            await DisplayAlert("Success", "Added Successfully", "OK");
        }
        catch (Exception ex)
        {
          await  DisplayAlert("Error", ex.Message, "OK");
        }


    }

    private async void UploadBtn_Clicked(object sender, EventArgs e)
    {
        var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
 {
     { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } }, // iOS identifiers
     { DevicePlatform.Android, new[] { "image/*", "application/pdf" } }, // Android MIME types
     { DevicePlatform.WinUI, new[] { ".jpg", ".png", ".pdf" } }, // Windows extensions
     { DevicePlatform.MacCatalyst, new[] { "public.image", "com.adobe.pdf" } } // Mac identifiers
 });

        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a receipt or invoice",
            FileTypes = customFileTypes
        });

        if (result != null)
        {
            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            fileBytes = ms.ToArray();

            string filePath = result.FullPath;
            ReceiptImage.Source = ImageSource.FromStream(() => new MemoryStream(fileBytes));
            //ReceiptImage.Source = fileBytes;
            await DisplayAlert("File Selected", $"You picked: {filePath}", "OK");

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

                using (SqlCommand command = new SqlCommand(retrieveQuery, connection)) { 
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
          await  DisplayAlert("Error", ex.Message, "OK");
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

        addStockBtn.IsEnabled = true;
    }
}