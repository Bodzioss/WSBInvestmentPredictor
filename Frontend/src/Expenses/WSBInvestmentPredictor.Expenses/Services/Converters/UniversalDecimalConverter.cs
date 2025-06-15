using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace WSBInvestmentPredictor.Expenses.Services.Converters;

public class UniversalDecimalConverter : DecimalConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return 0m;

        // Remove any whitespace
        text = text.Trim();

        // Try parsing with Polish culture (comma as decimal separator)
        var polishCulture = new CultureInfo("pl-PL");
        polishCulture.NumberFormat.NumberDecimalSeparator = ",";
        polishCulture.NumberFormat.NumberGroupSeparator = " ";
        polishCulture.NumberFormat.NegativeSign = "-";

        if (decimal.TryParse(text, NumberStyles.Any, polishCulture, out decimal result))
            return result;

        // Try parsing with English culture (dot as decimal separator)
        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            return result;

        // If all parsing attempts fail, throw an exception
        throw new FormatException($"Could not parse '{text}' as decimal using either Polish or English format.");
    }
}