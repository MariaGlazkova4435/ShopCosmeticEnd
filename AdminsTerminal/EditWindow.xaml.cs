using Microsoft.Win32;
using ShopCosmetic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public EditWindow(Product selectedProduct, bool withClosingLast)
        {
            InitializeComponent();
            Closing = withClosingLast;
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            CpPurpose.ItemsSource = Cosmetics.GetContext().TypePurpose.ToList();
            CpBrand.ItemsSource = Cosmetics.GetContext().Brand.ToList();
            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                Skidka.IsChecked = _currentProduct.promotion;
            }
            else
            {
                Skidka.IsChecked = false;
                _currentProduct.promotion = false;
            }
            DataContext = _currentProduct;
            CpBrand.SelectedItem = _currentProduct.Brand1;
            CpPurpose.SelectedItem = _currentProduct.TypePurpose1;
        }
        private Product _currentProduct = new Product();
        bool Closing = false;
        private void Save(object sender, RoutedEventArgs e)
        {
            sav.Focus();
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentProduct.name))
                errors.AppendLine("Не введено название\nУкажите название продукта");
            if (_currentProduct.Brand1 == null)
                errors.AppendLine("Не введен бренд\nУкажите бренд продукта");
            if (_currentProduct.TypePurpose1 == null)
                errors.AppendLine("Не введено назначение\nУкажите назначение продукта");
            if (_currentProduct.sclad ==0 || _currentProduct.sclad == null)
                errors.AppendLine("Не введено количество в пачке!\nУкажите, сколько товара содержится в пачке");
            if (string.IsNullOrWhiteSpace(Convert.ToString(_currentProduct.price)))
                errors.AppendLine("Не введена цена\nУкажите цену");
            if (string.IsNullOrWhiteSpace(Sclad.Text))
                _currentProduct.sclad = 0;
            if ((string.IsNullOrWhiteSpace(Convert.ToString(Percent.Text)) || _currentProduct.promotionPercent == 0
                || string.IsNullOrWhiteSpace(Convert.ToString(_currentProduct.promotionPercent)))
                && _currentProduct.promotion == true)
                errors.AppendLine("Не введен процент акции\nВведите процент акции");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка получения данных");
                return;
            }
            if (_currentProduct.id == 0)
            {
                if (Cosmetics.GetContext().Product.Where(x => x.name == _currentProduct.name).FirstOrDefault() == null)
                    Cosmetics.GetContext().Product.Add(_currentProduct);
                else
                {
                    MessageBox.Show("Продукт с таким названием уже есть в продаже!", "Ошибка получения данных");
                    return;
                }
            }
            try
            {

                Cosmetics.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена", "Окно редактирования");
                if (Closing)
                {
                    MainWindow adminsWindow = new MainWindow(true);
                    adminsWindow.Show();
                    Close();
                }
                else
                    Close();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            if (Closing)
            {
                MainWindow adminsWindow = new MainWindow(false);
                adminsWindow.Show();
                Close();
            }
            else
                Close();
        }

        private void ChoosePhoto(object sender, RoutedEventArgs e)
        {
            string path = "";
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.jfif";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                path = myDialog.FileName;
                Phot.Text = path;
                _currentProduct.photo = path;
                BitmapImage biImg = new BitmapImage();
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
                biImg.BeginInit(); biImg.StreamSource = ms; biImg.EndInit();
                ImageSource imgSrc = biImg;
                _currentProduct.photoBinary = File.ReadAllBytes(path);
                Img.Source = imgSrc;
                Cosmetics.GetContext().SaveChanges();
                Phot.Focus();
                CpBrand.Focus();
            }
        }

        private void Skidka_Checked(object sender, RoutedEventArgs e)
        {
            Percent.IsEnabled = true;
            _currentProduct.promotion = true;
        }

        private void Skidka_Unchecked(object sender, RoutedEventArgs e)
        {
            _currentProduct.promotionPercent = null;
            _currentProduct.promotion = false;
            Percent.Text = "";
            Percent.IsEnabled = false;
        }

        private void Phot_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataContext = _currentProduct;
        }
    }
}
