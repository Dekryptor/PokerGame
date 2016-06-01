using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    enum Shapes { spade = 1, heart, diamond, club };
    enum Signs { T = 10, J, Q, K, A };
    public class Card
    {
        public int Value;
        public int Suite;
        public Card(int Value, int Shape)
        {
            this.Value = Value;
            this.Suite = Shape;
        }
        public override string ToString()
        {
            return (Value + "-" + Suite);
        }
    }
  

    class Deck
    {
        private List<Card> deck = new List<Card>();
        public Deck()
        {
            for (int i = 2; i <= 14; i++)
                for (int n = (int)Shapes.spade; n <= (int)Shapes.club; n++)
                    deck.Add(new Card(i, n));
        }

        public int Count
        {
            get
            {
                int i = 0;
                foreach (Card n in deck)
                    i++;
                return i;
            }
        }

        public void Shuffle()
        {
            List<Card> tmp = new List<Card>(52);
            Random rand = new Random();
            for (int i = 0; i < 52; i++)
            {
                int c = rand.Next(deck.Count);
                tmp.Add(deck[c]);
                deck.RemoveAt(c);
            }
            deck = tmp;
        }
        public Card Draw()
        {
            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        }
        public void Add(Card a)
        {
            deck.Add(a);
        }
        public void Add(Card[] a)
        {
            foreach (Card i in a)
                deck.Add(i);
        }
    }
}
