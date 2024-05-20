using ShopCosmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для MoreOrderWindow.xaml
    /// </summary>
    public partial class MoreOrderWindow : Window
    {
        public MoreOrderWindow(Order1 ord)
        {
            InitializeComponent();
            DataContext = ord;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var basket in ord.Basket)
                stringBuilder.AppendLine($"{basket.Product1.name} - {basket.amount} шт.*{Math.Round(basket.finalPrice,2)} руб. = {Math.Round(basket.amount * basket.finalPrice,2)} руб.\n");
            Products.Text = $"---Продукты в заказе---\n\n{stringBuilder}--------------------------";
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
