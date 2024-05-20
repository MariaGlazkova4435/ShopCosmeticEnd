using ShopCosmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Data.Entity;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для OtchetsWindow.xaml
    /// </summary>
    public partial class OtchetsWindow : System.Windows.Window
    {
        OrdersDataGrid ordersDataGrid = new OrdersDataGrid();
        ProductsListView products = new ProductsListView();
        ChangoPhotoPath path = new ChangoPhotoPath();
        public OtchetsWindow()
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            NachDate = DateTime.Now;
            KonDate = DateTime.Now;
            Nach.Text = NachDate.ToString("d");
            Kon.Text = KonDate.ToString("d");
            napolnenie = Cosmetics.GetContext().Order.ToList();
        }
        DateTime NachDate;
        DateTime KonDate;
        List<Order> napolnenie { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            napolnenie = Cosmetics.GetContext().Order.ToList();
            if (OtchetProdazh.IsChecked == true || UnPopular.IsChecked == true)
            {
                if (!String.IsNullOrWhiteSpace(Nach.Text) && !String.IsNullOrWhiteSpace(Kon.Text))
                {
                    NachDate = Convert.ToDateTime(Nach.Text);
                    KonDate = Convert.ToDateTime(Kon.Text);
                    napolnenie = napolnenie.Where(x => x.date <= KonDate && x.date >= NachDate).ToList();
                }
                else
                {
                    MessageBox.Show("Вы не ввели период формирования отчета полностью!\nПожалуйста, введите корректные значения в поля для ввода даты", "Ошибка получения данных");
                    return;
                }
            }
            if (OtchetProdazh.IsChecked == true) //выбран 1 отчет
            {
                if (napolnenie.Count == 0)
                { TbInfo.Text = "Продаж за выбранный период не было"; }
                else
                {
                    Otchet1.Visibility = Visibility.Visible;
                    BtnExcelOtch2.Visibility = Visibility.Visible;
                    DtOtchet1.ItemsSource = ordersDataGrid.ListOrder(napolnenie);
                    int prodano = napolnenie.Sum(x => x.Basket.Sum(c => c.amount));
                    decimal prodanoMoney = napolnenie.Sum(x => x.total);
                    Total.Text = $"Продано: {prodano} шт. продукции на сумму {Math.Round(prodanoMoney, 2)} руб.";
                }
            }
            else { Otchet1.Visibility = Visibility.Collapsed; BtnExcelOtch2.Visibility = Visibility.Collapsed; }
            if (UnPopular.IsChecked == true) // выбран 2 отчет
            {
                if (napolnenie.Count == 0)
                { TbInfo.Text = "Продаж за выбранный период не было"; Otchet2.Visibility = Visibility.Collapsed; }
                else
                    RefreshGrid2();
            }
            else
                Otchet2.Visibility = Visibility.Collapsed;
            if (endingProducts.IsChecked == true) //выбран 3 отчет
            {
                var endingProd = Cosmetics.GetContext().Product.Where(x => x.sclad <= 10).ToList();
                Otchet3.Visibility = Visibility.Visible; BtnExcelOtch2.Visibility = Visibility.Collapsed;
                if (endingProd.Count != 0)
                {
                    DtOtchet3.ItemsSource = endingProd;
                    TbOtch3.Text = $"Заканчивающихся продуктов: {endingProd.Count} шт.";
                }
                else
                { Otchet3.Visibility = Visibility.Collapsed; TbInfo.Text = "Нет заканчивающихся продуктов"; }
            }
            else
                Otchet3.Visibility = Visibility.Collapsed;
            if (missingProducts.IsChecked == true)//выбран 4 отчет
            {
                SearchEndProd(); BtnExcelOtch2.Visibility = Visibility.Collapsed;
            }
            else
                Otchet4.Visibility = Visibility.Collapsed;
            if (Orders.IsChecked == true)//выбран 5 отчет
            {
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("dd.MM.yyyy"));
                var orders = ordersDataGrid.ListOrder(Cosmetics.GetContext().Order.Where(x => x.delivery && x.dateHand == now).ToList());
                Otchet5.Visibility = Visibility.Visible;
                if (orders.Count != 0)
                {
                    DtOtchet5.ItemsSource = orders;
                    BtnExcelOtch2.Visibility = Visibility.Visible;
                    OrdCount.Text = $"Заказов: {orders.Count} шт.";
                }
                else
                {
                    Otchet5.Visibility = Visibility.Collapsed; TbInfo.Text = "Нет заказов с доставкой на сегодня";
                    BtnExcelOtch2.Visibility = Visibility.Collapsed;
                }
            }
            else { Otchet5.Visibility = Visibility.Collapsed; }
            if (OrdersCollect.IsChecked == true) //выбран 6 отчет
            {
                var orders = ordersDataGrid.ListOrder(Cosmetics.GetContext().Order.Where(x => x.status==2 && DbFunctions.DiffDays(x.dateCollect, DateTime.Now) >2).ToList());
                Otchet6.Visibility = Visibility.Visible;
                if (orders.Count != 0)
                {
                    DtOtchet6.ItemsSource = orders;
                    BtnExcelOtch2.Visibility = Visibility.Visible;
                    OrdCount6.Text = $"Заказов: {orders.Count} шт.";
                }
                else
                {
                    Otchet6.Visibility = Visibility.Collapsed; TbInfo.Text = "Нет заказов с истекшим сроком хранения";
                    BtnExcelOtch2.Visibility = Visibility.Collapsed;
                }
            }
            else { Otchet6.Visibility = Visibility.Collapsed; }
        }

        private void SearchEndProd()
        {
            var endProd = Cosmetics.GetContext().Product.Where(x => x.Basket.Sum(c => c.lack) > 0).ToList();
            Otchet4.Visibility = Visibility.Visible;
            if (endProd.Count != 0)
            {
                DtOtchet4.ItemsSource = products.ListProd(endProd);
                TbOtch4.Text = $"Отсутствующих продуктов: {endProd.Count} шт.";
            }
            else
            { Otchet4.Visibility = Visibility.Collapsed; TbInfo.Text = "Нет заказанных и отсутствующих на складе продуктов"; }
        }

        private void ShowDetails(object sender, RoutedEventArgs e)
        {
            MoreOrderWindow orderMore = new MoreOrderWindow((sender as System.Windows.Controls.Button).DataContext as Order1);
            orderMore.Show();
        }
        private void RefreshGrid2()
        {
            Otchet2.Visibility = Visibility.Visible;
            napolnenie = Cosmetics.GetContext().Order.Where(x => x.date <= KonDate && x.date >= NachDate).ToList();
            var ObshAmountSale = napolnenie.Sum(x => x.Basket.Sum(c => c.amount)); //общее
            List<UnPopularProduct> list = new List<UnPopularProduct>();
            foreach (var product in Cosmetics.GetContext().Product)
            {
                var prodano = napolnenie.Sum(x => x.Basket.Where(c => c.Product1 == product).Sum(c => c.amount)); //колвоПродажДанногоПродукта
                if ((prodano * 100 / ObshAmountSale) < 5)
                {
                    UnPopularProduct unPopupalProduct = new UnPopularProduct()
                    {
                        name = product.name,
                        amountSale = prodano,
                        percent = $"{prodano * 100 / ObshAmountSale}%",
                        price = (bool)product.promotion ? Math.Round(product.price - (product.price * (Convert.ToDecimal(product.promotionPercent) / 100)), 2) : Math.Round(product.price, 2),
                        pricePercent = (bool)product.promotion ? (int)product.promotionPercent : 0
                    };
                    list.Add(unPopupalProduct);
                }
            }
            if (list.Count == 0)
            { TbInfo.Text = "Нет непопулярных продуктов"; Otchet2.Visibility = Visibility.Collapsed; return; }
            DtOtchet2.ItemsSource = list.ToList().OrderByDescending(x => x.amountSale);
            TbOtch2.Text = $"Непопулярных товаров: {list.Count} шт.";
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(false);
            mainWindow.Show();
            Close();
        }
        RadioButton rb = new RadioButton();
        bool FirstRun = true;
        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!FirstRun)
            {
                rb = (RadioButton)sender;
                if (rb == OtchetProdazh || rb == UnPopular)
                    Dates.Visibility = Visibility.Visible;
                else
                    Dates.Visibility = Visibility.Hidden;
            }
            FirstRun = false;
        }
        private void Edit(object sender, RoutedEventArgs e)
        {
            string name = ((sender as System.Windows.Controls.Button).DataContext as UnPopularProduct).name;
            Product product = Cosmetics.GetContext().Product.Where(x=>x.name==name).SingleOrDefault();
            EditWindow editWindow = new EditWindow(product, withClosingLast: false);
            editWindow.ShowDialog();
            Cosmetics.context = null;
            RefreshGrid2();
        }

        class UnPopularProduct
        {
            public string name { get; set; }
            public int amountSale { get; set; }
            public string percent { get; set; }
            public decimal price { get; set; }
            public int pricePercent { get; set; }
        }

        private void InExcel(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];
            Range myRange;
            if (Otchet1.IsVisible)
            {
                sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[2, 4]].Merge();
                sheet1.Cells[1, 1].Value2 = $"Отчет по продажам\nПериод с {NachDate.ToString("dd.MM.yyyy")} по {KonDate.ToString("dd.MM.yyyy")}";
                for (int j = 0; j < DtOtchet1.Columns.Count - 1; j++)
                {
                    myRange = (Range)sheet1.Cells[3, j + 1];
                    sheet1.Cells[3, j + 1].Font.Bold = true;
                    sheet1.Columns[j + 1].ColumnWidth = 25;
                    myRange.Value2 = DtOtchet1.Columns[j].Header;
                }
                int i = 3;
                sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[DtOtchet1.Items.Count + 4, 4]].HorizontalAlignment = -4108;
                sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[DtOtchet1.Items.Count + 4, 4]].VerticalAlignment = -4108;
                foreach (Order1 ord in DtOtchet1.Items)
                {
                    i++;
                    myRange = (Range)sheet1.Cells[i, 1];
                    myRange.Value2 = ord.date.ToString("dd.MM.yyyy");
                    myRange = (Range)sheet1.Cells[i, 2];
                    myRange.Value2 = ord.idOrder;
                    myRange = (Range)sheet1.Cells[i, 3];
                    myRange.Value2 = ord.Basket.Sum(x => x.amount);
                    myRange = (Range)sheet1.Cells[i, 4];
                    myRange.Value2 = ord.total;
                }
                myRange = (Range)sheet1.Cells[DtOtchet1.Items.Count + 4, 1]; sheet1.Cells[DtOtchet1.Items.Count + 4, 1].Font.Bold = true;
                myRange.Value2 = "ИТОГО";
                myRange = (Range)sheet1.Cells[DtOtchet1.Items.Count + 4, 3]; sheet1.Cells[DtOtchet1.Items.Count + 4, 3].Font.Bold = true;
                myRange.Value2 = napolnenie.Sum(x => x.Basket.Sum(c => c.amount));
                myRange = (Range)sheet1.Cells[DtOtchet1.Items.Count + 4, 4]; sheet1.Cells[DtOtchet1.Items.Count + 4, 4].Font.Bold = true;
                myRange.Value2 = napolnenie.Sum(x => x.total); excel = null; workbook = null; sheet1 = null; myRange = null;
            }
            else if(Otchet5.IsVisible)
            {
                sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[1, 3]].Merge();
                sheet1.Cells[1, 1].HorizontalAlignment = -4108;
                sheet1.Cells[1, 1].VerticalAlignment = -4108;
                sheet1.Cells[1, 1].Value2 = $"Отчет по заказам, у которых дата доставки {DateTime.Now.ToString("dd.MM.yyyy")}";
                for (int j = 0; j < DtOtchet5.Columns.Count - 1; j++)
                {
                    myRange = (Range)sheet1.Cells[2, j + 1];
                    sheet1.Cells[2, j + 1].Font.Bold = true;
                    sheet1.Columns[j + 1].ColumnWidth = 25;
                    myRange.Value2 = DtOtchet5.Columns[j].Header;
                }
                int i = 2;
                sheet1.Range[sheet1.Cells[2, 1], sheet1.Cells[DtOtchet5.Items.Count + 3, 4]].HorizontalAlignment = -4108;
                sheet1.Range[sheet1.Cells[2, 1], sheet1.Cells[DtOtchet5.Items.Count + 3, 4]].VerticalAlignment = -4108;
                sheet1.Columns[3].ColumnWidth = 35;
                foreach (Order1 ord in DtOtchet5.Items)
                {
                    i++;
                    myRange = (Range)sheet1.Cells[i, 1];
                    myRange.Value2 = ord.idOrder;
                    myRange = (Range)sheet1.Cells[i, 2];
                    myRange.Value2 = ord.address;
                    myRange = (Range)sheet1.Cells[i, 3];
                    myRange.Value2 = ord.phoneClient;
                }
                myRange = (Range)sheet1.Cells[DtOtchet5.Items.Count + 3, 1]; sheet1.Cells[DtOtchet5.Items.Count + 4, 1].Font.Bold = true;
                myRange.Value2 = "ИТОГО";
                myRange = (Range)sheet1.Cells[DtOtchet5.Items.Count + 3, 2]; sheet1.Cells[DtOtchet5.Items.Count + 4, 3].Font.Bold = true;
                myRange.Value2 = DtOtchet5.Items.Count + " заказ(-а/ов)";
            }
            else
            {
                sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[1, 3]].Merge();
                sheet1.Cells[1, 1].HorizontalAlignment = -4108;
                sheet1.Cells[1, 1].VerticalAlignment = -4108;
                sheet1.Cells[1, 1].Value2 = $"Отчет по заказам с истекшим сроком хранения на {DateTime.Now.ToString("dd.MM.yyyy")}";
                for (int j = 0; j < DtOtchet6.Columns.Count - 1; j++)
                {
                    myRange = (Range)sheet1.Cells[2, j + 1];
                    sheet1.Cells[2, j + 1].Font.Bold = true;
                    sheet1.Columns[j + 1].ColumnWidth = 25;
                    myRange.Value2 = DtOtchet6.Columns[j].Header;
                }
                int i = 2;
                sheet1.Range[sheet1.Cells[2, 1], sheet1.Cells[DtOtchet6.Items.Count + 3, 4]].HorizontalAlignment = -4108;
                sheet1.Range[sheet1.Cells[2, 1], sheet1.Cells[DtOtchet6.Items.Count + 3, 4]].VerticalAlignment = -4108;
                sheet1.Columns[3].ColumnWidth = 35;
                foreach (Order1 ord in DtOtchet6.Items)
                {
                    i++;
                    myRange = (Range)sheet1.Cells[i, 1];
                    myRange.Value2 = Convert.ToDateTime(ord.dateCollect).ToString("dd.MM.yyyy");
                    myRange = (Range)sheet1.Cells[i, 2];
                    myRange.Value2 = ord.idOrder;
                    myRange = (Range)sheet1.Cells[i, 3];
                    myRange.Value2 = ord.phoneClient;
                }
                myRange = (Range)sheet1.Cells[DtOtchet6.Items.Count + 3, 1]; sheet1.Cells[DtOtchet6.Items.Count + 4, 1].Font.Bold = true;
                myRange.Value2 = "ИТОГО";
                myRange = (Range)sheet1.Cells[DtOtchet6.Items.Count + 3, 2]; sheet1.Cells[DtOtchet6.Items.Count + 4, 3].Font.Bold = true;
                myRange.Value2 = DtOtchet6.Items.Count + " заказ(-а/ов)";
            }
        }

        private void AddProducts(object sender, RoutedEventArgs e)
        {
            bool SendMessages = false;
            var p = (sender as System.Windows.Controls.Button).DataContext as Product1;
            if (p == null)
                p = products.ReturnProduct((sender as System.Windows.Controls.Button).DataContext as Product);
            var product = Cosmetics.GetContext().Product.Where(x=>x.id==p.id).FirstOrDefault();
            var baskets = Cosmetics.GetContext().Basket.Where(x => x.product == product.id && x.lack > 0).ToList();
            product.sclad += product.pack;
            foreach (var basket in baskets)
                if (product.sclad >= basket.lack)
                {
                    product.sclad -= basket.lack;
                    basket.lack = 0;
                    Cosmetics.GetContext().SaveChanges();
                    if (Cosmetics.GetContext().Basket.Where(x => x.lack > 0 && x.numberOrder == basket.numberOrder).ToList().Count >= 1)
                        MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, product.name, Polniy: false);
                    else
                        MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, product.name, Polniy: true);
                    SendMessages = true;
                }
                else
                    break;
            if(SendMessages)
                MessageBox.Show("Оповещение о доставке продуктов на склад было отправлено клиентам\n");
            Cosmetics.GetContext().SaveChanges();
            MessageBox.Show($"К товару {product.name} было добавлено {product.pack} шт.", "Успешное выполнение");
            if (Otchet3.IsVisible)
                DtOtchet3.Items.Refresh(); 
            else
                SearchEndProd();
        }
        
    }

}
