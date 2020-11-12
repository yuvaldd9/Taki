using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taki_Client
{
    class GamePanel: Panel
    {
        private string gameID;
        private string password;
        private string jwt;
        private Socket sock;
        public Card currCard;
        private Bitmap cardCover;
        private Bitmap currCardPic;
        private List<Enemy> enemies;
        private List<Card> myCards;
        private List<Bitmap> myCardsPics;
        private Dictionary<string, string> cardsDir = new Dictionary<string, string>(); //Cards pics directory
        Dictionary<string, int> enemiesCardsAmount = new Dictionary<string, int>();

        public GamePanel(string gameID, string password, string jwt, Socket sock, List<Enemy> enemies,  List<Card> myCards, Card currCard) : base()
        {
            cardsDir.Add("red", "C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Red");
            cardsDir.Add("yellow", "C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Yellow");
            cardsDir.Add("green", "C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Green");
            cardsDir.Add("blue", "C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Blue");
            cardsDir.Add("special", "C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Special");

            this.gameID = gameID;
            this.password = password;
            this.jwt = jwt;
            this.sock = sock;
            this.Dock = DockStyle.Fill;
            this.myCards = myCards;
            this.enemies = enemies;
            this.currCard = currCard;
            //this.FindCardImage(currCard.Value, currCard.Color)
            this.currCardPic = new Bitmap("C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Green\\4.jpg");
            cardCover = new Bitmap("C:\\Users\\yuval\\Desktop\\Taki\\Taki_Client\\Sources\\Special\\1.jpg");
            this.myCardsPics = this.LoadMyCardsBitMaps(myCards);
            
            foreach (Enemy enemy in this.enemies)
            {
                this.enemiesCardsAmount.Add(enemy.name, enemy.NumOfCards);
            }

        }

        private List<Bitmap> LoadMyCardsBitMaps(List<Card> myCards)
        {
            List<Bitmap> cards = new List<Bitmap>();
            Bitmap cardP;
            string colorDir;
            foreach (Card card in myCards)
            {
                colorDir = this.FindCardImage(card.Value, card.Color);

                cardP = new Bitmap(colorDir);
                cards.Add(cardP);
            }

            return cards;
        }

        private string FindCardImage(string value, string color)
        {
            Console.WriteLine(this.cardsDir[color] + "\\" + value + ".jpg");
            return this.cardsDir[color] + "\\" + value + ".jpg";
            
            
        }

        public void Initialize()
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = this.Parent.Size;
            this.BackColor = System.Drawing.Color.Gray;

            PictureBox currCard = new PictureBox();
            Bitmap card = new Bitmap(this.currCardPic);
            currCard.SizeMode = PictureBoxSizeMode.StretchImage;
            currCard.ClientSize = new Size(card.Width / 3, card.Height / 3);
            currCard.Image = (Image)card;
            currCard.Location = new Point(this.Width / 2 - card.Width , this.Height / 2 - card.Height);

            int side = 0; // 0 = right, 1 = up, 2 = left
            PictureBox cardCover;
            Bitmap cover = new Bitmap(this.cardCover);
            Label playerName;

            foreach (KeyValuePair<string, int> kvp in this.enemiesCardsAmount)
            {
                playerName = new Label();
                playerName.Font = new Font("Arial", 14);
                playerName.AutoSize = true;
                playerName.Text = kvp.Key;

                cardCover = new PictureBox();
                cardCover.SizeMode = PictureBoxSizeMode.StretchImage;
                cardCover.ClientSize = new Size(cover.Width / 3, cover.Height / 3);
                cardCover.Image = (Image)cover;
                cardCover.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                for (int i = 0; i < kvp.Value; i++)
                {
                    switch (side)
                    {
                        case 0:
                            playerName.Location = new Point( 99 * this.Width / 100 , this.Height/ 5 -  3 * cardCover.Width / 4);
                            cardCover.Location = new Point(99 * this.Width / 100, this.Height / 5 + i * 3 * cardCover.Width / 4);
                            break;
                        case 1:
                            playerName.Location = new Point(this.Width / 5 - 3 * cardCover.Width / 4, this.Height / 100);
                            cardCover.Location = new Point(this.Width / 5 + 3 *i * cardCover.Width / 4, this.Height / 100);
                            break;
                        default:
                            playerName.Location = new Point(this.Width / 100, this.Height / 5 - 3 * cardCover.Width / 4);
                            cardCover.Location = new Point(this.Width / 100, this.Height / 5 + i * 3 * cardCover.Width / 4);
                            break;
                    }
                    this.Controls.Add(playerName);
                    this.Controls.Add(cardCover);
                    side++;
                    
                }


                cardCover = new PictureBox();
                cardCover.SizeMode = PictureBoxSizeMode.StretchImage;
                cardCover.ClientSize = new Size(cover.Width / 3, cover.Height / 3);
                cardCover.Image = (Image)cover;
                cardCover.Location = new Point(this.Width / 2 + card.Width, this.Height / 2 + card.Height);


                PictureBox mycard;
                int index = 0;
                foreach (Bitmap cardPic in this.myCardsPics)
                {
                    mycard = new PictureBox();
                    mycard.SizeMode = PictureBoxSizeMode.StretchImage;
                    mycard.ClientSize = new Size(cardPic.Width / 3, cardPic.Height / 3);
                    mycard.Image = (Image)cardPic;
                    cardCover.Location = new Point(this.Width / 5 + 3 * index * cardCover.Width / 4, this.Height / 100);
                    this.Controls.Add(mycard);
                }
            }

        }
    }
}
