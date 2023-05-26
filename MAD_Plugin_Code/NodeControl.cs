using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Utility;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MAD_Plugin
{
    public class NodeControl : GroupBox
    {
      

        /// <summary>
        ///  the enumeration encodeing the trafficlight colors
        /// </summary>
        public enum TrafficLightColorsEnum
        {
            Red,
            Yellow,
            Green,
            Gray
        }

        /// <summary>
        ///  the enumeration encodeing the accounttypes
        /// </summary>
        public enum AccountTypeEnum 
        {
            NoType,
            Banking,
            Mail,
            SocialMedia,
            Shopping,
            Custom
        }
        /// <summary>
        ///  the enumeration encodeing the types of images displayed insinde of a node
        /// </summary>
        public enum ImageEnum
        {
            Accounttype,
            Identity,
            TrafficLight,
            Clock,
            Lock,
            PKQ,
            Mail,
            Phone,
            Backup
        }


        /// <summary>Gets or sets the list containing all other nodes with the same identity.</summary>
        /// <value>The con identifier.</value>
        public List<string> ConId
        {
            get { return m_conIds; }
            set { m_conIds = value; }
        }
        /// <summary>Gets or sets the list containing all other nodes with the same recovery phone number.</summary>
        /// <value>The con phone.</value>
        public List<string> conPhone
        {
            get { return m_conPhone; }
            set { m_conPhone = value; }
        }
        /// <summary>Gets or sets the list containing all other nodes with the same recovery email address.</summary>
        /// <value>The con mail.</value>
        public List<string> ConMail
        {
            get { return m_conMail; }
            set { m_conMail = value; }
        }


        private List<string> m_conIds = new List<string>();
        private List<string> m_conPhone = new List<string>();
        private List<string> m_conMail = new List<string>();


        private Point m_pos;

        private PwUuid m_VisualId;
        private string m_VisualIdString;
        private PwEntry m_Entry = null;
        private PwDatabase m_Db = null;
        public bool upForDelete = false;

        //important for the evaluation
        public int setScore = -1;
        public int realScore = -1;
        public int numberOfFixes = 0;
        public int numberOfWarnings = 0;
    
        public List<string> MailEdges = new List<string>(); // used for visualization of edges

        private const int m_standartWidth = 175;
        private const int m_standartHeight = 150;


        private Control m_selContr = null;
        public AccountTypeEnum m_accountType = AccountTypeEnum.NoType;
        private ToolTip m_ttPictureBox = new ToolTip();
      
        // the pictures to be used as sysmbols inside the nodecontrols
        private Bitmap m_redLight = global::MAD_Plugin.Properties.Resources.RedLight;
        private Bitmap m_yellowLight = global::MAD_Plugin.Properties.Resources.YellowLight;
        private Bitmap m_greenLight = global::MAD_Plugin.Properties.Resources.GreenLight;
        private Bitmap m_clockRed = global::MAD_Plugin.Properties.Resources.ClockRed;
        private Bitmap m_clockGreen = global::MAD_Plugin.Properties.Resources.ClockGreen;
        private Bitmap m_bankingAccount = global::MAD_Plugin.Properties.Resources.Euro;
        private Bitmap m_mailAccount = global::MAD_Plugin.Properties.Resources.Email;
        private Bitmap m_customAccount = global::MAD_Plugin.Properties.Resources.ServiceImage; 
        private Bitmap m_socialMediaAccount = global::MAD_Plugin.Properties.Resources.SocialMedia;
        private Bitmap m_shoppingAccount = global::MAD_Plugin.Properties.Resources.Shopping;
        private Bitmap m_lock = global::MAD_Plugin.Properties.Resources.Lock;
        private Bitmap m_phone = global::MAD_Plugin.Properties.Resources.Phone;
        private Bitmap m_mail = global::MAD_Plugin.Properties.Resources.RecoveryMail;
        private Bitmap m_backup = global::MAD_Plugin.Properties.Resources.Backup;
        private Bitmap m_pkq = global::MAD_Plugin.Properties.Resources.PKQ;
        private Bitmap m_identity = global::MAD_Plugin.Properties.Resources.Identity;
        private Bitmap m_greyLight = global::MAD_Plugin.Properties.Resources.GreyLight;
        private bool m_isInitialDraw = true;

        private Point m_locOfFaBox;


        private EvalResultForm m_evalResultForm = new EvalResultForm();


        private bool m_evalFormenabled = false;



        /// <summary>Gets the entry.</summary>
        /// <value>The entry.</value>
        public PwEntry Entry
        {
            get { return this.m_Entry; }
        }

        /// <summary>Gets the visual identifier.</summary>
        /// <value>The visual identifier.</value>
        public PwUuid VisualId
        {
            get
            {
                return m_VisualId;
            }
        }

        /// <summary>Gets the visual identifier as a string.</summary>
        /// <value>The visual identifier string.</value>
        public string VisualIdString
        {
            get
            {
                return m_VisualIdString;
            }
        }

        /// <summary>Gets the node eval form.</summary>
        /// <value>The node eval form.</value>
        public EvalResultForm NodeEvalForm
        {
            get
            {
                return m_evalResultForm;
            }
        }


        /// <summary>Initializes a new instance of the <see cref="NodeControl" /> class.</summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="id">The VisualId.</param>
        /// <param name="pos">The position of the node on the dashboard.</param>
        /// <param name="db">The database of the correlation KeePass entry.</param>
        /// <param name="entry">The correlation KeePass entry.</param>
        public NodeControl(String title, PwUuid id, Point pos, PwDatabase db, PwEntry entry)
        {
            this.DoubleBuffered = true; // makes moving the node look smoother
            this.m_VisualId = id;
            this.m_VisualIdString = MemUtil.ByteArrayToHexString(id.UuidBytes);
            this.m_pos = pos;
            this.Name = title + VisualIdString;
            if (title.Length > 12) { string s = title.Substring(0, 9) + "..."; this.Text = s; }
            else { this.Text = title; }
            
            this.Anchor = AnchorStyles.None;
            this.BackColor = SystemColors.InactiveCaption;
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = pos;
            this.TabStop = false;

            if (TextRenderer.MeasureText(this.Text, new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)))).Width + 60 > m_standartWidth)
            {
                this.Size = new Size(TextRenderer.MeasureText(this.Text, new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)))).Width + 60, m_standartHeight);
            }
            else
            {
                this.Size = new Size(m_standartWidth, m_standartHeight);
            }
            this.Controls.Add(CreateTrafficLight(new Point(this.Size.Width - 33, 2)));
            this.m_Entry = entry;
            this.m_Db = db;
            this.m_ttPictureBox.AutoPopDelay = 5000;
            this.m_ttPictureBox.InitialDelay = 1000;
            this.m_ttPictureBox.ReshowDelay = 500;
  
        }



        /// <summary>Initializes a new instance of the <see cref="NodeControl" /> class when no visualId is already present.</summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="pos">The position of the node on the dashboard.</param>
        /// <param name="db">The database of the correlation KeePass entry.</param>
        /// <param name="entry">The correlation KeePass entry.</param>
        public NodeControl(String title, Point pos, PwDatabase db, PwEntry entry)
        {
            this.DoubleBuffered = true; // makes moving nodes look smoother
            this.m_VisualId = new PwUuid(true);
            this.m_VisualIdString = MemUtil.ByteArrayToHexString(m_VisualId.UuidBytes);
            this.m_pos = pos;
            this.Name = title + VisualIdString;
            if (title.Length > 12) { string s = title.Substring(0, 9) + "..."; this.Text = s; }
            else { this.Text = title; }
            
            this.Anchor = AnchorStyles.None;
            this.BackColor = SystemColors.InactiveCaption;
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = pos;
            this.TabStop = false;
            if (TextRenderer.MeasureText(this.Text, new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)))).Width + 60 > m_standartWidth)
            {
                this.Size = new Size(TextRenderer.MeasureText(this.Text, new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)))).Width + 60, m_standartHeight);
            }
            else
            {
                this.Size = new Size(m_standartWidth, m_standartHeight);
            }
            this.Controls.Add(CreateTrafficLight(new Point(this.Size.Width - 33, 2)));
            this.m_Entry = entry;
            this.m_Db = db;
            this.m_ttPictureBox.AutoPopDelay = 5000;
            this.m_ttPictureBox.InitialDelay = 1000;
            this.m_ttPictureBox.ReshowDelay = 500;
           




        }



        /// <summary>Gets the rectangle shape of the node .</summary>
        public Rectangle GetRectangle()
        {
            Rectangle r = new Rectangle(this.Location, this.Size);
            return r;
       
        }


        /// <summary>Updates all images that part of a node or sets them if not alread present.</summary>
        public void UpdateAllImages()
        {
            UpdateClockImages();
            UpdateAccountTypeImages();
            UpdateIdentityImage();
            UpdateLockImage();
            UpdateBackupImage();
            UpdateMailImage();
            UpdatePhoneImage();
            UpdatePkqImage();
          if (!m_isInitialDraw) { this.Invalidate(); }
        }



        /// <summary>Creates a PictureBox in which a symbol of the node is presented.</summary>
        /// <param name="pos">The position of the box inside the node.</param>
        /// <param name="size">The size of the box.</param>
        /// <param name="image">The image to be displayed.</param>
        /// <param name="type">The type of image to be displayed.</param>
        /// <returns>
        ///   a Picturebox with the given parameters set
        /// </returns>
        public PictureBox CreatePictureBox(Point pos, Size size, Bitmap image, ImageEnum type)
        {
            PictureBox pb = new PictureBox();
            switch (type)
            {
                case ImageEnum.Mail: { pb.Name = "Mail" + this.Name; break; }
                case ImageEnum.Phone: { pb.Name = "Phone" + this.Name; break; }
                case ImageEnum.Backup: { pb.Name = "Backup" + this.Name; break; }
                case ImageEnum.Lock: { pb.Name = "Lock" + this.Name; break; }
                case ImageEnum.PKQ: { pb.Name = "Pkq" + this.Name; break;  }
                case ImageEnum.Identity: { pb.Name = "IdentityImage" + this.Name; break; }
                case ImageEnum.Accounttype: { pb.Name = "Accounttype" + this.Name; break; }
                case ImageEnum.Clock: { pb.Name = "Clock" + this.Name; break; }
                case ImageEnum.TrafficLight: { pb.Name = "TrafficLight" + this.Name; break; }
            }
            pb.Image = image;
            pb.AccessibleRole = AccessibleRole.None;
            pb.BackColor = Color.Transparent;
            pb.BackgroundImageLayout = ImageLayout.Zoom;
            pb.Location = pos;
            pb.Size = size;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.TabStop = false;
            return pb;
        }
        /// <summary>Creates the traffic light.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <returns>
        ///   a PictureBox control with the given image
        /// </returns>
        private PictureBox CreateTrafficLight(Point pos)
        {
            PictureBox trafficLight = new PictureBox();
            trafficLight.AccessibleRole = AccessibleRole.None;
            trafficLight.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            trafficLight.BackgroundImageLayout = ImageLayout.Zoom;
            trafficLight.Image = m_greyLight;
            m_ttPictureBox.SetToolTip(trafficLight, "Security evaluation has not been started");
            trafficLight.Location = pos;
            trafficLight.MaximumSize = new Size(27, 27);
            trafficLight.Name = "TrafficLight" + this.Name;
            trafficLight.Size = new Size(27, 27);
            trafficLight.SizeMode = PictureBoxSizeMode.Zoom;
            trafficLight.TabIndex = 3;
            trafficLight.TabStop = false;
            trafficLight.Click += TrafficLight_Clicked;


            return trafficLight;
        }

        /// <summary>Creates the account type icon.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///    PictureBox control with the given image
        /// </returns>
        private PictureBox CreateAccountTypeIcon(Point pos, Image image)
        {

            PictureBox AccountTypeIcon = new PictureBox();
            AccountTypeIcon.AccessibleRole = AccessibleRole.None;
            AccountTypeIcon.BackColor = Color.White;
            AccountTypeIcon.BackgroundImageLayout = ImageLayout.Zoom;
            AccountTypeIcon.Location = pos;
            AccountTypeIcon.Name = "Accounttype" + this.Name;
            AccountTypeIcon.Size = new Size(35, 35);
            AccountTypeIcon.SizeMode = PictureBoxSizeMode.Zoom;
            AccountTypeIcon.TabIndex = 4;
            AccountTypeIcon.TabStop = false;

            AccountTypeIcon.Image = image;

            return AccountTypeIcon;
        }

        /// <summary>Creates the clock.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///    a PictureBox control with the given image
        /// </returns>
        private PictureBox CreateClock(Point pos, Bitmap image)
        {
            PictureBox clock = new PictureBox();
            clock.AccessibleRole = AccessibleRole.None;
            clock.BackColor = Color.White;
            clock.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            clock.BackgroundImageLayout = ImageLayout.Zoom;
            clock.Image = image;
            clock.Location = pos;
            clock.MaximumSize = new Size(33, 33);
            clock.Name = "Clock" + this.Name;
            clock.Size = new Size(34, 34);
            clock.SizeMode = PictureBoxSizeMode.Zoom;
            clock.TabIndex = 3;
            clock.TabStop = false;



            return clock;
        }



        /// <summary>Creates the lock image.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///    a PictureBox control with the given image
        /// </returns>
        private PictureBox CreateLockImage(Point pos, Image image)
        {
            PictureBox Lock = new PictureBox();
            Lock.AccessibleRole = AccessibleRole.None;
            Lock.Image = image;
            Lock.BackColor = Color.Transparent;
            Lock.BackgroundImageLayout = ImageLayout.Zoom;
            Lock.Location = pos;
            Lock.Name = "Lock" + this.Name;
            Lock.Size = new Size(20, 24);
            Lock.SizeMode = PictureBoxSizeMode.Zoom;
            Lock.TabIndex = 6;
            Lock.TabStop = false;
  
            return Lock;
        }

        /// <summary>Creates the phone image.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///   a PictureBox control with the given image
        /// </returns>
        private PictureBox CreatePhoneImage(Point pos, Image image)
        {
            PictureBox Phone = new PictureBox();
            Phone.AccessibleRole = AccessibleRole.None;
            Phone.Image = image;
            Phone.BackColor = Color.Transparent;
            Phone.BackgroundImageLayout = ImageLayout.Zoom;
            Phone.Location = pos;
            Phone.Name = "Phone" + this.Name;
            Phone.Size = new Size(30, 30);
            Phone.SizeMode = PictureBoxSizeMode.Zoom;
            Phone.TabIndex = 7;
            Phone.TabStop = false;

            return Phone;
        }


        /// <summary>Creates the backup image.</summary>
        /// <param name="pos">The positionof the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///   a PictureBox control with the given image
        /// </returns>
        private PictureBox CreateBackupImage(Point pos, Image image)
        {
            PictureBox backup = new PictureBox();
            backup.AccessibleRole = AccessibleRole.None;
            backup.Image = image;
            backup.BackColor = Color.Transparent;
            backup.BackgroundImageLayout = ImageLayout.Zoom;
            backup.Location = pos;
            backup.Name = "Backup" + this.Name;
            backup.Size = new Size(30, 30);
            backup.SizeMode = PictureBoxSizeMode.Zoom;
            backup.TabIndex = 8;
            backup.TabStop = false;
    
            return backup;
        }


        /// <summary>Creates the mail image.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///   a PictureBox control with the given image
        /// </returns>
        private PictureBox CreateMailImage(Point pos, Image image)
        {
            PictureBox Mail = new PictureBox();
            Mail.AccessibleRole = AccessibleRole.None;
            Mail.Image = image;
            Mail.BackColor = Color.Transparent;
            Mail.BackgroundImageLayout = ImageLayout.Zoom;
            Mail.Location = pos;
            Mail.Name = "Mail" + this.Name;
            Mail.Size = new Size(30, 30);
            Mail.SizeMode = PictureBoxSizeMode.Zoom;
            Mail.TabIndex = 7;
            Mail.TabStop = false;
      
            return Mail;
        }


        /// <summary>Creates the PKQ image.</summary>
        /// <param name="pos">The position of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        ///  a PictureBox control with the given image
        /// </returns>
        private PictureBox CreatePkqImage(Point pos, Image image)
        {
            PictureBox pkq = new PictureBox();
            pkq.AccessibleRole = AccessibleRole.None;
            pkq.Image = image;
            pkq.BackColor = Color.Transparent;
            pkq.BackgroundImageLayout = ImageLayout.Zoom;
            pkq.Location = pos;
            pkq.Name = "Pkq" + this.Name;
            pkq.Size = new Size(30, 30);
            pkq.SizeMode = PictureBoxSizeMode.Zoom;
            pkq.TabIndex = 9;
            pkq.TabStop = false;
      

            return pkq;
        }



        /// <summary>Changes the color of the traffic light and its tooltip depending on the evaluation result.</summary>
        /// <param name="color">The new color.</param>
        /// <param name="insufficientEvalInfo">if set to <c>true</c> then no evaluation has been undertaken due to a lack of information.</param>
        public void ChangeTrafficLightColor( TrafficLightColorsEnum color, bool insufficientEvalInfo )
        {
            IterateThroughControls("TrafficLight" + this.Name, this);
        
            PictureBox pb = (PictureBox)m_selContr;
            switch (color)
            {
                case (TrafficLightColorsEnum.Red):
                    {
                        m_evalFormenabled = true;
                        m_ttPictureBox.SetToolTip(pb, "This account is not sufficiently protected through its' fallback authentication methods. \n Click here for a detailed Report");

                        pb.Image = m_redLight; 

                       
                        break; }
                case (TrafficLightColorsEnum.Yellow):
                    {
                        m_evalFormenabled = true;
                        m_ttPictureBox.SetToolTip(pb, "This account may be susceptible to attacks. Click here for a detailed Report");
                        pb.Image = m_yellowLight;
                        
                        break; }
                case (TrafficLightColorsEnum.Green):
                    {
                        m_evalFormenabled = true;
                        m_ttPictureBox.SetToolTip(pb, "No security risks were found. Click here for a detailed Report");
                        
                        pb.Image = m_greenLight;


                        break;
                    }
                case (TrafficLightColorsEnum.Gray):
                    {
                        if (insufficientEvalInfo) { m_ttPictureBox.SetToolTip(pb, "Insufficient amount of data provided. Evaluation counld not be initiated. \nIs an Account-Type and at least one Fallback-Method provided?"); }
                        else { m_ttPictureBox.SetToolTip(pb, "Security evaluation has not been started"); }
                        pb.Image = m_greyLight;
                        m_evalFormenabled = false;

                        break;
                    }
            }
            m_selContr = null;
        }





        /// <summary>Updates the clock images  by hiding or showing a green or red clock depending on the account being expired or not and assigning it a tooltip.</summary>
        private void UpdateClockImages()
        {

            IterateThroughControls("Clock" + this.Name, this);
            bool isExpired = m_Entry.ExpiryTime < DateTime.Now;
            if (m_selContr == null)
            {
                
                if (m_Entry.Expires)

                {
              
                    if (isExpired) { PictureBox clock = CreateClock(GetImagePosition(ImageEnum.Clock), m_clockRed); this.Controls.Add(clock); }
                    else { PictureBox clock = CreateClock(GetImagePosition(ImageEnum.Clock), m_clockGreen); this.Controls.Add(clock); }

                    IterateThroughControls("Clock" + this.Name, this);
                   
                    string expireDays = Convert.ToInt16(( m_Entry.ExpiryTime - DateTime.Now).TotalDays).ToString();
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    if (!isExpired) { tt.SetToolTip(m_selContr, $"This Account is set to expire in {expireDays} days"); }
                    else { tt.SetToolTip(m_selContr, $"This Account has expired."); }
                       m_selContr.Show();
                }
            }

            else
            {

                if (m_Entry.Expires)

                {
                    //not
                    this.Controls.Remove(m_selContr);
                    if (isExpired) { PictureBox clock = CreateClock(GetImagePosition(ImageEnum.Clock), m_clockRed); this.Controls.Add(clock); }
                    else { PictureBox clock = CreateClock(GetImagePosition(ImageEnum.Clock), m_clockGreen); this.Controls.Add(clock); }

                    IterateThroughControls("Clock" + this.Name, this);

                    string expireDays = Convert.ToInt16((m_Entry.ExpiryTime - DateTime.Now).TotalDays).ToString();
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    if (!isExpired) { tt.SetToolTip(m_selContr, $"This Account is set to expire in {expireDays} days"); }
                    else { tt.SetToolTip(m_selContr, $"This Account has expired."); }
                    m_selContr.Show();
                } else 
                {
                    m_selContr.Hide();
                    
                }
               
            }
            m_selContr = null;
        }



        /// <summary>Updates the lock image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdateLockImage()
        {
            m_selContr = null;
            IterateThroughControls("Lock" + this.Name, this);
            List<ImageEnum> list = FindUsedFallbackMethods();
            if (m_selContr == null)
            {
                if (list.Count != 0)
                {
                    Point p = new Point((this.Width + 17) / 2, (this.Height) / 2);
                    PictureBox newLock = CreateLockImage(p, m_lock);
                   
                    this.Controls.Add(newLock);
                    IterateThroughControls("Lock" + this.Name, this);
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    tt.SetToolTip(m_selContr, "Fallback-Authentication methods are configured for this Account");
                    m_selContr.Show();

                }

            }
            else if (list.Count == 0)
            {
                m_selContr.Hide();
            }
            else { m_selContr.Show(); }
            m_selContr = null;


  
        }



        /// <summary>Updates the mail image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdateMailImage()
        {
            m_selContr = null;
            IterateThroughControls("Mail" + this.Name, this);
            List<ImageEnum> list = FindUsedFallbackMethods();
            if (m_selContr == null)
            {
                if (list.Contains(ImageEnum.Mail))
                {
                    PictureBox mailImage = CreateMailImage(GetImagePosition(ImageEnum.Mail), m_mail);
                    this.Controls.Add(mailImage);
                    IterateThroughControls("Mail" + this.Name, this);

                  //  this.Refresh(); // removed for loop testing
                    string s = Entry.Strings.ReadSafe("RecoveryMail:Address");
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    tt.SetToolTip(m_selContr, $"The Recovery-Mail for this account is {s}");
                    m_selContr.Show();
                }

            }
            else
                if (!list.Contains(ImageEnum.Mail))
            {
                m_selContr.Hide();
            }
            else { m_selContr.Show(); }

            m_selContr = null;
        }










        /// <summary>Updates the phone image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdatePhoneImage()
        {
            m_selContr = null;
            IterateThroughControls("Phone" + this.Name, this);
            List<ImageEnum> list = FindUsedFallbackMethods();
            if (m_selContr == null)
            {
                if (list.Contains(ImageEnum.Phone))
                {
                    PictureBox phone = CreatePhoneImage(GetImagePosition(ImageEnum.Phone), m_phone);
                    this.Controls.Add(phone);
                    IterateThroughControls("Phone" + this.Name, this);
                    string s = Entry.Strings.ReadSafe("RecoveryPhone:Number");
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    if (s == "") { tt.SetToolTip(m_selContr, $"Your phone is used for Account-Recovery"); }
                    else { tt.SetToolTip(m_selContr, $"The Recovery-Phonenumber for this account is {s}"); }
                    m_selContr.Show();
                }

            }
            else
            {
                if (!list.Contains(ImageEnum.Phone))
                {
                    m_selContr.Hide();
                }
                else { m_selContr.Show(); }








            }
            m_selContr = null;

        }

        /// <summary>Updates the backup image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdateBackupImage()
        {
            m_selContr = null;
            IterateThroughControls("Backup" + this.Name, this);
            List<ImageEnum> list = FindUsedFallbackMethods();
            if (m_selContr == null)
            {
                if (list.Contains(ImageEnum.Backup))
                {
                    PictureBox backup = CreateBackupImage(GetImagePosition(ImageEnum.Backup), m_backup);
                    this.Controls.Add(backup);
                    IterateThroughControls("Backup" + this.Name, this);
                    bool s1 = Entry.Strings.Get("BackupCode:Location").IsEmpty;
                    
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    if (s1) { tt.SetToolTip(m_selContr, $"You configured a location for an Backup-Code of the account"); }
                    else { tt.SetToolTip(m_selContr, "You configured a way to access a Backup-Code for this account"); }
                    m_selContr.Show();
                }

            }
            else
            {
                if (!list.Contains(ImageEnum.Backup))
                {
                    m_selContr.Hide();
                }
                else { m_selContr.Show(); }








            }
            m_selContr = null;
        }

        /// <summary>Updates the PKQ image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdatePkqImage()
        {
            m_selContr = null;
            IterateThroughControls("Pkq" + this.Name, this);
            List<ImageEnum> list = FindUsedFallbackMethods();
            if (m_selContr == null)
            {
                if (list.Contains(ImageEnum.PKQ))
                {
                    PictureBox pkq = CreatePkqImage(GetImagePosition(ImageEnum.PKQ), m_pkq);
                    this.Controls.Add(pkq);
                    IterateThroughControls("Pkq" + this.Name, this);

                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                     tt.SetToolTip(m_selContr, "You configured at least one security question-answer pair for this account"); 

                    m_selContr.Show();
                }

            }
            else
                            if (!list.Contains(ImageEnum.PKQ))
            {
                m_selContr.Hide();
            }
            else { m_selContr.Show(); }






            m_selContr = null;
        }


        /// <summary>Updates the identity image by hiding or showing it and assigning it a tooltip.</summary>
        private void UpdateIdentityImage()
        {
            m_selContr = null;
            IterateThroughControls("IdentityImage" + this.Name, this);
            if (m_selContr == null)
            {
                if (m_Entry.Strings.ReadSafe("Identity:Name") != string.Empty)
                {
                    PictureBox identity = CreatePictureBox(GetImagePosition(ImageEnum.Identity), new Size(35, 30), m_identity, ImageEnum.Identity);
                    this.Controls.Add(identity);
                    IterateThroughControls("IdentityImage" + this.Name, this);
                    ToolTip tt = new ToolTip();
                    tt.AutoPopDelay = 5000;
                    tt.InitialDelay = 1000;
                    tt.ReshowDelay = 500;
                    tt.SetToolTip(m_selContr, "You connected this account with an identity");
                    m_selContr.Show();
                    m_selContr = null;
                }

            }
            else
            {
                if (m_Entry.Strings.ReadSafe("Identity:Name") == string.Empty)
                {
                    m_selContr.Hide();
                }
                else
                {
                    m_selContr.Show();
                }
                m_selContr = null;

            }
        }

        /// <summary>Updates the account type images presented to the user on the dashboard. </summary>
        private void UpdateAccountTypeImages()
        {
            m_selContr = null;
            IterateThroughControls("Accounttype" + this.Name, this);
            Image image = null;
            if (m_selContr == null) //no account type image has been shown yet
            {

                {
                    switch (m_Entry.Strings.ReadSafe("Account:Type")) //find the right image
                    {
                        case "": { m_accountType = AccountTypeEnum.NoType; break; }
                        case "Banking-Account": { image = m_bankingAccount; m_accountType = AccountTypeEnum.Banking; break; }
                        case "Social-Media-Account": { image = m_socialMediaAccount; m_accountType = AccountTypeEnum.SocialMedia; break; }
                        case "Shopping-Account": { image = m_shoppingAccount; m_accountType = AccountTypeEnum.Shopping; break; }
                        case "Mail-Account": { image = m_mailAccount; m_accountType = AccountTypeEnum.Mail; break; }
                        default: { image = m_customAccount; m_accountType = AccountTypeEnum.Custom; break; }
                    }
                    if (m_accountType != AccountTypeEnum.NoType) //and show it
                    {
                        PictureBox type = CreateAccountTypeIcon(GetImagePosition(ImageEnum.Accounttype), image);
                        this.Controls.Add(type);
                        IterateThroughControls("Accounttype" + this.Name, this);
                        ToolTip tt = new ToolTip();
                        string toolTipText = m_Entry.Strings.ReadSafe("Account:Type");
                        tt.AutoPopDelay = 5000;
                        tt.InitialDelay = 1000;
                        tt.ReshowDelay = 500;
                        tt.SetToolTip(m_selContr, $"This is a {toolTipText}");
                        m_selContr.Show();
                        m_selContr = null;
                    }

                }


            }
            else //a account type image is already present, checks if it needs updating and does so by removing the only one and calling this funktion again to create a new one
            {
                switch (m_Entry.Strings.ReadSafe("Account:Type"))
                {
                    case "": { m_selContr.Hide(); m_accountType = AccountTypeEnum.NoType; break; }
                    case "Banking-Account":
                        {
                            if (m_accountType != AccountTypeEnum.Banking)
                            {
                                m_selContr.Hide();
                                this.Controls.Remove(m_selContr);
                                UpdateAccountTypeImages();
                            }
                            break;
                        }
                    case "Mail-Account":
                        {
                            if (m_accountType != AccountTypeEnum.Mail)
                            {
                                m_selContr.Hide();
                                this.Controls.Remove(m_selContr);
                                UpdateAccountTypeImages();
                            }
                            break;
                        }
                    case "Shopping-Account":
                        {
                            if (m_accountType != AccountTypeEnum.Shopping)
                            {
                                m_selContr.Hide();
                                this.Controls.Remove(m_selContr);
                                UpdateAccountTypeImages();
                            }
                            break;
                        }
                    case "Social-Media-Account":
                        {
                            if (m_accountType != AccountTypeEnum.SocialMedia)
                            {
                                m_selContr.Hide();
                                this.Controls.Remove(m_selContr);
                                UpdateAccountTypeImages();
                            }
                            break;
                        }
                    default:
                        if (m_accountType != AccountTypeEnum.Custom)
                        {
                            m_accountType = AccountTypeEnum.Custom;
                            m_selContr.Hide();
                            this.Controls.Remove(m_selContr);
                            UpdateAccountTypeImages();
                        }
                        break;
                }
            }
        }




        /// <summary>Gets the upper right corner of the FA symbols to draw the arrows.</summary>
        /// <param name="p">The position of the symbol.</param>
        /// <param name="image">The type of symbol.</param>
        private Point GetUpperRightCorner(Point p, ImageEnum image)
        {
            if (image == ImageEnum.Lock) { return new Point(p.X + 17, p.Y + 20); }
            return new Point(p.X + 25, p.Y + 2 );
        }

        /// <summary>Gets the lower right corner  of the FA symbols to draw the arrows.</summary>
        /// <param name="p">The position of the symbol.</param>
        /// <param name="image">The type of symbol.</param>
        private Point GetLowerRightCorner(Point p, ImageEnum image)
        {
            if (image == ImageEnum.Lock) { return new Point(p.X + 19, p.Y + 36); }
            return new Point(p.X + 26, p.Y + 21);
        }

        /// <summary>Gets the lower left corner of the FA symbols to draw the arrows.</summary>
        /// <param name="p">The position of the symbol.</param>
        /// <param name="image">The type of symbol.</param>
        private Point GetLowerLeftCorner(Point p , ImageEnum image)
        {
            if (image == ImageEnum.Lock) { return new Point(p.X , p.Y + 36); }
            return new Point(p.X + 4 , p.Y + 15);
        }







        /// <summary>Finds the used fallback methods of the entry represented by the node in order to visualize them.</summary>
        /// <returns>
        ///  List of FA imagetypes that need to be visualized
        /// </returns>
        private List<ImageEnum> FindUsedFallbackMethods()
        {
            List<ImageEnum> list = new List<ImageEnum>();
            bool pkqAdded = false;
            if(m_Entry.Strings.ReadSafe("PKQ:FirstQuestion") != String.Empty)
            {
                if (m_Entry.Strings.ReadSafe("PKQ:FirstAnswer") != String.Empty)
                {
                    list.Add(ImageEnum.PKQ);
                    pkqAdded = true;
                }

            }
            if ( m_Entry.Strings.ReadSafe("PKQ:SecondQuestion") != String.Empty)
            {
                if (m_Entry.Strings.ReadSafe("PKQ:SecondAnswer") != String.Empty)
                {
                    if(!pkqAdded) {
                    list.Add(ImageEnum.PKQ);
                    pkqAdded = true;
                    }
                }
            }
            if (m_Entry.Strings.ReadSafe("PKQ:ThirdQuestion") != String.Empty)
            {
                if (m_Entry.Strings.ReadSafe("PKQ:ThirdAnswer") != String.Empty)
                {
                    if (!pkqAdded)
                    {
                        list.Add(ImageEnum.PKQ);
                        pkqAdded = true;
                    }
                }

            }
            if (m_Entry.Strings.ReadSafe("RecoveryMail:Address") != String.Empty)
            {
                list.Add(ImageEnum.Mail);

            }
            if (m_Entry.Strings.ReadSafe("BackupCode:Location") != String.Empty)
            {
                list.Add(ImageEnum.Backup);
            }
            else if (m_Entry.Strings.ReadSafe("BackupCode:AccessInfo") != String.Empty)
            {
                list.Add(ImageEnum.Backup);

            }
            if (m_Entry.Strings.ReadSafe("RecoveryPhone:Number") != String.Empty)
            {
                list.Add(ImageEnum.Phone);
            }
            else if (m_Entry.Strings.ReadSafe("RecoveryPhone:UnlockPhone") != String.Empty)
            {
                list.Add(ImageEnum.Phone);
            }

            return list;
            
        }





        /// <summary>Gets the position of each image inside each node.</summary>
        /// <param name="image">The image to be positioned.</param>
        private Point GetImagePosition(ImageEnum image)
        {
            Point p = new Point();
            switch (image)
            {
                case ImageEnum.TrafficLight:
                    {
                        p.X = this.Width - 28;
                        p.Y = 1;
                        break;

                    }
                case ImageEnum.Accounttype:
                    {
                        if (m_accountType == AccountTypeEnum.Shopping) { p.X = 1; }
                        else { p.X = 3; }
                        p.Y = this.Height - 80;
                        break;

                    }
                case ImageEnum.Clock:
                    {
                        p.X = 4;
                        p.Y = this.Height - 37;
                        break;

                    }
                case ImageEnum.Identity:
                    {
                        p.X = 3;
                        p.Y = this.Height - 120;
                        break;

                    }
                case ImageEnum.Lock:
                    {
                        p.X = (this.Width + 17) / 2;
                        p.Y = (this.Height - m_locOfFaBox.Y) / 2 ; 
                        break;
                        

                    }
                case ImageEnum.Mail:
                    {
                        p.X = this.Width - (this.Width + - 40 );
                        p.Y = this.Height - 120;
                        break;

                    }
                case ImageEnum.Phone:
                    {
                        p.X = this.Width - 27;
                        p.Y = this.Height - 118;
                        break;

                    }
                case ImageEnum.Backup:
                    {
                        p.X = this.Width - 30;
                        p.Y = this.Height - 35;
                        break;

                    }
                case ImageEnum.PKQ:
                    {
                        p.X = this.Width - (this.Width - 42);
                        p.Y = this.Height - 26;
                        break;

                    }
            }
            return p;
        }


        /// <summary>Iterates the through controls by providing a imagetype instead a certian name.</summary>
        /// <param name="image">The image to look for.</param>
        private void IterateThroughControls(ImageEnum image)
        {
            switch (image)
            {
                case ImageEnum.Mail:
                    {
                        IterateThroughControls("Mail" + this.Name, this);
                        break;
                    }
                case ImageEnum.Phone:
                    {
                        IterateThroughControls("Phone" + this.Name, this);
                        break;
                    }
                case ImageEnum.Backup:
                    {
                        IterateThroughControls("Backup" + this.Name, this);
                        break;
                    }
                case ImageEnum.PKQ:
                    {
                        IterateThroughControls("PKQ" + this.Name, this);
                        break;
                    }

            }
        }
        /// <summary>Iterates the through controls recusivly to look for a control with a specific name.</summary>
        /// <param name="name">The name of the searches control.</param>
        /// <param name="control">The control from which the search is started.</param>
        private void IterateThroughControls(string name, Control control)
        {
            foreach(Control c in control.Controls)
            {
                if (c.Name == name) m_selContr = c;
                IterateThroughControls(name, c);
            }
     
        }




        /// <summary>Handles the Clicked event of the TrafficLight picturebox by showing the EvalResultForm of the node.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void TrafficLight_Clicked(Object sender, EventArgs e)
        {
            if (!this.FindForm().OwnedForms.Contains(m_evalResultForm)) { this.FindForm().AddOwnedForm(m_evalResultForm); }
            if (m_evalFormenabled) { m_evalResultForm.Show(); m_evalResultForm.BringToFront(); }
        
       
        }




        /// <summary>Gets the x coordinate.</summary>
        /// <param name="a">a.</param>
        public int GetX(Point a)
        {
            return a.X;
        }

        /// <summary>Gets the y coordinate.</summary>
        /// <param name="a">a.</param>
        public int GetY(Point a)
        {
            return a.Y;
        }

        /// <summary>Gets the center point of a NodeControl.</summary>
        /// <param name="ctl">The NodeControl.</param>
        public Point GetCenterCtl(NodeControl ctl)
        {
            Point l = ctl.Location;
            Point b = new Point(l.X + ctl.Width, l.Y);
            Point c = new Point(l.X + ctl.Width, l.Y + ctl.Height);
            Point d = new Point(l.X, l.Y + ctl.Height);
            return GetCenter(l, b, c, d);
        }


        /// <summary>Gets the center point of a line between two points.</summary>
        /// <param name="a">starting point of a line</param>
        /// <param name="b">ending point of the line</param>
        public Point GetCenter(Point a, Point b)
        {
            int middleX = a.X + b.X;
            int middleY = a.Y + b.Y;

            return new Point((middleX / 2), (middleY / 2));
        }

        /// <summary>Gets an approximation of a center point of a rectangle by using its corners. Also found in MAD_Form => bad practice</summary>
        /// <param name="a">upper left corner</param>
        /// <param name="b">upper right corner</param>
        /// <param name="c">lower left corner</param>
        /// <param name="d">lower right corner</param>
        public Point GetCenter(Point a, Point b, Point c, Point d)
        {
            Point m_ac = GetCenter(a, c);
            Point m_bd = GetCenter(b, d);
            return GetCenter(m_ac, m_bd);
        }



        /*
        ***************************************************************************************
        * Title: How to create a User Control with rounded corners?
        * Author:Reza Aghaei
        * Date: 15.10.15
        * Availability: https://stackoverflow.com/questions/32987649/how-to-create-a-user-control-with-rounded-corners
        *
        ***************************************************************************************
        */
        /// <summary>Gets the path of a round rectagle that is the border of the node. Used to draw the line that represents the border of the node</summary>
        /// <param name="b">The rectangle representing the standart border of the group box without rounded edges </param>
        /// <param name="r">The radius of the rounded edges</param>
        /// <returns>
        ///  A path which represents a rectangle with rounded edges
        /// </returns>
        private GraphicsPath GetRoundRectagle(Rectangle b, int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(b.X, b.Y, r, r, 180, 90);
            path.AddArc(b.X + b.Width - r - 1, b.Y, r, r, 270, 90);
            path.AddArc(b.X + b.Width - r - 1, b.Y + b.Height - r - 1, r, r, 0, 90);
            path.AddArc(b.X, b.Y + b.Height - r - 1, r, r, 90, 90);
            path.CloseAllFigures();
            return path;
        }






        /// <summary>Löst das <see cref="E:System.Windows.Forms.Control.Paint">Paint</see>-Ereignis aus.</summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see>, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); //call the overridden method to paint the groupbox to be visually modified
            GroupBoxRenderer.DrawParentBackground(e.Graphics, this.ClientRectangle, this); //draws this box on top
            var rect = ClientRectangle;
            using (var path = GetRoundRectagle(this.ClientRectangle, 25)) //using statement automatically disposes of all objects created within when its done
            {
                //lookes smoother
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
           
                int upperBoxHeight = new Font("Microsoft Sans Serif", Font.Size, FontStyle.Regular).Height + Padding.Bottom + Padding.Top;
                m_locOfFaBox = new Point(40, upperBoxHeight);
                rect = new Rectangle(0, 0, rect.Width, upperBoxHeight); //sets the borders of the title field of the node
                if (this.BackColor != Color.Transparent)
                    using (var brush = new SolidBrush(Color.White))
                        e.Graphics.FillPath(brush, path);

                var clip = e.Graphics.ClipBounds;
                e.Graphics.SetClip(rect); //to cut of the top two sharp edges to be replaced with round ones
                e.Graphics.FillPath(new SolidBrush(BackColor), path);
                
                
                using (var pen = new Pen(Color.SteelBlue, 1))
                    e.Graphics.DrawPath(pen, path);

                rect.Location = new Point(rect.Location.X - 25, rect.Y); // now drawing the lower portion of the box
                TextRenderer.DrawText(e.Graphics, Text, new Font("Microsoft Sans Serif", Font.Size, FontStyle.Regular), rect, Color.Black);

                e.Graphics.SetClip(clip); //and set the clip for the lower two edges

                using (var pen = new Pen(Color.SteelBlue, 1))
                    e.Graphics.DrawPath(pen, path);



                using (var newPen = new Pen(Color.SteelBlue, 1) ) // draw the line between the two parts lower parts of the box
                {
                    e.Graphics.DrawLine(newPen, new Point(40, upperBoxHeight), new Point(40, ClientRectangle.Height));
                }
                

           
              
                List<ImageEnum> faUsed = FindUsedFallbackMethods();
                if (faUsed.Count != 0)
                {
                    using (var pen = new Pen(Color.Black, 2)) // now drawing the arrows pointing to the lock symbolizing the fa methods
                    {
                        
                        pen.CustomEndCap = new AdjustableArrowCap((float)3.0, (float)3.0);

                        Point startArrow = new Point();
                        Point endArrow = new Point();

                        foreach (var method in faUsed)
                        {
                            switch (method)
                            {
                                case ImageEnum.PKQ:
                                    {

                                        startArrow = GetUpperRightCorner(GetImagePosition(ImageEnum.PKQ), ImageEnum.PKQ);

                                        endArrow = GetLowerLeftCorner(GetImagePosition(ImageEnum.Lock), ImageEnum.Lock);

                                        e.Graphics.DrawLine(pen, startArrow, endArrow); break;
                                    }
                                case ImageEnum.Mail:
                                    {
                                        startArrow = GetLowerRightCorner(GetImagePosition(ImageEnum.Mail), ImageEnum.Mail);

                                        endArrow.X = GetImagePosition(ImageEnum.Lock).X + 2;
                                        endArrow.Y = GetImagePosition(ImageEnum.Lock).Y + 20;

                                        e.Graphics.DrawLine(pen, startArrow, endArrow); break;
                                    }
                                case ImageEnum.Phone:
                                    {
                                        startArrow = GetLowerLeftCorner(GetImagePosition(ImageEnum.Phone), ImageEnum.Phone);

                                        endArrow = GetUpperRightCorner(GetImagePosition(ImageEnum.Lock), ImageEnum.Lock);

                                        e.Graphics.DrawLine(pen, startArrow, endArrow); break;
                                    }
                                case ImageEnum.Backup:
                                    {
                                        startArrow = GetImagePosition(ImageEnum.Backup);
                                        startArrow.Y += 11;

                                        endArrow = GetLowerRightCorner(GetImagePosition(ImageEnum.Lock), ImageEnum.Lock);

                                        e.Graphics.DrawLine(pen, startArrow, endArrow); break;
                                    }
                            }
                        }
                     

                    }
                    m_isInitialDraw = false;
                }
                
            

            }

        }
    }
}
