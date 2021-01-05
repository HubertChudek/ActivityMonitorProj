using System;
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

namespace ActivityMonitor
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        //lista dni w danym miesiacu
        private List<WrapPanel> daysList = new List<WrapPanel>();

       //aktualna data
        private DateTime currentDate = DateTime.Today;

        public CalendarWindow()
        {
            InitializeComponent();
            DisplayCalendar();
        }

        //metoda wyświetlająca kalendarz
        private void DisplayCalendar()  
        {
            GenerateDayPanel(42);
            AddDayLabelToWrap(GetFirstDayOfCurrentDate(), GetTotalDaysOfCurrentDate());
            DisplayCurrentDate();
        }

        //metoda zwracająca pierwszy dzień miesiąca
        private int GetFirstDayOfCurrentDate()
        {
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            int result = (int) firstDayOfMonth.DayOfWeek;
            if (result == 0)
            {
                result = 7;
            }
            return result;
        }

        //metoda zwracająca liczbe dni w miesiacu
        private int GetTotalDaysOfCurrentDate()
        {
            DateTime firstDayOfCurrentDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            return firstDayOfCurrentDate.AddMonths(1).AddDays(-1).Day;
        }

        //metoda wyświetlająca dany miesiąc
        private void DisplayCurrentDate()
        {
            labelMonthAndYear.Content = currentDate.ToString("MMMM, yyyy");
            AddDayLabelToWrap(GetFirstDayOfCurrentDate(), GetTotalDaysOfCurrentDate() + GetFirstDayOfCurrentDate() - 1);

        }

        //metoda ustawuająca miesiąc na poprzedni
        private void PreviousMonth()
        {
            currentDate = currentDate.AddMonths(-1);
            DisplayCurrentDate();
        }

        //metoda ustawiająca miesiąc na następny
        private void NextMonth()
        {
            currentDate = currentDate.AddMonths(1);
            DisplayCurrentDate();
        }

        //metoda ustawiająca miesiąc na aktualny
        private void Today()
        {
            currentDate = DateTime.Today;
            DisplayCurrentDate();
        }



        //metoda generująca dni tygodnia danego miesiąca
        private void GenerateDayPanel(int totalDays)
        {
            daysPanel.Children.Clear();
            daysList.Clear();
            WrapPanel wrap;
            Border border;
            for (int i = 1; i <= totalDays; i++)
            {
                wrap = new WrapPanel();
                wrap.Name = $"wrap{i}";
                wrap.ItemWidth = 200;
                wrap.ItemHeight = 100;
                border = new Border();
                border.Name = $"border{i}";
                border.Width = 200;
                border.Height = 100;
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.Child = wrap;
                daysPanel.Children.Add(border);
                daysList.Add(wrap);
            }
        }

        //metoda dodająca labele z numerami dni miesiąca
        private void AddDayLabelToWrap(int startDayAtPanel, int totalDaysInMonth)
        {
            foreach (WrapPanel wrap in daysList)
            {
                wrap.Children.Clear();
            }
            Label lab;
            for(int i = 0; i < startDayAtPanel - 1; i++)
            {
                lab = new Label();
                lab.Name = $"emptyLabel{i+1}";
                lab.Content = "";
                daysList[i].Children.Add(lab);
            }

            //przypisanie numerów dnia
            for (int i = startDayAtPanel; i <= totalDaysInMonth; i++)
            {
                lab = new Label();
                lab.Name = $"lblDay{i - startDayAtPanel + 1}";
                lab.Content = i - startDayAtPanel + 1;
                lab.HorizontalContentAlignment = HorizontalAlignment.Right;
                daysList[i - 1].Children.Add(lab);
            }
        }

        
        private void ButtonPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            PreviousMonth();
        }

        private void ButtonNextMonth_Click(object sender, RoutedEventArgs e)
        {
            NextMonth();
        }

        private void ButtonToday_Click(object sender, RoutedEventArgs e)
        {
            Today();
        }

    }
}
