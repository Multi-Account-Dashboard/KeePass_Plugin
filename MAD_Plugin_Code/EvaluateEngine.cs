using KeePassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{
    internal class EvaluateEngine
    {
        private List<NodeControl> m_Nodes = null;
        private PwDatabase m_db = null;
        private List<ProviderDataObject> m_providerDataList = null;

        private ProviderDataObject m_selProv = null;
        private string m_nameOfProvider = null;


        /// <summary>
        ///  the three outcomes of an evaluation
        /// </summary>
        private enum ResultEnum
        {
            Fine,
            Susceptiple,
            Insecure
        }

        /// <summary>Initializes a new instance of the <see cref="EvaluateEngine" /> class.</summary>
        /// <param name="db">The database of the entries to check.</param>
        /// <param name="nodes">The nodes to check.</param>
        /// <param name="providerData">The provider data from the JSON file.</param>
        public EvaluateEngine(PwDatabase db, List<NodeControl> nodes, List<ProviderDataObject> providerData)
        {
            this.m_db = db;
            this.m_Nodes = nodes;
            this.m_providerDataList = providerData;
        }



        /// <summary>Sets the sets scores and real scores.</summary>
        private void SetSetsAndReals()
        {
            foreach(NodeControl node in m_Nodes)
            {
                GetSetScore(node);
                GetRealScore(node);
            }
            foreach (NodeControl node in m_Nodes)
            {
                IsUsedAsFA(node);
            }

        }

        /// <summary>
        /// Evaluates all nodes passed to this instance
        /// </summary>
        public List<NodeControl> Evaluate()
        {
            SetSetsAndReals();
            foreach (NodeControl node in m_Nodes)
            {
                int set = node.setScore;
                int real = node.realScore;
                if ((set > 0 && real > 0) || (set > 0 && GetPhoneScore(node) == 0)) //if scores are greater than 0 or the phonescore is 0 it means that evaluation was successfull and we can create feedback
                {
                    
                    switch (set, real)
                    {
                        case var compare when set > real: // need to create a variable here that is never used to compare set and real in a switch statement... a C# thing
                            {
                                //adds text to the 3 components of a EvalResultFrom based on found issues and fixes
                                node.NodeEvalForm.EditHeaderLabel(GetEvalFormHeaderText(node, ResultEnum.Insecure, node.m_accountType, node.Text));
                                node.NodeEvalForm.EditFixesLabel(GetFixesLabelText(node));
                                node.NodeEvalForm.EditIssuesLabel(GetIssuesLabelText(node));

                                node.ChangeTrafficLightColor(NodeControl.TrafficLightColorsEnum.Red, false);
                                node.NodeEvalForm.ShowFixesBox();
                                node.NodeEvalForm.ShowWarningsBox();
                                break; 
                            }
                        case var compare when set == real:
                            {
                                //adds text to the 3 components of a EvalResultFrom based on found issues and fixes
                                node.NodeEvalForm.EditHeaderLabel(GetEvalFormHeaderText(node, ResultEnum.Susceptiple, node.m_accountType, node.Text));
                                node.NodeEvalForm.EditFixesLabel(GetFixesLabelText(node));
                                node.NodeEvalForm.EditIssuesLabel(GetIssuesLabelText(node));

                                node.ChangeTrafficLightColor(NodeControl.TrafficLightColorsEnum.Yellow, false);
                                node.NodeEvalForm.ShowFixesBox();
                                node.NodeEvalForm.ShowWarningsBox();
                                break;
                            }
                        case var compare when set < real: 
                            {
                                //no need to look for fixes here since there's no need for further improvement
                                node.NodeEvalForm.EditHeaderLabel(GetEvalFormHeaderText(node, ResultEnum.Fine, node.m_accountType, node.Text));
                                node.NodeEvalForm.HideFixesBox();
                                node.NodeEvalForm.EditIssuesLabel(GetIssuesLabelText(node));
                                node.ChangeTrafficLightColor(NodeControl.TrafficLightColorsEnum.Green, false);
                                break;
                            }
                    }
                }
                else { node.ChangeTrafficLightColor(NodeControl.TrafficLightColorsEnum.Gray, true); break; } // evaluation cound not be successfully finished for this node, not enough data provided
            }
            return new List<NodeControl>();
        }

        /// <summary>
        /// Gets the set score.
        /// </summary>
        /// <param name="node">The node to be checked.</param>
        /// <returns></returns>
        private int GetSetScore(NodeControl node)
        {
            bool isKnown = IsProviderKnown(node);
            int rtn = -1;
            switch (node.Entry.Strings.ReadSafe("Account:Type"))
            {
                case "": node.setScore = -1; return -1;
                default: { rtn = Convert.ToInt32(node.Entry.Strings.ReadSafe("Account:Score")); break; }
            }
            if (isKnown && HostsMultipleServices())
            {
                rtn += 1;
            }
            node.setScore = rtn;
            return rtn;
        }



        /// <summary>
        /// Gets the eval form header text.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <param name="result">The result of the evaluation.</param>
        /// <param name="type">The type of the account.</param>
        /// <param name="title">The title of the node.</param>
        /// <returns></returns>
        private string GetEvalFormHeaderText(NodeControl node, ResultEnum result, AccountTypeEnum type, string title)
        {
            string typeString = "";
            switch (type)
            {
                case AccountTypeEnum.Mail: { typeString = "Mail-Account"; break; }
                case AccountTypeEnum.Shopping: { typeString = "Shopping-Account"; break; }
                case AccountTypeEnum.Banking: { typeString = "Banking-Account";  break; }
                case AccountTypeEnum.Custom: { typeString = "Custom-Service-Account"; break; }
                case AccountTypeEnum.SocialMedia: { typeString = "Social-Media-Account"; break; }
            }
            StringBuilder sb = new StringBuilder();
            switch (result)
            {
                case ResultEnum.Fine: {
                        sb.Append("  No securit risks were found for your\n  ");
                        sb.Append(title);
                        sb.Append(" ");
                        sb.Append(typeString);
                        sb.Append(".");
                        break; 
                    }
                case ResultEnum.Susceptiple:
                    {
                        sb.Append("Your ");
                        sb.Append(title);
                        sb.Append(" ");
                        sb.Append(typeString);
                        sb.Append("\nmay be susceptible to attacks.");
                        break;
                    }
                case ResultEnum.Insecure:
                    {
                        sb.Append("Your ");
                        sb.Append(title);
                        sb.Append(" ");
                        sb.Append(typeString);
                        sb.Append(" is not \n sufficiently protected.");
                        break;
                    }
            }
            return sb.ToString();

        }


        /// <summary>
        /// Determines whether the provider of the account represented by the node is known to the dashboard.
        /// </summary>
        /// <param name="node">The node checked.</param>
        /// <returns>
        ///   <c>true</c> if the provider is known; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProviderKnown(NodeControl node)
        {
            string nameofProvider = node.Entry.Strings.ReadSafe("Account:Provider");
            foreach (var data in m_providerDataList)
            {
                if (data.Name == nameofProvider.ToLower()) { m_selProv = data; m_nameOfProvider = nameofProvider; return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the provider currently referenced in m_selProv uses 2 Step Verification
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is using 2sv]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUsing2SV()
        {
            if (m_selProv != null)
            {
                return m_selProv.Uses2StepVerificationFA;
            }
            return false;
        }

        /// <summary>
        /// Determines whether currently referenced in m_selProv uses risk based authentication
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is using rba]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUsingRBA()
        {
            if (m_selProv != null)
            {
                return m_selProv.UsesRBA;
            }
            return false;
        }

        /// <summary>
        /// Determines whether currently referenced in m_selProv hosts multiple services
        /// </summary>
        private bool HostsMultipleServices()
        {
            if (m_selProv != null)
            {
                return m_selProv.ProvidesServicesForMultipleDevices;
            }
            return false;
        }



        /// <summary>
        /// Gets the real score of a specific node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private int GetRealScore(NodeControl node)
        {
            int score = 0;
            //frstly determine what FA methods are used by the node
            bool nodeIsUsingPhone = IsUsingFaMethod(node, "RecoveryPhone:Number");
            bool nodeIsUsingBackupCode = IsUsingFaMethod(node, "Backup");
            bool nodeIsUsingMailRecover = IsUsingFaMethod(node, "RecoveryMail:Address");
            bool nodeIsUsingPKQS = IsUsingFaMethod(node, "PKQ");
            //then check what the provider is deploying on his side of the equation
            bool providerIsUsing2SV = false;
            bool providerIsKnown = IsProviderKnown(node);
            
            if (providerIsKnown) // adds the +1 for using RBA
            {
                providerIsUsing2SV = IsUsing2SV();
                if (IsUsingRBA()) { score += 1; }
            }
            if (providerIsUsing2SV) //adding two lowest method scores when using 2SV
            {
                if (nodeIsUsingPKQS)
                {
                    score += 1;
                    nodeIsUsingPKQS = false;
                    if (nodeIsUsingPhone)
                    {
                        score += GetPhoneScore(node);
                        nodeIsUsingPhone = false;
                    }

                    else if (nodeIsUsingMailRecover)

                    {
                        score += 2;
                        nodeIsUsingMailRecover = false;
                    }
                   
                    else if (nodeIsUsingBackupCode)
                    {
                        score += 3;
                        nodeIsUsingBackupCode = false;
                    }
                }
                else if (nodeIsUsingPhone)
                {
                    score += GetPhoneScore(node);
                    nodeIsUsingPhone = false;
                    if (nodeIsUsingMailRecover)
                    {
                        score += 2;
                        nodeIsUsingMailRecover = false;
                    }
                    else if (nodeIsUsingBackupCode)
                    {
                        score += 3;
                        nodeIsUsingBackupCode = false;
                    }
                }
                else if (nodeIsUsingMailRecover)
                {
                    score += 2;
                    nodeIsUsingMailRecover = false;

                     if (nodeIsUsingBackupCode)
                    {
                        score += 3;
                        nodeIsUsingBackupCode = false;
                    }
                }
          
            }
            else
            {
                if(GetPhoneScore(node) == 0) { score += 0; }
                else if (nodeIsUsingPKQS) { score += 1; }
                else if(GetPhoneScore(node) == 1) { score += 1; }
                else if (nodeIsUsingMailRecover) { score += 2; }
                if (nodeIsUsingPhone) { score += GetPhoneScore(node); }
                else if (nodeIsUsingBackupCode) { score += 3; }
            }
            if (score == 0 && GetPhoneScore(node) != 0) { node.realScore = -1; return -1; }
            else { node.realScore = score; return score; }
      
        }


        /// <summary>
        /// Gets the phone score. Its dependend on the weakest method securing the phone.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private int GetPhoneScore(NodeControl node)
        {
            int rtn = 0;
      
            switch (node.Entry.Strings.ReadSafe("Recovery:UnlockPhone"))
            {
                case "Biometry (Fingerprint or FaceID)": { rtn = 3; break; }
                case "Pin (only Digits)": { rtn = 1; break; }
                case "Password": { rtn = 2; break; }
                case "Biometry and Pin": { rtn = 1; break; }
                case "Biometry and Password": { rtn = 2; break; }
                case "Hardware Token": { rtn = 3; break; }
                case "No Security Mechanism": { rtn = 0; break; }
                default: { rtn = 1; break; } 

            }
            return rtn;
        }

        /// <summary>
        /// Gets the fixes label text.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <returns></returns>
        private string GetFixesLabelText(NodeControl node)
        {
            string fixes = GetFixes(node);
            StringBuilder sb = new StringBuilder();
            if(node.numberOfFixes == 0) 
            {
                sb.Append("Unfortunatly no fixes were found\n for this account!");
                return sb.ToString();
            } else if (node.numberOfFixes == 1)
            {
                sb.Append("There was ");
                sb.Append(node.numberOfFixes);
                sb.Append(" possible fix found\n for this account:\n");
                sb.Append(fixes);
                return sb.ToString();
            }
            {
                sb.Append("There were ");
                sb.Append(node.numberOfFixes);
                sb.Append(" possible fixes found\n for this account:\n");
                sb.Append(fixes);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the fixes strings.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <returns></returns>
        private string GetFixes(NodeControl node)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            if(GetPhoneScore(node) < 2)
            {
                count += 1;
                string number = node.Entry.Strings.ReadSafe("RecoveryPhone:Number");
                sb.Append("\n - Consider using at least a password\n   for protecting your phone with the\n   number ");
                sb.Append(number);
                sb.Append(".\n");
            }
            if(node.MailEdges.Count != 0) 
            {
                count += 1;
                string recoveryMailName = node.MailEdges[0];
                sb.Append("\n - Use a different Recovery-Email\n   or add more security to your\n   ");
                sb.Append(recoveryMailName);
                sb.Append("-Mail Account.\n");
            }
            if(IsUsingFaMethod(node, "PKQ"))
            {
                if (IsProviderKnown(node))
                {
                    if(m_selProv.UsingBackupCode && !IsUsingFaMethod(node, "Backup"))
                    {
                        count += 1;
                        sb.Append("\n - Your provider ");
                        sb.Append(m_nameOfProvider);
                        sb.Append(" is offering to use\n    a Backup-Code for account recovery.\n    Please consider using this method\n    instead of security questions.\n");
                    }
                    if (m_selProv.UsingRecoveryPhone && !IsUsingFaMethod(node, "RecoveryPhone:Number"))
                    {
                        count += 1;
                        sb.Append("\n - Your provider ");
                        sb.Append(m_nameOfProvider);
                        sb.Append(" is offering to use\n    a Phonenumber for account recovery.\n    Please consider using this method\n    instead of security questions.\n");
                    }
                    if (m_selProv.UsingRecoveryMail && !IsUsingFaMethod(node, "RecoveryMail:Address"))
                    {
                        count += 1;
                        sb.Append("\n - Your provider ");
                        sb.Append(m_nameOfProvider);
                        sb.Append(" is offering to use\n    a Emailaddress for account recovery.\n    Please consider using this method\n    instead of security questions.\n");
                    }
                } else
                {
                    count += 1;
                    
                    sb.Append("\n - The use of security questions as a\n    Fallback-Authentication method is\n    not recommended.  Consider\n    switching to stronger methods.\n");
               
                }
     
            }
            node.numberOfFixes = count;

            return sb.ToString();
        }

        /// <summary>
        /// Gets the issues label text.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <returns></returns>
        private string GetIssuesLabelText(NodeControl node)
        {
            StringBuilder sb = new StringBuilder();
            string warningsText = GetWarnings(node);
            if (node.numberOfWarnings == 0)
            {
                sb.Append(""); // maybe: No Issues were found with the\n   configuration of this account 
            }
            else if (node.numberOfWarnings == 1)
            {
                sb.Append("There was one Warning found for\nthis account: \n");
            }
            else
            {
                sb.Append("There was were multiple Warnings found\nfor this account: \n");
            }
            sb.Append(warningsText);
            return sb.ToString();
        
        }
        /// <summary>
        /// Gets the warning strings.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <returns></returns>
        private string GetWarnings(NodeControl node)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
   


            if (GetPhoneScore(node) < 2)
            {
                count += 1;
                string number = node.Entry.Strings.ReadSafe("RecoveryPhone:Number");
                sb.Append("\n - Your phone with the number\n   ");
                sb.Append(number);
                sb.Append(" is weakly protected.\n");
            }
            if (node.MailEdges.Count != 0)
            {
                count += 1;
                string recoveryMailName = node.MailEdges[0];
                sb.Append("\n - The Recovery-Email-Address for this\n   account is protected more weakly\n   than this account.\n");
            }
            if (IsUsingFaMethod(node, "PKQ"))
            {
                count += 1;

                sb.Append("\n - Security questions may be easily\n    guessed by attackers.\n");
            }
            int numberOfReuses = IdentityReusedOnOtherAccount(node);
            if(numberOfReuses > 0)
            {
                count += 1;
                sb.Append("\n - The Identity connected with this\n   account is reused on ");
                sb.Append(numberOfReuses);
                if(numberOfReuses == 1) { sb.Append(" occasion.\n   This increases the risk of personal\n   inforamtion being leaked"); }
                if (numberOfReuses > 1) { sb.Append(" occasions.\n   This increases the risk of personal\n   inforamtion being leaked"); }
                sb.Append("\n");

            }
            node.numberOfWarnings = count;
            return sb.ToString();

            }





        /// <summary>
        /// number of Identities reuses among all evaluated accounts.
        /// </summary>
        /// <param name="node">The correlating node.</param>
        /// <returns></returns>
        private int IdentityReusedOnOtherAccount(NodeControl node)
        {
            int counter = 0;
            foreach (NodeControl nc in m_Nodes)
            {
                if (node != nc && SameIdentityUsed(node, nc))
                {
                    counter += 1;
                }
            }
            return counter;
        }

        /// <summary>
        /// Checks whether two nodes use the same identity.
        /// </summary>
        /// <param name="node1">The first node of the two.</param>
        /// <param name="node2">The second node of the two.</param>
        /// <returns></returns>
        private bool SameIdentityUsed(NodeControl node1, NodeControl node2)
        {
            var identity1 = from kvp
                            in node1.Entry.Strings
                            where (kvp.Key.Contains("Identity") && !kvp.Value.IsEmpty)
                            select kvp.Value;
            var identity2 = from kvp
                            in node2.Entry.Strings
                            where (kvp.Key.Contains("Identity") && !kvp.Value.IsEmpty)
                            select kvp.Value;
            int i = identity1.Count();
            int y = identity2.Count();

            if (i == y)
            {
                foreach (var input1 in identity1)
                {
                    bool isSame = false;
                    foreach (var input2 in identity2)
                    {
                        if (!input1.Equals(input2, true)) { continue; }
                         isSame = true;
                    }
                    if (!isSame) { return false; }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given node is used as a FA method of another node. If its the case, set and real scores are updated for both nodes
        /// </summary>
        /// <param name="node">The given node.</param>
        /// <returns>
        ///   <c>true</c> if the given node is used as a FA method by at least one other account and scores were updated; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUsedAsFA(NodeControl node)
        {
            switch (node.Entry.Strings.ReadSafe("Account:Type"))
            {
                case "Mail-Account":
                    {
                        foreach (NodeControl n in m_Nodes)
                        {
                            if (node.Entry != n.Entry)
                            {
                                if ((n.Entry.Strings.ReadSafe("Identity:Mail") != null && n.Entry.Strings.ReadSafe("Identity:Mail") == node.Entry.Strings.ReadSafe("UserName")) ||
                                    (n.Entry.Strings.ReadSafe("RecoveryMail:Address") != null && n.Entry.Strings.ReadSafe("RecoveryMail:Address") == node.Entry.Strings.ReadSafe("UserName")) ||
                                    (n.Entry.Strings.ReadSafe("UserName") != null && n.Entry.Strings.ReadSafe("UserName") == node.Entry.Strings.ReadSafe("UserName")))
                                {
                                    if (node.setScore < n.setScore) { node.setScore = n.setScore; }
                                    if (node.realScore < n.realScore)
                                    {
                                        n.MailEdges.Clear();
                                        n.realScore = node.realScore;
                                        n.MailEdges.Add(node.Text);
                                    }
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                default: { return false; }
            }
        }

        /// <summary>
        /// Determines whether a node is cofigured with using FA methods.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <param name="partOfFieldString">The part of field string of the FA fields.</param>
        /// <returns>
        ///   <c>true</c> if [is using fa method] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUsingFaMethod(NodeControl node, string partOfFieldString)
        {
            var datas = from kvp in node.Entry.Strings
                        where (kvp.Key.Contains(partOfFieldString) && !kvp.Value.IsEmpty)
                        select kvp.Key;
            if (datas.Count() != 0) return true;
            else return false;
        }
    }
}