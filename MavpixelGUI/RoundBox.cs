/*
 * MavpixelGUI - Front end for Mavpixel Mavlink Neopixel bridge
 * (c) 2016 Nick Metcalfe
 *
 * Mavpixel is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Please read licensing, redistribution, modifying, authors and 
 * version numbering from main sketch file. This file contains only
 * a minimal header.
 *
 * MavpixelGUI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Mavpixel.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plasmoid.Extensions;
using System.ComponentModel.Design;

namespace MavpixelGUI
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class RoundBox : UserControl
    {
        Color border = Color.Black;
        public Color BorderColor
        {
            get { return border; }
            set
            {
                border = value;
                this.Invalidate();
            }
        }
        Color fill = Color.White;
        public Color FillColor
        {
            get { return fill; }
            set
            {
                fill = value;
                this.Invalidate();
            }
        }
        public RoundBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(pe);
            //pe.Graphics.CompositingMode = CompositingMode.SourceOver;
            // Declare and instantiate a new pen.
            Pen myPen = new Pen(border);
            Brush myBrush = new SolidBrush(fill);
            pe.Graphics.FillRoundedRectangle(myBrush, 0, 0, this.Width - 1, this.Height - 1, 4);
            pe.Graphics.DrawRoundedRectangle(myPen, 0, 0, this.Width - 1, this.Height - 1, 4);
        }

        private void RoundBox_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }


    }
}
