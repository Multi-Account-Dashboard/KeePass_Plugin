using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{
    public class MAD_Controller
    {
        private MAD_Form m_view;
        private MAD_Model m_model;
        private IPluginHost m_host;
        private bool m_EvaluationStarted = false;

        /// <summary>Initializes a new instance of the <see cref="MAD_Controller" /> class.</summary>
        /// <param name="view">The view.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">The reference to the running KeePass application.</param>
        public MAD_Controller(MAD_Form view, MAD_Model model, IPluginHost host)
        {
            if (Properties.Settings.Default.NodesLayoutInfo == null) { Properties.Settings.Default.NodesLayoutInfo = new StringCollection(); }

            this.m_view = view;
            this.m_model = model;
            this.m_view.SetMADController(this);
            this.m_host = host;
        }

        /// <summary>Gets the provider information from the json database.</summary>
        /// <returns>A list of all Entries in JSON database formatted as ProviderDataObjects</returns>
        private List<ProviderDataObject> GetProviderInfoFromJson()
        {
            string dllExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dllDirPath = Path.GetDirectoryName(dllExePath) + "\\..";
            string jsonPath = (dllDirPath + "\\ProvidersDatabase.json");

            string jsonDataAsString = File.ReadAllText(jsonPath);
            JObject providerDataJsonObject = JObject.Parse(jsonDataAsString);

            List<JToken> dataEntries = providerDataJsonObject["Providers"].Children().ToList();
            List<ProviderDataObject> providerData = new List<ProviderDataObject>();

            foreach (JToken porviderName in dataEntries)
            {
                foreach (JToken providerDataInJsonFormat in porviderName)
                {
                    ProviderDataObject providerDataInNetFormat = providerDataInJsonFormat.ToObject<ProviderDataObject>();
                    providerData.Add(providerDataInNetFormat);
                }
            }
            return providerData;
        }

        /// <summary>Handles the new entry click event. Delegates to another event with the same outcome</summary>
        public void HandleNewEntryClick()
        {
            HandleAddEntryButtonClicked();

        }

        /// <summary>Issues a new node to be added to the dashboard.</summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="pos">The position of the node.</param>
        /// <param name="entry">The KeePass entry in reference to the node.</param>
        /// <param name="db">The database of the entry.</param>
        /// <returns>
        ///   the visualId of the newly created node to be added to the user.config file in the StringCollection NodesLayoutInfo
        /// </returns>
        private PwUuid IssueNewNode(string title, Point pos, PwEntry entry, PwDatabase db)
        {
            PwUuid visualId = m_model.CreateNewNode(title, pos, entry, db);
            string visualIdStr = MemUtil.ByteArrayToHexString(visualId.UuidBytes);
            string layoutInfo = String.Format("{0}/{1}/{2}", pos.X, pos.Y, visualIdStr);
            Properties.Settings.Default.NodesLayoutInfo.Add(layoutInfo);
            Properties.Settings.Default.Save();
            return visualId;
        }

        /// <summary>Handles the add entry button clicked event. Creates a AccountInfoForm from which new nodes can be added to the dashboard </summary>
        internal void HandleAddEntryButtonClicked()
        {
           
            m_view.TempUncheckAllLinks();
            m_view.Enabled = false;
            AccountInfoForm aif = new AccountInfoForm(false, this, m_view);
            aif.InitEx(m_host.Database);

            aif.Show();
        }



        /// <summary>Called from the AccountInfoFrom when its closed succesfully and was initiated by the <see cref="HandleAddEntryButtonClicked" />  function then issues a new node from the model</summary>
        /// <param name="entry">The keePass entry for which the node is created.</param>
        internal void AddSingleNewNodeFromEntry(PwEntry entry)
        {
            Point p = new Point(25, 25);
            PwUuid visualId = IssueNewNode(entry.Strings.ReadSafe("Title"),  p, entry, m_host.Database);
            SetNewStringField(entry, "VisualId", new ProtectedString(false, MemUtil.ByteArrayToHexString(visualId.UuidBytes)));
        }

        /// <summary>
        /// Notifies the view about change of the node to repaint it. Called from the AccountInfoForm when initiated for a node already present in the dashboard.
        /// The connections between nodes were uncheckted while the AccountInfoForm was open, so we need to recheck them.
        /// </summary>
        /// <param name="entry">The KeePass entry of the node.</param>
        public void UpdateNode(PwUuid entry)
        {
            m_view.Enabled = true;

            if (m_EvaluationStarted) { HandleEvaluateAll(); }

            m_model.UpdateNode(entry);



            m_view.RecheckTempUnchecked();


        }

        /// <summary>Called when a node is to be removed from the dashboard. Removes it from the user.config file and tells the model to delete it.</summary>
        /// <param name="vId">The visualId identifier of the node to be deleted.</param>
        internal void HandleRemoveNode(PwUuid vId)
        {
            string completeLine = "";
            foreach(string line in Properties.Settings.Default.NodesLayoutInfo)
            {
                if(line.Contains(MemUtil.ByteArrayToHexString(vId.UuidBytes))) { completeLine = line; break; }
            }
            if(completeLine != "") { Properties.Settings.Default.NodesLayoutInfo.Remove(completeLine); }
          
            m_model.DeleteNode(vId);
            
        }



        /// <summary>Created nodes for all KeePass entries that are without an visualId and therefore not already in the dashboard.</summary>
        internal void HandleAddFromExistingEntry()
        {
            PwDatabase db = m_host.Database;
            if ((db == null) || !db.IsOpen) { Debug.Assert(false); return; }
            else
            {
                Point p = new Point(25, 25);
                int counter = 0;
                foreach (var entry in db.RootGroup.GetEntries(true))
                {
                    if (entry.Strings.ReadSafe("VisualId") == String.Empty)
                    {
                        PwUuid visualId = IssueNewNode(entry.Strings.ReadSafe("Title"), p, entry, m_host.Database);
                        SetNewStringField(entry, "VisualId", new ProtectedString(false, MemUtil.ByteArrayToHexString(visualId.UuidBytes)));
                        p.Y += 150;
                        counter += 1;
                        if (counter > 3) { p.X += 300; p.Y -= 600; counter = 0; }
                      
                    }
                }
            }
        }


        /// <summary>Gets the title of a node from visual identifier.</summary>
        /// <param name="visualId">The visual identifier.</param>
        /// <returns>
        ///   title of the seearched node or an invalid string in case its not found
        /// </returns>
        private string GetTitleFromVisualId(PwUuid visualId)
        {
            PwDatabase db = m_host.Database;
            if (db == null | !db.IsOpen) { return "no DB"; }

            string title = "notValid";
            foreach (PwEntry entry in db.RootGroup.GetEntries(true))
            {
                try
                {
                    if (GetValueOfField(entry, "VisualId").Equals(new ProtectedString(false, MemUtil.ByteArrayToHexString(visualId.UuidBytes)), false)) { title = GetValueOfField(entry, "Title").ReadString(); break; }
                }
                catch { continue; }
            }
            return title;
        }

        /// <summary>Sets the new string field. This is the method creating the new Fields in the KeePass entry</summary>
        /// <param name="entry">The KeePass entry.</param>
        /// <param name="fieldName">Name of the field to be edited. Creates new field with this name if its not already present</param>
        /// <param name="value">The value of the field, i.e. the 128 bits of the visualId.</param>
        private void SetNewStringField(PwEntry entry, string fieldName, ProtectedString value)
        {
            entry.Strings.Set(fieldName, value);
        }

        /// <summary>Gets the value of a given Keepass entry field.</summary>
        /// <param name="entry">The entry.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        ///  The value of the field which is always a ProtectedString
        /// </returns>
        private ProtectedString GetValueOfField(PwEntry entry, string field)
        {
            return entry.Strings.Get(field);
        }





        /// <summary>
        /// Called when the evaluation of the nodes is initiated by the user.
        /// Creates an instance of EvalEngine to compute the Evaluation and passes the modified nodes to the model.
        /// </summary>
        internal void HandleEvaluateAll()
        {
            m_EvaluationStarted = true;
            List<ProviderDataObject> dataList = GetProviderInfoFromJson();
            List<NodeControl> nodes = m_model.Nodes();
            EvaluateEngine ee = new EvaluateEngine(m_host.Database, nodes, dataList);
            nodes = ee.Evaluate();
            foreach (NodeControl node in nodes)
            {
                m_model.UpdateNode(node.VisualId);
            }
            
        }





        /// <summary>Handles the account node clicked event. Opens an AccountInfoForm to edit the node </summary>
        /// <param name="nodeClicked">The node that got clicked.</param>
        internal void HandleAccountNodeClicked(NodeControl nodeClicked)
        {
            m_view.Enabled = false;
            AccountInfoForm aAF = new AccountInfoForm(true, this, m_view);

            m_view.TempUncheckAllLinks();

            
                aAF.InitEx(m_host.Database, nodeClicked.Entry);
                aAF.Show();
            
        }

        /// <summary>Handles the form loaded event. Uses the data stored in NodesLayoutInfo to refill the dashboard with the nodes that were present when itshut down the last time </summary>
        internal void HandleFormLoaded()
        {
            if (Properties.Settings.Default.NodesLayoutInfo != null && Properties.Settings.Default.NodesLayoutInfo.Count > 0)
            {
                foreach (string line in Properties.Settings.Default.NodesLayoutInfo)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
       
                        string[] nodeInfo = line.Split('/');
                        if (nodeInfo.Length >= 3)
                        {
                            int x = Convert.ToInt16(nodeInfo[0]);
                            int y = Convert.ToInt16(nodeInfo[1]);
                            Point pos = new Point(x, y);

                            PwEntry temp = null;
                            foreach (var e in m_host.Database.RootGroup.GetEntries(true))
                            {
                                if (e.Strings.ReadSafe("VisualId") == nodeInfo[2]) { temp = e; break; }
                            }

                            // dont add nodes that were deleted from KeePass when the dashboard was not running
                            if (temp != null && temp.ParentGroup.Uuid != m_host.Database.RecycleBinUuid)
                            {
                                PwUuid visualId = new PwUuid(MemUtil.HexStringToByteArray(nodeInfo[2]));

                                string title = GetTitleFromVisualId(visualId);

                                m_model.AddNodesFormLoaded(title, pos, visualId, temp, m_host.Database);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Handles the formclosing event. Saves all nodes into the user.config file, then saves changes made to the KeePass database.</summary>
        internal void HandleFormclosing()
        {
            List<string> tempMem = new List<string>();
            foreach (string line in Properties.Settings.Default.NodesLayoutInfo)
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] nodeInfo = line.Split('/');
                    if (nodeInfo.Length >= 3)
                    {
                        PwUuid visualId = new PwUuid(MemUtil.HexStringToByteArray(nodeInfo[2]));

                        NodeControl node = m_model.Nodes().Find(x => x.VisualId.UuidBytes.SequenceEqual(visualId.UuidBytes));

                        if (node != null)
                        {
                            tempMem.Add(String.Format("{0}/{1}/{2}", node.Location.X, node.Location.Y, MemUtil.ByteArrayToHexString(visualId.UuidBytes)));
                        }
                    }
                }
            }

            Properties.Settings.Default.Reset();
            Properties.Settings.Default.NodesLayoutInfo = new StringCollection();

            foreach (string line in tempMem)
            {
                Properties.Settings.Default.NodesLayoutInfo.Add(line);

     
            }

            Properties.Settings.Default.Save();
            
            m_host.MainWindow.SaveDatabase(m_host.Database, null);
           

        }



        /// <summary>Called from the AccountInfoFrom when a new Node is to be added.</summary>
        /// <param name="entry">The KeePass entry that was updated though the form to now create a node for.</param>
        public void Finished(PwEntry entry)
        {
            m_view.Enabled = true;
            if (m_EvaluationStarted) { HandleEvaluateAll(); }
            AddSingleNewNodeFromEntry(entry);
            m_view.RecheckTempUnchecked();
 



        }


        /// <summary>Handles the show linked identities clicked event. Finds all nodes with connedted identities and hands them to the model to update the nodes with that info.</summary>
        internal void HandleShowLinkedIdentitiesClicked()
        {
            List<List<String>> sameIdNodes = FindConnectedIdentities();
            m_model.UpdateNodesForLinks(sameIdNodes, ImageEnum.Identity);

        }

        /// <summary>Handles the show linked recoverymail clicked event.  Finds all nodes with connected recoverymail and hands them to the model to update the nodes with that info. </summary>
        internal void HandleShowLinkedrRecoveryMailClicked()
        {
            List<List<String>> sameRecoveryMail = FindConnectedMails();
        

            m_model.UpdateNodesForLinks(sameRecoveryMail, ImageEnum.Mail);
        }

        /// <summary>Handles the show linkedr recoveryphones clicked. inds all nodes with connected recoveryphones and hands them to the model to update the nodes with that info.</summary>
        internal void HandleShowLinkedrRecoveryPhonesClicked()
        {
            List<List<String>> sameRecoveryPhones = FindConnectedPhones();
       
           
            m_model.UpdateNodesForLinks(sameRecoveryPhones, ImageEnum.Phone);
        }

        /// <summary>Finds a node by its visualId.</summary>
        /// <param name="vid">The visualId of the node searched for.</param>
        /// <returns>
        ///  the searched node or null when not found
        /// </returns>
        private NodeControl FindNodeByVid(string vid)
        {
            foreach(var node in m_model.Nodes())
            {
                if (node.VisualIdString == vid) return node;
            }
            return null; 
        }
        /// <summary>
        /// Finds nodes that share a commen field value. Uses LINQ to search the database and single out the 
        /// necessary fields and then iterates over the found data to find all nodes that share a field with another node
        /// </summary>
        /// <param name="fieldToCheck">The field value to check.</param>
        /// <returns>
        ///   A List of Nodes that share a common field value
        /// </returns>
        private List<NodeControl> FindLinks(string fieldToCheck)
        {
            List<NodeControl> returnList = new List<NodeControl>();
            var data = from field in m_host.Database.RootGroup.GetEntries(true)
                       where (field.Strings.ReadSafe(fieldToCheck) != string.Empty && field.Strings.ReadSafe("VisualId") != string.Empty)
                       select new
                       {
                           value = field.Strings.ReadSafe(fieldToCheck),
                           vID = field.Strings.ReadSafe("VisualId"),
                           
                       };
            foreach (var pair1 in  data)
            {
               foreach(var pair2 in data)
                {
                    if(pair1.vID != pair2.vID && pair1.value == pair2.value)
                    {
                        try
                        {
                            NodeControl node = FindNodeByVid(pair1.vID);
                            
                            returnList.Add(node);
                            

                        }
                        catch { Debug.Assert(false); }
                        
                    }
                }
            }
            return returnList;
            
        }


        /// <summary>Finds all the nodes that share an identity. To be reguarded as equal, nodes must share a name, date of birth and a way to reach out to that identity</summary>
        /// <returns>
        ///   A list with each item being a list of visualIds of nodes that share an identity.
        /// </returns>
        private List<List<string>> FindConnectedIdentities()
        {
            var commonNamesNodes1 = FindLinks("Identity:Name");
            var commonDoBNodes1 = FindLinks("Identity:DateOfBirth");
            var commonPhoneNodes1 = FindLinks("Identity:PhoneNumber");
            var commonMailNodes1 = FindLinks("Identity:Mail");
            List<List<string>> rtnList = new List<List<string>>();
            var commonNamesNodes = FindLinks("Identity:Name");
            var commonDoBNodes = FindLinks("Identity:DateOfBirth");
            var commonPhoneNodes = FindLinks("Identity:PhoneNumber");
            var commonMailNodes = FindLinks("Identity:Mail");
            var commonReachString = commonPhoneNodes.Select(x => x.VisualIdString).Union(commonMailNodes.Select(x => x.VisualIdString));

            var linkedNames = commonNamesNodes.GroupBy(x => x.Entry.Strings.ReadSafe("Identity:Name"));
            foreach(var group in linkedNames)
            {
                List<string> tempList = new List<string>(group.Select(x => x.VisualIdString).Intersect(commonDoBNodes.Select(x => x.VisualIdString).Intersect(commonReachString)));
                if(tempList.Count != 0) { rtnList.Add(tempList);  }
               
            }
            return rtnList;
          



        }


        /// <summary>Finds all the nodes that share an recovery phone number.</summary>
        /// <returns>
        ///   A list with each item being a list of visualIds of nodes that share a recovery phone number.
        /// </returns>
        private List<List<string>> FindConnectedPhones()
        {
            List<List<string>> rtnList = new List<List<string>>();
            var commonRecoveryPhoneNodes = FindLinks("RecoveryPhone:Number");
      

            var linkedPhones = commonRecoveryPhoneNodes.GroupBy(x => x.Entry.Strings.ReadSafe("RecoveryPhone:Number"));
      
            foreach (var group in linkedPhones)
            {
                List<string> tempList = new List<string>(group.Select(x => x.VisualIdString));
                if (tempList.Count != 0) { rtnList.Add(tempList); }

            }
            return rtnList;
         



        }



        /// <summary>Finds all the nodes that share an recovery mail address.</summary>
        ///   A list with each item being a list of visualIds of nodes that share a recovery mail address.
        /// </returns>
        private List<List<string>> FindConnectedMails()
        {
            List<List<string>> rtnList = new List<List<string>>();
            var commonRecoveryMailNodes = FindLinks("RecoveryMail:Address");


            var linkedMails = commonRecoveryMailNodes.GroupBy(x => x.Entry.Strings.ReadSafe("RecoveryMail:Address"));

            foreach (var group in linkedMails)
            {
                List<string> tempList = new List<string>(group.Select(x => x.VisualIdString));
                if (tempList.Count != 0) { rtnList.Add(tempList); }

            }
            return rtnList;

        }
        /// <summary>called when an AccountInfoForm is closed by pressing the red X in the top right corner of the form.</summary>
        public void AccountInfoFormClosed()
        {
            m_view.Enabled = true;
            m_view.RecheckTempUnchecked();
        }
    }


}