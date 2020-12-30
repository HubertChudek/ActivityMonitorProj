using System;
using System.Data.OleDb;
using System.Windows;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy FormActivityWindow.xaml
    /// </summary>
    public partial class FormActivityWindow : Window
    {
        //zmienna reprezentująca połączenie z bazą
        private OleDbConnection cn = new OleDbConnection(
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb"); //connection string

        public FormActivityWindow()
        {
            InitializeComponent();
            InitializeControlsValues();
        }

        //metoda otwierająca połaczenie z bazą danych
        public void Connect()
        {
            if (cn.State == System.Data.ConnectionState.Closed)
            {
                cn.Open();
            }
        }

        //metoda do wydawania poleceń SQL bazie danych
        //przyjmuje gotowy string polecenia w SQL
        //zwaraca true w przypadku powodzenia wykonania zapytania
        public bool InsertUpdateDelete(string sql)
        {
            Connect();
            OleDbCommand cmd = new OleDbCommand(sql, cn);
            return cmd.ExecuteNonQuery() > 0;
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

            MessageBoxResult confirmResult = MessageBox.Show("Are you sure to save?",
                "Please confirm.",
                MessageBoxButton.YesNo);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            InsertActivityAndNotify();
        }

        //metoda wprowadza do bazy informacje z formularza
        private void InsertActivityAndNotify()
        {
            string sql =
                $"insert into activity(AppDate, StartTime, EndTime, Type) " +
                $"values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', " +
                $"'{tpStartTime.Value}' , '{tpEndTime.Value}', '{cbxType.Text}')";
            if (InsertUpdateDelete(sql))
            {
                MessageBox.Show("Inserted successfully");
            }
            else
            {
                MessageBox.Show("Insert failed");
            }
        }

        //metoda sprawdza czy podane dane są poprawne
        private bool CheckIfInputCorrect()
        {
            if (string.IsNullOrWhiteSpace(cbxType.Text))
            {
                MessageBox.Show("Activity type must be specified.");
                return true;
            }

            return false;
        }

        //wyświetlanie okienek ze stanem połaczenia na potrzeby testów
        private void ShowConnectionResult()
        {
            try
            {
                Connect();
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
        }
    }
}
