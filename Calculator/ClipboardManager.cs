using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calculator
{
    public class ClipboardManager
    {
        private string storedText = "";

        public void Copy(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                storedText = text;
                Clipboard.SetText(storedText);
            }
        }

        public void Cut(ref string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                storedText = text;
                Clipboard.SetText(storedText);
                text = ""; 
            }
        }

        public string Paste()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            return "";
        }
    }

}
