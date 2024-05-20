using ShopCosmetic;
using System.Windows;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для MoreProdWindow.xaml
    /// </summary>
    public partial class MoreProdWindow : Window
    {
        public MoreProdWindow(Product1 product)
        {
            InitializeComponent();
            DataContext = product;
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            ClientsWindow mainClientWindow = new ClientsWindow(null);
            mainClientWindow.Activate();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClientsWindow mainClientWindow = new ClientsWindow(null);
            mainClientWindow.Activate();
        }
    }
}
