using System;
using System.Data;
using System.Windows;
using ExToolkit = Xceed.Wpf.Toolkit;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy FormActivityWindow.xaml
    /// </summary>
    public partial class FormActivityWindow : Window
    {
        //obiekt zarządzający bazą dancyh
        private DatabaseManager dbManager = new DatabaseManager();
        //identyfikator wiersza z bazy danych, 0 - nowe zdarzenie, <0 - zdarzenie pobrane z bazy
        private int AppId = 0;

        public FormActivityWindow()
        {
            InitializeComponent();
            InitializeControlsValues();
        }

        public FormActivityWindow(int id)
        {
            InitializeComponent();
            AppId = id;
            InitializeControlsValues();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ShowConnectionResult();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckIfInputCorrect())
            {
                return;
            }

            MessageBoxResult confirmResult = ExToolkit.MessageBox.Show("Are you sure to save?",
                "Please confirm.",
                MessageBoxButton.YesNo);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            if (AppId == 0)
            {
                InsertActivity();
            }
            else
            {
                UpdateActivity();
            }
        }
        
        //metoda aktualizująca informacje z formularza
        private void UpdateActivity()
        {
            string sql =
                $"update activity set AppDate = '{dtpDate.SelectedDate.Value.Date.ToShortDateString()}'," +
                                    $"StartTime = '{tpStartTime.Value}'," +
                                    $"EndTime = '{tpEndTime.Value}'," +
                                    $" Type = '{cbxType.Text}'" +
                                    $"where ID = {AppId}";
            bool updateStatus = dbManager.InsertUpdateDelete(sql);
            NotifyOperationStatus(updateStatus);
        }

        //metoda wprowadza do bazy informacje z formularza
        private void InsertActivity()
        {
            string sql =
                $"insert into activity(AppDate, StartTime, EndTime, Type) " +
                $"values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', " +
                $"'{tpStartTime.Value}' , '{tpEndTime.Value}', '{cbxType.Text}')";
            bool insertStatus = dbManager.InsertUpdateDelete(sql);
            NotifyOperationStatus(insertStatus);
        }
        
        //usuwa wiersz z bazy danych wg podanego ID
        private void DeleteActivity()
        {
            string sql =
                $"delete from activity where ID = {AppId}";
            bool deleteStatus = dbManager.InsertUpdateDelete(sql);
            NotifyOperationStatus(deleteStatus);
            Close();
        }

        //okienko powiadomienia czy operacja się udała
        private void NotifyOperationStatus(bool status)
        {
            if (status)
            {
                ExToolkit.MessageBox.Show("Operation successful");
            }
            else
            {
                ExToolkit.MessageBox.Show("Operation failed");
            }
        }

        //metoda sprawdza czy podane dane są poprawne
        private bool CheckIfInputCorrect()
        {
            if (string.IsNullOrWhiteSpace(cbxType.Text))
            {
                ExToolkit.MessageBox.Show("Activity type must be specified.");
                return true;
            }

            return false;
        }

        //wyświetlanie okienek ze stanem połaczenia na potrzeby testów
        private void ShowConnectionResult()
        {
            try
            {
                dbManager.Connect();
                MessageBox.Show("Connected");
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }

        //metoda inicjalizuje wartości kontrolek domyślną wartością
        private void InitializeControlsValues()
        {
            dtpDate.SelectedDate = DateTime.Today;
            tpStartTime.Value = DateTime.Now;
            tpEndTime.Value = DateTime.Now.AddHours(1);
            if (AppId == 0)
            {
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            dbManager.cn.Close();
            Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmResult = ExToolkit.MessageBox.Show("Are you sure to delete?",
                "Please confirm.",
                MessageBoxButton.YesNo);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }
            DeleteActivity();
        }

        //wypełnia pola formularza danymi pobranymi z bazy w celu podglądu
        public void FillFieldsFromDataRow(DataRow dataRow)
        {
            AppId = int.Parse(dataRow["ID"].ToString());
            dtpDate.SelectedDate = DateTime.Parse(dataRow["AppDate"].ToString());
            tpStartTime.Value = DateTime.Parse(dataRow["StartTime"].ToString());
            tpEndTime.Value = DateTime.Parse(dataRow["EndTime"].ToString());
            cbxType.Text = dataRow["Type"].ToString();
        }
    }
}
