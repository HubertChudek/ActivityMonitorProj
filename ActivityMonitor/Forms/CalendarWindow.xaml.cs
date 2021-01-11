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
using System.Data;

namespace ActivityMonitor
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {

        //lista dni w danym miesiacu
        private List<StackPanel> daysList = new List<StackPanel>();
        private DatabaseManager dm = new DatabaseManager();

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
            DisplayCurrentDate();
        }

        //metoda dodająca nową aktywnosc
        private void AddNewActivity()
        {
            ActivityMonitor.Forms.FormChoose chooseWindow = new Forms.FormChoose();
            chooseWindow.ShowDialog();
            DisplayCurrentDate();
        }
/*
        //metoda wyświetlająca aktywności w kalendarzu                            !!!!!!!!!!!!!!  NIE DZIAŁA POPRAWNIEE  !!!!!!!!!!!!!!!
        private void AddActivityToWrapPanel(int startDayAtWrapNumber)
        {
            string format = "dd-MM-yyyy";
            DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            String sql = $"SELECT * FROM activity WHERE AppDate BETWEEN #{startDate.ToString(format)}# AND #{endDate.ToString(format)}#";
            DataTable dt = dm.QueryAsDataTable(sql);

            foreach(DataRow row in dt.Rows)
            {
                String date = row["AppDate"].ToString();
                DateTime appDay = DateTime.Parse(date);
                Button activity = new Button();
                activity.Name = $"activity{row["ID"]}";
                activity.Content = row["Type"];
                //activity.Visibility = Visibility.Visible;
                activity.HorizontalContentAlignment = HorizontalAlignment.Center;

                daysList[(appDay.Day -1) + (startDayAtWrapNumber - 1)].Children.Add(activity);
            }
        }
*/     
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
            int firstDayAtWrapNumber = GetFirstDayOfCurrentDate();
            int totalDays = GetTotalDaysOfCurrentDate() + GetFirstDayOfCurrentDate() - 1;
            labelMonthAndYear.Content = currentDate.ToString("MMMM, yyyy");
            AddDayLabelToWrap(firstDayAtWrapNumber, totalDays);
            //AddActivityToWrapPanel(firstDayAtWrapNumber);   
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
            StackPanel wrap;
            Border border;
            for (int i = 1; i <= totalDays; i++)
            {

                wrap = new StackPanel();
                wrap.Name = $"wrap{i}"; 
                wrap.Width = 200;   
                wrap.Height = 120;
                wrap.Cursor = Cursors.Hand;
                wrap.MouseDown += new MouseButtonEventHandler(this.panelClick);
                border = new Border();
                border.Name = $"border{i}";
                border.Width = 200;
                border.Height = 120;
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
            DataTable dtActivity = new DataTable();
            DataTable dtMeal = new DataTable();

            dtActivity.Clear();
            dtMeal.Clear();


            int firstDayAtWrapNumber = GetFirstDayOfCurrentDate();
            foreach (StackPanel wrap in daysList)
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

            //przypisanie numerów dnia i aktywnosci do danego dnia
            for (int i = startDayAtPanel; i <= totalDaysInMonth; i++)
            {
                lab = new Label();
                lab.Name = $"lblDay{i - startDayAtPanel + 1}";
                lab.Content = i - startDayAtPanel + 1;
                lab.VerticalAlignment = VerticalAlignment.Top;
                lab.HorizontalContentAlignment = HorizontalAlignment.Right;
                daysList[i - 1].Children.Add(lab);

                string format = "dd-MM-yyyy";
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                String sqlActivity = $"SELECT * FROM activity WHERE AppDate BETWEEN #{startDate.ToString(format)}# AND #{endDate.ToString(format)}#";
                String sqlMeal = $"SELECT * FROM meal WHERE AppDate BETWEEN #{startDate.ToString(format)}# AND #{endDate.ToString(format)}#";

                dtActivity = dm.QueryAsDataTable(sqlActivity);
                dtMeal = dm.QueryAsDataTable(sqlMeal);

                addActivity(dtActivity, i, firstDayAtWrapNumber);
                addActivity(dtMeal, i, firstDayAtWrapNumber);
            }
        }
        
        private void addActivity(DataTable dt, int iterator, int firstDay)
        {
            foreach (DataRow row in dt.Rows)
            {
                String date = row["AppDate"].ToString();
                DateTime appDay = DateTime.Parse(date);
                if (appDay.Day == iterator && appDay.Month == currentDate.Month)
                {
                    Button activity = new Button();
                    activity.Height = 30;
                    activity.Width = 100;
                    //activity.Name = $"activity{row["ID"]}";
                    activity.Content = row["Type"];
                    activity.HorizontalAlignment = HorizontalAlignment.Center;
                    activity.VerticalAlignment = VerticalAlignment.Bottom;


                    daysList[(appDay.Day - 1) + (firstDay - 1)].Children.Add(activity);
                }
                else
                {
                    continue;
                }
            }
        }
        
        private void panelClick(object sender, RoutedEventArgs e)
        {
            AddNewActivity();
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

        private void ButtonAddEvent_Click(object sender, RoutedEventArgs e)
        {
            AddNewActivity();
        }
    }
}
