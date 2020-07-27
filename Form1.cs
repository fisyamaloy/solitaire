using pass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < ObjectsList.DataSaving.Count; i++)
                toolStripComboBox1.Items.Add(ObjectsList.DataSaving[i].NameOfSaving);

            MouseDown += new MouseEventHandler(Form1_MouseDown);
            MouseMove += new MouseEventHandler(Form1_MouseMove);
            MouseUp += new MouseEventHandler(Form1_MouseUp);
        }

        const int ROW_COUNT = 1;  
        private int COL_COUNT = 6;  
        private int LEFT = 30;      
        const int TOP = 208;
        const int CARD_WIDTH = 72;    
        const int CARD_HEIGHT = 100;  
        const int H_SHIFT = CARD_WIDTH + 10;    // сдвиг между стопками по горизонтали
        const int V_SHIFT = CARD_HEIGHT + 30;   // сдвиг между рядами по вертикали
        const int MIN_DIST = 8;     // расстояние между картами в стопке
        const int MAX_DIST = 24;
        private int TalonClickQuantity = 3;

        // empCard в imageList2
        const int empCard = 2;   // индекс изображения пустой колоды

        private bool newGame;
        bool isGame = false;

        private List<PictureBox> boxes;
        private List<string> kings;
        private Pile[,] Piles;
        private Deck deck;      // колода

        private bool drag;      // флаги выполнения drag & drop
        private bool view;
        private Graphics grf;

        private Pile TalonPile;
        private static PictureBox pictureBox;

        private Card dragCard;      // перемещаемая карта
        private int dragX, dragY;   
        private int deltaX, deltaY; // разница между координатами перемещаемой карты и координатами курсора на ней 
        private int dragRow = 0;    // ряд, с которого перемещается карта 
        private int dragCol = 0;    // столбец, с которого перемещается карта
        private int dragPos = 0;
        private int QuantityOfDeckCards;

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void ShowCards(Graphics g, int ColCount)
        {
            int dy = 0;

            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int col = 0; col < ColCount; col++)
                {
                    for (int pos = 0; pos < Piles[row, col].Count; pos++)
                    {
                        if (view && row == dragRow && col == dragCol)
                        {
                            imageList1.Draw(g, Piles[row, col].X, Piles[row, col].Y + dy, Piles[row, col][pos].ImageIndex); // рисуем карту  
                            dy = pos == dragPos ? dy + MAX_DIST : dy + MIN_DIST;
                        }
                        else
                        {
                            if (Piles[row, col][pos] != null)
                                imageList1.Draw(g, Piles[row, col].X, Piles[row, col].Y + pos * MIN_DIST, Piles[row, col][pos].ImageIndex); // рисуем карту
                        }
                    }
                }
            }

            // прорисовка перемещаемой карты
            if (drag)
            {
                imageList1.Draw(g, dragX, dragY, dragCard.ImageIndex);
            }
        }

        private void NewGame()
        {
            initialisation();
            QuantityOfDeckCards = deck.Count - 6;
            DeckClickLabel.Text = QuantityOfDeckCards.ToString();
            pictureBox1.Image = imageList1.Images["26.bmp"]; // червонный туз
            pictureBox1.Tag = "26";
            pictureBox5.Image = imageList1.Images["38.bmp"]; // червонный король
            KingsInitialisationAndShuffle();
            ДобавитьИзображенияКоролей();
            AddCards(COL_COUNT);
            ShowCards(grf, 6);
        }

        private void initialisation()
        {
            DoubleBuffered = true;
            isGame = true;
            CreatePictureBoxesList();
            ShowComponents();
            newGame = false;
            grf = CreateGraphics();
            Refresh();

            //создаем колоду
            deck = new Deck();
            deck.X = pictureBox10.Location.X;
            deck.Y = pictureBox10.Location.Y;

            Piles = new Pile[ROW_COUNT, COL_COUNT];
            TalonPile = new Pile();
            TalonPile.X = TalonBox.Location.X;
            TalonPile.Y = TalonBox.Location.Y;
            newGame = true;
        }

        private void CreatePictureBoxesList()
        {
            boxes = new List<PictureBox>();
            boxes.Add(pictureBox1);
            boxes.Add(pictureBox2);
            boxes.Add(pictureBox3);
            boxes.Add(pictureBox4);
            boxes.Add(pictureBox6);
            boxes.Add(pictureBox7);
            boxes.Add(pictureBox8);
            boxes.Add(pictureBox9);
        }

        private void ShowComponents()
        {
            DeckClickLabel.Visible = true;
            pictureBox5.Visible = true;
            pictureBox10.Visible = true;
            TalonBox.Visible = true;
            foreach (PictureBox box in boxes)
                box.Visible = true;
        }

        private void KingsInitialisationAndShuffle()
        {
            kings = new List<string>();
            kings.Add("12.bmp");
            kings.Add("12.bmp");
            kings.Add("25.bmp");
            kings.Add("25.bmp");
            kings.Add("38.bmp");
            kings.Add("51.bmp");
            kings.Add("51.bmp");
            Shuffle();
        }

        public void Shuffle()
        {
            Random rand = new Random();
            for (int i = kings.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);
                string tmp = kings[j];
                kings[j] = kings[i];
                kings[i] = tmp;
            }
        }

        private void AddCards(int ColCount)
        {
            deck.Shuffle();
            LEFT = 30;
            Card card;
            int с = 0;
            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int col = 0; col < ColCount; col++)
                {
                    Piles[row, col] = new Pile();
                    Piles[row, col].X = LEFT + H_SHIFT * col;
                    Piles[row, col].Y = TOP + V_SHIFT * row;
                    card = deck.GetCard();
                    Piles[row, col].AddCard(card);
                    с++;
                    if (с == 3)
                        LEFT = 395;
                }
            }
        }

        private void ДобавитьИзображенияКоролей()
        {
            for (int i = 1; i < boxes.Count; i++)
            {
                boxes[i].Image = imageList1.Images[kings[i - 1]];
                boxes[i].Tag = GetIndexOfImage(kings[i - 1]);
            }
        }

        private static string GetIndexOfImage(string IndexPenBmp)
        {
            string index = "";
            int i = 0;
            while (IndexPenBmp[i] != '.')
            {
                index += IndexPenBmp[i];
                i++;
            }
            return index;
        }


        private void AddCardsFromTalonToDeck()
        {
            for (int i = 0; i < TalonPile.Count; i++)
                deck.AddCard(TalonPile[i]);
            TalonPile = new Pile();
            TalonBox.Image = imageList2.Images[empCard];
            TalonClickQuantity--;
            DeckClickLabel.Text = deck.Count.ToString();
        }

        private void AddCardsFromPilesToTalon()
        {
            for (int col = 0; col < COL_COUNT; col++)
            {
                TalonPile.AddCard(Piles[0, col].GetCard());
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (drag && isGame)
            {
                if (CanDrop())
                {
                    Piles[dragRow, dragCol].AddCard(deck.GetCard());
                    pictureBox.Image = imageList1.Images[dragCard.ImageIndex];
                    pictureBox.Tag = dragCard.ImageIndex;
                    QuantityOfDeckCards--;
                    if (QuantityOfDeckCards == 0 && TalonClickQuantity != 0)
                    {
                        AddCardsFromPilesToTalon();
                        AddCardsFromTalonToDeck();
                        deck.Shuffle();
                        AddCards(COL_COUNT);
                        ShowCards(grf, COL_COUNT);
                    }
                    else if (QuantityOfDeckCards <= 0 && TalonClickQuantity <= 0)
                    {
                        if (isVictory())
                            MessageBox.Show("Победа! Пасьянс сошелся!!!");
                    }
                    DisplayDeckCardsQuantity();
                }
                else
                    Piles[dragRow, dragCol].AddCard(dragCard);  // Возвращаем на старое место.
            }

            dragCard = null;
            dragRow = 0;
            dragPos = 0;
            dragCol = 0;
            dragX = 0;
            dragY = 0;
            drag = false;
            view = false;
        }

        private bool isVictory()
        {
            int КоличествоДамВПикчерБоксах = boxes.Where(g => Convert.ToInt32(g.Tag) % Constants.RANK_COUNT == 11).Count();
            return КоличествоДамВПикчерБоксах == 8 ? true : false;
        }

        private bool CanDrop()
        {
            if (Drop(pictureBox1) || Drop(pictureBox2) || Drop(pictureBox3) || Drop(pictureBox4) || Drop(pictureBox6) || Drop(pictureBox7)
                || Drop(pictureBox8) || Drop(pictureBox9))
            {
                int PictureIndex = Convert.ToInt32(pictureBox.Tag.ToString());
                int PictureRank = PictureIndex % Constants.RANK_COUNT;
                int PictureSuit = PictureIndex / Constants.RANK_COUNT;
                if (PictureSuit == dragCard.Suit && PictureRank == 0 && dragCard.Rank == 1)
                    return true;
                else if (PictureSuit == dragCard.Suit && PictureRank == 12 && dragCard.Rank == 0)
                    return true;
                else if (PictureSuit == dragCard.Suit && PictureRank == dragCard.Rank - 1)
                    return true;
                else
                    return false;
            }
            else if (Drop(TalonBox))
            {
                TalonPile.AddCard(dragCard);
                TalonBox.Image = imageList1.Images[dragCard.ImageIndex];
                return true;
            }
            return false;
        }

        private bool Drop(PictureBox box)
        {
            if (Math.Abs(dragX - box.Location.X) < 50 && Math.Abs(dragY - box.Location.Y) < 60)
            {
                pictureBox = box;
                return true;
            }
            return false;
        }

        // возвращает перемещаемую карту
        private Card CaptureCard(int x, int y)
        {
            Card card = null;
            for (int col = 0; col < COL_COUNT; col++)
            {
                if (x >= Piles[0, col].X && x <= Piles[0, col].X + CARD_WIDTH) // попали в стопку
                {
                    dragCol = col;
                    // перемещать можно только последнюю карту стопки
                    for (int row = 0; row < ROW_COUNT; row++)
                    {
                        if (Piles[row, col].Count > 0)
                        {
                            if (y >= Piles[row, col].Y + (Piles[row, col].Count - 1) * MIN_DIST
                              && y <= Piles[row, col].Y + (Piles[row, col].Count - 1) * MIN_DIST + CARD_HEIGHT)
                            {
                                dragRow = row;
                                dragX = Piles[row, col].X;
                                dragY = Piles[row, col].Y + (Piles[row, col].Count - 1) * MIN_DIST;
                                card = Piles[row, col].GetCard(); // забираем из стопки верхнюю карту
                                return card;
                            }
                        }
                    }
                }
            }
            return card;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isGame && e.Button == MouseButtons.Left)
            {
                dragCard = CaptureCard(e.X, e.Y); // перемещаемая карта
                if (dragCard != null)
                {
                    deltaX = e.X - dragX;
                    deltaY = e.Y - dragY;
                    drag = true;
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ifrm = Application.OpenForms[0];
            ifrm.Show();
            ifrm.Close();
        }

        private void правилаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help rules = new Help();
            rules.ShowDialog();
        }

        private void сохранитьИгруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isGame)
            {
                DB db = new DB();
                string RowPictureBoxesIndexes;
                string RowTalonPileIndexes;
                string RowDeckIndexes;
                string RowSofaIndexes;
                GetValues(out RowPictureBoxesIndexes, out RowTalonPileIndexes, out RowDeckIndexes, out RowSofaIndexes);

                if (PlayerInfo.isPlayer)
                {
                    // Если играет зарегестрированный пользователь.
                    db.AddToBD("INSERT INTO continueplaying(`PictureBoxesCards`, `TalonCards`, `DeckCards`, `TalonClickQuantity`, `SofaIndexes`, `PlayerID`) " +
                    "VALUES(@PictureBoxesCards, @TalonCards, @DeckCards, @TalonClickQuantity, @SofaIndexes, @PlayerID)",
                    RowPictureBoxesIndexes, RowTalonPileIndexes, RowDeckIndexes, TalonClickQuantity.ToString(), RowSofaIndexes, PlayerInfo.id.ToString());
                }
                else
                    SavePlayingForNotRegistrated(RowPictureBoxesIndexes, RowTalonPileIndexes, RowDeckIndexes, RowSofaIndexes);
                UpdateSavingList();
            }
            else
                MessageBox.Show("Игра не начата!");
        }

        private void UpdateSavingList()
        {
            ObjectsList.DataSaving.Clear();
            toolStripComboBox1.Items.Clear();
            LoadingForm loading = new LoadingForm();
            loading.FullObjectList();
            for (int i = 0; i < ObjectsList.DataSaving.Count; i++)
                toolStripComboBox1.Items.Add(ObjectsList.DataSaving[i].NameOfSaving);
        }

        private void SavePlayingForNotRegistrated(string RowPictureBoxesIndexes, string RowTalonPileIndexes, string RowDeckIndexes, string RowSofaIndexes)
        {
            // Первая строка - индексы игры.
            // Вторая - индексы пикчебоксов посередине формы.
            // Третья - индексы талона.
            // Четвертая - индексы колоды.
            // Пятая - индексы дивана.
            // Шестая - количество нажатий по талону.
            string path = Environment.CurrentDirectory + @"\Data.txt";
            if (!File.Exists(path))
                File.Create(path);
            int index = IndexOfSavingGame(path);
            StreamWriter fout = new StreamWriter(path, true);
            fout.WriteLine(index);
            fout.WriteLine(RowPictureBoxesIndexes);
            fout.WriteLine(RowTalonPileIndexes);
            fout.WriteLine(RowDeckIndexes);
            fout.WriteLine(RowSofaIndexes);
            fout.WriteLine(TalonClickQuantity);
            fout.Close();
        }

        private static int IndexOfSavingGame(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Count() > 5)
                    return Convert.ToInt32(lines[lines.Count() - 6]) + 1;
            }
            return 0;
        }

        private void GetValues(out string RowPictureBoxesIndexes, out string RowTalonPileIndexes, out string RowDeckIndexes, out string RowSofaIndexes)
        {
            DB db = new DB();
            RowPictureBoxesIndexes = "";
            RowTalonPileIndexes = "";
            RowDeckIndexes = "";
            RowSofaIndexes = "";
            List<Card> CardList = new List<Card>();

            for (int i = 0; i < boxes.Count; i++)
                RowPictureBoxesIndexes += boxes[i].Tag.ToString() + " ";
            for (int i = 0; i < TalonPile.Count; i++)
                RowTalonPileIndexes += TalonPile[i].ImageIndex + " ";
            if (RowTalonPileIndexes == "")
                RowTalonPileIndexes = "-1";

            while (deck.Count != 0)
            {
                Card card = null;
                card = deck.GetCard();
                RowDeckIndexes += card.ImageIndex + " ";
                CardList.Add(card);
            }
            if (RowDeckIndexes == "")
                RowTalonPileIndexes = "-1";

            // Добавляем эти же карты в колоду обратно
            for (int i = 0; i < CardList.Count; i++)
                deck.AddCard(CardList[i]);

            for (int i = 0; i < Piles.Length; i++)
                RowSofaIndexes += Piles[0, i].GetCardNotRemoving().ImageIndex + " ";
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag && isGame)
            {
                dragX = e.X - deltaX;
                dragY = e.Y - deltaY;
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (newGame)
                ShowCards(e.Graphics, 6);
        }

        private void RemoveDeckCards()
        {
            while (deck.Count != 0)
                deck.GetCard();
        }

        private void PictureBoxesInitialisation(SavingInfo elem)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].Image = imageList1.Images[elem.PictureBoxesIndexes[i]];
                boxes[i].Tag = elem.PictureBoxesIndexes[i];
            }
            pictureBox5.Image = imageList1.Images["38.bmp"];
        }

        private void SofaInitialisation(SavingInfo elem)
        {
            LEFT = 30;
            Card card;
            int с = 0;
            for (int col = 0; col < elem.SofaIndexes.Count; col++)
            {
                Piles[0, col] = new Pile();
                Piles[0, col].X = LEFT + H_SHIFT * col;
                Piles[0, col].Y = TOP;
                card = new Card(elem.SofaIndexes[col]);
                Piles[0, col].AddCard(card);
                с++;
                if (с == 3)
                    LEFT = 395;
            }
        }

        private void TalonInitialisation(SavingInfo elem)
        {
            Card card;
            if (CardsExist(elem.TalonIndexes[0]))
            {
                for (int i = 0; i < elem.TalonIndexes.Count; i++)
                {
                    if (!elem.PictureBoxesIndexes.Contains(elem.DeckCardsIndexes[i]))
                    {
                        card = null;
                        card = new Card(elem.TalonIndexes[i]);
                        TalonPile.AddCard(card);
                    }
                    if (i == elem.TalonIndexes.Count - 1)
                        TalonBox.Image = imageList1.Images[elem.TalonIndexes[i]];
                }
            }
            else
                TalonBox.Image = imageList2.Images[empCard];
        }

        private bool CardsExist(int CardIndex)
        {
            return CardIndex != -1;
        }

        private void DeckInitialisation(SavingInfo elem)
        {
            Card card;
            if (CardsExist(elem.DeckCardsIndexes[0]))
            {
                for (int i = 0; i < elem.DeckCardsIndexes.Count; i++)
                {
                    card = null;
                    card = new Card(elem.DeckCardsIndexes[i]);
                    deck.AddCard(card);
                }
            }
        }

        private void DisplayDeckCardsQuantity()
        {
            QuantityOfDeckCards = deck.Count;
            DeckClickLabel.Text = QuantityOfDeckCards.ToString();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int NameOfLoading = Convert.ToInt32(toolStripComboBox1.Text);
            initialisation();
            foreach (SavingInfo elem in ObjectsList.DataSaving)
            {
                if (elem.NameOfSaving == NameOfLoading)
                {
                    RemoveDeckCards();
                    PictureBoxesInitialisation(elem);
                    SofaInitialisation(elem);
                    TalonInitialisation(elem);
                    DeckInitialisation(elem);
                    TalonClickQuantity = elem.TalonClickQuantity;
                    DisplayDeckCardsQuantity();
                    ShowCards(grf, elem.SofaIndexes.Count);
                    break;
                }
            }
        }
    }
}
