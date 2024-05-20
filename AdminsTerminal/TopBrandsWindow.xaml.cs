using ShopCosmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для TopBrandsWindow.xaml
    /// </summary>
    public partial class TopBrandsWindow : Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public TopBrandsWindow()
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            var a = Cosmetics.GetContext().Basket.GroupBy(x => x.Product1.Brand1, x => x.amount) //топ 5 брендов по продажам
                .Select(g => new { Brand = g.Key, Amount = g.Sum() }).OrderByDescending(o => o.Amount).Take(5).ToList();
            List<CombinedListItem> combinedListItems = new List<CombinedListItem>();
            List<string> brands = new List<string>();
            foreach (var br in a)
                brands.Add(br.Brand.name);
            List<int> am = new List<int>();
            foreach (var amo in a)
                am.Add(amo.Amount);
            for (int i = 1; i < a.Count + 1; i++)
            {
                combinedListItems.Add(new CombinedListItem()
                {
                    Brand = brands[i - 1],
                    Amount = am[i - 1],
                    Count = i
                });
            }
            List.ItemsSource = combinedListItems;
        }
        Brand brand = new Brand();
        private void Back(object sender, RoutedEventArgs e)
        {
            ClientsWindow mainClient = new ClientsWindow(null);
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
                ClientsWindow mainClientWindow = new ClientsWindow(brand);
                mainClientWindow.Show();
                Close();
            }

        }
    }
}
