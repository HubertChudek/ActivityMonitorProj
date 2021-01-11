using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
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
        //obiekt zarządzający bazą dancyh
        private DatabaseManager dbManager = new DatabaseManager();

        public FormMealWindow()
        {
            InitializeComponent();
            InitializeControlsValues();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var nutri = new NutritionixControl();
            Nutritionix.Item foo = nutri.LookupNutritionInfo(txtName.Text);

            PopulateFormFields(foo);

            //string toPrint = $"Product name: {foo.Name}, KCal: {foo.NutritionFact_Calories.ToString()}\n"; 
            //Debug.Print(toPrint);
        }

        //metoda zapełnia pola formularza danymi pobranymi z API
        private void PopulateFormFields(Nutritionix.Item item)
        {
            txtName.Text = item.Name;
            txtCalories.Text = Math.Floor((decimal)item.NutritionFact_Calories).ToString();
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
            $"insert into meal(AppDate, StartTime, EndTime, Calories, Type, MealName, Quantity, Unit) " +
            $"values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', '{tpStartTime.Value}', " +
            $"'{tpEndTime.Value}', '{int.Parse(txtCalories.Text)}', '{cbxType.Text}', '{txtName.Text}', " +
            $"'{int.Parse(txtQuantity.Text)}', '{cbxUnit.Text}')";
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
