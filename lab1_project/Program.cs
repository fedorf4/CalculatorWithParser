using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace lab1_project
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            try
            {
                string in_expr = Console.ReadLine();
                if (in_expr.Length == 0)
                    throw new ArgumentException("Empty string entered.");

                RPN_Translator rpn_translator = new RPN_Translator();
                List<string> prepared_elems = rpn_translator.PrepareToTranslation(in_expr);
                List<string> rpn_expr = rpn_translator.TranslateToRPNWithCheck(prepared_elems);
                if (rpn_expr == null) return;
                Console.Write("Reverse Polish notation:  ");
                foreach (var elem in rpn_expr)
                    Console.Write(elem + " ");

                Calculator calculator = new Calculator();
                double result = calculator.Calculate(rpn_expr);

                Console.WriteLine("\nResult: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nIncorrect expression. " + ex.Message);
            }
        }
    }
}
