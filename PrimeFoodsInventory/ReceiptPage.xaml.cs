using Microsoft.Data.SqlClient;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace PrimeFoodsInventory;

public partial class ReceiptPage : ContentPage
{
    public ReceiptPage()
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
        byte[] receiptData;
        DateTime date;
        decimal TotalAmount = 0;
        string SupplierName = "";
        ReceiptLayout.Children.Clear();


      //  List<IncomingInfo> receiptInfo = new List<IncomingInfo>();

        DateTime StartDate = Convert.ToDateTime(StartDateEntry.Text?.Trim());
        DateTime EndDate = Convert.ToDateTime(EndDateEntry.Text?.Trim());
        try
        {
            string retrieveQuery3 = "select DateRecorded,ReceiptData,sum(ItemPrice) as TotalCost,SupplierName  from InventoryIncoming where DateRecorded Between @StartDate and @EndDate group by DateRecorded,ReceiptData,SupplierName;";

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

                            date = reader.GetDateTime(0);
                            receiptData = reader["ReceiptData"] as byte[];
                            TotalAmount = reader.GetDecimal(2);
                            SupplierName = reader.GetString(3);

                            CustomControls.ReceiptControl receiptControl = new CustomControls.ReceiptControl(receiptData, date, TotalAmount, SupplierName);
                            ReceiptLayout.Children.Add(receiptControl);
                        }
                        await connection.CloseAsync();

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