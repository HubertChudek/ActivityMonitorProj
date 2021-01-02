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
            //AddDayLabelToWrap(GetFirstDayOfCurrentDate(), GetTotalDaysOfCurrentDate());
            DisplayCurrentDate();
        }

        //metoda 
        private int GetFirstDayOfCurrentDate()
        {
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            return (int) firstDayOfMonth.DayOfWeek + 1;
        }

        private int GetTotalDaysOfCurrentDate()
        {
            DateTime firstDayOfCurrentDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            return firstDayOfCurrentDate.AddMonths(1).AddDays(-1).Day;
        }

        private void DisplayCurrentDate()
        {
            labelMonthAndYear.Content = currentDate.ToString("MMMM, yyyy");
            AddDayLabelToWrap(GetFirstDayOfCurrentDate(), GetTotalDaysOfCurrentDate());

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
            for (int i = 1, loopTo = totalDays; i <= loopTo; i++)
            {
                var wrap = new WrapPanel();
                //var lab = new Label();
                wrap.Name = $"wrap{i}";
                //lab.Content = $"{i}";
                //lab.HorizontalContentAlignment = HorizontalAlignment.Right;
                //wrap.Children.Add(lab);
                wrap.ItemWidth = 200;
                wrap.ItemHeight = 200;
                wrap.Background = new SolidColorBrush(Colors.Orange);
                daysPanel.Children.Add(wrap);
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
            //int day = 1;
            for (int i = 1, loopTo = totalDaysInMonth; i <= loopTo; i++)
            {
                var lab = new Label();
                lab.Name = $"lblDay{i}";
                lab.Content = i;
                //lab.Width = 200;
                //lab.Height = 103;
                lab.HorizontalContentAlignment = HorizontalAlignment.Right;
                //daysList[(i - 1) + (startDayAtPanel - 1)].Children.Clear();
                daysList[(i - 1) + (startDayAtPanel - 1)].Children.Add(lab);
                //day += 1;
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
