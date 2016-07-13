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
using System.Drawing.Drawing2D;
using Plasmoid.Extensions;
using System.Globalization;

namespace MavpixelGUI
{
    public partial class LedControl : UserControl
    {
        bool focus = false;

        Pen blackPen;
        Pen greyPen;
        Pen fadePen1;
        Pen fadePen2;
        Pen fadePen3;
        Brush fadeBrush;
        LinearGradientBrush rainbowBrush;
        Brush shadeBrush;
        Brush warningBrush;
        Brush modeBrush;
        Brush indicatorBrush;
        Brush armBrush;
        Brush throttleBrush;
        Brush ringBrush;
        Brush gpsBrush;
        Brush arrowBrush;
        Brush inactiveBrush;
        Brush disabledBrush;
        Font indexFont;
        Font smallFont;

        int index = -1;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Invalidate();
            }
        }

        bool highlighted;
        public bool Highlighted
        {
            get { return highlighted; }
            set
            {
                if (highlighted != value)
                {
                    highlighted = value;
                    this.Invalidate();
                }
            }
        }

        bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    this.Invalidate();
                }
            }
        }

        Modes mode = new Modes(0);
        public Modes Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                this.Invalidate();
            }
        }

        Directions direction = new Directions(0);
        public Directions Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.Invalidate();
            }
        }

        int colorIndex = 0;
        public int ColorIndex
        {
            get { return colorIndex; }
            set { colorIndex = value; }
        }
        
        BorderLines borderLine = new BorderLines(0);
        public BorderLines BorderLine
        {
            get { return borderLine; }
            set
            {
                borderLine = value;
                this.Invalidate();
            }
        }


        public LedControl()
        {
            prepareDrawingObjects();
            InitializeComponent();
        }

        void prepareDrawingObjects()
        {
            indexFont = new Font("Segoe UI", 9);
            smallFont = new Font("Segoe UI", 7);
            blackPen = new Pen(Color.Black);
            greyPen = new Pen(Color.Gray);
            Color selected = Color.DeepPink;
            fadePen1 = new Pen(Color.FromArgb(150, selected.R, selected.G, selected.B), 2);
            fadePen2 = new Pen(Color.FromArgb(100, selected.R, selected.G, selected.B), 2);
            fadePen3 = new Pen(Color.FromArgb(50, selected.R, selected.G, selected.B), 2);
            fadeBrush = new SolidBrush(Color.FromArgb(100, selected.R, selected.G, selected.B));
            shadeBrush = new SolidBrush(Color.FromArgb(96, 0, 0, 0));
            warningBrush = new SolidBrush(Color.Red);
            modeBrush = new SolidBrush(Color.LimeGreen);
            indicatorBrush = new SolidBrush(Color.Yellow);
            armBrush = new SolidBrush(Color.Blue);
            throttleBrush = new SolidBrush(Color.Orange);
            ringBrush = new SolidBrush(Color.Black);
            gpsBrush = new SolidBrush(Color.Green);
            arrowBrush = new SolidBrush(Color.Black);
            inactiveBrush = new SolidBrush(Color.LightGray);
            disabledBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
            rainbowBrush = new LinearGradientBrush(
                new Rectangle(0, 0, 20, 20), Color.Blue, Color.Violet, (float)45);
            //Fix brush colours
            int brightness = 200;
            int washout = 64;
            Color[] colors = new Color[]
            {
                Color.FromArgb(brightness, washout, washout),
                Color.FromArgb(brightness, brightness, washout),
                Color.FromArgb(washout, brightness, washout),
                Color.FromArgb(washout, brightness, brightness),
                Color.FromArgb(washout, washout, brightness),
                Color.FromArgb(brightness, washout, brightness),
                Color.FromArgb(brightness, washout, washout),
            };
            int num_colors = colors.Length;
            float[] blend_positions = new float[num_colors];
            for (int i = 0; i < num_colors; i++)
            {
                blend_positions[i] = i / (num_colors - 1f);
            }

            ColorBlend color_blend = new ColorBlend();
            color_blend.Colors = colors;
            color_blend.Positions = blend_positions;
            rainbowBrush.InterpolationColors = color_blend;
        }

        void twoColor(Graphics g, Brush c1, Brush c2, int x, int y, int w, int h, int r)
        {
            int div = (int)(h / 2);
            g.SetClip(new Rectangle(x, y, w, div));
            g.FillRoundedRectangle(c1, x, y, w - 1, h - 1, r);
            g.SetClip(new Rectangle(x, y + div, w, div + 1));
            g.FillRoundedRectangle(c2, x, y, w - 1, h - 1, r);
            g.SetClip(new Rectangle(0, 0, this.Width, this.Height));
        }

        void threeColor(Graphics g, Brush c1, Brush c2, Brush c3, int x, int y, int w, int h, int r)
        {
            int div1 = (int)(h / 3);
            int div2 = (int)(div1 * 2);
            g.SetClip(new Rectangle(x, y, w, div1));
            g.FillRoundedRectangle(c1, x, y, w - 1, h - 1, r);
            g.SetClip(new Rectangle(x, y + div1, w, div1 + 1));
            g.FillRoundedRectangle(c2, x, y, w - 1, h - 1, r);
            g.SetClip(new Rectangle(x, y + div2, w, div1 + 1));
            g.FillRoundedRectangle(c3, x, y, w - 1, h - 1, r);
            g.SetClip(new Rectangle(0, 0, this.Width, this.Height));
        }
        
        bool fillButton(Graphics g, int x, int y, int w, int h, int r)
        {
            if (mode.WARNING && mode.MODE && mode.THROTTLE)
            {   //orange, lime, red
                threeColor(g, throttleBrush, modeBrush, warningBrush, x, y, w, h, r);
            }
            else if (mode.WARNING && mode.MODE)
            {   //lime, red
                twoColor(g, modeBrush, warningBrush, x, y, w, h, r);
            }
            else if (mode.WARNING && mode.ARM && mode.INDICATOR)
            {   //yellow, red, blue
                threeColor(g, indicatorBrush, warningBrush, armBrush, x, y, w, h, r);
            }
            else if (mode.WARNING && mode.INDICATOR)
            {   //yellow, red
                twoColor(g, indicatorBrush, warningBrush, x, y, w, h, r);
            }
            else if (mode.MODE && mode.INDICATOR && mode.ARM)
            {   //yellow, lime, blue
                threeColor(g, indicatorBrush, modeBrush, armBrush, x, y, w, h, r);
            }
            else if (mode.MODE && mode.THROTTLE)
            {   //lime, orange
                twoColor(g, modeBrush, throttleBrush, x, y, w, h, r);
            }
            else if (mode.INDICATOR && mode.ARM)
            {   //yellow, blue
                twoColor(g, indicatorBrush, armBrush, x, y, w, h, r);
            }
            else if (mode.RING)
            {   //Ring
                g.FillRoundedRectangle(ringBrush, x, y, w - 1, h - 1, r);
                g.DrawRoundedRectangle(Pens.White, x + 1, y + 1, w - 3, h - 3, (w - 3) / 2);
            }
            else if (mode.COLOR)
            {
                g.FillRoundedRectangle(rainbowBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.THROTTLE)
            {   //Orange 
                g.FillRoundedRectangle(throttleBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.ARM)
            {   //Blue
                g.FillRoundedRectangle(armBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.WARNING)
            {   //Red
                g.FillRoundedRectangle(warningBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.MODE)
            {   //Green
                g.FillRoundedRectangle(modeBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.INDICATOR)
            {   //Yellow
                g.FillRoundedRectangle(indicatorBrush, x, y, w - 1, h - 1, r);
            }
            else if (mode.GPS)
            {   //Green
                g.FillRoundedRectangle(gpsBrush, x, y, w - 1, h - 1, r);
            }
            else
            {
                g.FillRoundedRectangle(inactiveBrush, x, y, w - 1, h - 1, 5);
                return false;
            }
            if (!focus && !selected && !highlighted)
                g.FillRoundedRectangle(shadeBrush, x, y, w - 1, h - 1, 5);
            return true;
        }

        int border = 4;

        void arrows(Graphics g, int x, int y, int w, int h)
        {
            int side = (int)(border / 2f);
            if (direction.NORTH)
            {
                int mid = (int)((w / 2f) + x);
                Point[] p = { new Point(mid, y - border - 1), new Point(mid - border - 2, y + 1), new Point(mid + border + 2, y + 1) };
                g.FillPolygon(arrowBrush, p); 
            }
            if (direction.SOUTH)
            {
                int mid = (int)((w / 2f) + x);
                Point[] p = { new Point(mid, y + h + border + 1), new Point(mid - border - 1, y + h - 1), new Point(mid + border + 1, y + h - 1) };
                g.FillPolygon(arrowBrush, p);
            }
            if (direction.WEST)
            {
                int mid = (int)((h / 2f) + y);
                Point[] p = { new Point(x - border - 1, mid), new Point(x + 1, mid - border - 2), new Point(x + 1, mid + border + 1) };
                g.FillPolygon(arrowBrush, p);
            }
            if (direction.EAST)
            {
                int mid = (int)((h / 2f) + y);
                Point[] p = { new Point(x + w + border + 1, mid), new Point(x + w - 1, mid - border - 1), new Point(x + w - 1, mid + border + 1) };
                g.FillPolygon(arrowBrush, p);
            }
            if (direction.UP)
            {
                SizeF textSize = g.MeasureString("U", indexFont);
                int xLoc = (int)((this.Size.Width / 2) - textSize.Width + 1);
                int yLoc = (int)((this.Size.Height / 2) - textSize.Height + 5);
                g.DrawString("U", smallFont, Brushes.Black, xLoc, yLoc);
            }
            if (direction.DOWN)
            {
                int xLoc = (int)(this.Size.Width / 2);
                int yLoc = (int)(this.Size.Height / 2) - 1;
                g.DrawString("D", smallFont, Brushes.Black, xLoc, yLoc);
            }
        }

        private void drawGrid(Graphics g, int x, int y, int w, int h)
        {
            if (borderLine.TOP) g.DrawLine(greyPen, x, y, x + w, y);
            if (borderLine.BOTTOM) g.DrawLine(greyPen, x, y + h, x + w, y + h);
            if (borderLine.LEFT) g.DrawLine(greyPen, x, y, x, y + h);
            if (borderLine.RIGHT) g.DrawLine(greyPen, x + w, y, x + w, y + h);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(pe);

            Pen myPen;
            if (focus || selected || highlighted) myPen = blackPen;
            else myPen = greyPen;
            Graphics g = pe.Graphics;
            int x = border;
            int y = border;
            int w = this.Size.Width - (border * 2);
            int h = this.Size.Height - (border * 2);
            bool active = fillButton(g, x, y, w - 1, h - 1, 5);
            // Draw border.
            g.DrawRoundedRectangle(myPen, x, y, w - 2, h - 2, 5);
            //Draw shadow
            if (focus || selected || highlighted)
            {
                if (highlighted)
                    g.FillRoundedRectangle(fadeBrush, x + 1, y + 1, w - 4, h - 4, 5);
                else
                {
                    g.DrawRoundedRectangle(fadePen1, x + 1, y + 1, w - 4, h - 4, 5);
                    g.DrawRoundedRectangle(fadePen2, x + 2, y + 2, w - 6, h - 6, 5);
                    g.DrawRoundedRectangle(fadePen3, x + 3, x + 3, w - 8, h - 8, 5);
                }
            }
            //Draw grid
            drawGrid(g, 0, 0, this.Width - 1, this.Height - 1);
            //Draw directions
            arrows(g, x, y, w - 1, h - 1);
            //Draw text
            if (index >=0 && index < 32)
            {
                SizeF textSize = g.MeasureString(index.ToString(), indexFont);
                int xLoc = (int)((this.Size.Width - textSize.Width) / 2);
                int yLoc = (int)((this.Size.Height - textSize.Height) / 2);
                g.DrawString(index.ToString(), indexFont, Brushes.Black, xLoc, yLoc);
                g.DrawString(index.ToString(), indexFont, Brushes.LightGray, xLoc - 1, yLoc - 1);
            }
            if (!Enabled) g.FillRectangle(disabledBrush, 0, 0, this.Width, this.Height);

        }

        private void LedControl_Enter(object sender, EventArgs e)
        {
            focus = true;
            this.Invalidate();
        }

        private void LedControl_Leave(object sender, EventArgs e)
        {
            focus = false;
            this.Invalidate();
        }

        //Border lines display class
        [TypeConverterAttribute(typeof(BorderLineConverter)),
        DescriptionAttribute("Expand to alter border lines.")]
        public class BorderLines
        {
            bool _top;
            [DescriptionAttribute("Line on top."),
            DefaultValueAttribute(false)]
            public bool TOP
            {
                get { return _top; }
                set { _top = value; }
            }
            bool _bottom;
            [DescriptionAttribute("Line underneath."),
            DefaultValueAttribute(false)]
            public bool BOTTOM
            {
                get { return _bottom; }
                set { _bottom = value; }
            }
            bool _left;
            [DescriptionAttribute("Line to left."),
            DefaultValueAttribute(false)]
            public bool LEFT
            {
                get { return _left; }
                set { _left = value; }
            }
            bool _right;
            [DescriptionAttribute("Line to right."),
            DefaultValueAttribute(false)]
            public bool RIGHT
            {
                get { return _right; }
                set { _right = value; }
            }

            public BorderLines()
            {
                TOP = BOTTOM = LEFT = RIGHT = false;
            }

            public void Set(int mask)
            {
                TOP = ((mask & 1) == 1);
                BOTTOM = ((mask & 2) == 2);
                LEFT = ((mask & 4) == 4);
                RIGHT = ((mask & 8) == 8);
            }

            public int Get()
            {
                return (TOP ? 1 : 0) + (BOTTOM ? 2 : 0) + (LEFT ? 4 : 0) + (RIGHT ? 8 : 0);
            }

            public BorderLines(int mask)
            {
                Set(mask);
            }

            public override string ToString()
            {
                if (Get() == 0) return "NONE";
                return (TOP ? "TOP " : "") + (BOTTOM ? "BOTTOM " : "") + (LEFT ? "LEFT " : "") + (RIGHT ? "RIGHT " : "");
            }

        }

        public class BorderLineConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context,
                                          System.Type destinationType)
            {
                if (destinationType == typeof(BorderLines)) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                                   CultureInfo culture,
                                   object value,
                                   System.Type destinationType)
            {
                if (destinationType == typeof(System.String) && value is BorderLines)
                {
                    BorderLines c = (BorderLines)value;
                    return c.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                  System.Type sourceType)
            {
                if (sourceType == typeof(string)) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                                  CultureInfo culture, object value)
            {
                if (value is string)
                {
                    try
                    {
                        string s = ((string)value).ToUpper();
                        int val = 0;
                        if (s.Contains("TOP")) val += 1;
                        if (s.Contains("BOTTOM")) val += 2;
                        if (s.Contains("LEFT")) val += 4;
                        if (s.Contains("RIGHT")) val += 8;
                        BorderLines c = new BorderLines(val);
                        return c;
                    }
                    catch
                    {
                        throw new ArgumentException(
                            "Can not convert '" + (string)value +
                                               "' to type BorderLines");
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        //Direction arrows display class
        [TypeConverterAttribute(typeof(DirectionConverter)),
        DescriptionAttribute("Expand to alter direction indicators.")]
        public class Directions
        {
            bool _north;
            [DescriptionAttribute("Arrow to North."),
            DefaultValueAttribute(false)]
            public bool NORTH
            {
                get { return _north; }
                set { _north = value; }
            }
            bool _south;
            [DescriptionAttribute("Arrow to south."),
            DefaultValueAttribute(false)]
            public bool SOUTH
            {
                get { return _south; }
                set { _south = value; }
            }
            bool _west;
            [DescriptionAttribute("Arrow to west."),
            DefaultValueAttribute(false)]
            public bool WEST
            {
                get { return _west; }
                set { _west = value; }
            }
            bool _east;
            [DescriptionAttribute("Arrow to east."),
            DefaultValueAttribute(false)]
            public bool EAST
            {
                get { return _east; }
                set { _east = value; }
            }
            bool _up;
            [DescriptionAttribute("Indicate upward."),
            DefaultValueAttribute(false)]
            public bool UP
            {
                get { return _up; }
                set { _up = value; }
            }
            bool _down;
            [DescriptionAttribute("Indicate downward."),
            DefaultValueAttribute(false)]
            public bool DOWN
            {
                get { return _down; }
                set { _down = value; }
            }

            public Directions()
            {
                NORTH = SOUTH = EAST = WEST = UP = DOWN = false;
            }

            public void Set(int mask)
            {
                NORTH = ((mask & 1) == 1);
                SOUTH = ((mask & 2) == 2);
                WEST = ((mask & 4) == 4);
                EAST = ((mask & 8) == 8);
                UP = ((mask & 16) == 16);
                DOWN = ((mask & 32) == 32);
            }

            public int Get()
            {
                return (NORTH ? 1 : 0) + (SOUTH ? 2 : 0) + (WEST ? 4 : 0) + (EAST ? 8 : 0) + (UP ? 16 : 0) + (DOWN ? 32 : 0);
            }

            public Directions(int mask)
            {
                Set(mask);
            }

            public override string ToString()
            {
                if (Get() == 0) return "NONE";
                return (NORTH ? "NORTH " : "") + (SOUTH ? "SOUTH " : "") + (WEST ? "WEST " : "") + (EAST ? "EAST " : "") + (UP ? "UP " : "") + (DOWN ? "DOWN " : "");
            }

        }

        public class DirectionConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context,
                                          System.Type destinationType)
            {
                if (destinationType == typeof(Directions)) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                                   CultureInfo culture,
                                   object value,
                                   System.Type destinationType)
            {
                if (destinationType == typeof(System.String) && value is Directions)
                {
                    Directions c = (Directions)value;
                    return c.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                  System.Type sourceType)
            {
                if (sourceType == typeof(string)) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                                  CultureInfo culture, object value)
            {
                if (value is string)
                {
                    try
                    {
                        string s = ((string)value).ToUpper();
                        int val = 0;
                        if (s.Contains("NORTH")) val += 1;
                        if (s.Contains("SOUTH")) val += 2;
                        if (s.Contains("WEST")) val += 4;
                        if (s.Contains("EAST")) val += 8;
                        if (s.Contains("UP")) val += 16;
                        if (s.Contains("DOWN")) val += 32;
                        Directions c = new Directions(val);
                        return c;
                    }
                    catch
                    {
                        throw new ArgumentException(
                            "Can not convert '" + (string)value +
                                               "' to type Directions");
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

/*
W - Warnings. Red
M - Flight mode & Orientation. LimeGreen
I - Indicator. Yellow
A - Arm state. Blue
T - Throttle. Orange
R - Ring thrust state. Black
C - Color. Rainbow
G - Gps.Green
*/
        //Mode colour display class
        [TypeConverterAttribute(typeof(ModeConverter)),
        DescriptionAttribute("Expand to alter mode colours.")]
        public class Modes
        {
            bool _warning;
            [DescriptionAttribute("Warning LED mode."),
            DefaultValueAttribute(false)]
            public bool WARNING
            {
                get { return _warning; }
                set { _warning = value; }
            }
            bool _mode;
            [DescriptionAttribute("Mode LED mode."),
            DefaultValueAttribute(false)]
            public bool MODE
            {
                get { return _mode; }
                set { _mode = value; }
            }
            bool _indicator;
            [DescriptionAttribute("Indicator LED mode."),
            DefaultValueAttribute(false)]
            public bool INDICATOR
            {
                get { return _indicator; }
                set { _indicator = value; }
            }
            bool _arm;
            [DescriptionAttribute("Armed LED mode."),
            DefaultValueAttribute(false)]
            public bool ARM
            {
                get { return _arm; }
                set { _arm = value; }
            }
            bool _throttle;
            [DescriptionAttribute("Throttle LED mode."),
            DefaultValueAttribute(false)]
            public bool THROTTLE
            {
                get { return _throttle; }
                set { _throttle = value; }
            }
            bool _ring;
            [DescriptionAttribute("Thrust ring LED mode"),
            DefaultValueAttribute(false)]
            public bool RING
            {
                get { return _ring; }
                set { _ring = value; }
            }
            bool _color;
            [DescriptionAttribute("Solid colour LED mode"),
            DefaultValueAttribute(false)]
            public bool COLOR
            {
                get { return _color; }
                set { _color = value; }
            }
            bool _gps;
            [DescriptionAttribute("GPS LED mode"),
            DefaultValueAttribute(false)]
            public bool GPS
            {
                get { return _gps; }
                set { _gps = value; }
            }


            public Modes()
            {
                WARNING = MODE = INDICATOR = ARM = THROTTLE = RING = COLOR = GPS = false;
            }

            public void Set(int mask)
            {
                WARNING = ((mask & 1) == 1);
                MODE = ((mask & 2) == 2);
                INDICATOR = ((mask & 4) == 4);
                ARM = ((mask & 8) == 8);
                THROTTLE = ((mask & 16) == 16);
                RING = ((mask & 32) == 32);
                COLOR = ((mask & 64) == 64);
                GPS = ((mask & 128) == 128);
            }

            public int Get()
            {
                return (WARNING ? 1 : 0) + (MODE ? 2 : 0) + (INDICATOR ? 4 : 0) + (ARM ? 8 : 0) + (THROTTLE ? 16 : 0) + (RING ? 32 : 0) + (COLOR ? 64 : 0) + (GPS ? 128 : 0);
            }

            public Modes(int mask)
            {
                Set(mask);
            }

            public override string ToString()
            {
                if (Get() == 0) return "NONE";
                return (WARNING ? "WARNING " : "") + (MODE ? "MODE " : "") + (INDICATOR ? "INDICATOR " : "") + (ARM ? "ARM " : "") + (THROTTLE ? "THROTTLE " : "") + (RING ? "RING " : "") + (COLOR ? "COLOR " : "") + (GPS ? "GPS " : "");
            }

        }

        public class ModeConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context,
                                          System.Type destinationType)
            {
                if (destinationType == typeof(Modes)) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                                   CultureInfo culture,
                                   object value,
                                   System.Type destinationType)
            {
                if (destinationType == typeof(System.String) && value is Modes)
                {
                    Modes c = (Modes)value;
                    return c.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                  System.Type sourceType)
            {
                if (sourceType == typeof(string)) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                                  CultureInfo culture, object value)
            {
                if (value is string)
                {
                    try
                    {
                        string s = ((string)value).ToUpper();
                        int val = 0;
                        if (s.Contains("WARNING")) val += 1;
                        if (s.Contains("MODE")) val += 2;
                        if (s.Contains("INDICATOR")) val += 4;
                        if (s.Contains("ARM")) val += 8;
                        if (s.Contains("THROTTLE")) val += 16;
                        if (s.Contains("RING")) val += 32;
                        if (s.Contains("COLOR")) val += 64;
                        if (s.Contains("GPS")) val += 128;
                        Modes c = new Modes(val);
                        return c;
                    }
                    catch
                    {
                        throw new ArgumentException(
                            "Can not convert '" + (string)value +
                                               "' to type Modes");
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
