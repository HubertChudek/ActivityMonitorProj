using System;
using System.Data.OleDb;
using System.Windows;
using ExToolkit = Xceed.Wpf.Toolkit;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy FormActivityWindow.xaml
    /// </summary>
    public partial class FormActivityWindow : Window
    {
        //zmienna reprezentująca połączenie z bazą
        //obiekt zarządzający bazą dancyh
        private DatabaseManager dbManager = new DatabaseManager();

        public FormActivityWindow()
        {
            InitializeComponent();
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

            InsertActivityAndNotify();
        }

        //metoda wprowadza do bazy informacje z formularza
        private void InsertActivityAndNotify()
        {
            string sql =
                $"insert into activity(AppDate, StartTime, EndTime, Type) " +
                $"values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', " +
                $"'{tpStartTime.Value}' , '{tpEndTime.Value}', '{cbxType.Text}')";
            if (dbManager.InsertUpdateDelete(sql))
            {
                ExToolkit.MessageBox.Show("Inserted successfully");
            }
            else
            {
                ExToolkit.MessageBox.Show("Insert failed");
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
        }
    }
}
