using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    class Deck
    {
        public List<Card> cards { get; }
        public int Length { get { return this.cards.Count; } }

        public Deck(List<Card> cards)
        {
            this.cards = new List<Card>();
            foreach(var card in cards)
            {
                this.cards.Add(card);
            }
        }

        public bool RemoveCard(Card card)
        {
            if (this.cards.Contains(card))
            {
                this.cards.Remove(card);
                return true;
            }
            else
                return false;
        }

        public void AddCard(Card card)
        {
            this.cards.Add(new Card(card));
        }
    }
}
