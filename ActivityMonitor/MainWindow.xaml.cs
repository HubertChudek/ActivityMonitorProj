using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActivityMonitor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OleDbConnection cn = new OleDbConnection(
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb");

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Connect()
        {
            if(cn.State == System.Data.ConnectionState.Closed)
            {
                cn.Open();
            }
        }

        public bool InsertUpdateDelete(string sql)
        {
            Connect();
            var cmd = new OleDbCommand(sql, cn);
            return cmd.ExecuteNonQuery() > 0;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Activity type must be specified.");
                return;
            }

            string sql = $"insert into activity(AppDate, StartTime, EndTime, Type) values('{dtpDate.SelectedDate.Value.Date.ToShortDateString()}', '{tpStartTime.Value}' , '{tpEndTime.Value}', '{txtType.Text}')";
            if (InsertUpdateDelete(sql))
            {
                MessageBox.Show("Inserted succefully");
            }
            else
            {
                MessageBox.Show("Insert failed");
            }
        }
    }
}
