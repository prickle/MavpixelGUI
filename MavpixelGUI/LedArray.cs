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

namespace MavpixelGUI
{
    public partial class LedArray : UserControl
    {
        //Double buffering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        LedControl[] leds = new LedControl[256];
        public LedControl[] Leds { get { return leds; } }

        bool orderMode = false;
        public bool OrderMode
        {
            get { return orderMode; }
            set
            {
                orderMode = value;
                setOrderMode();
            }
        }

        //Click location and index
        int xLoc = 0;
        int yLoc = 0;
        int ledIndex = 0;
        //Selection rectangle
        int xStart = 0;
        int yStart = 0;
        int yEnd = 0;
        int xEnd = 0;
        //Selection size
        int xSpread = 0;
        int ySpread = 0;
        //Mouse state
        bool isDragging = false;

        public LedArray()
        {
            InitializeComponent();
            foreach (Control c in tableLayoutPanel1.Controls)
                if (c is LedControl)
                {
                    LedControl led = (LedControl)c;
                    int index = int.Parse((string)(led.Tag));
                    leds[index] = led;
                    led.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ledControl_MouseMove);
                    led.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ledControl_MouseDown);
                    led.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ledControl_MouseUp);
                }
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void setOrderMode()
        {
            if (orderMode) this.BackColor = Color.LightGreen;
            else this.BackColor = Color.Transparent;
            this.Invalidate();
        }

        //Selection event
        public delegate void SelectedEventHandler(object sender, SelectedEventArgs e);
        public event SelectedEventHandler Selected;
        public void OnSelected()
        {
            if (Selected != null)
                Selected(this, new SelectedEventArgs(selectedLeds, xStart, yStart, xEnd, yEnd));
        }

        //RemainingChanged event
        public delegate void RemainingChangedEventHandler(object sender, RemainingChangedEventArgs e);
        public event RemainingChangedEventHandler RemainingChanged;
        public void OnRemainingChanged(int Remaining)
        {
            if (RemainingChanged != null)
                RemainingChanged(this, new RemainingChangedEventArgs(Remaining));
        }

        private void ledControl_MouseDown(object sender, MouseEventArgs e)
        {
            LedControl led = (LedControl)sender;
            ledIndex = int.Parse((string)(led.Tag));
            led.Highlighted = true;
            isDragging = true;
            xLoc = xStart = xEnd = (int)(ledIndex % 16);
            yLoc = yStart = yEnd = (int)(ledIndex / 16);
            clearSelections();
        }

        int getNextIndex()
        {
            int next;
            bool[] indexes = new bool[33];
            foreach (LedControl led in leds)
                if (led.Index >= 0 && led.Index < 33)
                    indexes[led.Index] = true;
            for (next = 0; indexes[next] == true; next++) ;
            if (next == 32) next = -1;
            return next;
        }

       public int CountRemaining()
        {
            int count = 0;
            foreach (LedControl led in leds)
                if (led.Index >= 0 && led.Index < 33)
                    count++;
            return 32 - count;
        }

        int wasRemaining = 0;
        private void ledControl_MouseUp(object sender, MouseEventArgs e)
        {
            clearHighlights(true);
            isDragging = false;
            xSpread = 0;
            ySpread = 0;
            if (orderMode)
            {
                foreach (LedControl led in selectedLeds)
                {
                    if (led.Index < 0)
                    {
                        int next = getNextIndex();
                        if (next >= 0)
                        {
                            led.Index = next;
                            led.Invalidate();
                        }
                    }
                }
                UpdateRemaining();
            }
            OnSelected();
        }

        public void UpdateRemaining()
        {
            int remain = CountRemaining();
            if (remain != wasRemaining)
            {
                wasRemaining = remain;
                OnRemainingChanged(remain);
            }
        }

        private void clearHighlights(bool select)
        {
            foreach (LedControl led in leds)
                if (led.Highlighted)
                {
                    led.Highlighted = false;
                    if (select)
                    {
                        led.Selected = true;
                        selectedLeds.Add(led);
                    }
                }
        }

        private void clearSelections()
        {
            selectedLeds.Clear();
            foreach (LedControl led in leds)
                led.Selected = false;
        }

        List<LedControl> selectedLeds = new List<LedControl>();

        private void ledControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                LedControl led = (LedControl)sender;
                int xExtent = (int)e.X / led.Width;
                int yExtent = (int)e.Y / led.Height;
                if (e.X < 0) xExtent -= 1;
                if (e.Y < 0) yExtent -= 1;
                if (xExtent != xSpread || yExtent != ySpread)
                {
                    xSpread = xExtent;
                    ySpread = yExtent;
                    xStart = xLoc;
                    yStart = yLoc;
                    xEnd = xStart + xSpread;
                    if (xEnd < 0) xEnd = 0;
                    if (xEnd > 15) xEnd = 15;
                    if (xEnd < xStart) { xStart = xEnd; xEnd = xLoc; }
                    yEnd = yStart + ySpread;
                    if (yEnd < 0) yEnd = 0;
                    if (yEnd > 15) yEnd = 15;
                    if (yEnd < yStart) { yStart = yEnd; yEnd = yLoc; }
                    for (int x = 0; x < 16; x++)
                        for (int y = 0; y < 16; y++)
                        {
                            int ledAddress = y * 16 + x;
                            if (x < xStart || x > xEnd || y < yStart || y > yEnd)
                                leds[ledAddress].Highlighted = false;
                            else
                                leds[ledAddress].Highlighted = true;
                        }

                }
            }
        }

        public void Clear()
        {
            foreach (LedControl led in leds)
            {
                led.Mode.Set(0);
                led.Direction.Set(0);
                led.ColorIndex = 0;
                //led.Invalidate();
            }
        }

        internal void ClearWiring()
        {
            foreach (LedControl led in leds)
                if (led.Index >= 0)
                {
                    led.Index = -1;
                    //led.Invalidate();
                }
        }
    }

    //Outside LedArray class

    public class SelectedEventArgs : EventArgs
    {
        public List<LedControl> SelectedLeds;
        public int Xstart, Ystart, Xend, Yend;
        public SelectedEventArgs(List<LedControl> selectedLeds, int xStart, int yStart, int xEnd, int yEnd)
        {
            SelectedLeds = selectedLeds;
            Xstart = xStart; Xend = xEnd; Ystart = yStart; Yend = yEnd;
        }
    }
    
    public class RemainingChangedEventArgs : EventArgs
    {
        public int Remaining;
        public RemainingChangedEventArgs(int remaining)
        {
            Remaining = remaining;
        }
    }


}
