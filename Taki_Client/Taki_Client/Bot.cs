using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    public enum StateCodes
    {
        next_1_low_cards, next_2_low_cards, next_3_low_cards, taki, super_taki,
        plus2, stop, change_color, last_card, no_match, change_direction, plus, got_plus2, number_card
    }
    class Bot
    {
        private Card topCard;
        public bool isTopCardActive; //In case of special card, determines if its funtionality is relevant in the current turn
        public Deck deck;
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
            this.GenerateRules();
        }

        private void GenerateRules()
        {
            //Example for a how a rule might be
            Rule r1 = new Rule(StateCodes.plus2.ToString(),
                    new ValidTypes[] { ValidTypes.plus_2 });
            this.rules.Add(r1);

            Rule r2 = new Rule(StateCodes.next_1_low_cards.ToString(),
                    new ValidTypes[] { ValidTypes.plus_2, ValidTypes.stop, ValidTypes.change_direction });
            this.rules.Add(r2);
            Rule r3 = new Rule(StateCodes.taki.ToString(),
                    new ValidTypes[] { ValidTypes.taki });
            this.rules.Add(r3);
            Rule r4 = new Rule(StateCodes.super_taki.ToString(),
                new ValidTypes[] { ValidTypes.super_taki });
            this.rules.Add(r4);
            Rule r5 = new Rule(StateCodes.plus.ToString(),
                new ValidTypes[] { ValidTypes.plus });
            this.rules.Add(r5);
            Rule r7 = new Rule(StateCodes.stop.ToString(),
                new ValidTypes[] { ValidTypes.stop });
            this.rules.Add(r7);
            Rule r8 = new Rule(StateCodes.change_direction.ToString(),
                new ValidTypes[] { ValidTypes.change_direction });
            this.rules.Add(r8);
            Rule r9 = new Rule(StateCodes.number_card.ToString(),
                new ValidTypes[] { ValidTypes.number_card });
            this.rules.Add(r9);
            Rule r10 = new Rule(StateCodes.change_color.ToString(),
                new ValidTypes[] { ValidTypes.change_color });
            this.rules.Add(r10);
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

                }
            }
            //priorityList include all the recommened cards which we have in the current turn

            //Check if the top card forces the bot to do a specific action
            if (this.topCard != null && (this.topCard.Type != ValidTypes.number_card.ToString()) && this.isTopCardActive)
            {
                if (this.topCard.Type == ValidTypes.plus_2.ToString() && currentState.Contains(StateCodes.plus2.ToString()))
                {
                    foreach (Card card in this.deck)
                    {
                        if (card.Type == ValidTypes.plus_2.ToString())
                        {
                            this.deck.RemoveCard(card);
                            return new List<Card>(new Card[] { card });
                        }
                    }
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
                            action.AddRange(this.GetCardsByColor(card.Color));
                        if (card.Type == ValidTypes.super_taki.ToString())
                            action.AddRange(this.GetCardsByColor(this.topCard.Color));
                        if (card.Type == ValidTypes.plus.ToString())
                            action.AddRange(this.GetCardsAfterPlus(card, priorityList));
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
                            if (card.Type == ValidTypes.taki.ToString())
                                action.AddRange(this.GetCardsByColor(card.Color));
                            if (card.Type == ValidTypes.super_taki.ToString())
                            {
                                action.Remove(card);
                                action.Add(new Card("super_taki", this.topCard.Color, ""));
                                action.AddRange(this.GetCardsByColor(this.topCard.Color));
                            }

                            if (card.Type == ValidTypes.plus.ToString())
                            {
                                if (this.GetCardsAfterPlus(card, priorityList) != null)
                                    action.AddRange(this.GetCardsAfterPlus(card, priorityList));
                                else
                                    action.Remove(card);
                            }
                            if (card.Type == ValidTypes.change_color.ToString())
                            {
                                action.Remove(card);
                                action.Add(new Card("change_color", this.DominantColor(), ""));
                            }
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
                        if (card.Type == ValidTypes.super_taki.ToString() || card.Type == ValidTypes.change_color.ToString())
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
                    Random random = new Random();
                    return new string[] { "red", "blue", "yellow", "green" }[random.Next(4)];
                default:
                    return "yellow";
            }
        }
        private string GenerateState()
        {
            //Returns string that contains all the relevant flags for the current state
            int matches = 0;

            string state = DominantColor() + " ";//state = dominant color -> states

            if (this.deck.Length == 1)
                state += StateCodes.last_card + " ";


            if (this.topCard != null && this.topCard.Type == ValidTypes.plus_2.ToString())
                state += StateCodes.got_plus2 + " ";

            foreach (Enemy enemy in this.enemies)
            {
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == 1)
                    state += StateCodes.next_1_low_cards + " ";
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == this.numOfPlayers - 1)
                    state += StateCodes.next_2_low_cards + " ";
                if (enemy.NumOfCards < LOW_CARDS && enemy.Position == this.numOfPlayers - 2)
                    state += StateCodes.next_3_low_cards + " ";
            }
            foreach (Card card in this.deck.cards)
            {
                //find general cards
                if (card.Type == ValidTypes.number_card.ToString() && !state.Contains(StateCodes.number_card + " "))
                    state += StateCodes.number_card + " ";
                if (card.Type == ValidTypes.change_color.ToString())
                {
                    matches++;
                    state += StateCodes.change_color + " ";
                }

                if (card.Type == ValidTypes.change_direction.ToString())
                {
                    matches++;
                    state += StateCodes.change_direction + " ";
                }
                if (card.Type == ValidTypes.super_taki.ToString())
                {
                    matches++;
                    state += StateCodes.super_taki + " ";
                }
                if (card.Type == ValidTypes.plus.ToString())
                    state += StateCodes.plus + " ";
                if (card.Type == ValidTypes.stop.ToString())
                    state += StateCodes.stop + " ";
                if (card.Type == ValidTypes.taki.ToString())
                    state += StateCodes.taki + " ";
                if (card.Type == ValidTypes.plus_2.ToString())
                    state += StateCodes.plus2 + " ";
                if (matches == 0)
                {
                    state += StateCodes.no_match + " ";
                    if (state.Contains(ValidTypes.plus.ToString()))
                        state.Replace(StateCodes.plus.ToString() + " ", "");
                }
            }

            return state;
        }

        private bool CheckCard(Card card, Card topCard)
        {
            if (topCard == null)
                return true;
            if (card.Type == ValidTypes.super_taki.ToString() || card.Type == ValidTypes.change_color.ToString())
                return true;
            if (topCard.Type == ValidTypes.number_card.ToString())
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
            else if (topCard.Type == ValidTypes.super_taki.ToString() || topCard.Type == ValidTypes.change_color.ToString())
                return topCard.Color == card.Color;
            else
            {
                //Number cards must have the same value or color of the top card
                if (card.Type == ValidTypes.number_card.ToString())
                    return card.Color == topCard.Color;

                //Super taki and change color don't have a value or a color
                if (card.Type == ValidTypes.super_taki.ToString() || card.Type == ValidTypes.change_color.ToString())
                    return topCard.Color == card.Color;

                //Other special cards must have the same color or type of the top card
                return card.Color == topCard.Color || card.Type == topCard.Type;
            }
        }

        private List<Card> GetCardsByColor(string color)
        {
            List<Card> cards = new List<Card>();
            List<Card> endTurnCards = new List<Card>();
            foreach (Card card in this.deck)
            {
                if (card.Color == color)
                {
                    cards.Add(card);
                    if (card.Type == ValidTypes.change_color.ToString() || card.Type == ValidTypes.change_direction.ToString()
                            || card.Type == ValidTypes.plus_2.ToString() || card.Type == ValidTypes.stop.ToString())
                    {
                        cards.Remove(card);
                        endTurnCards.Add(card);
                    }
                }



            }
            if (endTurnCards.Count > 0)
                cards.Add(endTurnCards.ElementAt(0));
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
                            if (card.Type == ValidTypes.taki.ToString())
                                action.AddRange(this.GetCardsByColor(card.Color));
                            if (card.Type == ValidTypes.super_taki.ToString())
                            {
                                action.Remove(card);
                                action.Add(new Card("super_taki", this.topCard.Color, ""));
                                action.AddRange(this.GetCardsByColor(this.topCard.Color));
                            }

                            if (card.Type == ValidTypes.plus.ToString())
                            {
                                if (this.GetCardsAfterPlus(card, priorityList) != null)
                                    action.AddRange(this.GetCardsAfterPlus(card, priorityList));
                                else
                                    action.Remove(card);
                            }
                            if (card.Type == ValidTypes.change_color.ToString())
                            {
                                action.Remove(card);
                                action.Add(new Card("change_color", this.DominantColor(), ""));
                            }
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