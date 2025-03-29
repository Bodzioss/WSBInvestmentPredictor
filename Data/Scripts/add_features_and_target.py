import os
import pandas as pd
import numpy as np
import ta  # pip install ta - biblioteka do analizy technicznej

# 🔧 Ścieżki do katalogów danych
RAW_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "RawData"))
OUTPUT_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "TargetData"))
os.makedirs(OUTPUT_DIR, exist_ok=True)

# 📁 Przetwarzamy każdy plik CSV w katalogu RawData/
for file in os.listdir(RAW_DIR):
    if not file.endswith(".csv"):
        continue

    path = os.path.join(RAW_DIR, file)
    df = pd.read_csv(path)

    # ⛔ Pomijamy zbyt krótkie lub uszkodzone pliki
    if df.shape[0] < 60 or 'Close' not in df.columns:
        continue

    # 📅 Sortujemy dane chronologicznie
    df.sort_values("Date", inplace=True)

    # -------------------------------
    # 🔍 GENEROWANIE CECH (features)
    # -------------------------------

    # 📈 Średnie kroczące: SMA z 5, 10 i 20 dni
    df["SMA_5"] = df["Close"].rolling(5).mean()
    df["SMA_10"] = df["Close"].rolling(10).mean()
    df["SMA_20"] = df["Close"].rolling(20).mean()

    # 📊 Zmienność – odchylenie standardowe z ostatnich 10 dni
    df["Volatility_10"] = df["Close"].rolling(10).std()

    # 🔄 RSI (Relative Strength Index) – momentum rynku
    df["RSI_14"] = ta.momentum.RSIIndicator(close=df["Close"], window=14).rsi()

    # -------------------------------
    # 🎯 GENEROWANIE CELU (Target)
    # -------------------------------

    # Target = zwrot 30-dniowy (% zmiana ceny Close)
    df["Target"] = (df["Close"].shift(-30) - df["Close"]) / df["Close"]

    # 🧹 Usuwamy wiersze z brakującymi wartościami
    df.dropna(inplace=True)

    # 🧾 Wybieramy tylko potrzebne kolumny do ML.NET
    df_out = df[[
        "Open", "High", "Low", "Close", "Volume",
        "SMA_5", "SMA_10", "SMA_20", "Volatility_10", "RSI_14",
        "Target"
    ]]

    # 💾 Zapis do TargetData/
    df_out.to_csv(os.path.join(OUTPUT_DIR, file), index=False)
    print(f"✅ Przetworzono: {file}")
