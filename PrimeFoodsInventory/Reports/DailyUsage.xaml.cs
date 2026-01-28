//using Android.Widget;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.Reports;

public partial class DailyUsage : ContentPage
{
	public DailyUsage()
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
        if (!string.IsNullOrEmpty(StartDateEntry.Text?.Trim()) &&DateTime.TryParse(StartDateEntry.Text?.Trim(), out DateTime start)) {
        } else {
            await DisplayAlert("ERROR", "Please enter the correct date format for the start Date", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(EndDateEntry.Text?.Trim()) && DateTime.TryParse(EndDateEntry.Text?.Trim(), out DateTime end))
        {
        }
        else
        {
            await DisplayAlert("ERROR", "Please enter the correct date format for the end Date", "OK");
            return;

        }


        DateTime StartDate = Convert.ToDateTime(StartDateEntry.Text?.Trim());
        DateTime EndDate = Convert.ToDateTime(EndDateEntry.Text?.Trim());
        try
        {
            List<Items> ItemsInventoryList = new List<Items>();
            string retrieveQuery3 = "SELECT \r\n    o.ItemName,\r\n    o.ItemCategory,\r\n\to.UnitOfMeasurement,\r\n    o.DateRecorded,\r\n    SUM(o.Quantity) - ISNULL(SUM(r.Quantity), 0) AS DailyUsage\r\nFROM InventoryOutGoing o\r\nLEFT JOIN ReturnedProducts r\r\n    ON o.ItemName = r.ItemName\r\n    AND o.ItemCategory = r.ItemCategory\r\n    AND o.DateRecorded = r.DateRecorded\r\n    AND r.ReturnType = 'NOT REJECT'\r\nWHERE o.DateRecorded BETWEEN @StartDate AND @EndDate\r\nGROUP BY \r\n    o.ItemName,\r\n\to.UnitOfMeasurement,\r\n    o.ItemCategory,\r\n    o.DateRecorded\r\nORDER BY \r\n    o.DateRecorded, o.ItemName;\r\n";

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
                                UnitOfMeasurement = reader.GetString(2),
                                date = reader.GetDateTime(3),
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
            return;
        }
        //CancellationTokenSource cts = new CancellationTokenSource();
        //string text = "Successfull Retrieved";
        //ToastDuration duration = ToastDuration.Short;
        //double textSize = 14;
        //var toast = Toast.Make(text, duration, textSize);
        //await toast.Show(cts.Token);

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
        StartDateEntry.Text =Convert.ToString(StartDatePicker.Date.ToString()?.Trim());
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