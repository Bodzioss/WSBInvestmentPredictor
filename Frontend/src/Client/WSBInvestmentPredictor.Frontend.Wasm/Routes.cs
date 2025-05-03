using System.Reflection;
using WSBInvestmentPredictor.Prediction;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Frontend.Wasm;

public partial class Routes
{
    public static readonly List<Assembly> AdditionalAssemblies = new()
    {
        typeof(DI).Assembly,
        typeof(NavigationRegistry).Assembly
    };
}
