using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для DiagrammBrandsWindow.xaml
    /// </summary>
    public partial class DiagrammBrandsWindow : Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public DiagrammBrandsWindow(List<Basket> baskets)
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            basket = baskets;
            var a = Cosmetics.GetContext().Basket.GroupBy(x => x.Product1.Brand1, x => x.amount) //топ 10 брендов по продажам
                .Select(g => new { Brand = g.Key, Amount = g.Sum()}).OrderByDescending(o => o.Amount).Take(5).ToList();
            List<CombinedListItem> combinedListItems = new List<CombinedListItem>();
            List<string> brands = new List<string>();
            foreach(var br in a)
            {
                brands.Add(br.Brand.name);
            }
            List<int> am = new List<int>();
            foreach (var amo in a)
            {
                am.Add(amo.Amount);
            }
            for (int i = 1; i < a.Count + 1; i++)
            {
                combinedListItems.Add(new CombinedListItem()
                {
                    Brand = brands[i-1],
                    Amount = am[i-1],
                    Count = i
                });
            }
            List.ItemsSource = combinedListItems;
        }

        List<Basket> basket;
        Brand brand = new Brand();
        private void Back(object sender, RoutedEventArgs e)
        {
            MainClientWindow mainClient = new MainClientWindow(basket, null);
            mainClient.Show();
            Close();
        }

        public class CombinedListItem
        {
            public string Brand { get; set; }
            public int Amount { get; set; }
            public int Count { get; set; }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = List.SelectedItem as CombinedListItem;
            if (a != null)
            {
                brand = Cosmetics.GetContext().Brand.Where(x => x.name == a.Brand).FirstOrDefault();
                MainClientWindow mainClientWindow = new MainClientWindow(basket, brand);
                mainClientWindow.Show();
                Close();
            }
            
        }
    }
}
