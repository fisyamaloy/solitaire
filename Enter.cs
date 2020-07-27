using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Enter : Form
    {
        public struct Players
        {
            public int ID { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public Players(int ID, string Login, string Password)
            {
                this.Login = Login;
                this.Password = Password;
                this.ID = ID;
            }
        }

        List<Players> PlayersData;
        private bool CheckConnection;

        public Enter()
        {
            InitializeComponent();
            try
            {
                PlayersData = new List<Players>();
                AddLogsAndPassesAtStruct();
                CheckConnection = true;
            }
            catch
            {
                CheckConnection = false;
            }
            timer1.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (CheckConnection)
            {
                Registration newFrom = new Registration();
                newFrom.Show();
                this.Hide();
            }
            else
            {
                DialogResult result = MessageBox.Show("Отсутсвует потключение к серверу! Перейти в демо-версию?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    GoPlay();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckConnection)
            {
                bool flag = false;
                for (int i = 0; i < PlayersData.Count; i++)
                {
                    if (login.Text == PlayersData[i].Login && password.Text == PlayersData[i].Password)
                    {
                        flag = true;
                        PlayerInfo.isPlayer = true;
                        PlayerInfo.id = PlayersData[i].ID;
                        PlayerInfo.username = PlayersData[i].Login;
                        PlayerInfo.password = PlayersData[i].Password;
                        GoPlay();
                        break;
                    }
                }
                if (!flag)
                    Registration.GetMessage(label4, "Неправильно введен логин или пароль!", Color.Red);
            }
            else
            {
                DialogResult result = MessageBox.Show("Отсутсвует потключение к серверу! Перейти в демо-версию?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    GoPlay();
            }
        }

        private void GoPlay()
        {
            timer1.Stop();
            LoadingForm newForm = new LoadingForm();
            newForm.Show();
            Hide();
        }

        private void AddLogsAndPassesAtStruct()
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand("SELECT * FROM players", db.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();
            // Формирую объекты c логином и паролем.
            while (reader.Read())
                PlayersData.Add(new Players(Convert.ToInt32(reader[0]), reader[1].ToString(), reader[2].ToString()));
            reader.Close();
            db.CloseConnection();
        }

        private void login_MouseClick(object sender, MouseEventArgs e)
        {
            label2.Text = "";
        }

        private void password_MouseClick(object sender, MouseEventArgs e)
        {
            label3.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!CheckConnection)
            {
                DialogResult result = MessageBox.Show("Отсутсвует потключение к серверу! Перейти в демо-версию?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    GoPlay();
            }
        }
    }

    public static class PlayerInfo
    {
        public static bool isPlayer = false;
        public static int id { get; set; }
        public static string username { get; set; }
        public static string password { get; set; }
    }
}