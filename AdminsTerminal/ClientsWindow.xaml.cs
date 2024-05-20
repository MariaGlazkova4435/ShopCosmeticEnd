using ShopCosmetic;
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

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        ProductsListView products = new ProductsListView();
        ChangoPhotoPath path = new ChangoPhotoPath();
        public ClientsWindow(Brand brand)
        {
            InitializeComponent();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            var a = Cosmetics.GetContext().Basket.GroupBy(x => x.Product1, x => x.amount) //топ 10 товаров по продажам
                .Select(g => new { Product1 = g.Key, Amount = g.Sum() }).OrderByDescending(o => o.Amount).Take(10).ToList();
            List<Bests> bests = new List<Bests>(); //создание списка бестселлеров

            allPurposes = Cosmetics.GetContext().TypePurpose.ToList();
            allBrands = Cosmetics.GetContext().Brand.ToList();
            foreach (var s in a)
            {
                bests.Add(new Bests
                {
                    Path = s.Product1.photoBinary,
                    Name = s.Product1.name,
                    Amount = s.Amount
                });
            }
            LBestProduct.ItemsSource = bests;
            allPurposes.Insert(0, new TypePurpose
            {
                name = "Все типы"
            });
            ComboTypePurpose.ItemsSource = allPurposes;
            ComboTypePurpose.Items.Refresh();
            allBrands.Insert(0, new Brand
            {
                name = "Все бренды"
            });
            ComboBrand.ItemsSource = allBrands;
            ComboBrand.Items.Refresh();
            ComboTypePurpose.SelectedIndex = 0;
            if (brand == null)
                ComboBrand.SelectedIndex = 0;
            else
                ComboBrand.SelectedItem = brand;
            ListViewPurpose.ItemsSource = Cosmetics.GetContext().TypePurpose.ToList();
        }
        private class Bests
        {
            public byte[] Path { get; set; }
            public string Name { get; set; }
            public int Amount { get; set; }
        }
        List<TypePurpose> allPurposes { get; set; }
        List<Brand> allBrands { get; set; }

        
        private void UpdateProducts()
        {
            List<Product1> currentProd = products.ListProd(Cosmetics.GetContext().Product.ToList());
            if (ComboTypePurpose.SelectedIndex > 0)
            {
                var p = currentProd.Where(r => r.TypePurpose1 == ComboTypePurpose.SelectedItem);
                currentProd = p.ToList();
            }

            if (ComboBrand.SelectedIndex > 0)
            {
                var q = currentProd.Where(r => r.Brand1 == ComboBrand.SelectedItem);
                currentProd = q.ToList();
            }

            decimal u;
            decimal n = 0;
            decimal k = 0;

            currentProd = currentProd.Where(p => p.name.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (TBoxNach.Text != "" && decimal.TryParse(TBoxNach.Text, out u))
                n = Convert.ToDecimal(TBoxNach.Text);

            if (TBoxKon.Text != "" && decimal.TryParse(TBoxKon.Text, out u))
                k = Convert.ToDecimal(TBoxKon.Text);


            if (n != 0)
                currentProd = currentProd.Where(p => p.price >= n).ToList();
            if (k != 0)
                if (k > n)
                    currentProd = currentProd.Where(p => p.price <= k).ToList();
            LViewProducts.ItemsSource = currentProd.OrderByDescending(z => z.promotion).ToList();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(TBoxNach.Text))
            {
                TBoxNach.Text = Math.Round(Cosmetics.GetContext().Product.OrderBy(x => x.price).FirstOrDefault().price, 0).ToString();
                return;
            }
            if (String.IsNullOrEmpty(TBoxKon.Text))
            {
                TBoxKon.Text = Math.Round(Cosmetics.GetContext().Product.OrderByDescending(x => x.price).FirstOrDefault().price, 0).ToString();
                return;
            }
            UpdateProducts();
        }

        private void ComboTypePurpose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTypePurpose.SelectedIndex > 0)//поменять бренды
            {
                var q = allBrands.Where(x => x.idBrand > 0).ToList(); //удаление брендов
                foreach (var b in q) { allBrands.Remove(b); }
                var prod = Cosmetics.GetContext().Product.ToList();
                var a = prod.Where(x => x.TypePurpose1 == ComboTypePurpose.SelectedItem).ToList();
                List<Brand> brands = new List<Brand>();
                foreach (var w in a)
                {
                    if (!brands.Contains(w.Brand1))
                    {
                        brands.Add(w.Brand1);
                    }
                }
                allBrands.AddRange(brands);
                ComboBrand.ItemsSource = allBrands; ComboBrand.Items.Refresh(); ComboBrand.SelectedIndex = 0;
            }
            else
            {
                var q = allBrands.Where(x => x.idBrand > 0).ToList(); //удаление брендов
                foreach (var b in q) { allBrands.Remove(b); }
                List<Brand> brands = Cosmetics.GetContext().Brand.ToList();
                allBrands.AddRange(brands);
                ComboBrand.ItemsSource = allBrands; ComboBrand.Items.Refresh(); ComboBrand.SelectedIndex = 0;
            }
            UpdateProducts();
        }

        private void ComboBrand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void Del(object sender, RoutedEventArgs e)
        {
            LViewProducts.ItemsSource = products.ListProd(Cosmetics.GetContext().Product.OrderByDescending(z => z.promotion).ToList());

            var q = allBrands.Where(x => x.idBrand > 0).ToList();
            foreach (var b in q) { allBrands.Remove(b); }
            allBrands.AddRange(Cosmetics.GetContext().Brand.ToList());
            ComboBrand.ItemsSource = allBrands; ComboBrand.Items.Refresh();

            var k = allPurposes.Where(x => x.idTypePurpose > 0).ToList();
            foreach (var b in k) { allPurposes.Remove(b); }
            allPurposes.AddRange(Cosmetics.GetContext().TypePurpose.ToList());
            ComboTypePurpose.ItemsSource = allPurposes; ComboTypePurpose.Items.Refresh();

            ComboTypePurpose.SelectedIndex = 0;
            ComboBrand.SelectedIndex = 0;

            TBoxKon.Text = "";
            TBoxNach.Text = "";
            TBoxSearch.Text = "";
        }
        private void SelectItem(object sender, SelectionChangedEventArgs e)
        {
            var a = LViewProducts.SelectedItem as Product1;
            if (a != null)
            {
                MoreProdWindow more = new MoreProdWindow(a);
                more.ShowDialog();
            }
            LViewProducts.SelectedItem = null;
        }

        private void SelectPurpose(object sender, SelectionChangedEventArgs e)
        {
            ComboTypePurpose.SelectedItem = ListViewPurpose.SelectedItem;
        }

        private void BrandsPop(object sender, RoutedEventArgs e)
        {
            TopBrandsWindow diagrammBrandsWindow = new TopBrandsWindow();
            diagrammBrandsWindow.Show();
            Close();
        }

        private void SelectItemBest(object sender, SelectionChangedEventArgs e)
        {
            var b = LBestProduct.SelectedItem as Bests;
            if (b != null)
            {
                var c = products.ReturnProduct(Cosmetics.GetContext().Product.Where(x => x.name == b.Name).FirstOrDefault());
                MoreProdWindow more = new MoreProdWindow(c);
                more.ShowDialog();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(false);
            mainWindow.Show();
            Close();
        }
    }
}
