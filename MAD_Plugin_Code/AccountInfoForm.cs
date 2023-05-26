using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MAD_Plugin
{
    public partial class AccountInfoForm : Form
    {
        private PwDatabase m_db = null;
        private PwEntry m_entry = null;
        private bool m_initForExistingNode = false;
        private Control m_selContr = null;
        private int m_accountScore = 3;
        private bool m_tbProtected = true;
        private MAD_Controller m_controller;


        /// <summary>The textbox and KeePass entry field pairs</summary>
        private Dictionary<string, string> m_textboxStringFieldPairs = new Dictionary<string, string>
        {
            {"addressRichTextBox", "Identity:Address"},
            {"phoneNumberTextBox", "Identity:PhoneNumber" },
            {"mailTextBox", "Identity:Mail"},
            {"dateOfBirthTextBox", "Identity:DateOfBirth" },
            {"givenNameTextBox", "Identity:Name" },
            {"userNameTextBox", "UserName" },
            {"accountProviderTextBox", "Account:Provider" },
            {"firstPkqTextBox", "PKQ:FirstQuestion" },
            {"secondPkqTextBox", "PKQ:SecondQuestion" },
            {"thirdPkqTextBox", "PKQ:ThirdQuestion" },
            {"firstPkqSecureTextBox", "PKQ:FirstAnswer" },
            {"secondPkqSecureTextBox", "PKQ:SecondAnswer" },
            {"thirdPkqSecureTextBox", "PKQ:ThirdAnswer" },
            {"recoveryMailTextBox", "RecoveryMail:Address" },
            {"recoveryPhoneNumberTextBox", "RecoveryPhone:Number" },
            {"locationOfCodeTextBox", "BackupCode:Location" },
            {"accessInfoTextBox", "BackupCode:AccessInfo" },
            {"selectedAccountTypeTextBox", "Account:Type" },
            {"accessPhoneComboBox" ,"Recovery:UnlockPhone" }
        };

        /// <summary>Initializes a new instance of the <see cref="AddAccountForm" /> class.</summary>
        /// <param name="initForExistingNode">if set to <c>true</c> this form opens to edit an existing node.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="owner">The owner of the form.</param>
        public AccountInfoForm(bool initForExistingNode, MAD_Controller controller, MAD_Form owner)
        {
            this.m_controller = controller;
            this.m_initForExistingNode = initForExistingNode;
            InitializeComponent();
            this.Owner = owner;
        }

        /// <summary>Initializes a new instance of the <see cref="AddAccountForm" /> class.</summary>
        /// <param name="controller">The controller.</param>
        public AccountInfoForm(MAD_Controller controller)
        {
            this.m_controller = controller;
            InitializeComponent();
        }

        /// <summary>Initializes the form externally by giving it an entry and database.</summary>
        /// <param name="db">The database.</param>
        /// <param name="entry">The entry.</param>
        public void InitEx(PwDatabase db, PwEntry entry)
        {
            this.m_entry = entry;
            this.m_db = db;

            if (m_initForExistingNode)
            {
                selEntryLabel.Text = "You are editing the Multi-Account-Dashboard node of your " + "\n" + "\"" + entry.Strings.ReadSafe("Title") + "\"" + "-Keepass entry";
                existingEntriesComboBox.Hide();
            }
        }

        /// <summary>Initializes the externally by only giving it a database.</summary>
        /// <param name="db">The database.</param>
        public void InitEx(PwDatabase db)
        {
            this.m_db = db;

            accountSecurityGroupBox.Hide();
            existingIdentityComboBox.Hide();
            accountExpireDateTimePicker.Hide();
            showExistingMailComboBox.Hide();
        }

        /// <summary>Sets the field string of a KeePass entry.</summary>
        /// <param name="field">The field f the entry.</param>
        /// <param name="input">Its desired value.</param>
        /// <param name="bProtected">if set to <c>true</c> the protection of the value is active.</param>
        private void SetFieldString(string field, string input, bool bProtected)
        {
            m_entry.Strings.Set(field, new ProtectedString(bProtected, input));
        }

        /// <summary>Saves the information of a given text box to the database.</summary>
        /// <param name="BoxName">Name of the box.</param>
        /// <param name="BoxText">The box text.</param>
        /// <param name="isProtected">if set to <c>true</c> the set value is protected from being displayed in clear text.</param>
        private void SaveTextBoxInfoToDb(string BoxName, string BoxText, bool isProtected)
        {
            string field = m_textboxStringFieldPairs[BoxName];
            m_entry.Strings.Set(field, new ProtectedString(isProtected, BoxText));
        }

        /// <summary>Handles the Click event of the tabPage1 control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

  

        /// <summary>Handles the TextChanged event of the secureTextBoxEx1 control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void secureTextBoxEx1_TextChanged(object sender, EventArgs e)
        {
        }

  
        /// <summary>Handles the Load event of the AccountInfoForm control. Loads all data in the textboxes when a entry is specified, supplies a list of possible entries otherwise</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AccountInfoForm_Load(object sender, EventArgs e)
        {
            if (m_entry != null)
            {
                IterateThoughAllChildControls(this.accountInfoTapControl, false);
            }
            else
            {
                foreach (var entry in m_db.RootGroup.GetEntries(true))
                {
                    if (entry.Strings.GetSafe("VisualId").ReadString() == string.Empty) existingEntriesComboBox.Items.Add(entry.Strings.ReadSafe("Title"));
                }
            }
        }

        /// <summary>Handles the FormClosing event of the AccountInfoForm control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs" /> instance containing the event data.</param>
        private void AccountInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }



        /// <summary>Handles the Click event of the tabPage2 control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tabPage2_Click(object sender, EventArgs e)
        {
        }


        /// <summary>Handles the CheckedChanged event of the CheckBox2 control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (accountExpireDateTimePicker.Visible) { accountExpireDateTimePicker.Hide(); m_entry.Expires = false; }
            else { accountExpireDateTimePicker.Show(); m_entry.Expires = true; }
        }

        /// <summary>Handles the Click event of the MailButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MailButton_Click(object sender, EventArgs e)
        {
            this.selectedAccountTypeTextBox.Text = "Mail-Account";
            SelectedAccountTypeTextBox_TextChanged(null, null);
        }

        /// <summary>Handles the MouseEnter event of the MailButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MailButton_MouseEnter(object sender, EventArgs e)
        {
            MailButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the MailButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MailButton_MouseLeave(object sender, EventArgs e)
        {
            MailButton.ImageIndex = 0;
        }

        /// <summary>Handles the Click event of the SocialMediaButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SocialMediaButton_Click(object sender, EventArgs e)
        {
            this.selectedAccountTypeTextBox.Text = "Social-Media-Account";
            SelectedAccountTypeTextBox_TextChanged(null, null);
        }

        /// <summary>Handles the MouseEnter event of the SocialMediaButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SocialMediaButton_MouseEnter(object sender, EventArgs e)
        {
            SocialMediaButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the SocialMediaButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SocialMediaButton_MouseLeave(object sender, EventArgs e)
        {
            SocialMediaButton.ImageIndex = 0;
        }

        /// <summary>Handles the Click event of the ShoppingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShoppingButton_Click(object sender, EventArgs e)
        {
            this.selectedAccountTypeTextBox.Text = "Shopping-Account";
            SelectedAccountTypeTextBox_TextChanged(null, null);
        }

        /// <summary>Handles the MouseEnter event of the ShoppingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShoppingButton_MouseEnter(object sender, EventArgs e)
        {
            ShoppingButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the ShoppingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShoppingButton_MouseLeave(object sender, EventArgs e)
        {
            ShoppingButton.ImageIndex = 0;
        }

        /// <summary>Handles the Click event of the BankingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BankingButton_Click(object sender, EventArgs e)
        {
            this.selectedAccountTypeTextBox.Text = "Banking-Account";
            SelectedAccountTypeTextBox_TextChanged(null, null);
        }

        /// <summary>Handles the MouseLeave event of the BankingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BankingButton_MouseLeave(object sender, EventArgs e)
        {
            BankingButton.ImageIndex = 0;
        }

        /// <summary>Handles the MouseEnter event of the BankingButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BankingButton_MouseEnter(object sender, EventArgs e)
        {
            BankingButton.ImageIndex = 1;
        }

        /// <summary>Handles the Click event of the CustomButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CustomButton_Click(object sender, EventArgs e)
        {
            this.selectedAccountTypeTextBox.Text = "Custom-Service-Account";
            SelectedAccountTypeTextBox_TextChanged(null, null);
        }

        /// <summary>Handles the MouseEnter event of the CustomButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CustomButton_MouseEnter(object sender, EventArgs e)
        {
            CustomButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the CustomButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CustomButton_MouseLeave(object sender, EventArgs e)
        {
            CustomButton.ImageIndex = 0;
        }

        /// <summary>Handles the Click event of the tabPage3 control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tabPage3_Click(object sender, EventArgs e)
        {
        }


        /// <summary>Handles the MouseEnter event of the BackupCodeButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BackupCodeButton_MouseEnter(object sender, EventArgs e)
        {
            backupCodeButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the BackupCodeButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BackupCodeButton_MouseLeave(object sender, EventArgs e)
        {
            backupCodeButton.ImageIndex = 0;
        }

        /// <summary>Handles the MouseEnter event of the PhoneRecoveryButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void PhoneRecoveryButton_MouseEnter(object sender, EventArgs e)
        {
            phoneRecoveryButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the PhoneRecoveryButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void PhoneRecoveryButton_MouseLeave(object sender, EventArgs e)
        {
            phoneRecoveryButton.ImageIndex = 0;
        }

        /// <summary>Handles the MouseEnter event of the MailRecoveryButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MailRecoveryButton_MouseEnter(object sender, EventArgs e)
        {
            mailRecoveryButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the MailRecoveryButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MailRecoveryButton_MouseLeave(object sender, EventArgs e)
        {
            mailRecoveryButton.ImageIndex = 0;
        }

        /// <summary>Handles the MouseEnter event of the PkqButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void PkqButton_MouseEnter(object sender, EventArgs e)
        {
            pkqButton.ImageIndex = 1;
        }

        /// <summary>Handles the MouseLeave event of the PkqButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void PkqButton_MouseLeave(object sender, EventArgs e)
        {
            pkqButton.ImageIndex = 0;
        }



        /// <summary>Handles the SelectedIndexChanged event of the ExistingEntries control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ExistingEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            string entryText = existingEntriesComboBox.GetItemText(existingEntriesComboBox.SelectedItem);
            try
            {
                PwEntry entry = FindEntryByTitle(entryText);
                m_entry = entry;
                IterateThoughAllChildControls(accountInfoTapControl, false);
            }
            catch (NullReferenceException) { Debug.Assert(false); }
        }

      

        /// <summary>Iterates the though all child controls. Callup upon loading the form to iterate through all controls and set 
        /// their value in case that value is linked to existing data or and called upon closing to set edited values as data in keepass,
        /// whsich is then safed in the onclosing function of the form</summary>
        /// <param name="control">The control to start the iteration from.</param>
        /// <param name="onClosing">if set to <c>true</c> the actions performed are related to wirting to the database instead of reading it.</param>
        private void IterateThoughAllChildControls(Control control, bool onClosing)
        {
            {
                if (onClosing)
                {
                    SetFieldString("Account:Score", securityTrackBar.Value + "", true);
                    foreach (Control contr in control.Controls)
                    {
                        if (contr.GetType() == typeof(TextBox) | contr.GetType() == typeof(SecureTextBoxEx) | contr.GetType() == typeof(RichTextBox))
                        {
                            SaveTextBoxInfoToDb(contr.Name, contr.Text, true);
                        }
                        IterateThoughAllChildControls(contr, true);
                    }
                }
                else
                {
                    foreach (Control contr in control.Controls)
                    {
                        if (contr.GetType() == typeof(TextBox) | contr.GetType() == typeof(RichTextBox) | contr.GetType() == typeof(SecureTextBoxEx))
                        {
                            string currentTextBoxText = m_entry.Strings.ReadSafe(m_textboxStringFieldPairs[contr.Name]);
                            contr.Text = currentTextBoxText;
                            if (contr.Name == "selectedAccountTypeTextBox")
                            {
                                if (!contr.Text.Contains("Shopping") && !contr.Text.Contains("Banking") && !contr.Text.Contains("Social") && !contr.Text.Contains("Mail") && contr.Text != string.Empty)
                                {
                                    accountSecurityGroupBox.Show();
                                }
                                else
                                {
                                    accountSecurityGroupBox.Hide();
                                }
                            }

                            if (contr.Name == "recoveryMailTextBox")
                            {
                                foreach (var entry in m_db.RootGroup.GetEntries(true))
                                {
                                    if (entry.Equals(m_entry)) { continue; }
                                    if (entry.Strings.GetSafe("Account:Type").ReadString() == "Mail-Account" && entry.Strings.GetSafe("UserName").ReadString() == currentTextBoxText)
                                    {
                                        linkExistingMailaccountCheckBox.Checked = true;
                                        showExistingMailComboBox.SelectedItem = currentTextBoxText;
                                        showExistingMailComboBox.Show();
                                        break; //assuming there is only one mailaccount with the chosen mailaddress present
                                    }
                                    else
                                    {
                                        showExistingMailComboBox.Hide();
                                    }
                                }
                            }
                        }

                        if (contr.GetType() == typeof(DateTimePicker))
                        {
                            DateTimePicker dtp = (DateTimePicker)contr;
                            dtp.Hide();
                            if (m_entry.Expires)
                            {
                                dtp.Value = m_entry.ExpiryTime;
                                FindControl("accountExpireCheckBox", contr.Parent);
                                CheckBox cb = (CheckBox)m_selContr;
                                cb.Checked = true;
                                dtp.Show();
                            }
                        }

                        if (contr.GetType() == typeof(TrackBar))
                        {
                            TrackBar tb = (TrackBar)contr;
                            if (m_entry.Strings.ReadSafe("Account:Score") != string.Empty)
                            {
                                tb.Value = Convert.ToInt16(m_entry.Strings.ReadSafe("Account:Score"));
                            }
                        }

                        if (contr.GetType() == typeof(ComboBox))
                        {
                            if (contr.Name == "existingIdentityComboBox")
                            {
                                List<string> identityNames = new List<string>();
                                foreach (var entry in m_db.RootGroup.GetEntries(true))
                                {
                                    string identityName = entry.Strings.GetSafe("Identity:Name").ReadString();
                                    ComboBox cb = (ComboBox)contr;
                                    if (identityName != string.Empty && !identityNames.Contains(identityName))
                                    {
                                        cb.Items.Add(identityName);
                                        identityNames.Add(identityName);
                                    }

                                    if (
                                    identityName != string.Empty &&
                                    entry.Strings.ReadSafe("Identity:Name") == m_entry.Strings.ReadSafe("Identity:Name") &&
                                    entry.Strings.ReadSafe("Identity:Address") == m_entry.Strings.ReadSafe("Identity:Address") &&
                                    entry.Strings.ReadSafe("Identity:PhoneNumber") == m_entry.Strings.ReadSafe("Identity:PhoneNumber") &&
                                    entry.Strings.ReadSafe("Identity:Mail") == m_entry.Strings.ReadSafe("Identity:Mail") &&
                                    entry.Strings.ReadSafe("Identity:DateOfBirth") == m_entry.Strings.ReadSafe("Identity:DateOfBirth")
                                    )
                                    {
                                        existingIdentityCheckBox.Checked = true;
                                        cb.SelectedItem = identityName;
                                        cb.Show();
                                    }
                                    else
                                    {
                                        existingIdentityCheckBox.Checked = false;
                                        cb.Hide();
                                    }
                                }
                            }

                            if (contr.Name == "showExistingMailComboBox")
                            {
                                ComboBox cb = (ComboBox)contr;
                                foreach (var entry in m_db.RootGroup.GetEntries(true))
                                {
                                    if (entry.Equals(m_entry)) { continue; }

                                    if (entry.Strings.GetSafe("Account:Type").ReadString() == "Mail-Account") cb.Items.Add(entry.Strings.ReadSafe("UserName"));  //Problem with multiple mailaccounts with the same emailaddress that were all added to the dashboard. could happen in theory, not further addressed
                                }
                                if (linkExistingMailaccountCheckBox.Checked = true && recoveryMailTextBox.Text != string.Empty)
                                {
                                    cb.SelectedItem = recoveryMailTextBox.Text; // is set further up with the textboxes but with the changes to the cb here overwritten if the controls are iterated in the wrong order, so here set again to be sure
                                }
                            }
                            if (contr.Name == "accessPhoneComboBox")
                            {
                                if (m_entry.Strings.GetSafe("Recovery:UnlockPhone").ReadString() != string.Empty)
                                {
                                    accessPhoneComboBox.SelectedItem = m_entry.Strings.GetSafe("Recovery:UnlockPhone").ReadString();
                                }
                            }
                        }

                        IterateThoughAllChildControls(contr, false);
                    }
                }
            }
        }

        /// <summary>Handles the Click event of the FinishedButton control. Actions performed vary depending on the circumstances that called the form</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void FinishedButton_Click(object sender, EventArgs e)
        {
            this.finishedButton.Enabled = false;
            if (m_entry != null)
            {
                IterateThoughAllChildControls(this.accountInfoTapControl, true);

                if (m_initForExistingNode)
                {
                    m_controller.UpdateNode(new PwUuid(MemUtil.HexStringToByteArray(m_entry.Strings.ReadSafe("VisualId"))));
                }
                else
                {
                    m_controller.Finished(m_entry);
                }

                this.Hide();
                this.Dispose();
                return;
            }
            else
            {
                GoBackForm dlg = new GoBackForm();
                dlg.ShowDialog();
                accountInfoTapControl.SelectedIndex = 0;
                this.finishedButton.Enabled = true;
            }
        }

        /// <summary>Löst das <see cref="E:System.Windows.Forms.Form.FormClosing">FormClosing</see>-Ereignis aus.</summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.FormClosingEventArgs">FormClosingEventArgs</see>, das die Ereignisdaten enthält.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            m_controller.AccountInfoFormClosed();
            base.OnFormClosing(e);
        }

        /// <summary>Finds the control recursivly and sets m_selContr to the found control.</summary>
        /// <param name="name">The name of the searched control.</param>
        /// <param name="control">The control to start the search from.</param>
        private void FindControl(string name, Control control)
        {
            foreach (Control contr in control.Controls)
            {
                if (contr.Name == name) { m_selContr = contr; break; }
                FindControl(name, contr);
            }
        }

        /// <summary>Finds KeePass database entries by the name of the safed identity.</summary>
        /// <param name="name">The name of the identity of the entry.</param>
        /// <returns>
        ///  a KeePass database entry with the given identity name or a exeption if not found
        /// </returns>
        /// <exception cref="System.NullReferenceException">name of entry not found</exception>
        private PwEntry FindEntryByName(string name)
        {
            foreach (var entry in m_db.RootGroup.GetEntries(true))
            {
                if (entry.Strings.GetSafe("Identity:Name").ReadString() == name) { return entry; }
            }
            throw new NullReferenceException("name of entry not found");
        }

        /// <summary>Finds a KeePass entry by its title.</summary>
        /// <param name="name">The title of the entry.</param>
        /// <returns>
        ///  a KeePass database entry with the given name or a exeption if not found
        /// </returns>
        /// <exception cref="System.NullReferenceException">name of entry not found</exception>
        private PwEntry FindEntryByTitle(string name)
        {
            foreach (var entry in m_db.RootGroup.GetEntries(true))
            {
                if (entry.Strings.GetSafe("Title").ReadString() == name) { return entry; }
            }
            throw new NullReferenceException("name of entry not found");
        }

        /// <summary>Handles the SelectedIndexChanged event of the ExistingIdentityComboBox control. Autofills the identitiy fields with the data from the selected identity</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ExistingIdentityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_entry == null) return;
            else
            {
                FindControl("existingIdentityComboBox", accountInfoTapControl);
                ComboBox cb = (ComboBox)m_selContr;

                string entryText = cb.GetItemText(cb.SelectedItem);

                try
                {
                    PwEntry entry = FindEntryByName(entryText);

                    foreach (var input in entry.Strings)
                    {
                        if (input.Key.Contains("Identity:"))
                        {
                            if (input.Key.Contains("Address"))
                            {
                                FindControl("addressRichTextBox", cb.Parent);
                                m_selContr.Text = input.Value.ReadString();
                            }
                            if (input.Key.Contains("PhoneNumber"))
                            {
                                FindControl("phoneNumberTextBox", cb.Parent);
                                m_selContr.Text = input.Value.ReadString();
                            }
                            if (input.Key.Contains("Mail"))
                            {
                                FindControl("mailTextBox", cb.Parent);
                                m_selContr.Text = input.Value.ReadString();
                            }

                            if (input.Key.Contains("Name"))
                            {
                                FindControl("givenNameTextBox", cb.Parent);
                                m_selContr.Text = input.Value.ReadString();
                            }
                            if (input.Key.Contains("Date"))
                            {
                                FindControl("dateOfBirthTextBox", cb.Parent);
                                m_selContr.Text = input.Value.ReadString();
                            }
                        }
                    }
                }
                catch (NullReferenceException) { Debug.Assert(false); }
            }
        }

        /// <summary>Handles the CheckedChanged event of the ExistingIdentityCheckBox control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ExistingIdentityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (existingIdentityCheckBox.Checked) existingIdentityComboBox.Show();
            else
            {
                existingIdentityComboBox.Hide();
            }
        }

        /// <summary>Handles the Click event of the IdentityNextButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void IdentityNextButton_Click(object sender, EventArgs e)
        {
            this.accountInfoTapControl.SelectedIndex = 1;
        }

        /// <summary>Handles the Click event of the AccountTypeNextButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AccountTypeNextButton_Click(object sender, EventArgs e)
        {
            this.accountInfoTapControl.SelectedIndex = 2;
        }

        /// <summary>Handles the Click event of the AccoutTypeBackButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AccoutTypeBackButton_Click(object sender, EventArgs e)
        {
            this.accountInfoTapControl.SelectedIndex = 0;
        }

        /// <summary>Handles the Click event of the FaBackButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void FaBackButton_Click(object sender, EventArgs e)
        {
            this.accountInfoTapControl.SelectedIndex = 1;
        }

        /// <summary>Handles the TextChanged event of the SelectedAccountTypeTextBox control. Autofills the acount type TextBox</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SelectedAccountTypeTextBox_TextChanged(object sender, EventArgs e)
        {
            switch (selectedAccountTypeTextBox.Text)
            {
                case "Banking-Account":
                    m_accountScore = 4;
                    accountSecurityGroupBox.Hide();
                    return;

                case "Mail-Account":
                    m_accountScore = 3;
                    accountSecurityGroupBox.Hide();
                    return;

                case "Social-Media-Account":
                    m_accountScore = 3;
                    accountSecurityGroupBox.Hide();
                    return;

                case "Shopping-Account":
                    m_accountScore = 2;
                    accountSecurityGroupBox.Hide();
                    return;
            }
            accountSecurityGroupBox.Show();
        }

        /// <summary>Handles the ValueChanged event of the AccountExpireDateTimePicker control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AccountExpireDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            m_entry.Expires = true;
            m_entry.ExpiryTime = accountExpireDateTimePicker.Value;
        }

        /// <summary>Handles the Scroll event of the SecurityTrackBar control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SecurityTrackBar_Scroll(object sender, EventArgs e)
        {
            m_accountScore = securityTrackBar.Value;
        }

        /// <summary>Handles the TextChanged event of the FirstPkqSecureTextBoxEx control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void FirstPkqSecureTextBoxEx_TextChanged(object sender, EventArgs e)
        {
        }

        /// <summary>Handles the CheckedChanged event of the ShowProtectedStringsCheckBox control. Shows the clear text thats inside of a SecureTextBox.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowProtectedStringsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_tbProtected)
            {
                m_tbProtected = false;
                firstPkqSecureTextBox.EnableProtection(m_tbProtected);
                secondPkqSecureTextBox.EnableProtection(m_tbProtected);
                thirdPkqSecureTextBox.EnableProtection(m_tbProtected);
            }
            else
            {
                m_tbProtected = true;
                firstPkqSecureTextBox.EnableProtection(m_tbProtected);
                secondPkqSecureTextBox.EnableProtection(m_tbProtected);
                thirdPkqSecureTextBox.EnableProtection(m_tbProtected);
            }
        }

        /// <summary>Handles the CheckedChanged event of the LinkExistingMailaccountCheckBox control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void LinkExistingMailaccountCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showExistingMailComboBox.Visible == true)
            {
                showExistingMailComboBox.Hide();
            }
            else
            {
                showExistingMailComboBox.Show();
            }
        }

        /// <summary>Handles the SelectedIndexChanged event of the ShowExistingMailComboBox control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowExistingMailComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            recoveryMailTextBox.Text = showExistingMailComboBox.GetItemText(showExistingMailComboBox.SelectedItem);
        }

        /// <summary>Handles the SelectedIndexChanged event of the AccessPhoneComboBox control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AccessPhoneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string input = accessPhoneComboBox.GetItemText(accessPhoneComboBox.SelectedItem);
            SetFieldString("Recovery:UnlockPhone", input, true);
        }

    
    }
}