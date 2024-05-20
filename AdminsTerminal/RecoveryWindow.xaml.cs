using ShopCosmetic;
using System.Linq;
using System.Windows;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для RecoveryWindow.xaml
    /// </summary>
    public partial class RecoveryWindow : Window
    {
        public RecoveryWindow()
        {
            InitializeComponent();
        }

        private void Kod(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(Log.Text))
            {
                if (poEmail)
                {
                    Admin ad = Cosmetics.GetContext().Admin.Where(x => x.email == Log.Text.Trim()).FirstOrDefault();
                    if (ad != null)
                    {
                        RecoveryKodwindow kodWindow = new RecoveryKodwindow(ad);
                        kodWindow.Show();
                        Close();
                    }
                    else
                        MessageBox.Show("Логина с такой почтой не существует!");
                }
                else
                {
                    Admin ad = Cosmetics.GetContext().Admin.Where(x => x.login == Log.Text.Trim()).FirstOrDefault();
                    if (ad != null)
                    {
                        RecoveryKodwindow kodWindow = new RecoveryKodwindow(ad);
                        kodWindow.Show();
                        Close();
                    }
                    else
                        MessageBox.Show("Логина не существует!");
                }
            }
            else
                MessageBox.Show("Введите логин!");
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            autorizAdmin autorization = new autorizAdmin();
            autorization.Activate();
            Close();
        }
        bool poEmail = false;

        private void Change(object sender, RoutedEventArgs e)
        {
            if (poEmail == false)
            {
                poEmail = true;
                Vvedite.Text = "Введите почту:";
                Chang.Content = "Войти по логину";
            }
            else
            {
                poEmail = false;
                Vvedite.Text = "Введите логин:";
                Chang.Content = "Войти по email";
            }
        }
    }
}
