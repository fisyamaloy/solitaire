using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    public class DB
    {
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=proger"); //Подключение к удаленной базе данных MySQL

        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        public virtual List<string> GetDataList(string require)
        {
            List<string> data = new List<string>();
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand(require, db.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                data.Add(reader[0].ToString());
            reader.Close();
            db.CloseConnection();
            return data;
        }

        public virtual string GetValue(string require)
        {
            string word = "";
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand(require, db.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            word = reader[0].ToString();
            reader.Close();
            db.CloseConnection();
            return word;
        }

        public virtual void AddToBD(string require, params string[] ToCorrespondinColumn)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand(require, db.GetConnection());
            List<string> DBColumns = GetColumnsList(require);
            for (int i = 0; i < ToCorrespondinColumn.Length; i++)
                command.Parameters.Add(DBColumns[i], MySqlDbType.VarChar).Value = ToCorrespondinColumn[i];
            command.ExecuteNonQuery();
            db.CloseConnection();
        }

        private static List<string> GetColumnsList(string require)
        {
            List<string> columns = new List<string>();
            int i = 0;
            while (require[i] != ')')
            {
                string TemproraryWord = "";
                if (require[i] == '`')
                {
                    i++;
                    while (require[i] != Convert.ToChar('`'))
                    {
                        TemproraryWord += require[i];
                        i++;
                    }
                    columns.Add(TemproraryWord);
                }
                i++;
            }
            return columns;
        }

        public virtual void DeleteFromBD(string require)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand(require, db.GetConnection());
            command.ExecuteNonQuery();
            db.CloseConnection();
        }
    }
}
