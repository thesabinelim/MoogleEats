namespace MoogleEats.Shared;

internal class StringUtils
{
    internal static string ToOrdinalString(int num)
    {
        return $"{num}{getOrdinalSuffix(num)}";
    }

    private static string getOrdinalSuffix(int num)
    {
        var numStr = num.ToString();
        if (numStr.EndsWith("11")) return "th";
        if (numStr.EndsWith("12")) return "th";
        if (numStr.EndsWith("13")) return "th";
        if (numStr.EndsWith('1')) return "st";
        if (numStr.EndsWith('2')) return "nd";
        if (numStr.EndsWith('3')) return "rd";
        return "th";
    }
}
