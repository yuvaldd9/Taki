using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    class Bot
    {
        private Card topCard;
        private Deck deck;
        private List<Enemy> enemies;

        public Bot(Card topCard, Deck deck, List<Enemy> enemies)
        {
            this.topCard = topCard;
            this.deck = deck;
            this.enemies = enemies;
        }

        public void UpdateTopCard(Card card)
        {
            this.topCard = card;
        }

        public List<Card> ChooseAction()
        {
            //Returns the list of cards to be played
            //Returns null when the chosen action is drawing cards
            return null;
        }
    }
}
