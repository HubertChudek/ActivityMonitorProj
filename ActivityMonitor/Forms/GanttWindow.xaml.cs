using nGantt.GanttChart;
using nGantt.PeriodSplitter;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using ExToolkit = Xceed.Wpf.Toolkit;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy GanttWindow.xaml
    /// </summary>
    public partial class GanttWindow : Window
    {
        private int GantLenght { get; set; }
        private ObservableCollection<ContextMenuItem> ganttTaskContextMenuItems = new ObservableCollection<ContextMenuItem>();
        private ObservableCollection<SelectionContextMenuItem> selectionContextMenuItems = new ObservableCollection<SelectionContextMenuItem>();
        private DateTime startUpDate;

        public GanttWindow()
        {
            InitializeComponent();
        }

        public GanttWindow(DateTime calendarDate)
        {
            InitializeComponent();
            startUpDate = calendarDate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GantLenght = 24;
            dateTimePicker.Value = startUpDate;
            DateTime minDate = (DateTime)dateTimePicker.Value;
            DateTime maxDate = minDate.AddHours(GantLenght);

            // Set selection -mode
            ganttControl1.TaskSelectionMode = nGantt.GanttControl.SelectionMode.Single;
            // Enable GanttTasks to be selected
            ganttControl1.AllowUserSelection = true;

            // listen to the GanttRowAreaSelected event
            ganttControl1.GanttRowAreaSelected += new EventHandler<PeriodEventArgs>(ganttControl1_GanttRowAreaSelected);

            // define ganttTask context menu and action when each item is clicked
            ganttTaskContextMenuItems.Add(new ContextMenuItem(ViewClicked, "View..."));
            ganttTaskContextMenuItems.Add(new ContextMenuItem(EditClicked, "Edit..."));
            ganttTaskContextMenuItems.Add(new ContextMenuItem(DeleteClicked, "Delete..."));
            ganttControl1.GanttTaskContextMenuItems = ganttTaskContextMenuItems;

            // define selection context menu and action when each item is clicked
            selectionContextMenuItems.Add(new SelectionContextMenuItem(NewClicked, "New..."));
            ganttControl1.SelectionContextMenuItems = selectionContextMenuItems;
        }

        private void NewClicked(Period selectionPeriod)
        {
            MessageBox.Show("New clicked for task " + selectionPeriod.Start.ToString() + " -> " + selectionPeriod.End.ToString());
        }

        private void ViewClicked(GanttTask ganttTask)
        {
            MessageBox.Show("New clicked for task " + ganttTask.Name);
        }


        private void EditClicked(GanttTask ganttTask)
        {
            //MessageBox.Show("Edit clicked for task " + ganttTask.Name);
            ShowEventDetail(ganttTask.AppID, ganttTask.Table);
        }

        private void DeleteClicked(GanttTask ganttTask)
        {
            //MessageBox.Show("Delete clicked for task " + ganttTask.Name);
            DeleteActivity(ganttTask.AppID, ganttTask.Table);
            Keyboard.ClearFocus();
        }

        private void ganttControl1_GanttRowAreaSelected(object sender, PeriodEventArgs e)
        {
            MessageBox.Show(e.SelectionStart.ToString() + " -> " + e.SelectionEnd.ToString());
        }

        private Brush DetermineBackground(TimeLineItem timeLineItem)
        {
            if (timeLineItem.End.Date.DayOfWeek == DayOfWeek.Saturday || timeLineItem.End.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightCoral);
            }
            else
            {
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightSteelBlue);
            }
        }

        private void ShowEventDetail(int appID, string table)
        {
            string sql = $"SELECT * FROM {table} WHERE ID = {appID}";
            DatabaseManager dbManager = new DatabaseManager();
            DataTable dt = dbManager.QueryAsDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (table == "activity")
                {
                    FormActivityWindow eventForm = new FormActivityWindow(appID);
                    eventForm.FillFieldsFromDataRow(row);
                    eventForm.Show();
                }
                else if (table == "meal")
                {
                    FormMealWindow eventForm = new FormMealWindow(appID);
                    eventForm.FillFieldsFromDataRow(row);
                    eventForm.Show();
                }
                else
                {
                    MessageBox.Show("No such table");
                }
            }
        }

        private void DeleteActivity(int id, string table)
        {
            MessageBoxResult confirmResult = ExToolkit.MessageBox.Show("Are you sure to delete?",
                "Please confirm.",
                MessageBoxButton.YesNo);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            string sql =
                $"delete from {table} where ID = {id}";
            DatabaseManager dbManager = new DatabaseManager();
            dbManager.InsertUpdateDelete(sql);
        }

        private void CreateData(DateTime minDate, DateTime maxDate)
        {
            // Set max and min dates
            ganttControl1.Initialize(minDate, maxDate);

            // Create timelines and define how they should be presented
            TimeLine gridLineTimeLine = ganttControl1.CreateTimeLine(new PeriodHourSplitter(minDate, maxDate), FormatHour);

            // Set the timeline to atatch gridlines to
            ganttControl1.SetGridLinesTimeline(gridLineTimeLine, DetermineBackground);

            // Create rows and data
            GanttRowGroup rowgroup1 = ganttControl1.CreateGanttRowGroup();
            GanttRow row1 = ganttControl1.CreateGanttRow(rowgroup1, "Training");
            GanttRow row2 = ganttControl1.CreateGanttRow(rowgroup1, "Meal");
            GenerateEventsFromTable(row1, minDate, maxDate, "activity");
            GenerateEventsFromTable(row2, minDate, maxDate, "meal");
        }

        private void GenerateEventsFromTable(GanttRow row, DateTime minDate, DateTime maxDate, string tableName)
        {
            string dateFormat = "yyyy-MM-dd";
            string sql = $"SELECT * FROM {tableName} WHERE AppDate BETWEEN #{minDate.ToString(dateFormat)}# AND #{maxDate.ToString(dateFormat)}#";
            DatabaseManager dbManager = new DatabaseManager();
            DataTable dt = dbManager.QueryAsDataTable(sql);

            foreach (DataRow dataRow in dt.Rows)
            {
                string date = dataRow["AppDate"].ToString();
                string startTime = dataRow["StartTime"].ToString();
                string endTime = dataRow["EndTime"].ToString();
                string id = dataRow["ID"].ToString();
                string eventColor = eventColor = Colors.Beige.ToString(); ;

                DateTime startDay = DateTime.Parse(startTime);
                DateTime entDay = DateTime.Parse(endTime);
                string eventName = " ";
                if (tableName == "activity")
                {
                    eventName = dataRow["Type"].ToString();
                    eventColor = Colors.DeepSkyBlue.ToString();
                }
                else if (tableName == "meal")
                {
                    eventName = dataRow["Type"].ToString() + ": " + dataRow["MealName"].ToString();
                    eventColor = Colors.GreenYellow.ToString();
                }

                ganttControl1.AddGanttTask(row, new GanttTask()
                {
                    Start = startDay,
                    End = entDay,
                    Name = eventName,
                    Table = tableName,
                    AppID = int.Parse(id),
                    Color = eventColor
                });
            }
        }

        private void DateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshEvents(); ;
        }

        private void Window_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (ganttControl1.IsLoaded)
            {
                RefreshEvents();
            }
        }

        private void RefreshEvents()
        {
            DateTime minDate = (DateTime) dateTimePicker.Value;
            DateTime maxDate = minDate.AddHours(GantLenght);
            ganttControl1.ClearGantt();
            CreateData(minDate, maxDate);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            dateTimePicker.Value = ganttControl1.GanttData.MinDate.AddHours(-GantLenght);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            dateTimePicker.Value = ganttControl1.GanttData.MaxDate;
        }

        private string FormatYear(Period period)
        {
            return period.Start.Year.ToString();
        }

        private string FormatMonth(Period period)
        {
            return period.Start.Month.ToString();
        }

        private string FormatDay(Period period)
        {
            return period.Start.Day.ToString();
        }

        private string FormatDayName(Period period)
        {
            return period.Start.DayOfWeek.ToString();
        }

        private string FormatHour(Period period)
        {
            return period.Start.Hour.ToString();
        }
    }
}
