using System.Diagnostics;
using Microsoft.ML;

namespace WSBInvestmentPredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var mlContext = new MLContext();

            // Ścieżka do przykładowego pliku CSV (np. AAPL.csv)
            var path = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data\TargetData\AAPL.csv");
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                Console.WriteLine($"Błąd: Plik nie istnieje: {path}");
                return;
            }

            // Wczytanie danych
            IDataView data = mlContext.Data.LoadFromTextFile<MarketData>(
                path: path,
                hasHeader: true,
                separatorChar: ',');

            var dataEnumerable = mlContext.Data.CreateEnumerable<MarketData>(data, reuseRowObject: false).ToList();

            // Statystyki danych wejściowych
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("STATYSTYKI DANYCH RYNKOWYCH");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Liczba wierszy: {dataEnumerable.Count}");
            Console.WriteLine($"Target (zwrot 30-dniowy):");
            Console.WriteLine($"  Min:     {dataEnumerable.Min(x => x.Target):P2}");
            Console.WriteLine($"  Max:     {dataEnumerable.Max(x => x.Target):P2}");
            Console.WriteLine($"  Średnia: {dataEnumerable.Average(x => x.Target):P2}");

            // Podział na dane treningowe i testowe
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // Pipeline modelu
            var pipeline = mlContext.Transforms.CopyColumns("Label", "Target")
                .Append(mlContext.Transforms.Concatenate("Features",
                    nameof(MarketData.Open),
                    nameof(MarketData.High),
                    nameof(MarketData.Low),
                    nameof(MarketData.Close),
                    nameof(MarketData.Volume),
                    nameof(MarketData.SMA_5),
                    nameof(MarketData.SMA_10),
                    nameof(MarketData.SMA_20),
                    nameof(MarketData.Volatility_10),
                    nameof(MarketData.RSI_14)))
                .Append(mlContext.Regression.Trainers.FastTree());

            // Czas treningu
            var trainStopwatch = Stopwatch.StartNew();
            var model = pipeline.Fit(split.TrainSet);
            trainStopwatch.Stop();

            // Ewaluacja modelu
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions);

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("METRYKI MODELU REGRESYJNEGO");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"R² (dopasowanie):           {metrics.RSquared:0.####}");
            Console.WriteLine($"MAE (średni błąd):          {metrics.MeanAbsoluteError:0.####}");
            Console.WriteLine($"RMSE (błąd średniokwadrat.): {metrics.RootMeanSquaredError:0.####}");
            Console.WriteLine($"Czas treningu:              {trainStopwatch.ElapsedMilliseconds} ms");

            // Predykcja przykładowa
            var engine = mlContext.Model.CreatePredictionEngine<MarketData, PredictionResult>(model);

            var sample = new MarketData
            {
                Open = 170.5f,
                High = 173.0f,
                Low = 169.0f,
                Close = 171.2f,
                Volume = 65000000f,
                SMA_5 = 172.0f,
                SMA_10 = 170.8f,
                SMA_20 = 168.7f,
                Volatility_10 = 2.3f,
                RSI_14 = 56.0f
            };

            var prediction = engine.Predict(sample);

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("PREDYKCJA DLA PRZYKŁADOWYCH DANYCH");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Prognozowany zwrot za 30 dni: {prediction.Score:P2}");

            if (prediction.Score > 0.05)
                Console.WriteLine("Sygnał: potencjalny wzrost (> +5%) – możliwa okazja inwestycyjna");
            else if (prediction.Score < -0.05)
                Console.WriteLine("Sygnał: możliwy spadek (< -5%) – zachowaj ostrożność");
            else
                Console.WriteLine("Sygnał: neutralny – brak wyraźnego trendu");

            stopwatch.Stop();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Całkowity czas wykonania programu: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine(new string('=', 60));
        }
    }
}
