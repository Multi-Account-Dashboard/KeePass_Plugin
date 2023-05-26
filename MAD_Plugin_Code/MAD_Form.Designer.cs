
using System.Drawing;
using System.Windows.Forms;


namespace MAD_Plugin
{
    partial class MAD_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>

       
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MAD_Form));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFromExistingEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowLinkedIdentitiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLinkedFaMethodsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usesSameMailMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usesSamePhoneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListAddButton = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.nodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dashboardMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.nodeMenuStrip.SuspendLayout();
            this.dashboardMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.evaluateToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(1382, 28);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewEntryToolStripMenuItem,
            this.addFromExistingEntryToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.editToolStripMenuItem.Text = "Add";
            // 
            // createNewEntryToolStripMenuItem
            // 
            this.createNewEntryToolStripMenuItem.Name = "createNewEntryToolStripMenuItem";
            this.createNewEntryToolStripMenuItem.Size = new System.Drawing.Size(283, 26);
            this.createNewEntryToolStripMenuItem.Text = "Create Account Node";
            this.createNewEntryToolStripMenuItem.Click += new System.EventHandler(this.CreateNewToolStripMenuItem_Click);
            // 
            // addFromExistingEntryToolStripMenuItem
            // 
            this.addFromExistingEntryToolStripMenuItem.Name = "addFromExistingEntryToolStripMenuItem";
            this.addFromExistingEntryToolStripMenuItem.Size = new System.Drawing.Size(283, 26);
            this.addFromExistingEntryToolStripMenuItem.Text = "Add All From Existing Entries";
            this.addFromExistingEntryToolStripMenuItem.Click += new System.EventHandler(this.AddFromExistingEntryToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowLinkedIdentitiesMenuItem,
            this.showLinkedFaMethodsMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // ShowLinkedIdentitiesMenuItem
            // 
            this.ShowLinkedIdentitiesMenuItem.CheckOnClick = true;
            this.ShowLinkedIdentitiesMenuItem.Name = "ShowLinkedIdentitiesMenuItem";
            this.ShowLinkedIdentitiesMenuItem.Size = new System.Drawing.Size(259, 26);
            this.ShowLinkedIdentitiesMenuItem.Text = "Show Linked Identities";
            this.ShowLinkedIdentitiesMenuItem.ToolTipText = "Connencts the added Account-Nodes that are linked via a common identity";
            this.ShowLinkedIdentitiesMenuItem.CheckedChanged += new System.EventHandler(this.ShowLinkedIdentitiesMenuItem_CheckedChanged);
            this.ShowLinkedIdentitiesMenuItem.Click += new System.EventHandler(this.ShowLinkedIdentitiesMenuItem_Click);
            // 
            // showLinkedFaMethodsMenuItem
            // 
            this.showLinkedFaMethodsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usesSameMailMenuItem,
            this.usesSamePhoneMenuItem});
            this.showLinkedFaMethodsMenuItem.Name = "showLinkedFaMethodsMenuItem";
            this.showLinkedFaMethodsMenuItem.Size = new System.Drawing.Size(259, 26);
            this.showLinkedFaMethodsMenuItem.Text = "Show Linked FA-Methods";
            this.showLinkedFaMethodsMenuItem.Click += new System.EventHandler(this.ShowLinkedFaMethodsMenuItem_Click);
            // 
            // usesSameMailMenuItem
            // 
            this.usesSameMailMenuItem.CheckOnClick = true;
            this.usesSameMailMenuItem.Name = "usesSameMailMenuItem";
            this.usesSameMailMenuItem.Size = new System.Drawing.Size(262, 26);
            this.usesSameMailMenuItem.Text = "Uses Same Recovery-Mail";
            this.usesSameMailMenuItem.CheckedChanged += new System.EventHandler(this.UsesSameMailMenuItem_CheckedChanged);
            this.usesSameMailMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItem_Click);
            // 
            // usesSamePhoneMenuItem
            // 
            this.usesSamePhoneMenuItem.CheckOnClick = true;
            this.usesSamePhoneMenuItem.Name = "usesSamePhoneMenuItem";
            this.usesSamePhoneMenuItem.Size = new System.Drawing.Size(262, 26);
            this.usesSamePhoneMenuItem.Text = "Uses Same Phone";
            this.usesSamePhoneMenuItem.CheckedChanged += new System.EventHandler(this.UsesSamePhoneMenuItem_CheckedChanged);
            this.usesSamePhoneMenuItem.Click += new System.EventHandler(this.UsesSamePhoneMenuItem_Click);
            // 
            // evaluateToolStripMenuItem
            // 
            this.evaluateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.evaluateAllToolStripMenuItem});
            this.evaluateToolStripMenuItem.Name = "evaluateToolStripMenuItem";
            this.evaluateToolStripMenuItem.Size = new System.Drawing.Size(79, 24);
            this.evaluateToolStripMenuItem.Text = "Evaluate";
            // 
            // evaluateAllToolStripMenuItem
            // 
            this.evaluateAllToolStripMenuItem.Name = "evaluateAllToolStripMenuItem";
            this.evaluateAllToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.evaluateAllToolStripMenuItem.Text = "Evaluate All";
            this.evaluateAllToolStripMenuItem.Click += new System.EventHandler(this.EvaluateAllToolStripMenuItem_Click);
            // 
            // imageListAddButton
            // 
            this.imageListAddButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListAddButton.ImageStream")));
            this.imageListAddButton.Tag = "";
            this.imageListAddButton.TransparentColor = System.Drawing.SystemColors.Window;
            this.imageListAddButton.Images.SetKeyName(0, "AddButton.png");
            this.imageListAddButton.Images.SetKeyName(1, "AddNewNodeText.png");
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "dddddddddddddddd";
            // 
            // nodeMenuStrip
            // 
            this.nodeMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.nodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.editMenuItem});
            this.nodeMenuStrip.Name = "removemenuStrip";
            this.nodeMenuStrip.Size = new System.Drawing.Size(133, 52);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // editMenuItem
            // 
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(132, 24);
            this.editMenuItem.Text = "Edit";
            // 
            // dashboardMenuStrip
            // 
            this.dashboardMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.dashboardMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem});
            this.dashboardMenuStrip.Name = "dashboardMenuStrip";
            this.dashboardMenuStrip.Size = new System.Drawing.Size(144, 28);
            this.dashboardMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.DashboardMenuStrip_Opening);
            // 
            // addEntryToolStripMenuItem
            // 
            this.addEntryToolStripMenuItem.Name = "addEntryToolStripMenuItem";
            this.addEntryToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.addEntryToolStripMenuItem.Text = "Add Entry";
            this.addEntryToolStripMenuItem.Click += new System.EventHandler(this.AddEntryToolStripMenuItem_Click);
            // 
            // MAD_Form
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1382, 953);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MAD_Form";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = " Multi Account Dashboard";
            this.Load += new System.EventHandler(this.MAD_Form_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.nodeMenuStrip.ResumeLayout(false);
            this.dashboardMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem createNewEntryToolStripMenuItem;
        private ToolStripMenuItem addFromExistingEntryToolStripMenuItem;
        private ToolStripMenuItem evaluateAllToolStripMenuItem;
        private ImageList imageListAddButton;
        private ToolTip toolTip1;
        private ToolStripMenuItem ShowLinkedIdentitiesMenuItem;
        private ToolStripMenuItem showLinkedFaMethodsMenuItem;
        private ToolStripMenuItem usesSameMailMenuItem;
        private ToolStripMenuItem usesSamePhoneMenuItem;
        private ContextMenuStrip nodeMenuStrip;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem editMenuItem;
        private ContextMenuStrip dashboardMenuStrip;
        private ToolStripMenuItem addEntryToolStripMenuItem;
    }
}