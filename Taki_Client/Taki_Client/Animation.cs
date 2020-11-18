using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Taki_Client
{
    abstract class Animation
    {
        protected PictureBox image;
        protected GamePanel panel;

        public Animation(PictureBox image, GamePanel panel)
        {
            this.image = image;
            this.panel = panel;
        }

        public virtual void Execute()
        {

        }
    }

    class Move : Animation
    {
        private Point end;

        public Move(Point end, PictureBox image, GamePanel panel) : base(image, panel)
        {
            this.end = end;
        }

        public override void Execute()
        {
            this.image.Location = end;

        }
    }

    class Rotate : Animation
    {
        private double angle;
        private Bitmap originalBitmap;

        public Rotate(double angle, Bitmap originalBitmap, PictureBox image, GamePanel panel) : base(image, panel)
        {
            this.angle = angle;
            this.originalBitmap = originalBitmap;
        }

        public override void Execute()
        {
            PointF center = new PointF(this.originalBitmap.Width / 2, this.originalBitmap.Height / 2);
            Bitmap rotatedImage = new Bitmap(this.originalBitmap.Width, this.originalBitmap.Height);
            Graphics graphics = Graphics.FromImage(rotatedImage);
            graphics.TranslateTransform(center.X, center.Y);
            graphics.RotateTransform((float)this.angle);
            graphics.TranslateTransform(-center.X, -center.Y);
            graphics.DrawImage(this.originalBitmap, 0, 0);
            this.image.Image = rotatedImage;
            graphics.ResetTransform();

        }
    }

    class ResizePictureBox : Animation
    {
        private Size size;
        public ResizePictureBox(Size size, PictureBox image, GamePanel panel) : base(image, panel)
        {
            this.size = size;
        }

        public override void Execute()
        {
            int dx = (this.size.Width - this.image.Width) / 2;
            int dy = (this.size.Height - this.image.Height) / 2;
            this.image.Location = new Point(this.image.Location.X - dx, this.image.Location.Y - dy);
            this.image.Size = this.size;

        }
    }

    class AddPicture : Animation
    {
        public AddPicture(PictureBox image, GamePanel panel) : base(image, panel) { }

        public override void Execute()
        {
            this.panel.Controls.Add(image);
            image.BringToFront();
        }

    }

    class ChangePictureBoxImage : Animation
    {
        private Bitmap newImage;

        public ChangePictureBoxImage(Bitmap newImage, PictureBox image, GamePanel panel) : base(image, panel)
        {
            this.newImage = newImage;
        }

        public override void Execute()
        {
            this.image.Image = newImage;
        }
    }

    class ReplaceTopCardPicture : Animation
    {
        public ReplaceTopCardPicture(PictureBox image, GamePanel panel) : base(image, panel) { }

        public override void Execute()
        {
            this.panel.Controls.Remove(this.panel.topCardPicture);
            this.panel.topCardPicture = this.image;
        }
    }

    class ChangeCurrentPlayer : Animation
    {
        private Label prevNameLabel;
        private Label currentNameLabel;

        public ChangeCurrentPlayer(Label prevNameLabel, Label currentNameLabel) : base(null, null)
        {
            this.prevNameLabel = prevNameLabel;
            this.currentNameLabel = currentNameLabel;
        }

        public override void Execute()
        {
            if (this.prevNameLabel != null)
                this.prevNameLabel.BackColor = Color.Transparent;
            if (this.currentNameLabel != null)
                this.currentNameLabel.BackColor = Color.Gold;
        }
    }
}
