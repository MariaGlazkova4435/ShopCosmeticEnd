using ShopCosmetic;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace AdminsTerminal
{
    /// <summary>
    /// Логика взаимодействия для SelectDateWindow.xaml
    /// </summary>
    public partial class SelectDateWindow : Window
    {
        public SelectDateWindow(Order ord)
        {
            InitializeComponent();
            o = ord;
            List<string> dates = new List<string>();
            DateTime date = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                dates.Add(date.AddDays(1 + i).ToString("dd.MM.yyyy"));
            }
            CbDates.ItemsSource = dates.ToList();
            CbDates.SelectedIndex = 0;
        }
        Order o;
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите изменить дату доставки заказа №{o.idOrder}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                o.dateHand = Convert.ToDateTime(CbDates.SelectedValue.ToString());
                Cosmetics.GetContext().SaveChanges();
                MessageBox.Show("Дата доставки изменена!", "Успешное выполнение задачи");
                Close();
            }
        }
    }
}
