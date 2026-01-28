using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.Reports;

public partial class DailyReturns : ContentPage
{
	public DailyReturns()
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
        string retrieveQuery3 = "SELECT ItemName,ItemCategory,DateRecorded,UnitOfMeasurement,SUM(Quantity) AS DailyTotal FROM ReturnedProducts WHERE DateRecorded BETWEEN @StartDate AND @EndDate GROUP BY ItemName,ItemCategory,UnitOfMeasurement,DateRecorded ORDER BY DateRecorded, ItemName;\r\n";

        if (RejectRDB.IsChecked) {
            retrieveQuery3 = "SELECT ItemName,ItemCategory,DateRecorded,UnitOfMeasurement,SUM(Quantity) AS DailyTotal FROM ReturnedProducts WHERE ReturnType='REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate GROUP BY ItemName,ItemCategory,UnitOfMeasurement,DateRecorded ORDER BY DateRecorded, ItemName;\r\n";
        }

        if (NOTRejectRDB.IsChecked) {
            retrieveQuery3 = "SELECT ItemName,ItemCategory,DateRecorded,UnitOfMeasurement,SUM(Quantity) AS DailyTotal FROM ReturnedProducts WHERE ReturnType='NOT REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate GROUP BY ItemName,ItemCategory,UnitOfMeasurement,DateRecorded ORDER BY DateRecorded, ItemName;\r\n";
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
                                date = reader.GetDateTime(2),
                                UnitOfMeasurement = reader.GetString(3),
                                Quantity = reader.GetDecimal(4),
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
        public string UnitOfMeasurement { get; set; }
        public DateTime date { get; set; }
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