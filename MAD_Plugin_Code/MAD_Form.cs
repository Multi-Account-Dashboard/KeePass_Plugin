using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{
    public partial class MAD_Form : Form, IObserver<NodeControl>
    {



        private bool UpdateLinesBool = false;
        private bool m_isMovingNodes = false;
        private bool mailTempChecked = false;
        private bool phoneTempChecked = false;
        private bool identityTempChecked = false;

        private MoveControlHelper moveControlHelper = null;
        private List<NodeControl> nodes = new List<NodeControl>();

        private List<GraphLine> m_lines = new List<GraphLine>();
        private List<(string, string)> m_conByIds = new List<(string, string)>();
        private List<(string, string)> m_conByMail = new List<(string, string)>();
        private List<(string, string)> m_conByPhone = new List<(string, string)>();
        private Dictionary<GraphLine, PictureBox> m_linePicturesIdentity = new Dictionary<GraphLine, PictureBox>();
        private Dictionary<GraphLine, PictureBox> m_linePicturesPhone = new Dictionary<GraphLine, PictureBox>();
        private Dictionary<GraphLine, PictureBox> m_linePicturesMail = new Dictionary<GraphLine, PictureBox>();
        private Dictionary<string, string> m_mailAccounts = new Dictionary<string, string>();

        private IDisposable StopObserving;
        private MAD_Controller controller;

        //public getter and setter for m_isMovingNodes
        public bool IsMovingNodes { get => m_isMovingNodes; set => m_isMovingNodes = value; }



        /// <summary>Initializes a new instance of the <see cref="MAD_Form" /> class.</summary>
        public MAD_Form()
        {
            this.DoubleBuffered = true;
            this.moveControlHelper = new MoveControlHelper(this);
            this.ContextMenuStrip = CreateContextMenuDashboard() ;
            InitializeComponent();
        }

        /// <summary>Creates the context menu for the dashboard. It has two items which can be used to add a new node or to evaluate all nodes</summary>
        /// <returns>
        ///   the context menu
        /// </returns>
        private ContextMenuStrip CreateContextMenuDashboard()
        {
            ContextMenuStrip menuStrip = new ContextMenuStrip();

            ToolStripMenuItem addEntry = new ToolStripMenuItem("Add Entry");
        
            addEntry.Click += new EventHandler(Add_Clicked);

            addEntry.Name = "EditNodeItem";

            menuStrip.Items.Add(addEntry);

            ToolStripMenuItem evaluate = new ToolStripMenuItem("Evaluate");

            evaluate.Click += new EventHandler(EvaluateAllToolStripMenuItem_Click);

            evaluate.Name = "EvaluateDashboard";

            menuStrip.Items.Add(evaluate);

            return menuStrip;
        }



        /// <summary>Handles the Clicked event of the Add item of the dashboards context menu.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Add_Clicked(object sender, EventArgs e)
        {
            controller.HandleAddEntryButtonClicked();
        }

        /// <summary>Creates the context menu for the account nodes. It has two items which can be used to remove or edit the node it was added to.</summary>
        /// <returns>
        ///   the context menu for a node
        /// </returns>
        private ContextMenuStrip CreateContextMenuNodes()

        {

            ContextMenuStrip menuStrip = new ContextMenuStrip();

            ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove");
            ToolStripMenuItem editItem = new ToolStripMenuItem("Edit");

            editItem.Click += new EventHandler(Edit_Click);

            editItem.Name = "EditNodeItem";

            menuStrip.Items.Add(editItem);

            removeItem.Click += new EventHandler(Remove_Click);

            removeItem.Name = "RemoveNodeItem";

            menuStrip.Items.Add(removeItem);

            return menuStrip;


        }

        /// <summary>Handles the Click event of the Edit item of a node's context menu.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Edit_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            ContextMenuStrip cm = (ContextMenuStrip)mi.GetCurrentParent();

            NodeControl nc = (NodeControl)cm.SourceControl;

            controller.HandleAccountNodeClicked(nc);
        }

        /// <summary>Handles the Click event of the Remove item of a node's context menu.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Remove_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            ContextMenuStrip cm = (ContextMenuStrip)mi.GetCurrentParent();

            NodeControl nc = (NodeControl)cm.SourceControl;
            
            
            controller.HandleRemoveNode(nc.VisualId);
        }







        /// <summary>Gets the line between two nodes.</summary>
        /// <param name="node1">The node at the start of the line.</param>
        /// <param name="node2">The node at the end of the line.</param>
        /// <param name="image">The type of connection which is represented by the line.</param>
        /// <returns>
        ///   A GraphLine instance between the two nodes
        /// </returns>
        private GraphLine GetLineBetweenNodes(NodeControl node1, NodeControl node2, ImageEnum image)
        {
            if (node1 != null && node2 != null)
            {
                return new GraphLine(GetCenterCtl(node1).X, GetCenterCtl(node1).Y, GetCenterCtl(node2).X, GetCenterCtl(node2).Y, image);
            }
            return null;
        }



        /// <summary>Gets the center point of a GroupBox.</summary>
        /// <param name="ctl">The GroupBox.</param>
        private Point GetCenterCtl(GroupBox ctl)
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
        private Point GetCenter(Point a, Point b)
        {
            int middleX = a.X + b.X;
            int middleY = a.Y + b.Y;

            return new Point((middleX / 2), (middleY / 2));
        }

        /// <summary>Gets an approximation of a center point of a rectangle by using its corners.</summary>
        /// <param name="a">upper left corner</param>
        /// <param name="b">upper right corner</param>
        /// <param name="c">lower left corner</param>
        /// <param name="d">lower right corner</param>
        private Point GetCenter(Point a, Point b, Point c, Point d)
        {
            Point m_ac = GetCenter(a, c);
            Point m_bd = GetCenter(b, d);
            return GetCenter(m_ac, m_bd);
        }


        /// <summary>Löst das <see cref="E:System.Windows.Forms.Control.Paint">Paint</see>-Ereignis aus.</summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see>, das die Ereignisdaten enthält.</param>
        protected override void OnPaint( PaintEventArgs e)
        {
            //for better quality of the drawn lines
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //draws the lines that were created when looking at the connections stored inside the nodes
            bool minOneChecked = ShowLinkedIdentitiesMenuItem.Checked || usesSamePhoneMenuItem.Checked || usesSameMailMenuItem.Checked;
            if (!m_isMovingNodes && minOneChecked && UpdateLinesBool) {
                Pen pen = new Pen(Color.Black, 3);
                try {
                  
                    foreach (var line in m_lines)
                    {
                        if (line.image == ImageEnum.Identity && ShowLinkedIdentitiesMenuItem.Checked  ) {
                     
                            e.Graphics.DrawLine(pen, line.StartPoint, line.EndPoint);
                            this.Controls.Add(m_linePicturesIdentity[line]);
                            m_linePicturesIdentity[line].Show();

                        }

                        if (line.image == ImageEnum.Phone && usesSamePhoneMenuItem.Checked  )
                        {
                            e.Graphics.DrawLine(pen, line.StartPoint, line.EndPoint);
                            this.Controls.Add(m_linePicturesPhone[line]);
                            m_linePicturesPhone[line].Show();
                        }
                        if (line.image == ImageEnum.Mail  && usesSameMailMenuItem.Checked )
                        {
                            e.Graphics.DrawLine(pen, line.StartPoint, line.EndPoint);
                            this.Controls.Add(m_linePicturesMail[line]);
                            m_linePicturesMail[line].Show();
                        }
                    }
                } finally
                {
                    //pens must be manually disposed in C#
                    pen.Dispose();
                    UpdateLinesBool = false;
                }
            }

        }





        /// <summary>Löst das <see cref="E:System.Windows.Forms.Form.FormClosing">FormClosing</see>-Ereignis aus.</summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.FormClosingEventArgs">FormClosingEventArgs</see>, das die Ereignisdaten enthält.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //unchecks the shown connections to not further impact performance
            this.ShowLinkedIdentitiesMenuItem.Checked = false;
            this.usesSamePhoneMenuItem.Checked = false;
            this.usesSameMailMenuItem.Checked = false;

            //tell the controller that the form is closing
            controller.HandleFormclosing();
            //and close the form
            base.OnFormClosing(e);
            
        }

        /// <summary>Handles the Clicked event of a Node.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Node_Clicked(object sender, EventArgs e)
        {
            controller.HandleAccountNodeClicked((NodeControl)sender);
        }


        /// <summary>Handles the Load event of the MAD_Form control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MAD_Form_Load(object sender, EventArgs e)
        {

            controller.HandleFormLoaded();
        }


        /// <summary>Sets the mad controller.</summary>
        /// <param name="controller">The controller.</param>
        public void SetMADController(MAD_Controller controller)
        {
            this.controller = controller;
        }



        /// <summary>Receives new data from the model.</summary>
        /// <param name="value"> the NodeControl instance that either needs to be deleted, added or updated in the dashboard.</param>
        public void OnNext(NodeControl value)
        {
            if (value.upForDelete) 
            {
                string type = value.Entry.Strings.ReadSafe("Account:Type");
                if (type.Contains("Mail") || type.Contains("mail")) // if its a mail node we need to delete it out of the collection of safed mail nodes 
                {
                    string userName = value.Entry.Strings.ReadSafe("UserName");
                    m_mailAccounts.Remove(userName);
                }
                nodes.Remove(value);
                value.Dispose();
                UpdateLines();
            }
            else if (!nodes.Contains(value))
            {
                string type = value.Entry.Strings.ReadSafe("Account:Type");
                if (type.Contains("Mail") || type.Contains("mail")) // if its a mail node we need to add it to the collection of safed mail nodes to make visualization easier
                {
                    string userName = value.Entry.Strings.ReadSafe("UserName");
                    if (!m_mailAccounts.Keys.Contains(userName)) { m_mailAccounts.Add(userName, value.VisualIdString); }
                }
                value.UpdateAllImages();
                moveControlHelper.Init(value);
                nodes.Add(value);

                //adds the double and right click events
                value.DoubleClick += Node_Clicked;
                value.ContextMenuStrip = CreateContextMenuNodes();

                this.Controls.Add(value);
                
            }
            else
            {
                //node jsut needs updating
                NodeControl nc = (NodeControl)this.Controls.Find(value.Name, true)[0];
                nc.UpdateAllImages();
               
            }
            UpdateLines();
        }

        /// <summary>Disposes all or just one given image list. They contain the symbols that are shown above the GraphLines.</summary>
        /// <param name="image">The type of image.</param>
        /// <param name="all">if set to <c>true</c> all lists are emptied.</param>
        public void DisposeIageList(ImageEnum image, bool all)
        {
            if(image == ImageEnum.Identity || all)
            {
                foreach (var kvp in m_linePicturesIdentity)
                {
                    kvp.Value.Dispose();
                }

            }
            if (image == ImageEnum.Phone || all)
            {
                foreach (var kvp in m_linePicturesPhone)
                {
                    kvp.Value.Dispose();
                }

            }
            if (image == ImageEnum.Mail || all)
            {
                foreach (var kvp in m_linePicturesMail)
                {
                    kvp.Value.Dispose();
                }

            }
        }

       
        /// <summary>Updates the lines to be drawn depending on which ones ought to be shown. This is determined by the checkboxes of the items of the view main menu strip item</summary>
        public void UpdateLines()
        {

            m_lines.Clear(); //old lines are out of date
            UpdateLinesBool = true;

            if (ShowLinkedIdentitiesMenuItem.Checked) {
                
                DisposeIageList(ImageEnum.Identity, false);
                m_linePicturesIdentity.Clear();

                m_conByIds = FindConnectedIdPairs();
            if (m_conByIds.Count != 0)
            {

                foreach (var tuple in m_conByIds)
                {
                    NodeControl nodeStart = FindNodeByVid(tuple.Item1);
                    NodeControl nodeEnd = FindNodeByVid(tuple.Item2);
                    GraphLine newLine = GetLineBetweenNodes(nodeStart, nodeEnd, ImageEnum.Identity);
                        if (newLine != null)
                        {
                            bool alreadyContained = false;
                            foreach (GraphLine line in m_lines)
                            {
                                if (line.EndPoint == newLine.EndPoint && line.image == ImageEnum.Identity) alreadyContained = true; //only draw lines when necessary
                            }
                            if (!alreadyContained)
                            {
                                m_lines.Add(newLine);
                                m_linePicturesIdentity.Add(newLine, newLine.PictureBox(ImageEnum.Identity));
                            }
                         
                        }

                }
               
            }
            }
            if(usesSamePhoneMenuItem.Checked)
            {
                
                DisposeIageList(ImageEnum.Phone, false);
                m_linePicturesPhone.Clear();
                m_conByPhone = FindConnectedPhonePairs();
            
                if (m_conByPhone.Count != 0)
                {

                    foreach (var tuple in m_conByPhone)
                    {
                        NodeControl nodeStart = FindNodeByVid(tuple.Item1);
                        NodeControl nodeEnd = FindNodeByVid(tuple.Item2);
                        GraphLine newLine = GetLineBetweenNodes(nodeStart, nodeEnd, ImageEnum.Phone);
                        if (newLine != null)
                        {
                            bool alreadyContained = false;
                            foreach (GraphLine line in m_lines)
                            {
                                if (line.EndPoint == newLine.EndPoint && line.image == ImageEnum.Phone) alreadyContained = true;
                            }
                            if (!alreadyContained)
                            {
                                
                                m_lines.Add(newLine);
                                m_linePicturesPhone.Add(newLine, newLine.PictureBox(ImageEnum.Phone));
                            }
                        }

                    }

                }

            }

            if (usesSameMailMenuItem.Checked)
            {

                DisposeIageList(ImageEnum.Mail, false);
                m_linePicturesMail.Clear();
                m_conByMail = FindConnectedMailPairs();
            
                if (m_conByMail.Count != 0)
                {

                    foreach (var tuple in m_conByMail)
                    {
                        NodeControl nodeStart = FindNodeByVid(tuple.Item1);
                        NodeControl nodeEnd = FindNodeByVid(tuple.Item2);
                        GraphLine newLine = GetLineBetweenNodes(nodeStart, nodeEnd, ImageEnum.Mail);
                        if (newLine != null)
                        {
                            bool alreadyContained = false;
                            foreach (GraphLine line in m_lines)
                            {
                                if (line.image == ImageEnum.Mail && line.EndPoint == newLine.EndPoint) alreadyContained = true;
                            }
                            if (!alreadyContained)
                            {

                                m_lines.Add(newLine);
                                m_linePicturesMail.Add(newLine, newLine.PictureBox(ImageEnum.Mail));
                            }
                           
                        }

                    }

                }

            }
        
            // those two cause the whole dashboard to be redrawn hence the new lines are displayed
            Invalidate(false);
            Update();
        }

        /// <summary>temorarily unchecks all view main menu items.</summary>
        public void TempUncheckAllLinks()
        {
            if(usesSameMailMenuItem.Checked) { mailTempChecked = true; usesSameMailMenuItem.Checked = false; } 
            if(usesSamePhoneMenuItem.Checked) { phoneTempChecked = true; usesSamePhoneMenuItem.Checked = false; }
            if(ShowLinkedIdentitiesMenuItem.Checked) { identityTempChecked = true; ShowLinkedIdentitiesMenuItem.Checked = false; } 
        }

        /// <summary>Rechecks the temporary unchecked view main menu items .</summary>
        public void RecheckTempUnchecked()
        {
            if (mailTempChecked) { mailTempChecked = false; usesSameMailMenuItem.Checked = true; }
            if (phoneTempChecked) { phoneTempChecked = false; usesSamePhoneMenuItem.Checked = true; }
            if (identityTempChecked) { identityTempChecked = false; ShowLinkedIdentitiesMenuItem.Checked = true; }
        }



        /// <summary>Finds the connected mail pairs.</summary>
        /// <returns>
        ///  A list which contains tuples of visualIds of nodes that share a recovery email
        /// </returns>
        private List<(string, string)> FindConnectedMailPairs()
        {

            List<(string, string)> rtn = new List<(string, string)>();

            foreach (NodeControl node in nodes)
            {
                List<string> conMail = node.ConMail;
                if (RecoveryMailOfVIdIsInNodes(node.VisualIdString)) { rtn.Add((m_mailAccounts[node.Entry.Strings.ReadSafe("RecoveryMail:Address")], node.VisualIdString)); }
                else
                {
                    foreach (var id in conMail)
                    {
                        if (!rtn.Contains((node.VisualIdString, id)) && !rtn.Contains((id, node.VisualIdString)) && node.VisualIdString != "" && id != "" && node.Entry.Strings.ReadSafe("RecoveryMail:Address") != "")
                        {
                            rtn.Add((node.VisualIdString, id));
                        }
                    }
                }

            }

            return rtn;
        }

 
        /// <summary>Checks if a recovery email account of a ginven node is already included in the dashbaord.</summary>
        /// <param name="vIdString">The identifier string of the node to be checked.</param>
        private bool RecoveryMailOfVIdIsInNodes(string vIdString)
        {
            NodeControl nc = FindNodeByVid(vIdString);
            if (m_mailAccounts.Keys.Contains(nc.Entry.Strings.ReadSafe("RecoveryMail:Address"))) { return true; }
            return false;
 
        }

        /// <summary>Finds the connected phone pairs.</summary>
        /// <returns>
        ///   A list which contains tuples of visualIds of nodes that share a recovery phone number
        /// </returns>
        private List<(string, string)> FindConnectedPhonePairs()
        {
            List<(string, string)> rtn = new List<(string, string)>();

            foreach (NodeControl node in nodes)
            {
                List<string> conPhones = node.conPhone;
                foreach (var id in conPhones)
                {
                    if (!rtn.Contains((node.VisualIdString, id)) && !rtn.Contains((id, node.VisualIdString)) && node.VisualIdString != "" && id != "")
                    {
                        rtn.Add((node.VisualIdString, id));
                    }
                }

            }

            return rtn;

        }




        /// <summary>Finds a node by its visualId.</summary>
        /// <param name="vid">The to search with </param>
        /// <returns>
        ///   the node with the searched visualId or null if not found
        /// </returns>
        private NodeControl FindNodeByVid(string vid)
        {
            foreach (var node in nodes)
            {
                if (node.VisualIdString == vid) return node;
            }
            return null;
        }


        /// <summary>Finds the connected identity pairs.</summary>
        /// <returns>
        ///   A list which contains tuples of visualIds of nodes that share a identity
        /// </returns>
        private List<(string, string)> FindConnectedIdPairs()
        {
            List<(string, string)> rtn = new List<(string, string)>();
             
        foreach (NodeControl node in nodes)
        {
                List<string> conIds = node.ConId;
                foreach(var id in conIds)
                {
                    if(!rtn.Contains((node.VisualIdString, id)) && !rtn.Contains((id, node.VisualIdString)) && node.VisualIdString != "" && id != string.Empty)
                    {
                        rtn.Add((node.VisualIdString, id));
                    }
                }

        }
            
            return rtn;
            
        }

        /// <summary>Benachrichtigt den Beobachter, dass beim Anbieter eine Fehlerbedingung aufgetreten ist.</summary>
        /// <param name="error">Ein Objekt, das weitere Informationen zum Fehler enthält.</param>
        public void OnError(Exception error)
        {
            Debug.Assert(false);    
        }

        /// <summary>Benachrichtigt den Beobachter, dass der Anbieter das Senden von Pushbenachrichtigungen abgeschlossen hat.</summary>
        public void OnCompleted()
        {
            nodes.Clear();
        }

        /// <summary>Subscribes to the specified provider and creates a way to stop oberserving it.</summary>
        /// <param name="provider">The provider.</param>
        public void Subscribe(IObservable<NodeControl> provider)
        {
            StopObserving = provider.Subscribe(this);
        }

        /// <summary>Unsubscribes this instance.</summary>
        public void Unsubscribe()
        {
            StopObserving.Dispose();
            nodes.Clear();
        }


        /// <summary>Handles the Click event of the AddFromExistingEntryToolStripMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AddFromExistingEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.HandleAddFromExistingEntry();
        }

        /// <summary>Handles the Click event of the CreateNewToolStripMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CreateNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.HandleNewEntryClick();
        }



        /// <summary>Handles the Click event of the EvaluateAllToolStripMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void EvaluateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.HandleEvaluateAll();
        }





        /// <summary>Handles the Click event of the AddEntryButton.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AddEntryButton_Click(object sender, EventArgs e)
        {
            controller.HandleAddEntryButtonClicked();
        }




        /// <summary>Handles the Click event of the ShowLinkedIdentitiesMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowLinkedIdentitiesMenuItem_Click(object sender, EventArgs e)
        {
            
           
        }

        /// <summary>Handles the Click event of the ShowLinkedFaMethodsMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowLinkedFaMethodsMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>Handles the Click event of the ViewToolStripMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>Handles the CheckedChanged event of the ShowLinkedIdentitiesMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowLinkedIdentitiesMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowLinkedIdentitiesMenuItem.Checked) { controller.HandleShowLinkedIdentitiesClicked(); }
            else
            {
                DisposeIageList(ImageEnum.Identity, false);
                Invalidate(false);
                UpdateLinesBool = true;
                Update();
            }

           
        }

        /// <summary>Handles the Opening event of the DashboardMenuStrip.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void DashboardMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>Handles the Click event of the AddEntryToolStripMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AddEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.HandleAddEntryButtonClicked();
        }

        /// <summary>Handles the Click event of the UsesSamePhoneMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void UsesSamePhoneMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>Handles the CheckedChanged event of the UsesSameMailMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void UsesSameMailMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (usesSameMailMenuItem.Checked) { controller.HandleShowLinkedrRecoveryMailClicked(); }
            else
            {
                DisposeIageList(ImageEnum.Mail, false);
                Invalidate(false);
                UpdateLinesBool = true;
                Update();
            }

 

        }

        /// <summary>Handles the CheckedChanged event of the UsesSamePhoneMenuItem.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void UsesSamePhoneMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if(usesSamePhoneMenuItem.Checked) { controller.HandleShowLinkedrRecoveryPhonesClicked(); }
            else {
                DisposeIageList(ImageEnum.Phone, false);
                Invalidate(false);
                UpdateLinesBool = true;
                Update();
            }
 
            

        }

    }
}