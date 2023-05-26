using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{
    public class MAD_Model : IObservable<NodeControl>
    {
        private List<IObserver<NodeControl>> m_observers;
        private List<NodeControl> m_nodes;


        /// <summary>Initializes a new instance of the <see cref="MAD_Model" /> class.</summary>
        public MAD_Model()
        {
            this.m_observers = new List<IObserver<NodeControl>>();
            this.m_nodes = new List<NodeControl>();
        }

        /// <summary>Returns the list of nodes of this instance.</summary>
        public List<NodeControl> Nodes()
        {
            return m_nodes;
        }

    

        /// <summary>Creates a new entry in the model and notifies the view.</summary>
        /// <param name="title">The title of the new node.</param>
        /// <param name="pos">Its position.</param>
        /// <param name="entry">The KeePass entry in refernece to the node.</param>
        /// <param name="db">The database where the .</param>
        /// <returns>
        ///   the VisualId of the newly created node
        /// </returns>
        internal PwUuid CreateNewNode(String title,  Point pos, PwEntry entry, PwDatabase db)
        {

            NodeControl node = new NodeControl(title, pos, db, entry);


            m_nodes.Add(node);
            foreach (var observer in m_observers)
            {
                observer.OnNext(node);
            }
            return node.VisualId;

        }



        /// <summary>Benachrichtigt den Anbieter, dass ein Beobachter Benachrichtigungen zu empfangen hat.</summary>
        /// <param name="observer">Das Objekt, das Benachrichtigungen zu empfangen hat.</param>
        /// <returns>Ein Verweis auf eine Schnittstelle, über die Beobachter den Empfang von Benachrichtigungen beenden können, bevor der Anbieter deren Versand einstellt.</returns>
        public IDisposable Subscribe(IObserver<NodeControl> observer)
        {
            if (!m_observers.Contains(observer))
            {
                m_observers.Add(observer);

                foreach (var node in m_nodes)
                {
                    observer.OnNext(node);
                }
            }
            return new Unsubscriber<NodeControl>(m_observers, observer);
        }

        /// <summary>
        ///  Defines a non generic Unsubscriber class for the correct implementation of the observer pattern in C#
        /// </summary>
        /// <typeparam name="NodeControl">TThe non generic type.</typeparam>
        internal class Unsubscriber<NodeControl> : IDisposable
        {
            private List<IObserver<NodeControl>> m_observers;
            private IObserver<NodeControl> m_observer;

            /// <summary>Initializes a new instance of the <see cref="Unsubscriber{AccountNode}" /> class.</summary>
            /// <param name="observers">The observers.</param>
            /// <param name="observer">The observer.</param>
            internal Unsubscriber(List<IObserver<NodeControl>> observers, IObserver<NodeControl> observer)
            {
                this.m_observers = observers;
                this.m_observer = observer;
            }

            /// <summary>Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.</summary>
            public void Dispose()
            {
                if (m_observers.Contains(m_observer))
                    m_observers.Remove(m_observer);
            }
        }

        /// <summary>Deletes the node out of the model after marking it as deleted and notifying the view.</summary>
        /// <param name="vId">The identifier of the node to be deleted.</param>
        internal void DeleteNode(PwUuid vId)
        {
            NodeControl nc = null;
            foreach (NodeControl node in m_nodes)
            {

                if (node.Entry.Strings.ReadSafe("VisualId") == vId.ToHexString())
                {
                    node.Entry.Strings.Set("VisualId", new ProtectedString());
                    node.upForDelete = true;
     
                    foreach (var observer in m_observers)
                    {
                        observer.OnNext(node);
                    }
                    nc = node;

                }
            }
            m_nodes.Remove(nc);
        }

        /// <summary>Updates the node for its visual representation to change. Therefore just send it to the view, it will update all incoming nodes</summary>
        /// <param name="vId">The v identifier of the node to be updated</param>
        internal void UpdateNode(PwUuid vId)
        {
            foreach (var node in m_nodes)
            {
                
                if (node.Entry.Strings.ReadSafe("VisualId") == vId.ToHexString())
                {
         
                    foreach (var observer in m_observers)
                    {
                        observer.OnNext(node);
                    }

                }
            }
        }


        /// <summary>Finds a node by using its VisualId.</summary>
        /// <param name="vid">The VisualId to search with.</param>
        private NodeControl FindNodeByVid(string vid)
        {
            foreach (var node in m_nodes)
            {
                if (node.VisualIdString == vid) return node;
            }
            return null;
        }

        /// <summary>Updates the nodes with information reguarding all other nodes it shares a characteristc with.</summary>
        /// <param name="connections">A List of Lists. Each List inside the outer one contains VisualIds of Nodes that share a characteristic</param>
        /// <param name="image">The type of characteristic that is shared.</param>
        internal void UpdateNodesForLinks(List<List<string>> connections, ImageEnum image)
        {
            foreach(var node in m_nodes)
            {
                if (image == ImageEnum.Identity) { node.ConId.Clear(); }
                if (image == ImageEnum.Phone) { node.conPhone.Clear(); }
                if (image == ImageEnum.Mail) { node.ConMail.Clear(); }
                
            }
            string s = "";
            foreach (var list in connections)
            {
                foreach (var id in list)
                {
                    s += (id == null) + "";
                    s += id + "ö" + "\n";
                
                    if (id != null)
                    {
                        try
                        {
                            if (image == ImageEnum.Identity) { FindNodeByVid(id).ConId = list.Where(x => x != id).ToList(); }
                            if (image == ImageEnum.Phone) { FindNodeByVid(id).conPhone = list.Where(x => x != id).ToList(); }
                            if (image == ImageEnum.Mail) { FindNodeByVid(id).ConMail = list.Where(x => x != id).ToList();  }
           
                            UpdateNode(new PwUuid(MemUtil.HexStringToByteArray(id)));
                        }
                        catch { Debug.Assert(false); }
                    }
                }
            }
        }





        /// <summary>Add node to the dashboard. Called at the start of the dashboard. Diffrence to other methods is, that a visualId is already present</summary>
        /// <param name="title">The title.</param>
        /// <param name="pos">The position.</param>
        /// <param name="visualId">The visual identifier.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="db">The database.</param>
        internal void AddNodesFormLoaded(string title, Point pos, PwUuid visualId, PwEntry entry, PwDatabase db)
        {
            NodeControl node = new NodeControl(title, visualId, pos,  db, entry );
            m_nodes.Add(node);
            foreach (var observer in m_observers)
            {
                observer.OnNext(node);
            }


        }

   
    }
}