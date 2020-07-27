using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace WindowsFormsApp2
{
    public partial class LoadingForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
                int left,
                int top,
                int right,
                int bottom,
                int width,
                int height
            );

        private List<PictureBox> pictureBoxes = new List<PictureBox>();
        private int[] rainSpeeds = { 4, 6, 8, 3, 5, 6, 7, 4 };
        private int loadingSpeed = 12;
        private float initialPercentage = 0;

        public LoadingForm()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 7, 7));
            FullList();
        }

        private void FullList()
        {
            pictureBoxes.Add(pictureBox3);
            pictureBoxes.Add(pictureBox4);
            pictureBoxes.Add(pictureBox5);
            pictureBoxes.Add(pictureBox6);
            pictureBoxes.Add(pictureBox7);
            pictureBoxes.Add(pictureBox8);
            pictureBoxes.Add(pictureBox9);
            pictureBoxes.Add(pictureBox10);
        }

        public void FullObjectList()
        {
            if (!PlayerInfo.isPlayer) //Если игрок не зарегестрирован.
            {
                string path = Environment.CurrentDirectory + @"\Data.txt";
                if (File.Exists(path))
                {
                    string[] DataFile = File.ReadAllLines(path);
                    if (DataFile.Count() > 4)
                    {
                        for (int i = 6; i <= DataFile.Count() + 1; i += 6)
                        {
                            int SavingName = Convert.ToInt32(DataFile[i - 6].Trim());
                            List<int> PictureBoxesIndexes = conversion(DataFile[i - 5]);
                            List<int> TalonIndexes = conversion(DataFile[i - 4]);
                            List<int> DeckCardsIndexes = conversion(DataFile[i - 3]);
                            List<int> SofaIndexes = conversion(DataFile[i - 2]);
                            int TalonClickQuantity = Convert.ToInt32(DataFile[i - 1]);
                            ObjectsList.DataSaving.Add(new SavingInfo(SavingName, PictureBoxesIndexes, TalonIndexes, DeckCardsIndexes, TalonClickQuantity, SofaIndexes));
                        }
                    }
                }
            }
            // Если зарегестрирован.
            else
            {
                DB db = new DB();
                List<int> SavingNames = db.GetDataList("SELECT id FROM continueplaying WHERE PlayerID=" + PlayerInfo.id).Select(g => int.Parse(g)).ToList();
                for (int i = 0; i < SavingNames.Count; i++)
                {
                    List<int> PictureBoxesIndexes = conversion(db.GetValue("SELECT PictureBoxesCards FROM continueplaying WHERE id=" + SavingNames[i]));
                    List<int> TalonIndexes = conversion(db.GetValue("SELECT TalonCards FROM continueplaying WHERE id=" + SavingNames[i]));
                    List<int> DeckCardsIndexes = conversion(db.GetValue("SELECT DeckCards FROM continueplaying WHERE id=" + SavingNames[i]));
                    List<int> SofaIndexes = conversion(db.GetValue("SELECT SofaIndexes FROM continueplaying WHERE id=" + SavingNames[i]));
                    int TalonClickQuantity = Convert.ToInt32(db.GetValue("SELECT TalonClickQuantity FROM continueplaying WHERE id=" + SavingNames[i]));
                    ObjectsList.DataSaving.Add(new SavingInfo(SavingNames[i], PictureBoxesIndexes, TalonIndexes, DeckCardsIndexes, TalonClickQuantity, SofaIndexes));
                }
            }
        }

        private async void FullObjectListAsync()
        {
            await Task.Run(() => FullObjectList());
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            FullObjectListAsync();
        }

        private List<int> conversion(string row)
        {
            if (row != "")
                return row.Trim().Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(g => int.Parse(g)).ToList();
            return null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for(int i = 0; i < pictureBoxes.Count; i++)
            {
                pictureBoxes[i].Location = new Point(pictureBoxes[i].Location.X, pictureBoxes[i].Location.Y + rainSpeeds[i]);
                if (pictureBoxes[i].Location.Y > panel1.Size.Height + pictureBoxes[i].Size.Height)
                    pictureBoxes[i].Location = new Point(pictureBoxes[i].Location.X, 0 - pictureBoxes[i].Size.Height);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            initialPercentage += loadingSpeed;
            float percentage = initialPercentage / pictureBox2.Height * 100;

            label1.Text = (int)percentage + " %";

            panel2.Location = new Point(panel2.Location.X, panel2.Location.Y + loadingSpeed);
            if (panel2.Location.Y > pictureBox2.Location.Y + pictureBox2.Height)
            {
                label1.Text = "100 %";
                this.timer1.Stop();
                this.timer2.Stop();
                Form1 newForm = new Form1();
                newForm.Show();
                this.Hide();
            }
        }
    }

    public struct SavingInfo
    {
        public int NameOfSaving { get; set; }
        public int TalonClickQuantity { get; set; }
        public List<int> PictureBoxesIndexes;
        public List<int> TalonIndexes;
        public List<int> DeckCardsIndexes;
        public List<int> SofaIndexes;

        public SavingInfo(int NameOfSaving, List<int> PictureBoxesIndexes, List<int> TalonIndexes, List<int> DeckCardsIndexes, int TalonClickQuantity, List<int> SofaIndexes)
        {
            this.NameOfSaving = NameOfSaving;
            this.TalonClickQuantity = TalonClickQuantity;
            this.PictureBoxesIndexes = PictureBoxesIndexes;
            this.TalonIndexes = TalonIndexes;
            this.DeckCardsIndexes = DeckCardsIndexes;
            this.SofaIndexes = SofaIndexes;
        }
    }

    public static class ObjectsList
    {
        public static List<SavingInfo> DataSaving = new List<SavingInfo>();
    }
}
