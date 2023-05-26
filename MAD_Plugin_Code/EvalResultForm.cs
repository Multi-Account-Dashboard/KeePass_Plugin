using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAD_Plugin
{
    public partial class EvalResultForm : Form
    {
        public EvalResultForm()
        {
    
            InitializeComponent();
        }

        /// <summary>Löst das <see cref="E:System.Windows.Forms.Form.FormClosing">FormClosing</see>-Ereignis aus.</summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.FormClosingEventArgs">FormClosingEventArgs</see>, das die Ereignisdaten enthält.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // only hides the form when closed by the user since it gets disposed if closed normaly
            {
                e.Cancel = true;
                Hide();
            }
            else base.OnFormClosing(e);
        }

        /// <summary>Hides the warnings box.</summary>
        public void HideWarningsBox()
        {
            WarningsGroupBox.Hide();
        }
        /// <summary>Hides the fixes box.</summary>
        public void HideFixesBox()
        {
            FixesGroupBox.Hide();
        }

        /// <summary>Shows the warnings box.</summary>
        public void ShowWarningsBox()
        {
            WarningsGroupBox.Show();
        }
        /// <summary>Shows the fixes box.</summary>
        public void ShowFixesBox()
        {
            FixesGroupBox.Show();
        }




        /// <summary>Handles the Click event of the GoBackButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GoBackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


        /// <summary>Edits the header label.</summary>
        /// <param name="headerText">The header text.</param>
        public void EditHeaderLabel(string headerText)
        {
            HeaderLabel.Text = headerText;
            HeaderLabel.Left = HeaderGroupBox.Location.X + ((HeaderGroupBox.Width - HeaderLabel.Width- HeaderGroupBox.Left) / 2); // gets the header text in the middle of the headergroupbox

        }

        /// <summary>Edits the issues label.</summary>
        /// <param name="issuesText">The issues text.</param>
        public void EditIssuesLabel(string issuesText)
        {
            WarningsLabel.Text = issuesText;
        }
        /// <summary>Edits the fixes label.</summary>
        /// <param name="fixesText">The fixes text.</param>
        public void EditFixesLabel(string fixesText)
        {
            FixesLabel.Text = fixesText;
        }

   
    }
}
