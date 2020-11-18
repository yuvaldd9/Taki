using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Timers;

namespace Taki_Client
{
    class GamePanel : Panel
    {
        private Deck deck;
        private List<Enemy> enemies;
        private string name;
        private Dictionary<Key, Bitmap> images;
        private System.Timers.Timer timer;
        private Queue<Animation> moves;
        private PictureBox[,,] placesMatrix;
        private Card[,] cardsMatrix;
        private string currentPlayer;
        public PictureBox topCardPicture;

        public GamePanel(Deck deck, List<Enemy> enemies, string name) : base()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Gray;
            this.enemies = enemies;
            this.deck = deck;
            this.name = name;
            images = new Dictionary<Key, Bitmap>();
            this.timer = new System.Timers.Timer();
            this.timer.Interval = 1;
            this.timer.Elapsed += new ElapsedEventHandler(this.timer_Tick);
            this.moves = new Queue<Animation>();
            this.placesMatrix = new PictureBox[4, 2, 8]; // 4 - number of players, 2 - rows of cards, 8 - cards at each row
            this.cardsMatrix = new Card[2, 8];
            this.currentPlayer = "";
        }

        public void Initialize(Card topCard)
        {
            this.timer.Start();

            //Load cards images
            string[] filesList;
            foreach (string directory in new string[] { "GreenCards", "YellowCards", "RedCards", "BlueCards", "SpecialCards" })
            {
                filesList = Directory.GetFiles(@"..\\..\\Resources\\" + directory);
                foreach (string file in filesList)
                {
                    this.images.Add(new Key(directory, Path.GetFileName(file).Split(new char[] { '.' })[0]), new Bitmap(file));
                }
            }

            Label nameLabel = new Label();
            nameLabel.Text = this.name;
            nameLabel.Font = new Font("Arial", 14);
            nameLabel.AutoSize = true;
            this.Controls.Add(nameLabel);
            nameLabel.Location = new Point((this.Width - nameLabel.Width) / 2, this.Height - 20);

            Label enemy1Label = new Label();
            enemy1Label.Text = this.enemies.ElementAt(0).name;
            enemy1Label.Font = new Font("Arial", 14);
            enemy1Label.AutoSize = true;
            this.Controls.Add(enemy1Label);
            enemy1Label.Location = new Point(20, 160);

            Label enemy2Label = new Label();
            enemy2Label.Text = this.enemies.ElementAt(1).name;
            enemy2Label.Font = new Font("Arial", 14);
            enemy2Label.AutoSize = true;
            this.Controls.Add(enemy2Label);
            enemy2Label.Location = new Point((this.Width - enemy2Label.Width) / 2, 20);

            Label enemy3Label = new Label();
            enemy3Label.Text = this.enemies.ElementAt(2).name;
            enemy3Label.Font = new Font("Arial", 14);
            enemy3Label.AutoSize = true;
            this.Controls.Add(enemy3Label);
            enemy3Label.Location = new Point(this.Width - 20 - enemy3Label.Width, 160);

            //Load player's cards
            PictureBox cardPicture;
            Bitmap image, newImage;
            Graphics graphics;
            int index = 0;
            foreach (Card card in this.deck.cards)
            {
                image = GetImage(card);
                image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                graphics = Graphics.FromImage(newImage);
                graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                cardPicture = new PictureBox();
                cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                cardPicture.ClientSize = new Size(image.Width, image.Height);
                cardPicture.Image = newImage;
                cardPicture.Location = new Point(200 + 52 * index, this.Height - image.Height - 40);
                this.Controls.Add(cardPicture);
                this.placesMatrix[0, 0, index] = cardPicture;
                this.cardsMatrix[0, index] = card;
                index++;
            }



            //Load enemies cards
            for (int i = 0; i < this.enemies.ElementAt(0).NumOfCards; i++)
            {
                image = new Bitmap(GetImage(null)); //Get back card
                image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                graphics = Graphics.FromImage(newImage);
                graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                newImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                cardPicture = new PictureBox();
                cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                cardPicture.ClientSize = new Size(image.Height, image.Width);
                cardPicture.Image = newImage;
                cardPicture.Location = new Point(50, 200 + 52 * i);
                this.Controls.Add(cardPicture);
                this.placesMatrix[1, 0, i] = cardPicture;
            }

            for (int i = 0; i < this.enemies.ElementAt(1).NumOfCards; i++)
            {
                image = new Bitmap(GetImage(null)); //Get back card
                image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                graphics = Graphics.FromImage(newImage);
                graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                cardPicture = new PictureBox();
                cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                cardPicture.ClientSize = new Size(image.Width, image.Height);
                cardPicture.Image = newImage;
                cardPicture.Location = new Point(200 + 52 * i, 40);
                this.Controls.Add(cardPicture);
                this.placesMatrix[2, 0, i] = cardPicture;
            }

            for (int i = 0; i < this.enemies.ElementAt(2).NumOfCards; i++)
            {
                image = new Bitmap(GetImage(null)); //Get back card
                image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                graphics = Graphics.FromImage(newImage);
                graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                newImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                cardPicture = new PictureBox();
                cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                cardPicture.ClientSize = new Size(image.Height, image.Width);
                cardPicture.Image = newImage;
                cardPicture.Location = new Point(this.Width - cardPicture.Width - 50, 200 + 52 * i);
                this.Controls.Add(cardPicture);
                this.placesMatrix[3, 0, i] = cardPicture;
            }

            //Draw the card pile and the top card
            image = new Bitmap(GetImage(null)); //Get back card
            image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
            newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
            graphics = Graphics.FromImage(newImage);
            graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
            cardPicture = new PictureBox();
            cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
            cardPicture.ClientSize = new Size(image.Width, image.Height);
            cardPicture.Image = newImage;
            cardPicture.Location = new Point(this.Width / 2 - cardPicture.Width - 2, this.Height / 2 - cardPicture.Height);
            this.Controls.Add(cardPicture);

            image = new Bitmap(GetImage(topCard)); //Get back card
            image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
            newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
            graphics = Graphics.FromImage(newImage);
            graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
            this.topCardPicture = new PictureBox();
            this.topCardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
            this.topCardPicture.ClientSize = new Size(image.Width, image.Height);
            this.topCardPicture.Image = newImage;
            this.topCardPicture.Location = new Point(this.Width / 2 + 2, this.Height / 2 - cardPicture.Height);
            this.Controls.Add(this.topCardPicture);

        }

        private Bitmap GetImage(Card card)
        {
            string directory;
            string filename;
            if (card == null)
                return this.images[new Key("SpecialCards", "Back Card")];
            switch (card.Color)
            {
                case "blue":
                    directory = "BlueCards";
                    break;
                case "red":
                    directory = "RedCards";
                    break;
                case "green":
                    directory = "GreenCards";
                    break;
                case "yellow":
                    directory = "YellowCards";
                    break;
                default:
                    directory = "SpecialCards";
                    break;
            }

            switch (card.Type)
            {
                case "number_card":
                    filename = card.Value;
                    break;
                case "plus_2":
                    filename = "Plus 2";
                    break;
                case "plus":
                    filename = "Plus";
                    break;
                case "change_direction":
                    filename = "Change Direction";
                    break;
                case "stop":
                    filename = "Stop";
                    break;
                case "taki":
                    filename = "Taki";
                    break;
                case "super_taki":
                    filename = "Super Taki";
                    directory = "SpecialCards";
                    break;
                case "change_color":
                    filename = "Change Color";
                    directory = "SpecialCards";
                    break;
                default:
                    filename = "Back Card";
                    break;
            }

            return this.images[new Key(directory, filename)];

        }

        public void UpdateCurrentPlayer(string name)
        {
            Label prevNameLabel = null, currentNameLabel = null;
            foreach (Control control in this.Controls)
            {
                if (control is Label)
                {
                    if (control.Text == name)
                        currentNameLabel = (Label)control;
                    if (control.Text == this.currentPlayer)
                        prevNameLabel = (Label)control;
                }
            }
            this.moves.Enqueue(new ChangeCurrentPlayer(prevNameLabel, currentNameLabel));
            this.currentPlayer = name;
        }
        public void PlayCards(string playerName, List<Card> cardsToPlay)
        {
            int numOfPlayer = 0;
            if (playerName == this.name)
                numOfPlayer = 0;
            else
            {
                for (int i = 0; i < this.enemies.Count; i++)
                    if (this.enemies.ElementAt(i).name == playerName)
                    {
                        numOfPlayer = i + 1;
                        break;
                    }
            }
            int row = 0, col = 0;
            PictureBox cardPicture = null;
            Bitmap image, newImage = null;
            Graphics graphics;
            Point end;
            foreach (Card card in cardsToPlay)
            {
                cardPicture = null;
                if (numOfPlayer == 0)
                {
                    for (row = 0; row < this.cardsMatrix.GetLength(0) && cardPicture == null; row++)
                    {
                        for (col = 0; col < this.cardsMatrix.GetLength(1) && cardPicture == null; col++)
                        {
                            if (this.cardsMatrix[row, col] != null && this.cardsMatrix[row, col].Equals(card))
                                cardPicture = this.placesMatrix[numOfPlayer, row, col];
                        }
                    }
                    row--;
                    col--;
                }
                else
                {
                    for (row = this.placesMatrix.GetLength(1) - 1; row >= 0 && cardPicture == null; row--)
                    {
                        for (col = this.placesMatrix.GetLength(2) - 1; col >= 0 && cardPicture == null; col--)
                        {
                            if (this.placesMatrix[numOfPlayer, row, col] != null)
                                cardPicture = this.placesMatrix[numOfPlayer, row, col];
                        }
                    }
                    row++;
                    col++;
                }
                if (cardPicture == null)
                {
                    Console.WriteLine("Failed to find picture " + card.Type + card.Value + card.Color);
                    return;
                }

                this.Invoke(new MethodInvoker(delegate () { cardPicture.BringToFront(); }));

                if (numOfPlayer % 2 == 0)
                    end = new Point(this.Width / 2 + 2, this.Height / 2 - cardPicture.Height);
                else
                    end = new Point(this.Width / 2 + 2, this.Height / 2 - cardPicture.Width);
                double angle = 0;
                switch (numOfPlayer)
                {
                    case 0:
                        angle = 0;
                        break;
                    case 1:
                        angle = -90;
                        break;
                    case 2:
                        angle = 0;
                        break;
                    case 3:
                        angle = 90;
                        break;
                }
                if (numOfPlayer == 0)
                    this.CreateMoves(cardPicture.Location, end, angle, cardPicture);
                else
                {
                    image = this.GetImage(card);
                    image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                    newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                    graphics = Graphics.FromImage(newImage);
                    graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                    PointF center = new PointF(newImage.Width / 2, newImage.Height / 2);
                    Bitmap rotatedImage = new Bitmap(newImage.Width, newImage.Height);
                    graphics = Graphics.FromImage(rotatedImage);
                    graphics.TranslateTransform(center.X, center.Y);
                    graphics.RotateTransform(-(float)angle);
                    graphics.TranslateTransform(-center.X, -center.Y);
                    graphics.DrawImage(newImage, 0, 0);
                    this.moves.Enqueue(new ChangePictureBoxImage(rotatedImage, cardPicture, this));
                    this.CreateMoves(cardPicture.Location, end, angle, cardPicture, rotatedImage);
                }
                this.moves.Enqueue(new ReplaceTopCardPicture(cardPicture, this));
                this.placesMatrix[numOfPlayer, row, col] = null;
                if (numOfPlayer == 0)
                    this.cardsMatrix[row, col] = null;
            }

        }
        public void TakeCards(string playerName, List<Card> cardsToTake)
        {
            int numOfPlayer = 0;
            if (playerName == this.name)
                numOfPlayer = 0;
            else
            {
                for (int i = 0; i < this.enemies.Count; i++)
                    if (this.enemies.ElementAt(i).name == playerName)
                    {
                        numOfPlayer = i + 1;
                        break;
                    }
            }
            int row = 0, col = 0;
            bool foundPlace = false;
            PictureBox cardPicture;
            Bitmap image, newImage;
            Graphics graphics;
            Point end;
            foreach (Card card in cardsToTake)
            {
                foundPlace = false;
                for (row = 0; row < this.placesMatrix.GetLength(1) && !foundPlace; row++)
                {
                    for (col = 0; col < this.placesMatrix.GetLength(2) && !foundPlace; col++)
                    {
                        if (this.placesMatrix[numOfPlayer, row, col] == null)
                            foundPlace = true;
                    }
                }
                col--;
                row--;
                if (numOfPlayer == 0)
                    image = new Bitmap(GetImage(card));
                else
                    image = new Bitmap(GetImage(null));
                image = new Bitmap(image, new Size(50, (int)(50.0 / image.Width * image.Height)));
                newImage = new Bitmap((int)(image.Width * 2), (int)(image.Height * 2));
                graphics = Graphics.FromImage(newImage);
                graphics.DrawImageUnscaled(image, image.Width / 2, image.Height / 2, image.Width, image.Height);
                cardPicture = new PictureBox();
                cardPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                cardPicture.ClientSize = new Size(image.Width, image.Height);
                cardPicture.Image = newImage;
                cardPicture.Location = new Point(this.Width / 2 - cardPicture.Width - 2, this.Height / 2 - cardPicture.Height);
                this.moves.Enqueue(new AddPicture(cardPicture, this));
                this.placesMatrix[numOfPlayer, row, col] = cardPicture;

                switch (numOfPlayer)
                {
                    case 0:
                        end = new Point(200 + 52 * col, this.Height - image.Height - 40 - row * (image.Height + 2));
                        this.CreateMoves(cardPicture.Location, end, 0, cardPicture);
                        this.cardsMatrix[row, col] = card;
                        break;
                    case 1:
                        end = new Point(50 + row * (image.Height + 2), 200 + 52 * col);
                        this.CreateMoves(cardPicture.Location, end, 90, cardPicture, (Bitmap)cardPicture.Image);
                        break;
                    case 2:
                        end = new Point(200 + 52 * col, 40 + row * (image.Height + 2));
                        this.CreateMoves(cardPicture.Location, end, 0, cardPicture);
                        break;
                    case 3:
                        end = new Point(this.Width - image.Height - 50 - row * (image.Height + 2), 200 + 52 * col);
                        this.CreateMoves(cardPicture.Location, end, -90, cardPicture, (Bitmap)cardPicture.Image);
                        break;
                }


            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.moves.Count > 0)
            {
                Animation animation = this.moves.Dequeue();
                this.Invoke(new MethodInvoker(delegate () { animation.Execute(); }));

            }
        }

        private void CreateMoves(Point start, Point end, double angle, PictureBox cardPicture, Bitmap originalBitmap = null)
        {
            double slope, yIntersect;
            cardPicture.BringToFront();
            if (angle == 0)
            {
                if (end.X - start.X != 0)
                {
                    slope = (double)(end.Y - start.Y) / (end.X - start.X);
                    yIntersect = end.Y - slope * end.X;

                    if (end.X > start.X)
                    {
                        for (int i = start.X; i < end.X; i = i + 2)
                        {
                            this.moves.Enqueue(new Move(new Point(i, (int)(slope * i + yIntersect)), cardPicture, this));
                        }
                    }
                    else
                    {
                        for (int i = start.X; i > end.X; i = i - 2)
                        {
                            this.moves.Enqueue(new Move(new Point(i, (int)(slope * i + yIntersect)), cardPicture, this));
                        }
                    }
                    this.moves.Enqueue(new Move(end, cardPicture, this));
                }
                else if (end.Y - start.Y != 0)
                {
                    if (end.Y > start.Y)
                    {
                        for (int i = start.Y; i < end.Y; i = i + 2)
                        {
                            this.moves.Enqueue(new Move(new Point(end.X, i), cardPicture, this));
                        }
                    }
                    else
                    {
                        for (int i = start.Y; i > end.Y; i = i - 2)
                        {
                            this.moves.Enqueue(new Move(new Point(end.X, i), cardPicture, this));
                        }
                    }
                }
            }

            else if (angle == 90 || angle == 270 || angle == -90 || angle == -270)
            {
                Point middle = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

                cardPicture.BringToFront();
                this.CreateMoves(cardPicture.Location, middle, 0, cardPicture);
                if (originalBitmap == null)
                    originalBitmap = (Bitmap)cardPicture.Image.Clone();
                Size originalSize = cardPicture.Size;
                this.moves.Enqueue(new ResizePictureBox(new Size((int)(cardPicture.Width * 1.5), (int)(cardPicture.Width * 1.5)), cardPicture, this));
                if (angle > 0)
                {
                    for (int i = 1; i < angle; i++)
                    {
                        this.moves.Enqueue(new Rotate(i, originalBitmap, cardPicture, this));
                    }
                }
                else
                {
                    for (int i = -1; i > angle; i--)
                    {
                        this.moves.Enqueue(new Rotate(i, originalBitmap, cardPicture, this));
                    }
                }
                this.moves.Enqueue(new ResizePictureBox(new Size(originalSize.Height, originalSize.Width), cardPicture, this));
                this.CreateMoves(middle, end, 0, cardPicture);
            }


        }

    }

    class Key
    {
        private string directory;
        private string filename;

        public Key(string directory, string filename)
        {
            this.directory = directory;
            this.filename = filename;
        }

        public override bool Equals(object obj)
        {
            if (obj is Key)
                return this.Equals((Key)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return this.directory.GetHashCode() + this.filename.GetHashCode();
        }
        public bool Equals(Key key)
        {
            return this.directory == key.directory && this.filename == key.filename;
        }
    }

}
