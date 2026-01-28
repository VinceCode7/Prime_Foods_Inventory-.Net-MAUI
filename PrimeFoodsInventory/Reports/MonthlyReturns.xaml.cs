using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.Reports;

public partial class MonthlyReturns : ContentPage
{
	public MonthlyReturns()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        EndDateEntry.Text = Convert.ToString(EndDatePicker.Date.ToString()?.Trim());
        StartDateEntry.Text = Convert.ToString(StartDatePicker.Date.ToString()?.Trim());

    }

    private async Task ItemsInventoryAsync()
    {

        DateTime StartDate = Convert.ToDateTime(StartDateEntry.Text?.Trim());
        DateTime EndDate = Convert.ToDateTime(EndDateEntry.Text?.Trim());
        string retrieveQuery3 = "SELECT \r\n    ItemName,\r\n    ItemCategory,\r\nDATEPART(YEAR, DateRecorded) AS Year,\r\n    DATEPART(MONTH, DateRecorded) AS MonthNumber,\r\n\tUnitOfMeasurement,\r\n    SUM(Quantity) AS WeekTotal\r\nFROM ReturnedProducts \r\n\r\nWHERE DateRecorded BETWEEN @StartDate AND @EndDate\r\nGROUP BY \r\n    ItemName,\r\n    ItemCategory,\r\n\tDATEPART(YEAR, DateRecorded) ,\r\n   DATEPART(MONTH, DateRecorded) ,\r\n\tUnitOfMeasurement\r\nORDER BY \r\n   Year, MonthNumber, ItemName;";
        // ReturnType='REJECT' AND 
        if (RejectRDB.IsChecked)
        {
            retrieveQuery3 = "SELECT \r\n    ItemName,\r\n    ItemCategory,\r\nDATEPART(YEAR, DateRecorded) AS Year,\r\n    DATEPART(MONTH, DateRecorded) AS MonthNumber,\r\n\tUnitOfMeasurement,\r\n    SUM(Quantity) AS WeekTotal\r\nFROM ReturnedProducts \r\n\r\nWHERE ReturnType='REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate\r\nGROUP BY \r\n    ItemName,\r\n    ItemCategory,\r\n\tDATEPART(YEAR, DateRecorded) ,\r\n   DATEPART(MONTH, DateRecorded) ,\r\n\tUnitOfMeasurement\r\nORDER BY \r\n   Year, MonthNumber, ItemName;";
        }

        if (NOTRejectRDB.IsChecked)
        {
            retrieveQuery3 = "SELECT \r\n    ItemName,\r\n    ItemCategory,\r\nDATEPART(YEAR, DateRecorded) AS Year,\r\n    DATEPART(MONTH, DateRecorded) AS MonthNumber,\r\n\tUnitOfMeasurement,\r\n    SUM(Quantity) AS WeekTotal\r\nFROM ReturnedProducts \r\n\r\nWHERE ReturnType='NOT REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate\r\nGROUP BY \r\n    ItemName,\r\n    ItemCategory,\r\n\tDATEPART(YEAR, DateRecorded) ,\r\n   DATEPART(MONTH, DateRecorded) ,\r\n\tUnitOfMeasurement\r\nORDER BY \r\n   Year, MonthNumber, ItemName;";
        }


        try
        {
            List<Items> ItemsInventoryList = new List<Items>();

            using (SqlConnection connection = new SqlConnection("Data Source=YOUNGPENTESTER;Initial Catalog=PrimeFoods;Integrated Security=True;Trust Server Certificate=True"))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(retrieveQuery3, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ItemsInventoryList.Add(new Items
                            {
                                ItemName = reader.GetString(0),
                                ItemCategory = reader.GetString(1),
                                Year = reader.GetInt32(2),
                                MonthNumber = reader.GetInt32(3),
                                UnitOfMeasurement = reader.GetString(4),
                                Quantity = reader.GetDecimal(5),
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
        public int Year { get; set; }
        public int MonthNumber { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
    }

    private async void RetrieveUsageBtn_Clicked(object sender, EventArgs e)
    {
        await ItemsInventoryAsync();

    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (Convert.ToString(StartDatePicker.ToString()?.Trim()) != null)
        {
            StartDateEntry.Text = Convert.ToString(StartDatePicker.Date.ToString()?.Trim());
        }
        else
        {
            return;
        }
    }

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (Convert.ToString(EndDatePicker.ToString()?.Trim()) != null)
        {
            EndDateEntry.Text = Convert.ToString(EndDatePicker.Date.ToString()?.Trim());
        }
        else
        {
            return;
        }
    }
}