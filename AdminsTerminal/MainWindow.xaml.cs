using ShopCosmetic;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Collections.Generic;
namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public MainWindow(bool contextP)
        {
            InitializeComponent();
            if (!contextP)
                Cosmetics.context = null;
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            LViewProducts.ItemsSource = products.ListProd(Cosmetics.GetContext().Product.ToList());
            Redact.IsEnabled = false;
            Redact.Foreground = Brushes.Black;
        }
        ProductsListView products = new ProductsListView();

        private void Red(object sender, RoutedEventArgs e)
        {
            EditWindow addEdit = new EditWindow(product, true);
            addEdit.Show();
            Close();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            EditWindow addEdit = new EditWindow(null, true);
            addEdit.Show();
            Close();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var productsForRemoving = LViewProducts.SelectedItems.Cast<Product1>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {productsForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Product pr = new Product();
                foreach (var p in productsForRemoving)
                {
                    pr = Cosmetics.GetContext().Product.Where(x => x.id == p.id).SingleOrDefault();
                    Cosmetics.GetContext().Product.Remove(pr);
                }
                Cosmetics.GetContext().SaveChanges();
                MessageBox.Show("Данные удалены");
                LViewProducts.ItemsSource = products.ListProd(Cosmetics.GetContext().Product.ToList());
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            autorizAdmin autoriz = new autorizAdmin();
            autoriz.Show();
            Close();
        }

        Product product;
        bool vibor = false;
        private void Dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var prod = LViewProducts.SelectedItem as Product1;
            if (prod != null)
            {
                product = Cosmetics.GetContext().Product.Where(x => x.id == prod.id).FirstOrDefault();
                if (LViewProducts.SelectedItems.Count > 1 || LViewProducts.SelectedItems.Count == 0)
                {
                    CloseSclad();
                }
                else
                {
                    Redact.IsEnabled = true;
                    Redact.Foreground = Brushes.White;
                    Pack.Visibility = Visibility.Visible;
                    Kolvo.Text = "На складе: " + product.sclad + " шт.";
                    KolvoInPack.Text = "В пачке: " + product.pack + " шт.";
                    vibor = true;
                }
            }
            else
            {
                CloseSclad();
            }
        }
        private void CloseSclad()
        {
            Redact.IsEnabled = false;
            Redact.Foreground = Brushes.Black;
            Pack.Visibility = Visibility.Collapsed;
            vibor = false;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentProd = Cosmetics.GetContext().Product.ToList();
            currentProd = currentProd.Where(p => p.name.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            LViewProducts.ItemsSource = products.ListProd(currentProd);
        }

        private void Otchets(object sender, RoutedEventArgs e)
        {
            OtchetsWindow otchetsWindow = new OtchetsWindow();
            otchetsWindow.Show();
            Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow();
            ordersWindow.Show();
            Close();
        }

        private void ClientInterface(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow(null);
            clientsWindow.Show();
            Close();
        }

        private void AddPacks(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                bool messa =false;
                var baskets = Cosmetics.GetContext().Basket.Where(x => x.product == product.id && x.lack > 0).ToList();
                product.sclad += product.pack * kolvo;
                foreach (var basket in baskets)
                    if (product.sclad >= basket.lack)
                    {
                        product.sclad -= basket.lack;
                        basket.lack = 0;
                        Cosmetics.GetContext().SaveChanges();
                        if (Cosmetics.GetContext().Basket.Where(x => x.lack > 0 && x.numberOrder == basket.numberOrder).ToList().Count >= 1)
                            CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, product.name, Polniy: false);
                        else
                            CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, product.name, Polniy: true);
                        messa = true;
                    }
                    else
                        break;
                if (messa)
                    MessageBox.Show("Оповещение о доставке продуктов на склад было отправлено клиентам\n");
                Cosmetics.GetContext().SaveChanges();
                Kolvo.Text = "На складе: " + product.sclad + " шт.";
            }
        }

        public static void CreateTimeoutTestMessage(Basket basket, string email, string product, bool Polniy)
        {
            SmtpClient mySmtpClient = new SmtpClient("smtp.mail.ru");
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = true;//465 
            System.Net.NetworkCredential basicAuthenticationInfo = new
               System.Net.NetworkCredential("glazkovamaria5898@mail.ru", "Fkn0ma9RiANzNPjNQetB");
            mySmtpClient.Credentials = basicAuthenticationInfo;
            MailAddress from = new MailAddress("glazkovamaria5898@mail.ru", "GracefulShine");
            MailAddress to = new MailAddress(email);
            MailMessage myMail = new MailMessage(from, to);
            MailAddress replyTo = new MailAddress("glazkovamaria5898@mail.ru");
            Order order = Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).Single();
            myMail.ReplyToList.Add(replyTo);
            myMail.Subject = "Доставка товаров на склад. Заказ №" +order.idOrder;
            myMail.SubjectEncoding = Encoding.UTF8;
            
            var baskets = Cosmetics.GetContext().Basket.Where(x =>x.lack > 0 && x.numberOrder==basket.numberOrder).Select(x=>x.Product1.name).ToList();
            StringBuilder text = new StringBuilder();
            text.AppendLine($"Добрый день, {order.fullnameClient}!");
            text.AppendLine($"\nПродукт \"{product}\"\nиз вашего заказа №{order.idOrder} был доставлен на склад.");
            if (!Polniy)
            {
                text.AppendLine($"\nПродукты, ожидающие доставки на склад:");
                foreach (var item in baskets)
                    text.AppendLine("--- "+item+" ---");
            }
            else
                text.AppendLine("\nТовары в вашем заказе доставлены на склад в полном объеме.");
            text.AppendLine($"Для согласования даты доставки, пожалуйста, позвоните по номеру +7(927)836-36-36.");
            text.AppendLine("С уважением, Магазин декоративной косметики \"GracefulShine\"");
            
            myMail.Body = text.ToString();
            myMail.BodyEncoding = Encoding.UTF8;
            try
            { 
                mySmtpClient.Send(myMail); 
            }
            catch (Exception e)
            { MessageBox.Show($"Почты не существует!\n{e.Message}"); }
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            if (kolvo > 1)
            {
                kolvo--;
                Max.IsEnabled = true;
                PackAmount.Text = kolvo.ToString();
                if (kolvo == 1)
                    Min.IsEnabled = false;
                Max.IsEnabled = true;
            }
        }
        int kolvo = 1;
        private void Plus(object sender, RoutedEventArgs e)
        {
            if (kolvo<100)
            {
                kolvo++;
                Min.IsEnabled = true;
                PackAmount.Text = kolvo.ToString();
                if(kolvo==100)
                    Max.IsEnabled = false;
                Min.IsEnabled = true;
            }
        }

        private void AddFromExcel(object sender, RoutedEventArgs e)
        {
            PrihodWindow prihodWindow = new PrihodWindow();
            prihodWindow.Show();
            Close();
        }
    }
}
