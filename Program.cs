using System.Diagnostics;
using System.Globalization;
using Microsoft.ML;

namespace WSBInvestmentPredictor
{
    enum RunMode { CurrentPrediction, HistoricalBacktest }

    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var mlContext = new MLContext();

            // === KONFIGURACJA ===
            int maxTickersToAnalyze = 1;           // Ile spółek analizować (0 = wszystkie)
            RunMode mode = RunMode.HistoricalBacktest; // Tryb działania programu
            int minTrainSize = 500;                 // Minimalna liczba dni do trenowania modelu
            int maxBacktestPoints = 5000;            // Maksymalna liczba dat, dla których wykonujemy predykcję

            var dataDir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data\TargetData");
            dataDir = Path.GetFullPath(dataDir);

            if (!Directory.Exists(dataDir))
            {
                Console.WriteLine($"Brak katalogu danych: {dataDir}");
                return;
            }

            var files = Directory.GetFiles(dataDir, "*.csv");
            if (maxTickersToAnalyze > 0)
                files = files.Take(maxTickersToAnalyze).ToArray();

            Console.WriteLine(new string('=', 70));
            Console.WriteLine(mode == RunMode.CurrentPrediction ? "Tryb: BIEŻĄCA PREDYKCJA" : "Tryb: BACKTEST HISTORYCZNY");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Znaleziono {files.Length} plików do przetworzenia.\n");

            var results = new List<ModelResult>();

            var resultDir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data\Result");
            Directory.CreateDirectory(resultDir);
            var predictionPath = Path.Combine(resultDir, "predictions.csv");
            File.WriteAllText(predictionPath, "Date,Ticker,Prediction,Target\n");

            foreach (var path in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"SPÓŁKA: {fileName}");
                Console.WriteLine(new string('-', 70));

                try
                {
                    var data = mlContext.Data.LoadFromTextFile<MarketData>(
                        path: path,
                        hasHeader: true,
                        separatorChar: ',');

                    var dataEnum = mlContext.Data.CreateEnumerable<MarketData>(data, false).ToList();
                    if (dataEnum.Count < minTrainSize + 30)
                    {
                        Console.WriteLine("Za mało danych – pomijam.");
                        continue;
                    }

                    Console.WriteLine($"Wiersze: {dataEnum.Count}");
                    Console.WriteLine($"Target (zwrot 30d): min={(float)dataEnum.Min(x => x.Target):P2}, max={(float)dataEnum.Max(x => x.Target):P2}, avg={(float)dataEnum.Average(x => x.Target):P2}");

                    if (mode == RunMode.CurrentPrediction)
                    {
                        var split = mlContext.Data.TrainTestSplit(data, 0.2);
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

                        var trainTimer = Stopwatch.StartNew();
                        var model = pipeline.Fit(split.TrainSet);
                        trainTimer.Stop();

                        var predictions = model.Transform(split.TestSet);
                        var metrics = mlContext.Regression.Evaluate(predictions);

                        Console.WriteLine($"\nR²:   {(float)metrics.RSquared:0.####}");
                        Console.WriteLine($"MAE:  {(float)metrics.MeanAbsoluteError:0.####}");
                        Console.WriteLine($"RMSE: {(float)metrics.RootMeanSquaredError:0.####}");
                        Console.WriteLine($"Czas treningu: {trainTimer.ElapsedMilliseconds} ms");

                        var engine = mlContext.Model.CreatePredictionEngine<MarketData, PredictionResult>(model);
                        var sample = dataEnum.Last();
                        var prediction = engine.Predict(sample);

                        var parsedDate = DateTime.TryParse(sample.Date, out var date)
                            ? date.ToString("yyyy-MM-dd")
                            : "INVALID_DATE";

                        File.AppendAllText(predictionPath,
                            string.Format(CultureInfo.InvariantCulture,
                            "{0},{1},{2},{3}\n",
                            parsedDate, fileName, prediction.Score, sample.Target));

                        Console.WriteLine($"\nPrognozowany zwrot za 30 dni: {prediction.Score:P2}");

                        results.Add(new ModelResult
                        {
                            Ticker = fileName,
                            RSquared = (float)metrics.RSquared,
                            MAE = (float)metrics.MeanAbsoluteError,
                            RMSE = (float)metrics.RootMeanSquaredError,
                            Prediction = prediction.Score,
                            Rows = dataEnum.Count
                        });
                    }
                    else if (mode == RunMode.HistoricalBacktest)
                    {
                        int loopEnd = Math.Min(dataEnum.Count - 30, minTrainSize + maxBacktestPoints);
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

                        for (int i = minTrainSize; i < loopEnd; i++)
                        {
                            var trainSlice = dataEnum.Take(i).ToList();
                            var sample = dataEnum[i];

                            var tempData = mlContext.Data.LoadFromEnumerable(trainSlice);
                            var model = pipeline.Fit(tempData);
                            var engine = mlContext.Model.CreatePredictionEngine<MarketData, PredictionResult>(model);
                            var prediction = engine.Predict(sample);

                            var parsedDate = DateTime.TryParse(sample.Date, out var date)
                                ? date.ToString("yyyy-MM-dd")
                                : "INVALID_DATE";

                            File.AppendAllText(predictionPath,
                                string.Format(CultureInfo.InvariantCulture,
                                "{0},{1},{2},{3}\n",
                                parsedDate, fileName, prediction.Score, sample.Target));
                        }

                        Console.WriteLine($"Zakończono backtest historyczny dla {fileName} (przetworzono {loopEnd - minTrainSize} dat).\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przetwarzania pliku {fileName}: {ex.Message}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Zakończono. Całkowity czas wykonania: {stopwatch.Elapsed.TotalSeconds:N2} s");
        }
    }
}
