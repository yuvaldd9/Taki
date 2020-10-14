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
        private int relativePosition; // the number of the player according to the playing order if our bot is number 0

        public int NumOfCards { get { return numOfCards; } }
        public int Position { get { return this.relativePosition; } }
        private const int INITIAL_NUM_OF_CARDS = 8;

        public Enemy(int relativePosition)
        {
            this.numOfCards = INITIAL_NUM_OF_CARDS;
            this.relativePosition = relativePosition;
        }

        public void UpdateState(int numOfCardsPlayed)
        {
            /*
             * numOfCardsPlayed - positive if the enemy got rid of cards, negative if he drew cards
             */
            this.numOfCards -= numOfCardsPlayed;
        }

        public void FlipOrder(int numOfPlayers)
        {
            /*
             * To be executed when change direction card is played
             */ 
            this.relativePosition = numOfPlayers - this.relativePosition;
        }
    }
}
