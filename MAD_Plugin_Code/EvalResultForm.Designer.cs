
namespace MAD_Plugin
{
    partial class EvalResultForm
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
            this.goBackButton = new System.Windows.Forms.Button();
            this.HeaderLabel = new System.Windows.Forms.Label();
            this.WarningsGroupBox = new System.Windows.Forms.GroupBox();
            this.WarningsLabel = new System.Windows.Forms.Label();
            this.FixesGroupBox = new System.Windows.Forms.GroupBox();
            this.FixesLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HeaderGroupBox = new System.Windows.Forms.GroupBox();
            this.WarningsGroupBox.SuspendLayout();
            this.FixesGroupBox.SuspendLayout();
            this.HeaderGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // goBackButton
            // 
            this.goBackButton.Location = new System.Drawing.Point(487, 499);
            this.goBackButton.Name = "goBackButton";
            this.goBackButton.Size = new System.Drawing.Size(100, 41);
            this.goBackButton.TabIndex = 0;
            this.goBackButton.Text = "Go Back";
            this.goBackButton.UseVisualStyleBackColor = true;
            this.goBackButton.Click += new System.EventHandler(this.GoBackButton_Click);
            // 
            // HeaderLabel
            // 
            this.HeaderLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HeaderLabel.AutoSize = true;
            this.HeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLabel.Location = new System.Drawing.Point(6, 18);
            this.HeaderLabel.Name = "HeaderLabel";
            this.HeaderLabel.Size = new System.Drawing.Size(62, 24);
            this.HeaderLabel.TabIndex = 0;
            this.HeaderLabel.Text = "Result";
            this.HeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WarningsGroupBox
            // 
            this.WarningsGroupBox.Controls.Add(this.WarningsLabel);
            this.WarningsGroupBox.Location = new System.Drawing.Point(12, 105);
            this.WarningsGroupBox.Name = "WarningsGroupBox";
            this.WarningsGroupBox.Size = new System.Drawing.Size(285, 374);
            this.WarningsGroupBox.TabIndex = 2;
            this.WarningsGroupBox.TabStop = false;
            // 
            // WarningsLabel
            // 
            this.WarningsLabel.AutoSize = true;
            this.WarningsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WarningsLabel.Location = new System.Drawing.Point(7, 18);
            this.WarningsLabel.Name = "WarningsLabel";
            this.WarningsLabel.Size = new System.Drawing.Size(0, 18);
            this.WarningsLabel.TabIndex = 0;
            // 
            // FixesGroupBox
            // 
            this.FixesGroupBox.Controls.Add(this.FixesLabel);
            this.FixesGroupBox.Location = new System.Drawing.Point(303, 105);
            this.FixesGroupBox.Name = "FixesGroupBox";
            this.FixesGroupBox.Size = new System.Drawing.Size(284, 374);
            this.FixesGroupBox.TabIndex = 3;
            this.FixesGroupBox.TabStop = false;
            // 
            // FixesLabel
            // 
            this.FixesLabel.AutoSize = true;
            this.FixesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FixesLabel.Location = new System.Drawing.Point(7, 18);
            this.FixesLabel.Name = "FixesLabel";
            this.FixesLabel.Size = new System.Drawing.Size(0, 18);
            this.FixesLabel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 492);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(385, 40);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tipp: You can show links between accounts in the \r\ndashboard by using the \"View\" " +
    "Menu-Item!";
            // 
            // HeaderGroupBox
            // 
            this.HeaderGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.HeaderGroupBox.Controls.Add(this.HeaderLabel);
            this.HeaderGroupBox.Location = new System.Drawing.Point(12, 8);
            this.HeaderGroupBox.Name = "HeaderGroupBox";
            this.HeaderGroupBox.Size = new System.Drawing.Size(575, 77);
            this.HeaderGroupBox.TabIndex = 1;
            this.HeaderGroupBox.TabStop = false;
            // 
            // EvalResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 552);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FixesGroupBox);
            this.Controls.Add(this.WarningsGroupBox);
            this.Controls.Add(this.HeaderGroupBox);
            this.Controls.Add(this.goBackButton);
            this.Name = "EvalResultForm";
            this.Text = "Evaluation Results";
            this.WarningsGroupBox.ResumeLayout(false);
            this.WarningsGroupBox.PerformLayout();
            this.FixesGroupBox.ResumeLayout(false);
            this.FixesGroupBox.PerformLayout();
            this.HeaderGroupBox.ResumeLayout(false);
            this.HeaderGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button goBackButton;
        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.GroupBox WarningsGroupBox;
        private System.Windows.Forms.Label WarningsLabel;
        private System.Windows.Forms.GroupBox FixesGroupBox;
        private System.Windows.Forms.Label FixesLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox HeaderGroupBox;
    }
}