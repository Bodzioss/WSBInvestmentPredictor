using System.Text;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;

public class Sp500CsvTickerProvider : ISp500TickerProvider
{
    private readonly string _csvPath;
    private readonly Lazy<List<CompanyTicker>> _tickers;

    public Sp500CsvTickerProvider()
    {
        _csvPath = Path.Combine(AppContext.BaseDirectory, "Resources", "sp500.csv");
        _tickers = new Lazy<List<CompanyTicker>>(LoadTickers);
    }

    public IEnumerable<CompanyTicker> GetAll() => _tickers.Value;

    private List<CompanyTicker> LoadTickers()
    {
        if (!File.Exists(_csvPath))
            throw new FileNotFoundException("Nie znaleziono pliku sp500.csv w katalogu Resources.", _csvPath);

        var lines = File.ReadAllLines(_csvPath, Encoding.UTF8)
                        .Skip(1) // skip header
                        .Where(line => !string.IsNullOrWhiteSpace(line));

        var result = new List<CompanyTicker>();

        foreach (var line in lines)
        {
            var parts = line.Split(';');
            if (parts.Length >= 2)
            {
                result.Add(new CompanyTicker(parts[0].Trim(), parts[1].Trim()));
            }
        }

        return result;
    }
}