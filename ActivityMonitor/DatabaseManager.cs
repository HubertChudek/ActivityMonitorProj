using System.Data.OleDb;

namespace ActivityMonitor
{
    internal class DatabaseManager
    {
        private OleDbConnection cn;

        public DatabaseManager()
        {
            Initialize();
        }

        //inicjalizuje połączenie z bazą i zwaraca obiekt reprezentujący je
        private void Initialize()
        {
            cn = new OleDbConnection(
                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database.accdb"); //connection string
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
    }
}
