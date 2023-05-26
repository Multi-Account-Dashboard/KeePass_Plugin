using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{


    internal class GraphLine
    {
        public ImageEnum image;
        public Point StartPoint;
        public Point EndPoint;
        public PictureBox pb = null;

        private Bitmap m_identity = global::MAD_Plugin.Properties.Resources.Identity;
        private Bitmap m_phone = global::MAD_Plugin.Properties.Resources.Phone;
        private Bitmap m_mail = global::MAD_Plugin.Properties.Resources.RecoveryMail;

        /// <summary>Initializes a new instance of the <see cref="GraphLine" /> class.</summary>
        /// <param name="x1">The x coordinate of the start point.</param>
        /// <param name="y1">The y coordinate of the start point.</param>
        /// <param name="x2">The x coordinate of the end point.</param>
        /// <param name="y2">The y coordinate of the end point.</param>
        /// <param name="image">The image type to be placed on the line.</param>
        public GraphLine(int x1, int y1, int x2, int y2, ImageEnum image)
        {
            this.StartPoint = new Point(x1, y1);
            this.EndPoint = new Point(x2, y2);
            this.image = image;
        }
        /// <summary>Initializes a new instance of the <see cref="GraphLine" /> class.</summary>
        /// <param name="x1">The x coordinate of the start point.</param>
        /// <param name="y1">The y coordinate of the start point.</param>
        /// <param name="x2">The x coordinate of the end point.</param>
        /// <param name="y2">The y coordinate of the end point.</param>
        public GraphLine(int x1, int y1, int x2, int y2)
        {
            this.StartPoint = new Point(x1, y1);
            this.EndPoint = new Point(x2, y2);

        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return StartPoint.X * StartPoint.Y ^ (EndPoint.Y + EndPoint.Y);
        }

        /// <summary>Checks if this instance is equal to another GraphLine.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public bool Equals(GraphLine obj)
        {
            return (this.StartPoint.X == obj.StartPoint.X && this.StartPoint.Y == obj.StartPoint.Y && this.EndPoint.X == obj.EndPoint.X && this.EndPoint.X == obj.EndPoint.X && this.image == obj.image);
        }

        /// <summary>Determines whether the specified <see cref="System.Object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GraphLine);
        }

        /// <summary>Sets the image.</summary>
        /// <param name="image">The image.</param>
        public void SetImage(ImageEnum image)
        {
            this.image = image;
        }

        /// <summary>uses basic linear operations to calculate the right position of each image in the middle of the GraphLine .</summary>
        /// <returns>
        ///   returns the line image location
        /// </returns>
        public Point GetLineImageLocation()
        {
            //calculating the middle point of the line
            int xOfMiddle = (this.StartPoint.X + this.EndPoint.X) / 2;
            int yOfMiddle = (this.StartPoint.Y + this.EndPoint.Y) / 2;
            Point middlePoint = new Point(xOfMiddle, yOfMiddle);

            //gets the surface normal of the line which is orthogonal to the line
            int normalVectorX = this.EndPoint.X - this.StartPoint.X;
            int normalVectorY = this.EndPoint.Y - this.StartPoint.Y;
            Point normalVector = new Point(-normalVectorY, normalVectorX);

            //make the surface normal vector have a lenght of 1
            PointF normalizedNormalVector = Normalize(normalVector); 

            //now looking at the calculated midpoint of the line, apply the normal vector 20 times
            float x = middlePoint.X + (normalizedNormalVector.X * 20);
            float y = middlePoint.Y + (normalizedNormalVector.Y * 20);

            //if we put the picturebox on the current lokation of middlePoint, we get inconsistent results because thr position of the PictureBox is its upper left corner. B
            //But we need its center to be at the current middlePoint. hence the subtraction
            middlePoint.X = Convert.ToInt32(x - 12.5);
            middlePoint.Y = Convert.ToInt32(y - 12.5);

            //Because the symbols need to be ordered to not overlap, we need to create distance between the symbols
            PointF normalizedStartVector1F = Normalize(new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y));
            PointF normalizedStartVector2F = Normalize(new Point(StartPoint.X - EndPoint.X, StartPoint.Y - EndPoint.Y));

            //which is applied here depending on the symbol to be displayed
            if (image == ImageEnum.Identity) { return middlePoint; }
            else if (image == ImageEnum.Phone) { return new Point(middlePoint.X + Convert.ToInt32((30 * normalizedStartVector1F.X)), middlePoint.Y + Convert.ToInt32((30 * normalizedStartVector1F.Y))); }
            return new Point(middlePoint.X + Convert.ToInt32((30 * normalizedStartVector2F.X)), middlePoint.Y + Convert.ToInt32((30 * normalizedStartVector2F.Y)));




        }

        /// <summary>Disposes the image.</summary>
        public void DisposeImage()
        {
            pb.Dispose();
        }

        /// <summary>Creates a PictureBoxe for the specified image.</summary>
        /// <param name="image">The image to be displayed on top of the line.</param>
        public PictureBox PictureBox(ImageEnum image)
        {
            ToolTip tt = new ToolTip();
            PictureBox pb = new PictureBox();
            pb.AccessibleRole = AccessibleRole.None;
            pb.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            pb.BackgroundImageLayout = ImageLayout.Zoom;
            pb.BackColor = Color.Transparent;
            if (image == ImageEnum.Identity) { pb.Image = m_identity; tt.SetToolTip(pb, "These accounts are connected via same Identity"); }
            if (image == ImageEnum.Phone) { pb.Image = m_phone; tt.SetToolTip(pb, "These accounts are connected via same Recovery-Phone"); }
            if (image == ImageEnum.Mail) { pb.Image = m_mail; tt.SetToolTip(pb, "These accounts are connected via same Recovery-Email"); }
            pb.Location = GetLineImageLocation();
            pb.MaximumSize = new Size(24, 24);
            pb.Name = "PictureBox" + this.StartPoint + "" + this.EndPoint;
            pb.Size = new Size(24, 24);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.TabIndex = 3;
            pb.TabStop = false;
            return pb;
        }

        /// <summary>Normalizes the specified vector.</summary>
        /// <param name="vector">The vector to be normalized.</param>
        /// <returns>
        ///   returns the same vector but with length 1 or a standart vector if length is somehow smaller than 0
        /// </returns>
        public PointF Normalize(Point vector)
        {
            float a = vector.X * vector.X;
            float b = vector.Y * vector.Y;
            float length = Convert.ToInt32(Math.Sqrt(a + b));
            if (length > 0)
            {
                float rtnX = vector.X * (1 / length);
                float rtnY = vector.Y * (1 / length);
                return new PointF(rtnX, rtnY);
            }
            else
                return new PointF(100, 100);
        }
    }
}