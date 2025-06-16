using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace WSBInvestmentPredictor.Expenses.Services.Converters;

/// <summary>
/// Custom CSV type converter for DateTime values that supports both Polish and English date formats.
/// Extends the base DateTimeConverter to handle multiple culture-specific date formats.
/// </summary>
public class UniversalDateTimeConverter : DateTimeConverter
{
    /// <summary>
    /// Converts a string value to a DateTime object, attempting to parse it using both Polish and English formats.
    /// </summary>
    /// <param name="text">The string value to convert</param>
    /// <param name="row">The current CSV reader row</param>
    /// <param name="memberMapData">The member mapping data</param>
    /// <returns>The parsed DateTime value, or null if the input is empty</returns>
    /// <exception cref="FormatException">Thrown when the string cannot be parsed as a DateTime using either format</exception>
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        // Try parsing with Polish culture
        if (DateTime.TryParse(text, new CultureInfo("pl-PL"), DateTimeStyles.None, out DateTime result))
            return result;

        // Try parsing with English culture
        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return result;

        // If all parsing attempts fail, throw an exception
        throw new FormatException($"Could not parse '{text}' as DateTime using either Polish or English format.");
    }
}