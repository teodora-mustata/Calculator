using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private Operations _calculator = new Operations();
        private Memory _memory = new Memory();
        private ClipboardManager _clipboard = new ClipboardManager();
        private PostfixCalculator _postfixCalculator = new PostfixCalculator();
        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += Window_PreviewKeyDown;
            Settings settings = Settings.LoadSettings();
            foreach (var item in BaseMenu.Items)
            {
                if (item is MenuItem baseMenuItem)
                {
                    if (baseMenuItem.Tag.ToString() == settings.Base.ToString())
                    {
                        baseMenuItem.IsChecked = true;
                    }
                    else
                    {
                        baseMenuItem.IsChecked = false;
                    }
                }
            }
            foreach (var item in FileMenu.Items)
            {
                if (item is MenuItem fileMenuItem) 
                {
                    if (fileMenuItem.Header.ToString() == "Digit Grouping")
                    {
                        fileMenuItem.IsChecked = settings.DigitGrouping;
                    }
                    else if (fileMenuItem.Header.ToString() == "Advanced Calculation Mode")
                    {
                        fileMenuItem.IsChecked = settings.AdvancedMode;
                    }
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.C) // Ctrl + C
                {
                    _clipboard.Copy(DisplayTextBox.Text);
                    e.Handled = true;
                }
                else if (e.Key == Key.X) // Ctrl + X
                {
                    string text = DisplayTextBox.Text;
                    _clipboard.Cut(ref text);
                    DisplayTextBox.Text = text;
                    e.Handled = true;
                }
                else if (e.Key == Key.V) // Ctrl + V
                {
                    DisplayTextBox.Text = _clipboard.Paste();
                    e.Handled = true;
                }
            }
            else
            {
                if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) // number
                {
                    string number = e.Key.ToString().Replace("D", "").Replace("NumPad", "");
                    AddNumberToDisplay(number);
                    e.Handled = true;
                }
                else if (e.Key == Key.Back) // backspace
                {
                    BackspaceButton_Click(sender, e);
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter) // enter
                {
                    EqualsButton_Click(sender, e);
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape) // esc
                {
                    ClearButton_Click(sender, e);
                    e.Handled = true;
                }
                else if (e.Key == Key.OemPlus || e.Key == Key.Add) // +
                {
                    OperationButton_Click("+");
                    e.Handled = true;
                }
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract) // -
                {
                    OperationButton_Click("-");
                    e.Handled = true;
                }
                else if (e.Key == Key.Multiply) // *
                {
                    OperationButton_Click("*");
                    e.Handled = true;
                }
                else if (e.Key == Key.Divide || e.Key == Key.OemQuestion) // /
                {
                    OperationButton_Click("/");
                    e.Handled = true;
                }
                else if (e.Key == Key.M) // mod %
                {
                    OperationButton_Click("%");
                    e.Handled = true;
                }
                else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal) // .
                {
                    PointButton_Click(sender, e);
                    e.Handled = true;
                }
                else if (e.Key == Key.R) // rad
                {
                    UnaryOperationButton_Click("Rad");
                    e.Handled = true;
                }
                else if (e.Key == Key.I) // 1/x
                {
                    UnaryOperationButton_Click("1/x");
                    e.Handled = true;
                }
                else if (e.Key == Key.P) // +/-
                {
                    UnaryOperationButton_Click("+/-");
                    e.Handled = true;
                }
            }

        }

        private void AddNumberToDisplay(string number)
        {
            if (DisplayTextBox.Text == "0")
            {
                DisplayTextBox.Text = number;
            }
            else
            {
                DisplayTextBox.Text += number;
            }

            string lastNumber = _calculator.GetLastNumber(DisplayTextBox.Text).ToString();
            string[] parts = lastNumber.Split('.');
            string integerPart = parts[0];
            string decimalPart = (parts.Length > 1) ? parts[1] : "";

            integerPart = integerPart.Replace(",", "");

            Settings settings = Settings.LoadSettings();
            if (settings.DigitGrouping)
            {
                integerPart = FormatNumberWithGrouping(Convert.ToInt64(integerPart));
            }

            string formattedNumber = decimalPart == "" ? integerPart : integerPart + "." + decimalPart;

            int lastOperatorIndex = DisplayTextBox.Text.LastIndexOfAny(new char[] { '+', '-', '*', '/' , '%'});

            if (lastOperatorIndex != -1)
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Substring(0, lastOperatorIndex + 1) + formattedNumber;
            }
            else
            {
                DisplayTextBox.Text = formattedNumber;
            }

            //int currentBase = settings.Base;
            //string convertedValue = ConvertBase(DisplayTextBox.Text, 10, currentBase);
            //DisplayTextBox.Text = convertedValue;
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string number = button.Content.ToString();
                AddNumberToDisplay(number);
            }
        }

        public string FormatNumberWithGrouping(double number)
        {
            Settings settings = Settings.LoadSettings();

            if (settings.DigitGrouping)
            {
                return number.ToString("#,0.##########", CultureInfo.InvariantCulture);
            }
            else
            {
                return number.ToString("0.##########", CultureInfo.InvariantCulture);
            }
        }


        //private void OperationButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    if (button != null)
        //    {
        //        string operation = button.Content.ToString();

        //        Settings settings = Settings.LoadSettings();

        //        if (settings.AdvancedMode)
        //        {
        //            // DOAR adaugăm operatorul, fără a calcula imediat
        //            DisplayTextBox.Text += operation;
        //        }
        //        else
        //        {

        //            string lastNumber = _calculator.GetLastNumber(DisplayTextBox.Text).ToString();
        //            string valueWithoutGrouping = lastNumber.Replace(",", "");
        //            double numericValue = Convert.ToDouble(valueWithoutGrouping);

        //            _calculator.SetOperator(operation, numericValue);

        //            //Settings settings = Settings.LoadSettings();
        //            string formattedNumber = settings.DigitGrouping
        //? Convert.ToDecimal(_calculator.GetTotal()).ToString("#,0.##########", CultureInfo.InvariantCulture)
        //: Convert.ToDecimal(_calculator.GetTotal()).ToString("0.##########", CultureInfo.InvariantCulture);



        //            DisplayTextBox.Text = formattedNumber + operation;
        //        }
        //    }
        //}

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string operation = button.Content.ToString();
                Settings settings = Settings.LoadSettings();

                string lastNumber = _calculator.GetLastNumber(DisplayTextBox.Text).ToString();
                string valueWithoutGrouping = lastNumber.Replace(",", "");
                double numericValue = Convert.ToDouble(valueWithoutGrouping);

                if (settings.AdvancedMode)
                {
                    if (Char.IsDigit(DisplayTextBox.Text[DisplayTextBox.Text.Length - 1]))
                    {
                        _postfixCalculator.AddNumber(numericValue);
                        _postfixCalculator.AddOperator(operation);
                        DisplayTextBox.Text += operation;
                    }
                }
                else
                {
                    _calculator.SetOperator(operation, numericValue);
                    string formattedNumber = settings.DigitGrouping
                        ? Convert.ToDecimal(_calculator.GetTotal()).ToString("#,0.##########", CultureInfo.InvariantCulture)
                        : Convert.ToDecimal(_calculator.GetTotal()).ToString("0.##########", CultureInfo.InvariantCulture);
                    DisplayTextBox.Text = formattedNumber + operation;
                }
            }
        }

        private void OperationButton_Click(string operation)
        {
            string lastNumber = _calculator.GetLastNumber(DisplayTextBox.Text).ToString();
            string valueWithoutGrouping = lastNumber.Replace(",", "");
            double numericValue = Convert.ToDouble(valueWithoutGrouping);

            Settings settings = Settings.LoadSettings();

            if (settings.AdvancedMode)
            {
                if (Char.IsDigit(DisplayTextBox.Text[DisplayTextBox.Text.Length - 1]))
                {
                    _postfixCalculator.AddNumber(numericValue);
                    _postfixCalculator.AddOperator(operation);
                    DisplayTextBox.Text += operation;
                }
            }
            else
            {
                _calculator.SetOperator(operation, numericValue);
                string formattedNumber = settings.DigitGrouping
                    ? Convert.ToDecimal(_calculator.GetTotal()).ToString("#,0.##########", CultureInfo.InvariantCulture)
                    : Convert.ToDecimal(_calculator.GetTotal()).ToString("0.##########", CultureInfo.InvariantCulture);
                DisplayTextBox.Text = formattedNumber + operation;
            }
        }


        private void UnaryOperationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string operation = button.Content.ToString();
                double currentDisplayValue = _calculator.GetLastNumber(DisplayTextBox.Text);
                if (operation == "1/x" && currentDisplayValue == 0) DisplayTextBox.Text = "error";
                else
                    {
                        _calculator.UnaryOperator(operation, currentDisplayValue);
                    //DisplayTextBox.Text = FormatNumberWithGrouping(_calculator.GetTotal());

                    string total = FormatNumberWithGrouping(_calculator.GetTotal());
                    string currentText = DisplayTextBox.Text;
                    int lastNumberIndex = currentText.LastIndexOf(currentDisplayValue.ToString(CultureInfo.InvariantCulture));

                    if (lastNumberIndex >= 0)
                    {
                        DisplayTextBox.Text = currentText.Substring(0, lastNumberIndex) + total;
                    }
                    else
                    {
                        DisplayTextBox.Text = total;
                    }

                }

            }
        }

        private void UnaryOperationButton_Click(string operation)
        {
            double currentDisplayValue = _calculator.GetLastNumber(DisplayTextBox.Text);

            if (operation == "1/x" && currentDisplayValue == 0)
            {
                DisplayTextBox.Text = "error";
            }
            else
            {
                _calculator.UnaryOperator(operation, currentDisplayValue);
                string total = FormatNumberWithGrouping(_calculator.GetTotal());
                string currentText = DisplayTextBox.Text;
                int lastNumberIndex = currentText.LastIndexOf(currentDisplayValue.ToString(CultureInfo.InvariantCulture));

                if (lastNumberIndex >= 0)
                {
                    DisplayTextBox.Text = currentText.Substring(0, lastNumberIndex) + total;
                }
                else
                {
                    DisplayTextBox.Text = total;
                }
            }
        }


        private void PointButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_calculator.GetLastNumber(DisplayTextBox.Text).ToString().Contains("."))
            {
                DisplayTextBox.Text += ".";
            }
        }
        //private void EqualsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Settings settings = Settings.LoadSettings();

        //    if (settings.AdvancedMode)
        //    { 

        //    }
        //    else
        //    {
        //        double currentDisplayValue = _calculator.GetLastNumber(DisplayTextBox.Text);
        //        _calculator.Equals(currentDisplayValue);
        //        DisplayTextBox.Text = FormatNumberWithGrouping(_calculator.CurrentValue);
        //    }

        //}

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.LoadSettings();

            if (settings.AdvancedMode)
            {
                _postfixCalculator.AddNumber(_calculator.GetLastNumber(DisplayTextBox.Text));
                double result = _postfixCalculator.EvaluateExpression();
                DisplayTextBox.Text = FormatNumberWithGrouping(result);
                _postfixCalculator.ClearExpression();
            }
            else
            {
                double currentDisplayValue = _calculator.GetLastNumber(DisplayTextBox.Text);
                _calculator.Equals(currentDisplayValue);
                DisplayTextBox.Text = FormatNumberWithGrouping(_calculator.CurrentValue);
            }
        }


        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayTextBox.Text.Length > 0)
            {
                char lastChar = DisplayTextBox.Text.Last();
                string operators = "+-*/%";

                DisplayTextBox.Text = DisplayTextBox.Text.Substring(0, DisplayTextBox.Text.Length - 1);

                if (operators.Contains(lastChar))
                {
                    _calculator.ResetLastOperator();
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) // C
        {
            DisplayTextBox.Text="0";
            _calculator.Reset();
            _postfixCalculator.Reset();
        }

        //private void ClearEntryButton_Click(object sender, RoutedEventArgs e) // CE
        //{
        //    string currentDisplay = DisplayTextBox.Text.Trim();

        //    if (string.IsNullOrEmpty(currentDisplay))
        //        return;

        //    double lastNumber = _calculator.GetLastNumber(currentDisplay); 

        //    if (lastNumber != 0)
        //    {
        //        int lastIndex = currentDisplay.LastIndexOf(lastNumber.ToString());
        //        currentDisplay = currentDisplay.Remove(lastIndex);

        //        if (string.IsNullOrEmpty(currentDisplay))
        //        {
        //            DisplayTextBox.Text = "0";
        //        }
        //        else
        //        {
        //            DisplayTextBox.Text = currentDisplay;
        //        }
        //    }
        //}

        private void ClearEntryButton_Click(object sender, RoutedEventArgs e) // CE
        {
            string currentDisplay = DisplayTextBox.Text.Trim();

            if (string.IsNullOrEmpty(currentDisplay))
                return;

            double lastNumber = _calculator.GetLastNumber(currentDisplay);
            if (lastNumber != 0)
            {
                string lastNumberStr = lastNumber.ToString(CultureInfo.InvariantCulture);
                int lastIndex = currentDisplay.LastIndexOf(lastNumberStr);
                if (lastIndex >= 0)
                {
                    currentDisplay = currentDisplay.Remove(lastIndex);

                    if (string.IsNullOrEmpty(currentDisplay))
                    {
                        DisplayTextBox.Text = "0";
                    }
                    else
                    {
                        DisplayTextBox.Text = currentDisplay;
                    }
                }
                else
                {
                    DisplayTextBox.Text = "0";
                }
            }
        }


        private void MemoryAdd_Click(object sender, RoutedEventArgs e) // M+
        {
            double value = _calculator.GetLastNumber(DisplayTextBox.Text);
            _memory.AddToMemory(value);
        }

        private void MemorySubtract_Click(object sender, RoutedEventArgs e) // M-
        {
            double value = _calculator.GetLastNumber(DisplayTextBox.Text);
            _memory.SubtractFromMemory(value);
        }

        private void MemoryStore_Click(object sender, RoutedEventArgs e) // MS
        {
            double value = _calculator.GetLastNumber(DisplayTextBox.Text);
            _memory.StoreInMemory(value);
        }

        private void MemoryRecall_Click(object sender, RoutedEventArgs e) // MR
        {
            DisplayTextBox.Text = _memory.RecallMemory().ToString();
        }
        private void MemoryShow_Click(object sender, RoutedEventArgs e) // M>
        {
            ContextMenu contextMenu = new ContextMenu();

            List<double> values = _memory.GetMemoryStack();

            if (values.Count > 0)
            {
                foreach (double value in values)
                {
                    MenuItem item = new MenuItem { Header = value.ToString() };
                    item.Click += MemoryItem_Click;
                    contextMenu.Items.Add(item);
                }

                contextMenu.PlacementTarget = MemoryShow;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                contextMenu.IsOpen = true;
            }
        }

        private void MemoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                DisplayTextBox.Text = menuItem.Header.ToString();
            }
        }


        private void MemoryClear_Click(object sender, RoutedEventArgs e) // MC
        {
            _memory.ClearMemory();
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayTextBox.Text.Length > 0)
            {
            string text = DisplayTextBox.Text;
            _clipboard.Cut(ref text);
            DisplayTextBox.Text = text;
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayTextBox.Text.Length > 0)
            {
                _clipboard.Copy(DisplayTextBox.Text);
            }
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                DisplayTextBox.Text = _clipboard.Paste();
            }
        }

        private void DigitGroupingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.LoadSettings();

            settings.DigitGrouping = !settings.DigitGrouping;

            Settings.SaveSettings(settings);

            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                menuItem.IsChecked = settings.DigitGrouping;
            }
        }


        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Calculator made by Mustata Teodora :)",
                "10LF233",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void ChangeBase_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenuItem = sender as MenuItem;
            int selectedBase = Convert.ToInt32(selectedMenuItem.Tag);

            Settings settings = Settings.LoadSettings();

            settings.Base = selectedBase;

            if (selectedBase == 10)
            {
                settings.Mode = "Standard";
            }
            else
            {
                settings.Mode = "Programmer";
            }

            Settings.SaveSettings(settings);

            foreach (var item in BaseMenu.Items)
            {
                if (item is MenuItem currentMenuItem)
                {
                    currentMenuItem.IsChecked = false;
                }
            }

            selectedMenuItem.IsChecked = true;
        }

        private void AdvancedCalculationMode_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.LoadSettings();

            settings.AdvancedMode = !settings.AdvancedMode;

            Settings.SaveSettings(settings);

            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                menuItem.IsChecked = settings.AdvancedMode;
            }
        }

    }
}