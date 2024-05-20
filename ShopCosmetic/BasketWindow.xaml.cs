using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopCosmetic
{
    /// <summary>
    /// Логика взаимодействия для BasketWindow.xaml
    /// </summary>
    public partial class BasketWindow : Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public BasketWindow(List<Basket> b)
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            basket = b;
            LViewProducts.ItemsSource = basket;
            d.IsChecked = true;
            p.IsChecked = true;
            SumOrder();
            List<string> dates = new List<string>();
            DateTime date = DateTime.Now;
            for(int i = 0; i < 7; i++)
            {
                dates.Add(date.AddDays(1+i).ToString("dd.MM.yyyy"));
            }
            Dates.ItemsSource = dates.ToList();
            Dates.SelectedIndex = 0;
        }
        List<Basket> basket;
        bool vibordate = false;
        ChangoPhotoPath changoPhotoPath = new ChangoPhotoPath();
        DateTime dateHand = new DateTime();
        private void SumOrder()
        {
            string Sum1 = Math.Round(basket.Sum(x => x.amount * (double)x.finalPrice), 2).ToString();
            Sum.Text = "Сумма заказа: " + Sum1 + " руб";
            foreach (Basket ba in basket)
            {
                if (ba.Product1.sclad - ba.amount < 0)
                {
                    ViborDate.Visibility = Visibility.Hidden; vibordate = false; return;
                }
                else ViborDate.Visibility = Visibility.Visible; vibordate = true;
            }
        }
        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var b = (sender as Button).DataContext as Basket;
            var b1 = basket.Where(x => x.Product1 == b.Product1).FirstOrDefault();
            b1.amount++;
            LViewProducts.Items.Refresh();
            SumOrder();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var b = (sender as Button).DataContext as Basket;
            var b1 = basket.Where(x => x.Product1 == b.Product1).FirstOrDefault();
            if (b1.amount - 1 != 0)
                b1.amount--;
            LViewProducts.Items.Refresh();
            SumOrder();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainClientWindow mainClientWindow = new MainClientWindow(basket, null);
            mainClientWindow.Show();
            Close();
        }

        private void BtnDelete_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы уверены, что хотите удалить этот продукт из корзины?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var b = (sender as Button).DataContext as Basket;
                var b1 = basket.Where(x => x.Product1 == b.Product1).FirstOrDefault();
                basket.Remove(b1);
                LViewProducts.Items.Refresh();
                SumOrder();
            }
            if (LViewProducts.Items.Count == 0)
                SPOrder.IsEnabled = false;
        }

        bool deliv = true;

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            deliv = false;
            Address.Visibility = Visibility.Hidden;
            Addr.Visibility = Visibility.Hidden;
            ViborDate.Visibility = Visibility.Hidden; vibordate = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            deliv = true;
            Address.Visibility = Visibility.Visible;
            Addr.Visibility = Visibility.Visible;
            ViborDate.Visibility = Visibility.Visible; vibordate = true;
            SumOrder();
        }

        private void Order(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool nehvataet = false;
            if (!deliv)
            {
                foreach (Basket b in basket)
                {
                    if (b.Product1.sclad - b.amount < 0)
                    {
                        var a = (b.Product1.sclad - b.amount) * -1;
                        errors.AppendLine($"На складе не хватает {a} шт продукта \"{b.Product1.name}\"");
                        nehvataet = true;
                    }
                }
            }
            if (nehvataet)
            {
                errors.AppendLine("Измените количество в корзине или закажите доставку на дом!");
                MessageBox.Show(errors.ToString());
                return;
            }
            if (basket.Count == 0)
                errors.AppendLine("Нет товаров в корзине!\nДобавьте товары в корзину");
            if (deliv == true && String.IsNullOrWhiteSpace(Address.Text))
                errors.AppendLine("Выбрана доставка но не указан адрес!\nУкажите адрес доставки");
            if (string.IsNullOrWhiteSpace(Email.Text))
                errors.AppendLine("Не введена электронная почта!\nУкажите вашу почту, чтобы мы могли отправить вам чек");
            else
            {
                const string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                var email = Email.Text.Trim().ToLowerInvariant();
                if (!Regex.Match(email, pattern).Success)
                    errors.AppendLine("Некорректный формат почты\nПочта должна содержать знак \"@\" и домен");
            }
            if(!phone.IsMaskFull)
                errors.AppendLine("Номер телефона введен не полностью\nВведите ваш номер телефона, чтобы мы могли связаться с вами");
            if (string.IsNullOrWhiteSpace(Fio.Text))
                errors.AppendLine("Не введены ФИО\nВведите ФИО, чтобы мы знали как к Вам обращаться");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка получения данных");
                return;
            }
            var cont = Cosmetics.GetContext().Payment;
            Order order = new Order()
            {
                date = DateTime.Now,
                address = deliv ? Address.Text : null,
                delivery = deliv,
                Basket = basket,
                emailClient = Email.Text.Trim(),
                Payment1 = payKart ? cont.Where(x => x.idPayment == 2).SingleOrDefault() : cont.Where(x => x.idPayment == 1).SingleOrDefault(),
                payment = payKart ? 2 : 1,
                phoneClient = phone.Text.Trim(),
                fullnameClient = Fio.Text.Trim(),
                OrderStatus = Cosmetics.GetContext().OrderStatus.Where(x => x.idStatus == 1).SingleOrDefault(),
                status = 1,
                total = (decimal)Math.Round(basket.Sum(x => x.amount * (double)x.finalPrice), 2),
                dateHand = vibordate ? dateHand : Convert.ToDateTime("01.01.2001")
            };
        Cosmetics.GetContext().Order.Add(order);
            Cosmetics.GetContext().SaveChanges();
            try { CreateTimeoutTestMessage(order, order.idOrder); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return; // остановка выполнения программы
            }
            foreach (var b in basket) //убавление продукции со склада
            {
                if (b.Product1.sclad - b.amount >= 0)
                {
                    b.lack = 0;
                    b.Product1.sclad -= b.amount;
                }
                else //если продукта не хватает перенос в нехватку
                {
                    b.lack = -(int)(b.Product1.sclad - b.amount);
                    b.Product1.sclad = 0;
                }
            }
            Cosmetics.GetContext().SaveChanges();
            WriteToFile($"{changoPhotoPath.NewPath}\\ShopCosmetic\\ShopCosmetic\\Check.txt", order);
            ThanksWindow thanksWindow = new ThanksWindow(order);
            thanksWindow.ShowDialog();

            MainClientWindow mainClientWindow = new MainClientWindow(null, null);
            mainClientWindow.Show();
            Close();
        }

        public class DTnull
        {
            DateTime dt;
            bool Ok = true;
        }

        public void CreateTimeoutTestMessage(Order order, int idOr)
        {
            SmtpClient mySmtpClient = new SmtpClient("smtp.mail.ru");
            // set smtp-client with basicAuthentication
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = true;//465 

            System.Net.NetworkCredential basicAuthenticationInfo = new
               System.Net.NetworkCredential("glazkovamaria5898@mail.ru", "Fkn0ma9RiANzNPjNQetB");
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            MailAddress from = new MailAddress("glazkovamaria5898@mail.ru", "GracefulShine");
            MailAddress to = new MailAddress(order.emailClient, "Test");
            MailMessage myMail = new MailMessage(from, to);

            // add ReplyTo
            MailAddress replyTo = new MailAddress("glazkovamaria5898@mail.ru");
            myMail.ReplyToList.Add(replyTo);

            // set subject and encoding
            myMail.Subject = "Чек по заказу в магазине \"GracefulShine\"";
            myMail.SubjectEncoding = Encoding.UTF8;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (Basket b in order.Basket)
            {
                stringBuilder.AppendLine(b.Product1.name + " " + Math.Round((double)b.finalPrice, 2) + "*" + b.amount + " = " + Math.Round(b.amount * (double)b.finalPrice, 2) + " руб");
            }
            StringBuilder dostAndPay = new StringBuilder();
            if (order.delivery)
                dostAndPay.AppendLine("Доставка на адрес: " + order.address);
            if (order.payment == 1)
                dostAndPay.AppendLine("Оплата наличными");
            else
                dostAndPay.AppendLine("Оплата картой");

            if (order.dateHand!=null && order.dateHand!=Convert.ToDateTime("01.01.2001"))
                dostAndPay.AppendLine($"Доставка: {order.dateHand.Value.ToString("dd.MM.yyyy")}");

            // set body-message and encoding
            myMail.Body = $"Чек по заказу № {idOr}" +
                $"\r\n{order.date}" +
                $"\r\nМагазин декоративной косметики \"GracefulShine\"\r\n---------------------------" +
                $"\r\n{stringBuilder}\r---------------------------" +
                $"\r\nИТОГ = {order.total} руб\r\n " +
                $"\r\nФИО покупателя: {order.fullnameClient}" +
                $"\r\n{dostAndPay}\r\n " +
                $"\r\nТелефон обратной связи: 89278368606";
            myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = false;
            mySmtpClient.Send(myMail);
        }
        static void WriteToFile(string path, Order order)
        {
            if (File.Exists(path))
                File.Delete(path);
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Чек по заказу №" + order.idOrder);
                file.WriteLine(order.date);
                file.WriteLine("Магазин декоративной косметики \"GracefulShine\"");
                file.WriteLine("---------------------------");
                foreach (Basket b in order.Basket)
                    file.WriteLine(b.Product1.name + " " + Math.Round((double)b.finalPrice, 2) + "*" + b.amount + " = " + Math.Round(b.amount * (double)b.finalPrice, 2) + " руб");
                file.WriteLine("---------------------------");
                file.WriteLine("ИТОГ = " + order.total + " руб");
                file.WriteLine();
                file.WriteLine("ФИО покупателя: " + order.fullnameClient);
                if (order.delivery)
                    file.WriteLine("Доставка на адрес: " + order.address);
                if (order.payment == 1)
                    file.WriteLine("Оплата наличными");
                else
                    file.WriteLine("Оплата картой");
                file.WriteLine();
                file.WriteLine("Телефон обратной связи: 89278368606");
            }
        }
        bool payKart = true;
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            payKart = false;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            payKart = true;
        }

        private void Dates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dateHand = Convert.ToDateTime(Dates.SelectedValue.ToString());
        }
    }
}
