using Microsoft.Win32;
using ShopCosmetic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для PrihodWindow.xaml
    /// </summary>
    public partial class PrihodWindow : System.Windows.Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public PrihodWindow()
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            CbProducts.ItemsSource = Cosmetics.GetContext().Product.ToList();
            LViewProducts.ItemsSource = list;
        }

        public class MyData
        {
            public Product product { get; set; }
            public string amount { get; set; } = "1";
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(false);
            mainWindow.Show();
            Close();
        }
        List<MyData> list = new List<MyData>(); 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CbProducts.SelectedIndex > -1)
            {
                var product = CbProducts.SelectedItem as Product;
                if (list.Where(x => x.product == product).FirstOrDefault() != null)
                {
                    list.Where(x => x.product == product).FirstOrDefault().amount = (Convert.ToInt32(list.Where(x => x.product == product).FirstOrDefault().amount)+1).ToString();
                }
                else
                {
                    MyData myData = new MyData() { product = CbProducts.SelectedItem as Product };
                    list.Add(myData);
                }
                LViewProducts.Items.Refresh();
            }
            else
                MessageBox.Show("Выберите продукт", "Ошибка получения данных");
        }
        StringBuilder st = new StringBuilder();
        private void Add(object sender, RoutedEventArgs e)
        {
            if (LViewProducts.Items.Count > 0)
            {
                foreach(var item in LViewProducts.Items) //проверка
                {
                    if (!int.TryParse((item as MyData).amount.Trim(), out int a) || a <= 0)
                    {
                        MessageBox.Show("В строках указано неверное количество пачек! Количество не может быть пустым или 0", "Ошибка получения данных");
                        return;
                    }
                }
                st.AppendLine($"На склад добавлены продукты:");
                bool messa = false;
                foreach (var item in LViewProducts.Items)
                {
                    Product p = (item as MyData).product;
                    var baskets = Cosmetics.GetContext().Basket.Where(x => x.product == p.id && x.lack > 0).ToList();
                    int amount = Convert.ToInt32((item as MyData).amount) * p.pack;
                    p.sclad += amount;
                    foreach (var basket in baskets)
                        if (p.sclad >= basket.lack)
                        {
                            p.sclad -= basket.lack;
                            basket.lack = 0;
                            Cosmetics.GetContext().SaveChanges();
                            if (Cosmetics.GetContext().Basket.Where(x => x.lack > 0 && x.numberOrder == basket.numberOrder).ToList().Count >= 1)
                                MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, p.name, Polniy: false);
                            else
                                MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, p.name, Polniy: true);
                            messa = true;
                        }
                        else break;
                    st.AppendLine($"\"{p.name}\" в количестве {amount} шт.");
                }
                if (messa)
                    st.AppendLine("Оповещение о доставке продуктов на склад было отправлено клиентам\n");
                Cosmetics.GetContext().SaveChanges();
                MessageBox.Show(st.ToString(), "Успешное выполнение задачи");
                list.Clear(); LViewProducts.Items.Refresh();
            }
            else
                MessageBox.Show("Выберите продукты для добавления на склад", "Ошибка получения данных");
        }
        StringBuilder strb = new StringBuilder();
        private void Excel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файл Excel|*.XLSX;*.XLS";
            if (openDialog.ShowDialog() == true)
            {
                Excel.Application ExcelApp = new Excel.Application();
                Workbook WorkBookExcel = ExcelApp.Workbooks.Open(openDialog.FileName);
                Worksheet WorkSheetExcel = (Worksheet)WorkBookExcel.Sheets[1];
                Range ExcelRange = WorkSheetExcel.UsedRange;
                int rowCount = ExcelRange.Rows.Count;
                if (rowCount > 2)
                {
                    bool messa = false;
                    strb.AppendLine("На склад добавлены продукты:");
                    for (int v = 1; v < rowCount; v++)
                    {
                        productExcel product = new productExcel();
                        for (int j = 0; j < 2; j++)
                        {
                            ExcelRange = WorkSheetExcel.Cells[v + 1, j + 1] as Range;
                            if (ExcelRange != null && ExcelRange.Value2 != null)
                            {
                                if (j == 0)
                                    product.name = ExcelRange.Value.Trim();
                                else
                                    product.amount = (int)ExcelRange.Value;
                            }
                            else { MessageBox.Show("В таблице есть пустые ячейки!", "Ошибка получения данных"); return; }
                        }
                        var baskets = Cosmetics.GetContext().Basket.Where(x => x.Product1.name == product.name && x.lack > 0).ToList();
                        var p = Cosmetics.GetContext().Product.Where(x => x.name == product.name).FirstOrDefault();
                        p.sclad += product.amount * p.pack;
                        foreach (var basket in baskets)
                            if (p.sclad >= basket.lack)
                            {
                                p.sclad -= basket.lack;
                                basket.lack = 0;
                                Cosmetics.GetContext().SaveChanges();
                                if (Cosmetics.GetContext().Basket.Where(x => x.lack > 0 && x.numberOrder == basket.numberOrder).ToList().Count >= 1)
                                    MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, p.name, Polniy: false);
                                else
                                    MainWindow.CreateTimeoutTestMessage(basket, Cosmetics.GetContext().Order.Where(x => x.idOrder == basket.numberOrder).FirstOrDefault().emailClient, p.name, Polniy: true);
                                messa = true;
                            }
                            else break; //потомучто надо ждать пока у первого клиента продукта будет хватать для отправки
                        strb.AppendLine($"{p.name} в количестве {product.amount*p.pack} шт.");
                    }
                    Cosmetics.GetContext().SaveChanges();
                    if (messa)
                        st.AppendLine("Оповещение о доставке продуктов на склад было отправлено клиентам\n");
                    MessageBox.Show(strb.ToString());
                    WorkBookExcel.Close(false); ExcelApp.Quit(); ExcelApp = null; WorkBookExcel = null; WorkSheetExcel = null; ExcelRange = null; GC.Collect();
                }
                else MessageBox.Show("В таблице нет продуктов!", "Ошибка получения данных");
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (LViewProducts.SelectedItems.Count > 0)
            {
                var productsForRemoving = LViewProducts.SelectedItems.Cast<MyData>().ToList();
                foreach (var p in productsForRemoving)
                {
                    list.Remove(p);
                }
                LViewProducts.Items.Refresh();
            }
            else MessageBox.Show("Выберите строку для удаления!", "Ошибка получения данных");
            LViewProducts.SelectedItem = null;
        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        public class productExcel
        {
            public string name { get; set; }
            public int amount { get; set; }
        }
    }
}
