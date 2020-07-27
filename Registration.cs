using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Добавляем нового игрока в базу даных.
            try
            {
                DB db = new DB();
                if (login.Text.Trim() != "" && password.Text.Trim() != "")
                {
                    if (password.Text == password1.Text)
                    {
                        db.AddToBD("INSERT INTO players (`username`, `password`) VALUES (@username, @password)", login.Text, password.Text);
                        GetMessage(label4, "Игрок зарегестрирован!", Color.Green);
                    }
                    else
                        GetMessage(label4, "Пароли не совпадают!", Color.Red);
                }
                else
                    GetMessage(label4, "Заполните пустые поля!", Color.Red);
            }
            catch
            {
                DialogResult result = MessageBox.Show("Отсутсвует потключение к серверу! Перейти в демо-версию?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    LoadingForm newForm = new LoadingForm();
                    newForm.Show();
                    Close();
                }
            }
        }

        public static void GetMessage(Label label1, string message, Color color)
        {
            label1.ForeColor = color;
            label1.Text = message;
        }

        private void login_MouseClick(object sender, MouseEventArgs e)
        {
            label2.Text = "";
        }

        private void password_MouseClick(object sender, MouseEventArgs e)
        {
           label3.Text = "";
        }

        private void password1_MouseClick(object sender, MouseEventArgs e)
        {
            label1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Enter newForm = new Enter();
            newForm.Show();
            Close();
        }
    }
}
