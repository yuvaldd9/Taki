using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    public enum StateCodes
    {
        Next1LowCards, Next2LowCards, Next3LowCards, Taki, SuperTaki,
        Plus2, Stop, change_color, last_card, no_match, change_direction, plus, got_plus2
    }
    class Bot
    {
        private Card topCard;
        private bool isTopCardActive; //In case of special card, determines if its funtionality is relevant in the current turn
        private Deck deck;
        private List<Enemy> enemies;
        private List<Rule> rules;
        private int numOfPlayers;
        private const int LOW_CARDS = 3; //Constant for the state generator

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
            Rule r1 = new Rule(StateCodes.got_plus2.ToString(),
                    new ValidTypes[] { ValidTypes.plus_2 });
            this.rules.Add(r1);

            Rule r2 = new Rule(StateCodes.Next1LowCards.ToString(),
                    new ValidTypes[] { ValidTypes.plus_2, ValidTypes.stop, ValidTypes.change_direction });
            this.rules.Add(r2);
            Rule r3 = new Rule(StateCodes.Taki.ToString(),
                    new ValidTypes[] { ValidTypes.taki });
            this.rules.Add(r3);
            Rule r4 = new Rule(StateCodes.SuperTaki.ToString(),
                new ValidTypes[] { ValidTypes.super_taki });
            this.rules.Add(r4);
            Rule r5 = new Rule(StateCodes.plus.ToString(),
                new ValidTypes[] { ValidTypes.plus, ValidTypes.number_card });
            this.rules.Add(r5);
            Rule r6 = new Rule(StateCodes.no_match.ToString(),
                new ValidTypes[] { });

            this.rules.Add(r6);

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
            List<ValidTypes> priorityList = new List<ValidTypes>();
            List<Card> action = null;
            foreach (Rule rule in this.rules)
            {
                if (rule.Match(currentState))
                {
                    foreach (ValidTypes type in rule.PriorityList)
                    {
                        priorityList.Add(type);
                    }
                    break;
                }
            }
            foreach (ValidTypes type in priorityList)
            {
                if (!currentState.Contains(type.ToString()))
                {
                    priorityList.Remove(type);
                }
            }
            //priorityList include all the recommened cards which we have in the current turn

            //Check if the top card forces the bot to do a specific action
            if ((this.topCard.Type != ValidTypes.number_card.ToString()) && this.isTopCardActive)
            {
                if (this.topCard.Type == ValidTypes.plus_2.ToString() && currentState.Contains(ValidTypes.plus_2.ToString()))
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
                else
                {
                    return null;
                }

                if (this.topCard.Type == ValidTypes.stop.ToString())
                {
                    return null; //Must to do nothing
                }


            }

            if (priorityList == null && !currentState.Contains("no_numbers")) //No rule is relevant
            {
                //Play the first card that can be played
                foreach (Card card in this.deck)
                {
                    if (this.CheckCard(card, this.topCard))
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
            foreach (ValidTypes type in priorityList)
            {
                if (deckTypes.Contains(type.ToString()))
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

                /*if (action != null)
                    break;*/
            }
            foreach (Card card in action)
                this.deck.RemoveCard(card);
            return action;
        }

        public int FindMaxIndex(int[] arr)
        {
            int maxIndex = 0;
            int max = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > max)
                {
                    max = arr[i];
                    maxIndex = i;
                }
            }
            if (max == 0)
            {
                return -1;
            }
            return maxIndex;
        }
        public string DominantColor()
        {
            int dominant_index;
            int[] colorArr = new int[4];
            for (int i = 0; i < colorArr.Length; i++)
            {
                colorArr[i] = 0;
            }
            foreach (Card card in this.deck.cards)
            {
                switch (card.Color)
                {
                    case "red":
                        colorArr[0]++;
                        break;
                    case "blue":
                        colorArr[1]++;
                        break;
                    case "green":
                        colorArr[2]++;
                        break;
                    default:
                        colorArr[3]++;
                        break;
                }
            }
            dominant_index = FindMaxIndex(colorArr);
            switch (dominant_index)
            {
                case 0:
                    return "red";
                case 1:
                    return "blue";
                case 2:
                    return "green";
                case -1:
                    return "no_numbers";
                default:
                    return "yellow";
            }
        }
        private string GenerateState()
        {
            //Returns string that contains all the relevant flags for the current state
            int matches = 0;
            int simple_matches = 0; //number cards
            string state = DominantColor() + " ";//state = dominant color -> states

            if (this.deck.Length == 1)
                state += StateCodes.last_card + " ";

            if (this.topCard.Type == ValidTypes.plus_2.ToString())
                state += StateCodes.got_plus2 + " ";

            foreach (Enemy enemy in this.enemies)
            {
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == 1)
                    state += StateCodes.Next1LowCards + " ";
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == this.numOfPlayers - 1)
                    state += StateCodes.Next3LowCards + " ";
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == this.numOfPlayers - 2)
                    state += StateCodes.Next2LowCards + " ";
            }
            foreach (Card card in this.deck.cards)
            {
                //find general cards
                if (card.Type == ValidTypes.change_color.ToString())
                {
                    matches++;
                    state += ValidTypes.change_color + " ";
                }

                if (card.Type == ValidTypes.change_direction.ToString())
                {
                    matches++;
                    state += ValidTypes.change_direction + " ";
                }
                if (card.Type == ValidTypes.super_taki.ToString())
                {
                    matches++;
                    state += ValidTypes.super_taki + " ";
                }
                if (card.Color == this.topCard.Color)
                {
                    simple_matches++;
                    if (card.Type == ValidTypes.plus.ToString())
                        state += ValidTypes.plus + " ";

                    if (card.Type == ValidTypes.stop.ToString())
                        state += ValidTypes.stop + " ";
                    if (card.Type == ValidTypes.taki.ToString())
                        state += ValidTypes.taki + " ";
                }
                if (card.Type == ValidTypes.plus_2.ToString())
                    state += ValidTypes.plus_2 + " ";

                if (matches == 0)
                    state += StateCodes.no_match + " ";
                if (state.Contains(ValidTypes.plus.ToString()) && simple_matches == 0)
                    state.Replace(ValidTypes.plus.ToString() + " ", "");
            }

            return state;
        }

        private bool CheckCard(Card card, Card topCard)
        {
            //Number cards must have the same value or color of the top card
            if (card.Type == ValidTypes.number_card.ToString())
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
            foreach (Card card in this.deck)
            {
                if (card.Color == color)
                    cards.Add(card);
            }
            return cards;
        }

        private List<Card> GetCardsAfterPlus(Card plusCard, List<ValidTypes> priorityList)
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