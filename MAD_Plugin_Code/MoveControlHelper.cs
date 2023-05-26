using System;
using System.Drawing;
using System.Windows.Forms;
using static MAD_Plugin.NodeControl;

namespace MAD_Plugin
{
    class MoveControlHelper
    {
        /*
       ***************************************************************************************
       * Original class was taken from this source and then customized for the dashboard
       * Title: Move Controls With The Mouse On A Form At Runtime
       * Author: Vrushali Ghodke
       * Date: 16.12.2019
       * Availability: https://www.c-sharpcorner.com/article/move-controls-with-the-mouse-on-a-form-at-runtime/
       *
       ***************************************************************************************
       */

        /// <summary>
        ///  the directions a control can move
        /// </summary>
        public enum DirectionEnum
        {
            Any,
            Horizontal,
            Vertical
        }
        public int counter = 0;

        private MAD_Form m_view = null;

        /// <summary>Initializes a new instance of the <see cref="MoveControlHelper" /> class.</summary>
        /// <param name="view">The view that needs help to move controls.</param>
        public MoveControlHelper(MAD_Form view)
        {
            this.m_view = view;
        }




        /// <summary>Initializes the specified control to be able to move.</summary>
        /// <param name="control">The control.</param>
        public void Init(Control control)
        {
            Init(control, DirectionEnum.Any);
        }

        /// <summary>Initializes the specified control to be able to move.</summary>
        /// <param name="control">The control.</param>
        /// <param name="direction">The direction in which it can be moved.</param>
        public void Init(Control control, DirectionEnum direction)
        {
            Init(control, control, direction);
        }

        /// <summary>Initializes the specified control to be able to move. With this version it is possible to move a add the responible eventhandlers to one control and move another one in its place</summary>
        /// <param name="control">The control being selected by the mouse.</param>
        /// <param name="container">The container of to be moved.</param>
        /// <param name="direction">The direction to move.</param>
        public void Init(Control control, Control container, DirectionEnum direction)
        {
            bool Dragging = false;
            Point DragStart = Point.Empty;
            control.MouseDown += delegate (object sender, MouseEventArgs e) //using delegation to encapsulate the following calls and add a reference to them to the MouseDown event
            {
                m_view.IsMovingNodes = true;
                Dragging = true;
                DragStart = new Point(e.X, e.Y);
                m_view.Invalidate(false);
                m_view.DisposeIageList(ImageEnum.Identity, true);
            };
            control.MouseUp += delegate (object sender, MouseEventArgs e) //using delegation to encapsulate the following calls and add a reference to them to the MouseDown event
            {
                m_view.IsMovingNodes = false;
                Dragging = false;
                m_view.UpdateLines();
            };
            control.MouseMove += delegate (object sender, MouseEventArgs e) //using delegation to encapsulate the following calls and add a reference to them to the MouseDown event
            {
                if (Dragging)
                {
                    if (direction != DirectionEnum.Vertical) { container.Left = Math.Min(Math.Max(0, e.X + container.Left - DragStart.X), m_view.Width - container.Width); }
                    if (direction != DirectionEnum.Horizontal) { container.Top = Math.Min(Math.Max(25, e.Y + container.Top - DragStart.Y), m_view.Height - container.Height - 25); }
                }
            };
        }
    }
}