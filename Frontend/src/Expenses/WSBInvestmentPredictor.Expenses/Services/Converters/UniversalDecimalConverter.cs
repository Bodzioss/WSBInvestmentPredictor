using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace WSBInvestmentPredictor.Expenses.Services.Converters;

/// <summary>
/// Custom CSV type converter for decimal values that supports both Polish and English number formats.
/// Extends the base DecimalConverter to handle multiple culture-specific number formats.
/// </summary>
public class UniversalDecimalConverter : DecimalConverter
{
    /// <summary>
    /// Converts a string value to a decimal number, attempting to parse it using both Polish and English formats.
    /// Polish format uses comma as decimal separator and space as thousand separator.
    /// English format uses dot as decimal separator.
    /// </summary>
    /// <param name="text">The string value to convert</param>
    /// <param name="row">The current CSV reader row</param>
    /// <param name="memberMapData">The member mapping data</param>
    /// <returns>The parsed decimal value, or 0 if the input is empty</returns>
    /// <exception cref="FormatException">Thrown when the string cannot be parsed as a decimal using either format</exception>
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