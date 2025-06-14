using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace WSBInvestmentPredictor.Expenses.Services.Converters;

public class UniversalDateTimeConverter : DateTimeConverter
{
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