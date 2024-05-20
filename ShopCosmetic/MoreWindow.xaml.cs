using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
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
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace ShopCosmetic
{
    /// <summary>
    /// Логика взаимодействия для More.xaml
    /// </summary>
    public partial class More : Window
    {
        public More(Product1 product, List<Basket> listBas)
        {
            InitializeComponent();
            _currentProduct = product;
            DataContext = _currentProduct;
            _listBas= listBas;
        }
        Product1 _currentProduct = new Product1();
        public static List<Basket> _listBas;

        private void Minus(object sender, RoutedEventArgs e)
        {
            if (kolvo >= 1)
            {
                kolvo--;
                Max.IsEnabled = true;
                Kolvo.Text = kolvo.ToString();
                if (kolvo == 1)
                    Min.IsEnabled = false;
                Max.IsEnabled = true;
            }
        }
        int kolvo = 1;
        private void Plus(object sender, RoutedEventArgs e)
        {
            if (kolvo < 100)
            {
                kolvo++;
                Min.IsEnabled = true;
                Kolvo.Text = kolvo.ToString();
                if (kolvo == 100)
                    Max.IsEnabled = false;
                Min.IsEnabled = true;
            }
        }

        private void InBag(object sender, RoutedEventArgs e)
        {
            var prod = Cosmetics.GetContext().Product.Where(x => x.id == _currentProduct.id).FirstOrDefault();
            if (_listBas != null) //если корзина не пустая
                {
                
                    Basket a = _listBas.Where(x => x.Product1 == prod).FirstOrDefault();
                    if (a != null) //есть ли такой продукт уже в корзине 
                    {
                        a.amount += kolvo;
                        MessageBox.Show($"Товар в количестве {kolvo} шт. успешно обновлен в корзине", "Окно подробной информации");
                    }
                    else
                    {
                        decimal Price = _currentProduct.promotion ? _currentProduct.price * (1 - ((decimal)_currentProduct.promotionPercent / 100)) : _currentProduct.price;
                        var b = new Basket
                        {
                            Product1 = prod,
                            amount = kolvo,
                            finalPrice = Price,
                        };
                        _listBas.Add(b);
                        MessageBox.Show($"Товар в количестве {kolvo} шт. успешно добавлен в корзину", "Окно подробной информации");
                    }
                }
                else //корзина пустая
                {
                    decimal Price = _currentProduct.promotion ? _currentProduct.price * (1 - ((decimal)_currentProduct.promotionPercent / 100)) : _currentProduct.price;
                    var b = new Basket
                    {
                        Product1 = prod,
                        amount = kolvo,
                        finalPrice = Price,
                    };
                    _listBas = new List<Basket>{b};
                    MessageBox.Show($"Товар в количестве {kolvo} шт. успешно добавлен в корзину", "Окно подробной информации");
                }
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            MainClientWindow mainClientWindow = new MainClientWindow(_listBas, null);
            mainClientWindow.Activate();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainClientWindow mainClientWindow = new MainClientWindow(_listBas, null);
            mainClientWindow.Activate();
        }
    }
}
