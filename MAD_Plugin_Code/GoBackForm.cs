﻿using System;
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
    public partial class GoBackForm : Form
    {
        /// <summary>Initializes a new instance of the <see cref="GoBackForm" /> class.</summary>
        public GoBackForm()
        {
            InitializeComponent();
        }



        /// <summary>Handles the Click event of the boBackButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void boBackButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
