using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms; //подключаем библиотеку

namespace pass
{
    class Constants
    {
        public const int RANK_COUNT = 13; // количество карт одной масти
        public const int SUIT_COUNT = 4;  // количество мастей
        public const int PACK_COUNT = 2;  // количество колод
    }

    class Card // карта
    {
        private int suit;     // масть
        private int rank;     // ранг

        public int Suit { get { return suit; } }
        public int Rank { get { return rank; } }

        public int ImageIndex { get { return suit * Constants.RANK_COUNT + rank; } }

        // Конструктор карты
        public Card(int suit, int rank)
        {
            this.suit = suit;
            this.rank = rank;
        }
        // Конструктор карты
        public Card(int index)
        {
            rank = index % Constants.RANK_COUNT;
            suit = index / Constants.RANK_COUNT;
        }
    }

    class Pile // стопка карт
    {
        protected List<Card> cards;
        private int x, y; // координаты стопки

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }       // Возвращает карту заданной позиции стопки
        public Card this[int pos] { get { return (Card)cards[pos]; } }
        // Возвращает количество карт в стопке
        public int Count { get { return cards.Count; } }

        // Конструктор стопки карт
        public Pile()
        {
            cards = new List<Card>();
        }

        // Возвращает верхнюю карту с удалением ее из стопки
        public Card GetCard()
        {
            Card card = null;
            if (cards.Count > 0)
            {
                card = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
            }
            return card;
        }

        public Card GetCardNotRemoving()
        {
            Card card = null;
            if (cards.Count > 0)
                card = cards[cards.Count - 1];
            return card;
        }

        // Добавление карты в стопку
        public void AddCard(Card card)
        {
            cards.Add(card);
        }
    }

    class Deck : Pile// колода
    {
        // Конструктор колоды
        public Deck()
        {
            bool flag = false;
            for (int i = 0; i < Constants.PACK_COUNT; i++)
            {
                for (int j = 0; j < Constants.SUIT_COUNT; j++)
                {
                    for (int k = 0; k < Constants.RANK_COUNT; k++)
                    {
                        if (k != 12) // в данной задаче нам короли в колоде не нужны
                            AddCard(new Card(j, k));
                        if (k == 0 && j == 2)
                            if(!flag)
                                flag = true;
                            else
                                GetCard();
                    }
                }
            }
        }

        // Перемешивает карты в колоде
        public void Shuffle()
        {
            int n;
            Card card;
            Random random = new Random();
            for (int i = 0; i < cards.Count; i++)
            {
                card = cards[i];
                n = random.Next(cards.Count);
                cards[i] = cards[n];
                cards[n] = card;
            }
        }
    }
}
