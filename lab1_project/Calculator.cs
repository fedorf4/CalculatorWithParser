using System;
using System.Collections.Generic;
using System.Text;

namespace lab1_project
{
    class Calculator
    {
        double MyFunction(double first, double second)
        {
            return 2 * (first + second);
        }

        public bool IsOperation(string symbol)
        {
            return (symbol == "+" || symbol == "-" || symbol == "*" || symbol == "/");
        }

        public double Calculate(List<string> rpn_expr)
        {
            for (int i = 0; i < rpn_expr.Count; i++)
            {
                if (IsOperation(rpn_expr[i]) || rpn_expr[i] == "fun")
                {
                    double first = Double.Parse(rpn_expr[i - 2]), second = Double.Parse(rpn_expr[i - 1]);
                    double res;
                    switch (rpn_expr[i])
                    {
                        case "+":
                            res = first + second;
                            break;
                        case "-":
                            res = first - second;
                            break;
                        case "*":
                            res = first * second;
                            break;
                        case "/":
                            if (second == 0)
                                throw new DivideByZeroException("Division by zero.");
                            res = first / second;
                            break;
                        case "fun":
                            res = MyFunction(first, second);
                            break;
                        default:
                            throw new ArgumentException("Something went wrong.");
                    }
                    rpn_expr.RemoveAt(i - 2);
                    rpn_expr.RemoveAt(i - 1);
                    rpn_expr[i - 2] = res.ToString();
                    i -= 2;
                }
                if (rpn_expr.Count == 1)
                    break;
            }
            return Double.Parse(rpn_expr[0]);
        }
    }
}
