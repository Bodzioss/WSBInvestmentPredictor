import os
import pandas as pd
import ta

RAW_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "RawData"))
OUTPUT_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "TargetData"))
os.makedirs(OUTPUT_DIR, exist_ok=True)

for file in os.listdir(RAW_DIR):
    if not file.endswith(".csv"):
        continue

    path = os.path.join(RAW_DIR, file)
    df = pd.read_csv(path)

    if df.shape[0] < 60 or 'Close' not in df.columns:
        continue

    df["Date"] = pd.to_datetime(df["Date"]).dt.strftime("%Y-%m-%d")
    df = df.sort_values("Date")

    df["SMA_5"] = df["Close"].rolling(5).mean()
    df["SMA_10"] = df["Close"].rolling(10).mean()
    df["SMA_20"] = df["Close"].rolling(20).mean()
    df["Volatility_10"] = df["Close"].rolling(10).std()
    df["RSI_14"] = ta.momentum.RSIIndicator(close=df["Close"], window=14).rsi()
    df["Target"] = (df["Close"].shift(-30) - df["Close"]) / df["Close"]

    df.dropna(inplace=True)

    df_out = df[[
        "Date", "Open", "High", "Low", "Close", "Volume",
        "SMA_5", "SMA_10", "SMA_20", "Volatility_10", "RSI_14", "Target"
    ]]

    df_out.to_csv(os.path.join(OUTPUT_DIR, file), index=False)
    print(f"✅ Przetworzono: {file}")
