using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;

namespace MathPlugin;

public class Math
{
    [SKFunction, Description("Take the square root of a number")]
    public static string Sqrt(string number)
    {
        return System.Math.Sqrt(Convert.ToDouble(number, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
    }

    [SKFunction, Description("Multiply two numbers")]
    public static double Multiply(
        [Description("The first number to multiply")] double number1,
        [Description("The second number to multiply")] double number2)
    {
        return number1 * number2;
    }
}