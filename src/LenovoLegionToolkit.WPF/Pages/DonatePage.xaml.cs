using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class DonatePage
{
    public DonatePage()
    {
        InitializeComponent();
    }

    private void PayPalDonateButton_Click(object sender, RoutedEventArgs e)
    {
        Constants.DonateUri.Open();
        e.Handled = true;
    }
}
