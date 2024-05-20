using System;
using ShopCosmetic;
using System.Collections.Generic;
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
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для autorizAdmin.xaml
    /// </summary>
    public partial class autorizAdmin : Window
    {
        ChangoPhotoPath path = new ChangoPhotoPath();
        public autorizAdmin()
        {
            InitializeComponent();
            //photoPath.Change();
            byte[] Logo = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 1).SingleOrDefault().photoBinary;
            LogoIm.Source = path.ByteToImage(Logo);
            byte[] Back = Cosmetics.GetContext().OtherPhoto.Where(x => x.idPhoto == 3).SingleOrDefault().photoBinary;
            BackIm.Source = path.ByteToImage(Back);
        }
        ChangoPhotoPath photoPath = new ChangoPhotoPath();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            //if (!String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(Login.Text))
            //{
                //string password = Password.Text.Trim();
                //byte[] tmpSource = Encoding.ASCII.GetBytes(password);
                //byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                //string rez1 = BitConverter.ToString(tmpHash).Replace("-", string.Empty).ToLower();
                //var admin = Cosmetics.GetContext().Admin.Where(r => r.login == Login.Text && r.password == rez1).FirstOrDefault();
                //if (admin != null)
                //{
                    MainWindow main = new MainWindow(false);
                    main.Show();
                    Close();
                //}
                //else
                //    MessageBox.Show("Пароль или логин неверны!");
            //}
            //else
            //    MessageBox.Show("Введите значения во все поля!");
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecoveryWindow passwordRecoveryWindow = new RecoveryWindow();
            passwordRecoveryWindow.Show();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
