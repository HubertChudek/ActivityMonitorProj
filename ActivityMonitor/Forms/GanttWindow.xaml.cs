using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using nGantt.GanttChart;
using nGantt.PeriodSplitter;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

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

        public GanttWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GantLenght = 1;
            dateTimePicker.Value = DateTime.Parse("2012-02-01");
            //eTimePicker.DisplayDate = DateTime.Today;
            DateTime minDate = (DateTime)dateTimePicker.Value;
            DateTime maxDate = minDate.AddDays(GantLenght);

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
            MessageBox.Show("Edit clicked for task " + ganttTask.Name);
        }

        private void DeleteClicked(GanttTask ganttTask)
        {
            MessageBox.Show("Delete clicked for task " + ganttTask.Name);
        }

        void ganttControl1_GanttRowAreaSelected(object sender, PeriodEventArgs e)
        {
            MessageBox.Show(e.SelectionStart.ToString() + " -> " + e.SelectionEnd.ToString());
        }

        private Brush DetermineBackground(TimeLineItem timeLineItem)
        {
            if (timeLineItem.End.Date.DayOfWeek == DayOfWeek.Saturday || timeLineItem.End.Date.DayOfWeek == DayOfWeek.Sunday)
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            else
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }

        private void CreateData(DateTime minDate, DateTime maxDate)
        {
            // Set max and min dates
            ganttControl1.Initialize(minDate, maxDate);

            // Create timelines and define how they should be presented
            //ganttControl1.CreateTimeLine(new PeriodYearSplitter(minDate, maxDate), FormatYear);
            //ganttControl1.CreateTimeLine(new PeriodMonthSplitter(minDate, maxDate), FormatMonth);
            //var gridLineTimeLine = ganttControl1.CreateTimeLine(new PeriodDaySplitter(minDate, maxDate), FormatDay);
            //ganttControl1.CreateTimeLine(new PeriodDaySplitter(minDate, maxDate), FormatDayName);
            var gridLineTimeLine = ganttControl1.CreateTimeLine(new PeriodHourSplitter(minDate, maxDate), FormatHour);

            // Set the timeline to atatch gridlines to
            ganttControl1.SetGridLinesTimeline(gridLineTimeLine, DetermineBackground);

            // Create and data
            var rowgroup1 = ganttControl1.CreateGanttRowGroup("HeaderdGanttRowGroup");
            var row1 = ganttControl1.CreateGanttRow(rowgroup1, "GanttRow 1");
            ganttControl1.AddGanttTask(row1, new GanttTask() { Start = DateTime.Parse("2012-02-01"), End = DateTime.Parse("2012-03-01"), Name = "GanttRow 1:GanttTask 1", TaskProgressVisibility = System.Windows.Visibility.Hidden });
            ganttControl1.AddGanttTask(row1, new GanttTask() { Start = DateTime.Parse("2012-03-05"), End = DateTime.Parse("2012-05-01"), Name = "GanttRow 1:GanttTask 2" });
            ganttControl1.AddGanttTask(row1, new GanttTask() { Start = DateTime.Parse("2012-06-01"), End = DateTime.Parse("2012-06-15"), Name = "GanttRow 1:GanttTask 3" });

            var rowgroup2 = ganttControl1.CreateGanttRowGroup("ExpandableGanttRowGroup", true);
            var row2 = ganttControl1.CreateGanttRow(rowgroup2, "GanttRow 2");
            var row3 = ganttControl1.CreateGanttRow(rowgroup2, "GanttRow 3");
            ganttControl1.AddGanttTask(row2, new GanttTask() { Start = DateTime.Parse("2012-02-10"), End = DateTime.Parse("2012-03-10"), Name = "GanttRow 2:GanttTask 1" });
            ganttControl1.AddGanttTask(row2, new GanttTask() { Start = DateTime.Parse("2012-03-25"), End = DateTime.Parse("2012-05-10"), Name = "GanttRow 2:GanttTask 2" });
            ganttControl1.AddGanttTask(row2, new GanttTask() { Start = DateTime.Parse("2012-06-10"), End = DateTime.Parse("2012-09-15"), Name = "GanttRow 2:GanttTask 3", PercentageCompleted = 0.375 });
            ganttControl1.AddGanttTask(row3, new GanttTask() { Start = DateTime.Parse("2012-01-07"), End = DateTime.Parse("2012-09-15"), Name = "GanttRow 3:GanttTask 1", PercentageCompleted = 0.5 });

            var rowgroup3 = ganttControl1.CreateGanttRowGroup();
            var row4 = ganttControl1.CreateGanttRow(rowgroup3, "GanttRow 4");
            ganttControl1.AddGanttTask(row4, new GanttTask() { Start = DateTime.Parse("2012-02-14"), End = DateTime.Parse("2012-02-27"), Name = "GanttRow 4:GanttTask 1", PercentageCompleted = 1 });
            ganttControl1.AddGanttTask(row4, new GanttTask() { Start = DateTime.Parse("2012-04-8"), End = DateTime.Parse("2012-09-19"), Name = "GanttRow 4:GanttTask 2" });
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

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            dateTimePicker.Value = ganttControl1.GanttData.MinDate.AddDays(-GantLenght);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            dateTimePicker.Value = ganttControl1.GanttData.MaxDate;
        }

        private void DateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DateTime minDate = (DateTime)dateTimePicker.Value;
            DateTime maxDate = minDate.AddDays(GantLenght);
            ganttControl1.ClearGantt();
            CreateData(minDate, maxDate);
        }
    }
}
