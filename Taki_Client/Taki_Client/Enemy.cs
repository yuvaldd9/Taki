using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    class Enemy
    {
        private int numOfCards;
        public int NumOfCards { get { return numOfCards; } }
        private const int INITIAL_NUM_OF_CARDS = 8;

        public Enemy()
        {
            this.numOfCards = INITIAL_NUM_OF_CARDS;
        }

        public void UpdateState(int numOfCardsPlayed)
        {
            this.numOfCards += numOfCardsPlayed;
        }
    }
}
