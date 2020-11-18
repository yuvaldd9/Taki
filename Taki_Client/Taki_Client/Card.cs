using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    public enum ValidTypes { number_card, plus, plus_2, stop, change_direction, change_color, taki, super_taki }
    public enum ValidColors { red, blue, green, yellow, all };

    class Card
    {
        private string type;
        private string color;
        private string value;

        public string Type { get => type; }
        public string Color { get => color; }
        public string Value { get => value; }

        public Card(string type, string color, string value)
        {
            if (!Array.Exists(Enum.GetNames(typeof(ValidTypes)), card_type => card_type == type))
            {
                throw new ArgumentException("Illegal type", "type");
            }
            if (!Array.Exists(Enum.GetNames(typeof(ValidColors)), card_color => card_color == color) && color != "")
            {
                throw new ArgumentException("Illegal color", "color");
            }
            if (value != "" && (int.Parse(value) < 1 || int.Parse(value) > 9))
            {
                throw new ArgumentException("Illegal value", "value");
            }
            this.type = type;
            this.color = color;
            this.value = value;
        }

        public Card(Card card)
        {
            this.type = card.type;
            this.color = card.color;
            this.value = card.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Card) return this.Equals((Card)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool Equals(Card card)
        {
            if (this.type == ValidTypes.change_color.ToString() || this.type == ValidTypes.super_taki.ToString())
            {
                return this.type == card.type;
            }
            return this.type == card.type && this.color == card.color && this.value == card.value;
        }
    }
}
