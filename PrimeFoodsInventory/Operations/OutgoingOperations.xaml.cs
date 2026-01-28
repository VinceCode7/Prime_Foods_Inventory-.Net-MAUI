using Microsoft.Data.SqlClient;

namespace PrimeFoodsInventory.Operations;

public partial class OutgoingOperations : ContentPage
{
	public OutgoingOperations()
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
        try
        {
            List<Items> ItemsInventoryList = new List<Items>();
            string retrieveQuery3 = "SELECT ItemName,ItemCategory,Quantity,UnitOfMeasurement,GivenTo,GivenBy,DateRecorded FROM InventoryOutGoing WHERE DateRecorded BETWEEN @StartDate AND @EndDate;";

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
                                GivenTo = reader.GetString(4),
                                GivenBy = reader.GetString(5),
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
        public string GivenTo { get; set; }
        public string GivenBy { get; set; }
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