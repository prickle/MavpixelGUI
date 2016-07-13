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
using System.Windows.Forms;
using System.Globalization;

namespace MavpixelGUI
{
    //From http://technet.microsoft.com/en-US/LIBRary/ms229644(v=vs.90)
    public class NumericTextBox : TextBox
    {
        bool allowSpace = false;
        string term = "";

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (term != "")
            {
                skipTextChanged = true;
                Text = term;
                skipTextChanged = false;
            }
        }

        // Restricts the entry of characters to digits (including hex), the negative sign, 
        // the decimal point, and editing keystrokes (backspace). 
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            // Workaround for groupSeparator equal to non-breaking space 
            if (groupSeparator == ((char)160).ToString())
            {
                groupSeparator = " ";
            }

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
             keyInput.Equals(negativeSign))
            {
                // Decimal separator is OK
            }
            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0) 
            //    { 
            //     // Let the edit control handle control and alt key combinations 
            //    } 
            else if (this.allowSpace && e.KeyChar == ' ')
            {

            }
            else
            {
                // Consume this invalid key and beep
                e.Handled = true;
                //    MessageBeep();
            }
        }

        bool skipTextChanged = false;
        protected override void OnTextChanged(EventArgs e)
        {
            if (skipTextChanged) return;
            base.OnTextChanged(e);
            if (term != "")
            {
                skipTextChanged = true;
                int cursor = SelectionStart;
                Text = Text.Replace(term, "");
                Text = Text + term;
                if (cursor > Text.Length - term.Length)
                    cursor = Text.Length - term.Length;
                SelectionStart = cursor;
                skipTextChanged = false;
            }
        }

        private string strip(string Text, string term)
        {
            if (term == "") return Text;
            string newText = string.Copy(Text);
            return newText.Replace(term, "");
        }

        public int IntValue
        {
            get
            {
                return Int32.Parse(strip(Text, term));
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(strip(Text, term));
            }
        }

        public string PlainText
        {
            get
            {
                return strip(Text, term);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }

        public string Terminator
        {
            set
            {
                this.term = value;
            }

            get
            {
                return this.term;
            }
        }
    }
}
