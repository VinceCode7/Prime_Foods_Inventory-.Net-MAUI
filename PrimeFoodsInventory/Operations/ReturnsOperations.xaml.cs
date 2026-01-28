using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.Operations;

public partial class ReturnsOperations : ContentPage
{
	public ReturnsOperations()
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
        string retrieveQuery3 = "SELECT * FROM ReturnedProducts WHERE DateRecorded BETWEEN @StartDate AND @EndDate;";

        if (RejectRDB.IsChecked)
        {
            retrieveQuery3 = "SELECT * FROM ReturnedProducts WHERE ReturnType='REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate;";
        }

        if (NOTRejectRDB.IsChecked)
        {
            retrieveQuery3 = "SELECT * FROM ReturnedProducts WHERE ReturnType='NOT REJECT' AND DateRecorded BETWEEN @StartDate AND @EndDate;";
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
                                Quantity = reader.GetDecimal(2),
                                UnitOfMeasurement = reader.GetString(3),
                                ReturnedBy = reader.GetString(4),
                                ReturnedTo = reader.GetString(5),
                                ReturnType=reader.GetString(6),
                                date = reader.GetDateTime(7),
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
        public string ReturnedBy { get; set; }
        public string ReturnedTo { get; set; }
        public string ReturnType { get; set; }
        public DateTime date { get; set; }
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