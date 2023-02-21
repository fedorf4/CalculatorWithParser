using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab1_project
{
    class RPN_Translator
    {
        public bool IsOperation(string symbol)
        {
            return (symbol == "+" || symbol == "-" || symbol == "*" || symbol == "/");
        }

        public List<string> PrepareToTranslation(string in_expr)
        {
            in_expr = Regex.Replace(in_expr, @"\s+", "");
            in_expr = Regex.Replace(in_expr, @"\t+", "");

            string[] expr = Regex.Split(in_expr, @"([*(,)\^\/]|(?<!E)[\+\-])");
            List<string> elems = new List<string>(expr);
            elems.RemoveAll(r => r == "");

            if (elems[0] == "-")
            {
                elems[1] = elems[0] + elems[1];
                elems.RemoveAt(0);
            }
            for (int i = 0; i < elems.Count - 1; i++)
            {
                if (elems[i] == "(" && elems[i + 1] == "-")
                {
                    elems[i + 2] = elems[i + 1] + elems[i + 2];
                    elems.RemoveAt(i + 1);
                }
            }
            return elems;
        }

        public List<string> TranslateToRPNWithCheck(List<string> elems)
        {
            Stack<string> operations = new Stack<string>();
            List<string> rpn_expr = new List<string>();
            Dictionary<string, int> priorities = new Dictionary<string, int>{
                { "(", '1' },
                { ")", '1' },
                { "+", '2' },
                { "-", '2' },
                { "*", '3' },
                { "/", '3' },
                {"fun", '4' },
                };

            for (int i = 0; i < elems.Count; i++)
            {
                if (elems[i] == "fun")
                {
                    if (i != 0)
                        if (!IsOperation(elems[i - 1]) && elems[i - 1] != "(")
                            throw new ArgumentException("Missing operation before function.");

                    if (i == elems.Count - 1)
                        throw new ArgumentException("Unexpected function at the end.");

                    else if (elems[i + 1] != "(")
                        throw new ArgumentException("Missing opening bracket after using function.");

                    int skip = 0;
                    bool isMetComma = false;
                    int idxComma = 0;
                    Stack<int> bracket_counter = new Stack<int>();
                    for (int j = i + 1; j < elems.Count; j++)
                    {
                        if (elems[j] == "(") bracket_counter.Push(1);
                        if (elems[j] == ")")
                        {
                            if (bracket_counter.Count == 0)
                                throw new ArgumentException("Missing opening before closing.");

                            if (!isMetComma && bracket_counter.Count == 1)
                                throw new ArgumentException("Missing comma in function.");

                            bracket_counter.Pop();
                            if (bracket_counter.Count == 0)
                            {
                                int first = idxComma - (i + 1) - 1;
                                int second = j - idxComma - 1;
                                if (first > 1)
                                    rpn_expr.AddRange(TranslateToRPNWithCheck(elems.GetRange(i + 2, idxComma - (i + 1) - 1)));
                                else if (first == 0)
                                    throw new ArgumentException("Missing first argument in function.");
                                else
                                    rpn_expr.Add(elems[i + 2]);

                                if (second > 1)
                                    rpn_expr.AddRange(TranslateToRPNWithCheck(elems.GetRange(idxComma + 1, j - idxComma - 1)));
                                else if (second == 0)
                                    throw new ArgumentException("Missing second argument in function.");

                                else
                                    rpn_expr.Add(elems[idxComma + 1]);

                                rpn_expr.Add("fun");
                                skip = j - i;
                                break;
                            }
                        }
                        if (elems[j] == ",")
                        {
                            isMetComma = true;
                            if (bracket_counter.Count == 1) idxComma = j;
                            if (bracket_counter.Count < 1)
                                throw new ArgumentException("Error using function.");
                        }
                    }
                    i += skip;
                }

                else
                if (elems[i] == "(")
                {
                    if (i != 0)
                        if (!IsOperation(elems[i - 1]) && elems[i - 1] != "(")
                            throw new ArgumentException("Missing operation before opening bracket.");

                    if (i == elems.Count - 1)
                        throw new ArgumentException("Unexpected opening bracket at the end.");

                    else if (IsOperation(elems[i + 1]))
                        throw new ArgumentException("Missing number between opening bracket and operation.");

                    operations.Push(elems[i]);
                }

                else
                if (elems[i] == ")")
                {
                    if (i == 0)
                        throw new ArgumentException("Unexpected closing bracket at the beginning.");

                    else if (IsOperation(elems[i - 1]))
                        throw new ArgumentException("Missing number before operation and closing bracket.");

                    else if (elems[i - 1] == "(")
                        throw new ArgumentException("Missing expression in brackets.");

                    if (i != elems.Count - 1)
                        if (!IsOperation(elems[i + 1]) && elems[i + 1] != ")")
                            throw new ArgumentException("Missing operation after closing bracket.");

                    while (operations.Peek() != "(")
                    {
                        rpn_expr.Add(operations.Peek());
                        operations.Pop();
                        if (operations.Count == 0)
                            throw new ArgumentException("Missing openning bracket before closing.");
                    }
                    operations.Pop();
                }

                else
                if (IsOperation(elems[i]))
                {
                    if (i != elems.Count - 1)
                        if (IsOperation(elems[i + 1]))
                            throw new ArgumentException("Missing number between operations.");

                    if (operations.Count == 0)
                        operations.Push(elems[i]);
                    else
                    {
                        while (priorities[operations.Peek()] >= priorities[elems[i]])
                        {
                            rpn_expr.Add(operations.Peek());
                            operations.Pop();
                            if (operations.Count == 0)
                                break;
                        }
                        operations.Push(elems[i]);
                    }
                }

                else
                {
                    if (!Double.TryParse(elems[i], out _))
                        throw new ArgumentException("Unexpected symbols.");
                    rpn_expr.Add(elems[i]);
                }

            }
            while (operations.Count != 0)
            {
                rpn_expr.Add(operations.Peek());
                operations.Pop();
            }
            return rpn_expr;
        }
    }
}
