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

namespace ShopCosmetic
{
    /// <summary>
    /// Логика взаимодействия для ThanksWindow.xaml
    /// </summary>
    public partial class ThanksWindow : Window
    {
        public ThanksWindow(Order ord)
        {
            InitializeComponent();
            string message;
            if (!ord.delivery)
                message = "Заказ будет собран в течение дня.";
            else if(ord.dateHand==null || ord.dateHand==Convert.ToDateTime("01.01.2001"))
                message = "Заказ будет доставлен после поступления продукции на склад.";
            else
                message = $"Заказ будет доставлен {ord.dateHand.Value.ToString("dd.MM.yyyy")}.";
            thank.Text = $"Ваш заказ успешно принят!\n Чек отправлен на почту, которую вы указали.\n{message}\n Будем ждать Вас снова!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
