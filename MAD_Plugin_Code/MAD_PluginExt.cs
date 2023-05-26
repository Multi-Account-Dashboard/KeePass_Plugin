/*
  Copyright (C) 2003-2019 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


/*
***************************************************************************************
* This class is based on a template for KeePass Plugins referenced here
* Title: Plugin Development (2.x)
* Author:Dominik Reichl
* Availability: https://keepass.info/help/v2_dev/plg_index.html
*
***************************************************************************************
*/
// The namespace name must be the same as the file name of the
// plugin without its extension.
namespace MAD_Plugin
{
    public sealed class MAD_PluginExt : Plugin
    {
        // The plugin remembers its m_host in this variable
        private IPluginHost m_host = null;


     

        /// <summary>
        /// The <c>Initialize</c> method is called by KeePass when
        /// you should initialize your plugin.
        /// </summary>
        /// <param name="host">Plugin m_host interface. Through this
        /// interface you can access the KeePass main window, the
        /// currently opened database, etc.</param>
        /// <returns>You must return <c>true</c> in order to signal
        /// successful initialization. If you return <c>false</c>,
        /// KeePass unloads your plugin (without calling the
        /// <c>Terminate</c> method of your plugin).</returns>
        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false; // Fail; we need the m_host
            m_host = host;

  

            return true; // Initialization successful
        }

        /// <summary>
        /// The <c>Terminate</c> method is called by KeePass when
        /// you should free all resources, close files/streams,
        /// remove event handlers, etc.
        /// </summary>
        public override void Terminate()
        {
            // Remove event handler (important!)
  
        }

        /// <summary>
        /// Get a menu item of the plugin. See
        /// https://keepass.info/help/v2_dev/plg_index.html#co_menuitem
        /// </summary>
        /// <param name="t">Type of the menu that the plugin should
        /// return an item for.</param>
        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            // Our menu item below is intended for the main location(s),
            // not for other locations like the group or entry menus
            if (t != PluginMenuType.Main) return null;

            ToolStripMenuItem tsmi = new ToolStripMenuItem("MAD_Plugin");



            ToolStripMenuItem tsmiOpenDashboard = new ToolStripMenuItem
            {
                Text = "Open MAD"
            };
            tsmiOpenDashboard.Click += this.OpenMAD;
            tsmi.DropDownItems.Add(tsmiOpenDashboard);

       

            ToolStripMenuItem tsmiClear = new ToolStripMenuItem
            {
                Text = "Clear Dashboard"
            };
            tsmiClear.Click += this.Clear;
            tsmi.DropDownItems.Add(tsmiClear);

            // By using an anonymous method as event handler, we do not
            // need to remember menu item references manually, and
            // multiple calls of the GetMenuItem method (to show the
            // menu item in multiple places) are no problem
            tsmi.DropDownOpening += delegate (object sender, EventArgs e)
            {
   

            };

            return tsmi;
        }

        /// <summary>Clears the whole dashboard.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Clear(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            foreach( PwEntry entry in m_host.Database.RootGroup.GetEntries(true)) {
                entry.Strings.Set("VisualId", new ProtectedString(false, String.Empty));
            }
            m_host.MainWindow.SaveDatabase(m_host.Database, sender);
        }

        /// <summary>Opens the dashbaord by initializing the mvc components.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OpenMAD(object sender, EventArgs e)
        {
            MAD_Model model = new MAD_Model();
            MAD_Form form  = new MAD_Form();
            form.Subscribe(model);
            MAD_Controller controller = new MAD_Controller(form, model, m_host);
            form.Show();
           
        }
    }
}