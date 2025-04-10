using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    internal class PostfixCalculator
    {
        private string _expression;
        private Stack<double> _stack; 

        public PostfixCalculator()
        {
            _expression = string.Empty;
            _stack = new Stack<double>();
        }

        public void Reset()
        { 
            _expression = string.Empty;
            _stack.Clear();
        }

        public void AddNumber(double number)
        {
            _expression += number.ToString(CultureInfo.InvariantCulture) + " ";
        }

        public void AddOperator(string operation)
        {
            _expression += operation + " ";
        }

        public double EvaluateExpression()
        {
            if (string.IsNullOrWhiteSpace(_expression))
                throw new InvalidOperationException("No expression to evaluate.");

            string postfixExpression = ConvertToPostfix(_expression);

            string[] tokens = postfixExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            _stack.Clear();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out double number))
                {
                    _stack.Push(number);
                }
                else if (IsOperator(token))
                {
                    if (_stack.Count < 2)
                        throw new InvalidOperationException("Not enough operands for operation.");

                    double b = _stack.Pop();
                    double a = _stack.Pop();
                    double result = ApplyOperator(a, b, token);
                    _stack.Push(result);
                }
                //else if (IsUnaryOperator(token))
                //{
                //    if (_stack.Count < 1)
                //        throw new InvalidOperationException("Not enough operands for unary operation.");

                //    double a = _stack.Pop();
                //    double result = ApplyUnaryOperator(a, token);
                //    _stack.Push(result);
                //}
            }

            double res = _stack.Pop();
            _expression = res.ToString(CultureInfo.InvariantCulture);
            return res;
        }


        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "%";
        }

        private bool IsUnaryOperator(string token)
        {
            return token == "1/x" || token == "Rad";
        }
        private double ApplyOperator(double a, double b, string operation)
        {
            switch (operation)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    if (b != 0)
                        return a / b;
                    else
                        throw new InvalidOperationException("Division by zero is not allowed.");
                case "%":
                    return a % b;
                default:
                    throw new InvalidOperationException("Invalid operator.");
            }
        }

        //private double ApplyUnaryOperator(double a, string operation)
        //{
        //    switch (operation)
        //    {
        //        case "1/x":
        //            if (a == 0) throw new InvalidOperationException("Division by zero.");
        //            return 1 / a;
        //        case "Rad":
        //            if (a < 0) throw new InvalidOperationException("Cannot compute square root of negative number.");
        //            return Math.Sqrt(a);
        //        case "^2":
        //            return Math.Pow(a, 2);
                   
        //        default:
        //            throw new InvalidOperationException("Invalid unary operator.");
        //    }
        //}

        private string ConvertToPostfix(string infixExpression)
        {
            string[] tokens = infixExpression.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Stack<string> operatorStack = new Stack<string>();
            StringBuilder output = new StringBuilder();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out _))
                {
                    output.Append(token).Append(" ");
                }
                else if (IsOperator(token))
                {
                    while (operatorStack.Count > 0 && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                    {
                        output.Append(operatorStack.Pop()).Append(" ");
                    }
                    operatorStack.Push(token);
                }
            }

            while (operatorStack.Count > 0)
            {
                output.Append(operatorStack.Pop()).Append(" ");
            }

            return output.ToString().Trim();
        }

        private int GetPrecedence(string op)
        {
            return op == "+" || op == "-" ? 1 :
                   op == "*" || op == "/" || op == "%" ? 2 : 0;
        }
        public void ClearExpression()
        {
            _expression = string.Empty;
        }

        public string GetExpression()
        {
            return _expression.Trim();
        }
    }
}
