using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using DrawingBoard;

namespace ProjectLibrary
{
    /// <summary>
    /// Drawingboard with alternative behavior that allows scribbling only
    /// </summary>
    public class DrawingBoard_ConstantPen : DrawingBoard.Controls.DrawingBoard
    {
        public DrawingBoard_ConstantPen()
            : base()
        {
            base.MouseDown -= base.DrawingBoard_MouseDown;
            base.MouseUp -= base.DrawingBoard_MouseUp;
            this.MouseDown += this.DrawingBoard_ConstantPen_MouseDown;
            this.MouseUp += this.DrawingBoard_ConstantPen_MouseUp;
        }

        public void DrawingBoard_ConstantPen_MouseDown(object sender, MouseEventArgs e)
        {
            base.Option = DrawingOption.Pen;
            base.DrawingBoard_MouseDown(sender, e);
        }

        public void DrawingBoard_ConstantPen_MouseUp(object sender, MouseEventArgs e)
        {
            base.DrawingBoard_MouseUp(sender, e);
            base.shapeManager.DeSelect();
            base.ReDraw(true);
        }

        protected override void OptionToNotRedrawInChild()
        {
            //we don't want to redraw in our custom drawing board
        }
    }
}
