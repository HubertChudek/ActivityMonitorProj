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
using System.Diagnostics;
using System.Net;
using ActivityMonitor.Forms;

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
        private int month = DateTime.Today.Month;

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
            FormActivityWindow chooseWindow = new Forms.FormActivityWindow();
            chooseWindow.ShowDialog();
            DisplayCurrentDate();
        }

        private void AddNewMeal()
        {
            ActivityMonitor.Forms.FormMealWindow chooseWindow = new Forms.FormMealWindow();
            chooseWindow.ShowDialog();
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
            int firstDayAtWrapNumber = GetFirstDayOfCurrentDate();
            int totalDays = GetTotalDaysOfCurrentDate() + GetFirstDayOfCurrentDate() - 1;
            labelMonthAndYear.Content = currentDate.ToString("MMMM, yyyy");
            AddDayLabelToWrap(firstDayAtWrapNumber, totalDays);
            //colorCurrentDay(firstDayAtWrapNumber, totalDays);  
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
            Ellipse circle;

            for (int i = 1; i <= totalDays; i++)
            {
            
                wrap = new StackPanel();
                wrap.Name = $"wrap{i}"; 
                wrap.Width = 200;   
                wrap.Height = 120;
                //wrap.Cursor = Cursors.Cross;
                //wrap.MouseDown += new MouseButtonEventHandler(this.ButtonAddActivity_Click);


                border = new Border();
                border.Name = $"border{i}";
                border.Width = 200;
                border.Height = 120;
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.Child = wrap;

                daysPanel.Children.Add(border);
                daysList.Add(wrap);
/*
                foreach (Object obj in daysList[i - 1].Children)
                {
                    if (obj is Label)
                    {
                        daysList[i - 1].Cursor = Cursors.Cross;
                        daysList[i - 1].MouseDown += new MouseButtonEventHandler(this.ButtonAddActivity_Click);
                    }
                }
*/
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
                wrap.Background = new SolidColorBrush(Colors.White);
            }
            Label lab;

            //przypisanie numerów dnia do danego dnia
            for (int i = startDayAtPanel; i <= totalDaysInMonth; i++)
            {
                lab = new Label();
                lab.Background = Brushes.Aqua;
                if (i - startDayAtPanel + 1 == DateTime.Today.Day && currentDate.Month == month)
                {
                    
                    lab.Background = new SolidColorBrush(Colors.Red);
                }
                lab.Name = $"lblDay{i - startDayAtPanel + 1}";
                lab.Content = i - startDayAtPanel + 1;
                lab.VerticalAlignment = VerticalAlignment.Top;
                lab.HorizontalContentAlignment = HorizontalAlignment.Right;
                lab.Cursor = Cursors.Hand;
                lab.MouseDown += new MouseButtonEventHandler(this.panelClick);
                daysList[i - 1].Children.Add(lab);
            }
            //przypisanie aktywnosci do danego dnia
            for (int i = startDayAtPanel; i <= totalDaysInMonth; i++)
            {
                string format = "dd-MM-yyyy";
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                String sqlActivity = $"SELECT * FROM activity WHERE AppDate BETWEEN #{startDate.ToString(format)}# AND #{endDate.ToString(format)}#";
                String sqlMeal = $"SELECT * FROM meal WHERE AppDate BETWEEN #{startDate.ToString(format)}# AND #{endDate.ToString(format)}#";

                dtActivity = dm.QueryAsDataTable(sqlActivity);
                dtMeal = dm.QueryAsDataTable(sqlMeal);

                addActivity(dtActivity, "activity", i, firstDayAtWrapNumber);
                addActivity(dtMeal, "meal", i, firstDayAtWrapNumber);
            }
        }

        //metoda wyświetlająca aktywnosci w kalendarzu
        private void addActivity(DataTable dt, string dataType, int iterator, int firstDay)
        {
            foreach (DataRow row in dt.Rows)
            {   
                String date = row["AppDate"].ToString();
                DateTime appDay = DateTime.Parse(date);
                if (appDay.Day == iterator && appDay.Month == currentDate.Month)
                {
                    Button activity = new Button();
                    activity.Height = 20;
                    activity.Width = 100;
                    //activity.Name = $"activity{row["ID"]}";
                    activity.Content = row["Type"];
                    activity.VerticalContentAlignment = VerticalAlignment.Center;
                    activity.HorizontalAlignment = HorizontalAlignment.Center;
                    activity.VerticalAlignment = VerticalAlignment.Bottom;

                    daysList[(appDay.Day - 1) + (firstDay - 1)].Children.Add(activity);

                    if (dataType == "activity")
                    {
                        activity.Background = new SolidColorBrush(Colors.Blue);
                    }
                    else if(dataType == "meal")
                    {
                        activity.Background = new SolidColorBrush(Colors.GreenYellow);
                    }
                }
                else
                {
                    continue;
                }

            }
        }
        
        private void panelClick(object sender, RoutedEventArgs e)
        {
            var label = (Label) sender;
            string dateString =
                $"{currentDate.Year.ToString()}-{currentDate.Month.ToString()}-{label.Content}";
            DateTime date = DateTime.Parse(dateString);
            GanttWindow ganttView = new GanttWindow(date);
            ganttView.Show();
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

        private void ButtonAddActivity_Click(object sender, RoutedEventArgs e)
        {
            AddNewActivity();
        }

        private void ButtonAddMeal_Click(object sender, RoutedEventArgs e)
        {
            AddNewMeal();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
