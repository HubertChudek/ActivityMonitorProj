using System;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using ExToolkit  = Xceed.Wpf.Toolkit;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy FormActivityWindow.xaml
    /// </summary>
    public partial class FormMealWindow : Window
    {
        //zmienna reprezentująca połączenie z bazą
        private OleDbConnection cn = new OleDbConnection(
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb"); //connection string

        public FormMealWindow()
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
                $"insert into meal(AppDate, StartTime, EndTime, Calories, Type, MealName) " +
                $"values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', '{tpStartTime.Value}', " +
                $"'{tpEndTime.Value}', '{int.Parse(txtCalories.Text)}', '{cbxType.Text}', '{txtName.Text}')";
            if (InsertUpdateDelete(sql))
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
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ExToolkit.MessageBox.Show("Product name type must be specified.");
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

        private void TxtCalories_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AllowOnlyDigitsAtInput(e);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AllowOnlyDigitsAtInput(e);
        }

        //metoda sprawdza znaki na wejsciu i pozwala wpisać tylko cyfry
        private static void AllowOnlyDigitsAtInput(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
