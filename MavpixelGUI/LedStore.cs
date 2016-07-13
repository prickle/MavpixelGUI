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
using System.Linq;
using System.Text;
using System.Drawing;

namespace MavpixelGUI
{
    //LED storage
    //This exists as an array of up to 32 LEDs as reported by Mavpixel
    // Comparison operations are used to determine changes
    class LedStore
    {
        public LedControl.Modes mode;
        public LedControl.Directions direction;
        public int colorIndex;
        public int address;
        
        public LedStore()
        {
            mode = new LedControl.Modes(0);
            direction = new LedControl.Directions(0);
            colorIndex = 0;
            address = 0;
        }

        public LedStore(LedControl led, int address)
        {
            mode = new LedControl.Modes(led.Mode.Get());
            direction = new LedControl.Directions(led.Direction.Get());
            colorIndex = led.ColorIndex;
            this.address = address;
        }

        public bool Compare(LedControl led, int address)
        {
            if (led.ColorIndex != colorIndex) return false;
            if (this.address != address) return false;
            if (led.Mode.Get() != mode.Get()) return false;
            if (led.Direction.Get() != direction.Get()) return false;
            return true;
        }

        public bool isEmpty()
        {
            if (colorIndex > 0) return false;
            if (address > 0) return false;
            if (mode.Get() > 0) return false;
            if (direction.Get() > 0) return false;
            return true;
        }

        public void update(LedControl led, int address)
        {
            mode.Set(led.Mode.Get());
            direction.Set(led.Direction.Get());
            colorIndex = led.ColorIndex;
            this.address = address;
        }

        internal void nullOut()
        {
            mode.Set(0);
            direction.Set(0);
            colorIndex = 0;
            this.address = 0;
        }
    }

    //Mode colour storage
    //This exists as an array of current user selections
    // A per-mode 'changed' flag array is maintained and used to determine changes 
    class ModeColorStore
    {
        public bool fetched;
        public bool[] changed;
        int[] colors;

        public ModeColorStore()
        {
            fetched = false;
            changed = Enumerable.Repeat<bool>(false, 6).ToArray();
            colors = new int[6];
        }

        public ModeColorStore(int[] colors)
        {
            fetched = true;
            changed = Enumerable.Repeat<bool>(false, 6).ToArray();
            Array.Copy(colors, this.colors, 6);
        }

        public void set(int index, int color) {
            if (colors[index] != color)
            {
                changed[index] = true;
                colors[index] = color;
            }
        }

        public void load(int[] colors)
        {
            Array.Copy(colors, this.colors, 6);
            fetched = true;
            changed = Enumerable.Repeat<bool>(false, 6).ToArray();
        }

        public void setAll(int[] colors)
        {
            for (int i = 0; i < 6; i++)
                set(i, colors[i]);
        }

        public int get(int index)
        {
            return colors[index];
        }

        public int countChanged() {
            return changed.Count(c => c);
        }

        public bool isChanged()
        {
            return countChanged() > 0;
        }

        public void update(bool changed)
        {
            this.changed = Enumerable.Repeat<bool>(changed, 6).ToArray();
        }

        public void update(bool changed, bool fetched)
        {
            this.changed = Enumerable.Repeat<bool>(changed, 6).ToArray();
            this.fetched = fetched;
        }
    }

    //Parameter storage
    //This exists as a single instance containing current user selections
    // A single 'changed' flag array is maintained and used to determine changes 
    class ParamStore
    {
        public int Length;
        public bool[] fetched;
        public bool[] changed;
        float[] parms;

        public ParamStore(int size)
        {
            changed = Enumerable.Repeat<bool>(false, size).ToArray();
            fetched = Enumerable.Repeat<bool>(false, size).ToArray();
            parms = new float[size];
            Length = size;
        }

        public void set(int param, float value)
        {
            if (parms[param] != value)
            {
                changed[param] = true;
                parms[param] = value;
            }
        }

        public void load(int param, float value)
        {
            parms[param] = value;
            changed[param] = false;
            fetched[param] = true;
        }

        public float get(int param)
        {
            return parms[param];
        }

        public void update(bool changed)
        {
            this.changed = Enumerable.Repeat<bool>(changed, Length).ToArray();
        }

        public void update(bool changed, bool fetched)
        {
            this.changed = Enumerable.Repeat<bool>(changed, Length).ToArray();
            this.fetched = Enumerable.Repeat<bool>(fetched, Length).ToArray();
        }
    }

    //HSV Colour class, used by palette
    public class HsvColor
    {
        public int hue;
        public int sat;
        public int val;
        public HsvColor()
        {
            hue = sat = val = 0;
        }
        public HsvColor(int hue, int sat, int val)
        {
            Set(hue, sat, val);
        }
        public HsvColor(Color color)
        {
            ColorToHSV(color, out hue, out sat, out val);
        }
        public Color rgb()
        {
            return ColorFromHSV(hue, sat, val);
        }
        public void Set(int hue, int sat, int val)
        {
            this.hue = hue;
            this.sat = sat;
            this.val = val;
        }
        public void Set(HsvColor hsv)
        {
            this.hue = hsv.hue;
            this.sat = hsv.sat;
            this.val = hsv.val;
        }
        public bool compare(HsvColor hsv)
        {
            if (hue != hsv.hue) return false;
            if (sat != hsv.sat) return false;
            if (val != hsv.val) return false;
            return true;
        }

        //Colorspace conversions, used by palette
        public static void ColorToHSV(Color color, out int hue, out int saturation, out int value)
        {
            int min, max, delta;

            if (color.R < color.G) min = color.R; else min = color.G;
            if (color.B < min) min = color.B;
            if (color.R > color.G) max = color.R; else max = color.G;
            if (color.B > max) max = color.B;
            value = max;                // v, 0..255
            delta = max - min;                      // 0..255, < v
            if (max != 0) saturation = (int)(255 - ((delta) * 255 / max));        // s, 0..255
            else
            {// r = g = b = 0        // s = 0, v is undefined
                saturation = 0;
                hue = 0;
                return;
            }
            if (delta == 0) hue = 0;
            else if (color.R == max) hue = (color.G - color.B) * 60 / delta;        // between yellow & magenta
            else if (color.G == max) hue = 120 + (color.B - color.R) * 60 / delta;    // between cyan & yellow
            else hue = 240 + (color.R - color.G) * 60 / delta;    // between magenta & cyan
            if (hue < 0) hue += 360;
        }

        // This algorithm is from Cleanflight, same as Mavpixel uses.
        public static Color ColorFromHSV(int hue, int saturation, int value)
        {
            int val = value;
            int sat = 255 - saturation;
            int basic;
            int r, g, b;
            if (sat == 0)
            { // Acromatic color (gray). Hue doesn't mind.
                r = val;
                g = val;
                b = val;
            }
            else
            {
                basic = ((255 - sat) * val) >> 8;
                switch (hue / 60)
                {
                    case 0:
                        r = val;
                        g = (((val - basic) * hue) / 60) + basic;
                        b = basic;
                        break;
                    case 1:
                        r = (((val - basic) * (60 - (hue % 60))) / 60) + basic;
                        g = val;
                        b = basic;
                        break;
                    case 2:
                        r = basic;
                        g = val;
                        b = (((val - basic) * (hue % 60)) / 60) + basic;
                        break;
                    case 3:
                        r = basic;
                        g = (((val - basic) * (60 - (hue % 60))) / 60) + basic;
                        b = val;
                        break;
                    case 4:
                        r = (((val - basic) * (hue % 60)) / 60) + basic;
                        g = basic;
                        b = val;
                        break;
                    default:
                        r = val;
                        g = basic;
                        b = (((val - basic) * (60 - (hue % 60))) / 60) + basic;
                        break;
                }
            }
            return Color.FromArgb(255, r, g, b);
        }
    }

    //Colour palette storage
    //This exists as an array containing current user selections
    // A per-colour 'changed' flag is maintained and used to determine changes 
    class ColorStore
    {
        public bool fetched;
        public bool changed;
        public HsvColor hsv;

        public ColorStore(HsvColor hsv, bool fetched)
        {
            changed = false;
            this.fetched = fetched;
            this.hsv = hsv;
        }

        public ColorStore(int hue, int sat, int val, bool fetched)
        {
            changed = false;
            this.fetched = fetched;
            hsv = new HsvColor(hue, sat, val);
        }

        public ColorStore()
            : this(0, 255, 255, false)
        { }

        public bool compare(HsvColor hsv)
        {
            return this.hsv.compare(hsv);
        }

        public void set(HsvColor hsv)
        {
            this.hsv.Set(hsv);
        }

        public HsvColor get()
        {
            return hsv;
        }

    }
}
