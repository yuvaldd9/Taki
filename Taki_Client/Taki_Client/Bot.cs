using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    public enum StateCodes { NextNeighborLowCards, LastNeighborLowCards}
    class Bot
    {
        private Card topCard;
        private Deck deck;
        private List<Enemy> enemies;
        private List<Rule> rules;
        private int numOfPlayers;
        private const int LOW_CARDS = 3;

        public Bot(Card topCard, Deck deck, List<Enemy> enemies)
        {
            this.topCard = topCard;
            this.deck = deck;
            this.enemies = enemies;
            this.rules = new List<Rule>();
        }

        private void GenerateRules()
        {
            //Example for a how a rule might be
            Rule rule = new Rule(StateCodes.NextNeighborLowCards.ToString(), 
                new ValidTypes[] { ValidTypes.plus_2, ValidTypes.stop, ValidTypes.change_direction});
            this.rules.Add(rule);
        }

        public void UpdateTopCard(Card card)
        {
            this.topCard = card;
        }

        public List<Card> ChooseAction()
        {
            //Returns the list of cards to be played
            //Returns null when the chosen action is drawing cards
            string currentState = GenerateState();
            ValidTypes[] priorityList;
            foreach(Rule rule in this.rules)
            {
                if (rule.Match(currentState))
                {
                    priorityList = rule.PriorityList;
                    break;
                }
            }
            //TODO: Check the priority list against the actual deck and determine the optimal action
            return null;
        }

        private string GenerateState()
        {
            //Returns string that contains all the relevant flags for the current state
            string state = "";
            foreach(Enemy enemy in this.enemies)
            {
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == 1)
                    state += StateCodes.NextNeighborLowCards + " ";
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == this.numOfPlayers - 1)
                    state += StateCodes.LastNeighborLowCards + " ";
            }
            return state;
        }
    }
}
