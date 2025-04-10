using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calculator
{
    class Operations
    {
        private double _total;
        private double _lastValue;
        private string _lastOperator= string.Empty;
        private int _currentBase = 10;
        public double CurrentValue
        {
            get { return _total; }
            set { _total = value; }
        }

        public Operations()
        {
            _total = 0;
            _lastValue = 0;
            _lastOperator = string.Empty;
        }

        public void Reset()
        {
            _total = 0;
            _lastValue = 0;
            _lastOperator = string.Empty;
        }
        public double GetTotal()
        {
            return _total;
        }
        public void SetOperator(string operatorSymbol, double currentDisplayValue)
        {
            if (!string.IsNullOrEmpty(_lastOperator))
            {
                Equals(currentDisplayValue);
            }
            else
            {
                _total = currentDisplayValue;
            }

            _lastOperator = operatorSymbol;
        }

        public void ResetLastOperator()
        {
        _lastOperator = string.Empty;
        }
        public void Equals(double currentDisplayValue)
        {
            _lastValue = currentDisplayValue;
            switch (_lastOperator)
            {
                case "+":
                    _total = _total + _lastValue;
                    break;
                case "-":
                    _total = _total - _lastValue;
                    break;
                case "*":
                    _total = _total * _lastValue;
                    break;
                case "/":
                    if (_lastValue != 0)
                        _total = _total / _lastValue;
                    else
                        throw new DivideByZeroException("Nu poți împărți la 0.");
                    break;
                case "%":
                    _total = _total % _lastValue;
                    break;
            }
            _lastValue = 0;
            _lastOperator = string.Empty; 
        }

        public void UnaryOperator(string op, double currentDisplayValue)
        {
            _total = currentDisplayValue;
            switch (op)
            {
                case "Rad":
                    _total = Math.Sqrt(_total);
                    break;
                case "^2":
                    _total = Math.Pow(_total, 2);
                    break;
                case "+/-":
                    if (_total != 0) _total = -_total;
                    break;
                case "1/x":
                    if (_total == 0) throw new DivideByZeroException("Nu poți împărți la 0.");
                    else
                    {
                        _total = 1 / _total;
                    }
                    break;
            }
        }

        public double GetLastNumber(string displayText)
        {
            if (string.IsNullOrEmpty(displayText))
                return 0;

            displayText = displayText.Trim();

            string normalizedDisplayText = displayText.Replace(",", "");

            while (normalizedDisplayText.Length > 0 && !Char.IsDigit(normalizedDisplayText[normalizedDisplayText.Length - 1]))
            {
                normalizedDisplayText = normalizedDisplayText.Substring(0, normalizedDisplayText.Length - 1);
            }

            if (normalizedDisplayText.StartsWith("+") || normalizedDisplayText.StartsWith("-"))
            {
                Match match = Regex.Match(normalizedDisplayText, @"([-+]?\d*\.?\d+)");
                if (match.Success)
                    return double.Parse(match.Value);
            }
            else
            {
                Match match = Regex.Match(normalizedDisplayText, @"\d+(\.\d+)?$");
                if (match.Success)
                    return double.Parse(match.Value);
            }

            return 0;
        }


        public double GetFirstNumber(string displayText)
        {
            if (string.IsNullOrEmpty(displayText))
                return 0;

            displayText = displayText.Trim();

            if (displayText.Length > 0 && "+-*/%".Contains(displayText[displayText.Length - 1]))
            {
                displayText = displayText.Substring(0, displayText.Length - 1);
            }

            double currentDisplayValue = 0;
            if (double.TryParse(displayText, out currentDisplayValue))
            {
                return currentDisplayValue;
            }

            return 0;
        }

        //public string GetLastNumber(string displayText)
        //{
        //    if (string.IsNullOrEmpty(displayText))
        //        return "0";

        //    displayText = displayText.Trim();
        //    string normalizedDisplayText = displayText.Replace(",", "");

        //    Match match = Regex.Match(normalizedDisplayText, @"[\dA-F]+(\.\d+)?$"); 
        //    if (match.Success)
        //        return match.Value;

        //    return "0";
        //}

        //public string GetFirstNumber(string displayText)
        //{
        //    if (string.IsNullOrEmpty(displayText))
        //        return "0";

        //    displayText = displayText.Trim();

        //    if (displayText.Length > 0 && "+-*/%".Contains(displayText[displayText.Length - 1]))
        //    {
        //        displayText = displayText.Substring(0, displayText.Length - 1);
        //    }

        //    Match match = Regex.Match(displayText, @"^[-]?[\dA-F]+(\.\d+)?");
        //    if (match.Success)
        //        return match.Value;

        //    return "0";
        //}

        public void SetBase(int newBase)
        {
            _currentBase = newBase;
        }
        public bool IsValidInput(string input)
        {
            return input.All(c => "0123456789ABCDEF".Substring(0, _currentBase).Contains(char.ToUpper(c)));
        }

        public double ConvertToBase10(string input)
        {
            try
            {
                return Convert.ToInt32(input, _currentBase);
            }
            catch
            {
                return 0;
            }
        }

        public string ConvertFromBase10(double value)
        {
            return Convert.ToString((int)value, _currentBase).ToUpper();
        }

        public string GetDisplayValue()
        {
            return ConvertFromBase10(_total);
        }
    }
}

