using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.UnitTests.Builders;

public class CompanyTickerBuilder
{
    private string _ticker = "AAPL";
    private string _name = "Apple Inc.";

    public CompanyTickerBuilder WithTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    public CompanyTickerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CompanyTicker Build()
    {
        return new CompanyTicker(_ticker, _name);
    }
}