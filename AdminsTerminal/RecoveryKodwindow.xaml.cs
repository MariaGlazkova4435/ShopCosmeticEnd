using ShopCosmetic;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для RecoveryKodwindow.xaml
    /// </summary>
    public partial class RecoveryKodwindow : Window
    {
        public RecoveryKodwindow(Admin admin)
        {
            InitializeComponent();
            Grid1.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Email.Text = admin.email;
            ad = admin;
            StartTimer();
            CreateTimeoutTestMessage(admin);
        }
        async void StartTimer()
        {
            AgainButton.Visibility = Visibility.Collapsed;
            AgainSend.Text = "Повторная отправка через 00:30 c";
            int startTime = 30; // Задаем начальное значение таймера
            while (startTime > 0)
            {
                startTime--; // Уменьшаем значение таймера на 1
                string time;
                await Task.Delay(1000); // Ждем одну секунду
                if (startTime < 10) // Переводим время в нужный формат
                    time = $"00:0{startTime % 60}";
                else
                    time = $"00:{startTime % 60}";
                // Выводим время на экран
                AgainSend.Text = "Повторная отправка через " + time + " c";
            }
            AgainButton.Visibility = Visibility.Visible;
        }

        Admin ad;
        double randomKod;

        private void Back(object sender, RoutedEventArgs e)
        {
            autorizAdmin autorization = new autorizAdmin();
            autorization.Activate();
            Close();
        }

        private void Again(object sender, RoutedEventArgs e)
        {
            StartTimer();
            CreateTimeoutTestMessage(ad);
        }

        private void Check(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(KodFromEmail.Text))
            {
                if (randomKod.ToString() == KodFromEmail.Text.Trim())
                {
                    Grid2.Visibility = Visibility.Visible;
                    Grid1.Visibility = Visibility.Collapsed;
                }
                else
                    MessageBox.Show("Неверный код!");
            }
            else
                MessageBox.Show("Введите код из письма!");
        }

        private void Change(object sender, RoutedEventArgs e)
        {
            RecoveryWindow passwordRecoveryWindow = new RecoveryWindow();
            passwordRecoveryWindow.Show();
            Close();
        }

        public void CreateTimeoutTestMessage(Admin ad)
        {
            Random random = new Random();
            randomKod = random.Next(100000, 1000000);
            try
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
                MailAddress to = new MailAddress(ad.email);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyTo = new MailAddress("glazkovamaria5898@mail.ru");
                myMail.ReplyToList.Add(replyTo);

                // set subject and encoding
                myMail.Subject = "Восстановление пароля";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "Здравствуйте!\r\n" +
                    "\r\nЧтобы восстановить доступ к вашему аккаунту, пожалуйста, введите данный код: " + randomKod +
                    "\r\nЕсли у вас возникнут дополнительные вопросы, обратитесь к администратору магазина." +
                    "\r\nС уважением," +
                    "\r\nМагазин декоративной косметики \"GracefulShine\"";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html

                mySmtpClient.Send(myMail);
            }

            catch (SmtpException ex)
            { throw new ApplicationException("SmtpException has occured: " + ex.Message); }
            catch (Exception ex)
            { throw ex; }
        }

        private void SaveNewPass(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(NewPass.Text))
            {
                string password = NewPass.Text.Trim();
                if (password.Length >= 8)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    if (!password.Any(char.IsUpper))
                        stringBuilder.AppendLine("Пароль не содержит заглавных букв.");
                    if (!password.Any(char.IsLower))
                        stringBuilder.AppendLine("Пароль не содержит строчных букв.");
                    bool dig = false;
                    foreach (char c in password)
                    {
                        if (Char.IsDigit(c))
                        {
                            dig = true;
                            break;
                        }
                    }
                    if (!dig)
                        stringBuilder.AppendLine("Пароль не содержит цифр.");
                    if (!new[] { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "[", "]", "{", "}", ";", ":", "'", "\"", "<", ">", ",", ".", "?", "/", "\\", "|" }
                    .Any(c => password.Contains(c)))
                    {
                        stringBuilder.AppendLine("Пароль не содержит специальных символов.");
                    }
                    if (stringBuilder.Length <= 0)
                    {
                        byte[] tmpSource = Encoding.ASCII.GetBytes(password);
                        byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                        string rez1 = BitConverter.ToString(tmpHash).Replace("-", string.Empty).ToLower();
                        if (rez1 != ad.password)
                        {
                            ad.password = rez1;
                            Cosmetics.GetContext().SaveChanges();
                            MessageBox.Show("Пароль изменен!");
                            autorizAdmin autorization = new autorizAdmin();
                            autorization.Activate();
                            Close();
                        }
                        else
                            MessageBox.Show("Нельзя использовать старый пароль!");
                    }
                    else
                        MessageBox.Show("Пароль не соответствует требованиям:\n" + stringBuilder.ToString());
                }
                else
                    MessageBox.Show("Пароль должен содержать не меньше 8 символов!");
            }
            else
                MessageBox.Show("Введите новый пароль!");
        }
    }
}
