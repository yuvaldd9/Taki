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
        private bool isTopCardActive; //In case of special card, determines if its funtionality is relevant in the current turn
        private Deck deck;
        private List<Enemy> enemies;
        private List<Rule> rules;
        private int numOfPlayers;
        private const int LOW_CARDS = 3;

        public Bot(Card topCard, Deck deck, List<Enemy> enemies)
        {
            this.topCard = topCard;
            this.isTopCardActive = true;
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
            ValidTypes[] priorityList = null;
            List<Card> action = null;
            foreach(Rule rule in this.rules)
            {
                if (rule.Match(currentState))
                {
                    priorityList = rule.PriorityList;
                    break;
                }
            }

            //Check if the top card forces the bot to do a specific action
            if((this.topCard.Type != ValidTypes.number_card.ToString()) && this.isTopCardActive)
            {
                if(this.topCard.Type == ValidTypes.plus_2.ToString())
                {
                    foreach (Card card in this.deck)
                    {
                        if (card.Type == ValidTypes.plus_2.ToString())
                        {
                            this.deck.RemoveCard(card);
                            return new List<Card>(new Card[] { card });
                        }
                    }
                }
                if(this.topCard.Type == ValidTypes.stop.ToString())
                {
                    return null; //Must to do nothing
                }
            }

            if(priorityList == null) //No rule is relevant
            {
                //Play the first card that can be played
                foreach(Card card in this.deck)
                {
                    if(this.CheckCard(card, this.topCard))
                    {
                        action = new List<Card>(new Card[] { card });
                        this.deck.RemoveCard(card); //Simulate the new deck
                        if (card.Type == ValidTypes.taki.ToString())
                            action.Concat(this.GetCardsByColor(card.Color));
                        if (card.Type == ValidTypes.super_taki.ToString())
                            action.Concat(this.GetCardsByColor(this.topCard.Color));
                        if (card.Type == ValidTypes.plus.ToString())
                            action.Concat(this.GetCardsAfterPlus(card, priorityList));
                        this.deck.AddCard(card); //Restore the original deck
                        break;
                    }
                }
            }

            //Find the best action according to the priority list
            IEnumerable<string> deckTypes = from card in this.deck.cards select card.Type;
            foreach(ValidTypes type in priorityList)
            {
                if(deckTypes.Contains(type.ToString()))
                {
                    foreach (Card card in this.deck)
                    {
                        if (card.Type == type.ToString() && this.CheckCard(card, this.topCard))
                        {
                            action = new List<Card>(new Card[] { card });
                            this.deck.RemoveCard(card); //Simulate the new deck
                            if (type == ValidTypes.taki)
                                action.Concat(this.GetCardsByColor(card.Color));
                            if (type == ValidTypes.super_taki)
                                action.Concat(this.GetCardsByColor(this.topCard.Color));
                            if (type == ValidTypes.plus)
                                action.Concat(this.GetCardsAfterPlus(card, priorityList));
                            this.deck.AddCard(card); //Restore the original deck
                            break;
                        }
                    }

                }
                
                if (action != null)
                    break;
            }
            foreach (Card card in action)
                this.deck.RemoveCard(card);
            return action;
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

        private bool CheckCard(Card card, Card topCard)
        {
            //Number cards must have the same value or color of the top card
            if(card.Type == ValidTypes.number_card.ToString())
                return card.Value == topCard.Value || card.Color == topCard.Color;

            //Super taki and change color don't have a value or a color
            if (card.Type == ValidTypes.super_taki.ToString() || card.Type == ValidTypes.change_color.ToString())
                return true;

            //Other special cards must have the same color or type of the top card
            return card.Color == topCard.Color || card.Type == topCard.Type;
        }

        private List<Card> GetCardsByColor(string color)
        {
            List<Card> cards = new List<Card>();
            foreach(Card card in this.deck)
            {
                if (card.Color == color)
                    cards.Add(card);
            }
            return cards;
        }

        private List<Card> GetCardsAfterPlus(Card plusCard, ValidTypes[] priorityList)
        {
            //Determine the best move after putting a plus according to the priorityList
            List<Card> action = null;
            IEnumerable<string> deckTypes = from card in this.deck.cards select card.Type;
            foreach (ValidTypes type in priorityList)
            {
                if (deckTypes.Contains(type.ToString()))
                {
                    foreach (Card card in this.deck)
                    {
                        if (card.Type == type.ToString() && this.CheckCard(card, plusCard))
                        {
                            action = new List<Card>(new Card[] { card });
                            this.deck.RemoveCard(card); //Simulate the new deck
                            if (type == ValidTypes.taki)
                                action.Concat(this.GetCardsByColor(card.Color));
                            if (type == ValidTypes.super_taki)
                                action.Concat(this.GetCardsByColor(this.topCard.Color));
                            if (type == ValidTypes.plus)
                                action.Concat(this.GetCardsAfterPlus(card, priorityList));
                            this.deck.AddCard(card); //Restore the original deck
                            break;
                        }
                    }

                }

                if (action != null)
                    break;
            }
            return action;
        }
    }
}
