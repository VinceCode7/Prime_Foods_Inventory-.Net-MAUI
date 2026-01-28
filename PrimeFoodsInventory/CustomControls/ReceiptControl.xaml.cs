using PrimeFoodsInventory.PopUpsPages;

namespace PrimeFoodsInventory.CustomControls;

public partial class ReceiptControl : ContentView
{
	byte[] _imageByte;
	DateTime _date;
	decimal _totalAmount;
	string _supplierName;
    public ReceiptControl(byte[] imageByte,DateTime date,decimal totalAmount,string supplierName)
	{
		InitializeComponent();

        _imageByte = imageByte; 
		_date=date;
		_totalAmount=totalAmount;
		_supplierName=supplierName;
		 ValueToControl();
	}

	private async  Task ValueToControl()
	{
        DateLbl.Text = _date.ToString();	
          

          TotalLbl.Text = _totalAmount.ToString();
            SupplierLbl.Text = _supplierName;
		ReceiptImage.Source = ImageSource.FromStream(() => new MemoryStream(_imageByte));
    }

    private async void ViewBtn_Clicked(object sender, EventArgs e)
    {
		var receiptpopup = new ReceiptPopPage(_imageByte);
		await Navigation.PushAsync(receiptpopup);
    }
}