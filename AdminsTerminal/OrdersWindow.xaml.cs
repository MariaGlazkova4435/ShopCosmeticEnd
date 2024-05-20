using ShopCosmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        OrdersDataGrid orders = new OrdersDataGrid();
        ChangoPhotoPath path = new ChangoPhotoPath();
        public OrdersWindow()
        {
            InitializeComponent();
            CbStatus.ItemsSource = Cosmetics.GetContext().OrderStatus.ToList();
            CbStatus.SelectedIndex = 0;
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
        }
        private void CloseOrders(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                var OrdersForClosing = Dg.SelectedItems.Cast<Order1>().ToList();
                if (MessageBox.Show($"Вы точно хотите закрыть следующий(-е) {OrdersForClosing.Count()} заказ(-ов)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var o = new Order();
                    foreach (var order in OrdersForClosing)
                    {
                        o = Cosmetics.GetContext().Order.Where(x => x.idOrder == order.idOrder).FirstOrDefault();
                        o.OrderStatus = Cosmetics.GetContext().OrderStatus.Where(x => x.idStatus == 3).SingleOrDefault(); 
                    }
                    Cosmetics.GetContext().SaveChanges();
                    MessageBox.Show("Заказы закрыты!", "Успешное выполнение");
                    Dg.ItemsSource = orders.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status == orderStatus.idStatus).ToList());
                }
                vibor = false;
            }
            else
                MessageBox.Show("Выберите заказы, которые необходимо закрыть!", "Ошибка получения данных");
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(false);
            main.Show();
            Close();
        }
        Order ord = new Order();
        bool vibor = false;
        private void Dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dg.SelectedItem != null)
            {
                if ((Dg.SelectedItem as Order1).idOrder != 0)
                {
                    var o = (Dg.SelectedItem as Order1);
                    ord = Cosmetics.GetContext().Order.Where(x=>x.idOrder == o.idOrder).FirstOrDefault();
                    vibor = true;
                }
            }
            else
                vibor = false;
        }

        private void ShowOrder(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                MoreOrderWindow moreOrderWindow = new MoreOrderWindow(orders.ReturnOrder(ord));
                moreOrderWindow.Show();
            }
            else
                MessageBox.Show("Выберите заказ для просмотра", "Ошибка получения данных");
            Dg.SelectedItem = null;
        }
        OrderStatus orderStatus;
        private void CbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orderStatus = CbStatus.SelectedItem as OrderStatus;
            Dg.ItemsSource = orders.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status == orderStatus.idStatus).ToList());
            if (orderStatus.idStatus == 1)
            {
                BtnClose.Visibility = Visibility.Collapsed; CancelOrd.Visibility = Visibility.Visible;
                BtnCollect.Visibility = Visibility.Visible; addDate.Visibility = Visibility.Visible; 
            }
            else if (orderStatus.idStatus == 2)
            {
                BtnClose.Visibility = Visibility.Visible; CancelOrd.Visibility = Visibility.Visible;
                BtnCollect.Visibility = Visibility.Collapsed; addDate.Visibility = Visibility.Visible; 
            }
            else
            {
                BtnClose.Visibility = Visibility.Collapsed; CancelOrd.Visibility = Visibility.Collapsed;
                BtnCollect.Visibility = Visibility.Collapsed; addDate.Visibility = Visibility.Collapsed; 
            }
        }

        private void CollectOrders(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                var OrdersForCollecting = Dg.SelectedItems.Cast<Order1>().ToList();
                if (MessageBox.Show($"Вы точно хотите подтвердить сборку {OrdersForCollecting.Count()} заказ(-ов)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Order o = new Order();
                    foreach (var order in OrdersForCollecting)
                    {
                        o = Cosmetics.GetContext().Order.Where(x => x.idOrder == order.idOrder).FirstOrDefault();
                        o.OrderStatus = Cosmetics.GetContext().OrderStatus.Where(x => x.idStatus == 2).SingleOrDefault();
                        o.dateCollect = DateTime.Now;
                    }
                    Cosmetics.GetContext().SaveChanges();
                    CreateTimeoutTestMessage(OrdersForCollecting);
                    MessageBox.Show("Заказы собраны!", "Успешное выполнение");
                    Dg.ItemsSource = orders.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status == orderStatus.idStatus).ToList());
                }
                vibor = false;
            }
            else
                MessageBox.Show("Выберите заказы для подтверждения сборки!", "Ошибка получения данных");
        }

        public void CreateTimeoutTestMessage(List<Order1> orders)
        {
            foreach (var ord in orders.Where(x => x.delivery == false))
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp.mail.ru");
                mySmtpClient.EnableSsl = true;
                mySmtpClient.UseDefaultCredentials = true;//465 
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential("glazkovamaria5898@mail.ru", "Fkn0ma9RiANzNPjNQetB");
                mySmtpClient.Credentials = basicAuthenticationInfo;
                MailAddress from = new MailAddress("glazkovamaria5898@mail.ru", "GracefulShine");
                MailAddress to = new MailAddress(ord.emailClient);
                MailMessage myMail = new MailMessage(from, to);
                MailAddress replyTo = new MailAddress("glazkovamaria5898@mail.ru");
                myMail.ReplyToList.Add(replyTo);
                myMail.Subject = "Ваш заказ №" + ord.idOrder + " собран!";
                myMail.SubjectEncoding = Encoding.UTF8;

                StringBuilder text = new StringBuilder();
                text.AppendLine($"Добрый день, {ord.fullnameClient}!");
                text.AppendLine("Ваш заказ №" + ord.idOrder + " собран и ожидает получения!" +
                    "\nДля отмены заказа, пожалуйста, позвоните по номеру +7(927)836-36-36.\nСрок ожидания получения - 2 суток");
                text.AppendLine("С уважением, Магазин декоративной косметики \"GracefulShine\"");
                myMail.Body = text.ToString();
                myMail.BodyEncoding = Encoding.UTF8;
                try
                {
                    mySmtpClient.Send(myMail);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Письмо по заказу №{ord.idOrder} не было отправлено!\n{e.Message}", "Ошибка отправки письма");
                    return;
                }
            }
            if (orders.Where(x => x.delivery == false).ToList().Count >= 1)
                MessageBox.Show("Оповещение о сборке заказа(-ов) было отправлено клиенту(-ам)\n");
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                var order = Dg.SelectedItem as Order1;
                if (MessageBox.Show($"Вы точно хотите отменить заказ №{order.idOrder}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (Basket b in order.Basket)
                    {
                        if (b.lack == 0)
                        {
                            var prod = Cosmetics.GetContext().Product.Where(x => x.id == b.product).FirstOrDefault();
                            prod.sclad += b.amount;
                            var baskets = Cosmetics.GetContext().Basket.Where(x => x.product == prod.id && x.lack > 0).ToList();
                            foreach (var basket in baskets)
                                if (prod.sclad >= basket.lack)
                                {
                                    prod.sclad -= basket.lack;
                                    basket.lack = 0;
                                    Cosmetics.GetContext().SaveChanges();
                                    if (Cosmetics.GetContext().Basket.Where(x => x.lack > 0 && x.numberOrder == basket.numberOrder).ToList().Count >= 1)
                                        MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, prod.name, Polniy: false);
                                    else
                                        MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, prod.name, Polniy: true);
                                }
                                else
                                    break;
                        }
                    }
                    var ord = Cosmetics.GetContext().Order.Where(x => x.idOrder == order.idOrder).FirstOrDefault();
                    Cosmetics.GetContext().Basket.RemoveRange(ord.Basket);
                    Cosmetics.GetContext().Order.Remove(ord);
                    Cosmetics.GetContext().SaveChanges();
                    MessageBox.Show($"Заказ №{order.idOrder} отменен!", "Успешное выполнение");
                    Dg.ItemsSource = orders.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status == orderStatus.idStatus).ToList());
                }
            }
            else
                MessageBox.Show("Выберите заказ для отмены", "Ошибка получения данных");
            Dg.SelectedItem = null;
        }

        private void AddDate(object sender, RoutedEventArgs e)
        {
            if (vibor)
            {
                var order = Dg.SelectedItem as Order1;
                Order o = Cosmetics.GetContext().Order.Where(x=>x.idOrder == order.idOrder).FirstOrDefault();
                if (o.delivery)
                {
                    SelectDateWindow selectDate = new SelectDateWindow(o);
                    selectDate.ShowDialog();
                    Dg.ItemsSource = orders.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status == orderStatus.idStatus).ToList());
                }
                else
                    MessageBox.Show("Изменить дату доставки можно только для заказов с доставкой!", "Ошибка выполнения задачи");
                vibor = false;
            }
            else
                MessageBox.Show("Выберите заказ для изменения даты доставки!", "Ошибка получения данных");
        }
    }
}
