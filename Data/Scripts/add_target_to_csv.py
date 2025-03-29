import pandas as pd
import os

# Ścieżki względem lokalizacji skryptu
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
RAW_DIR = os.path.join(BASE_DIR, "..", "RawData")
TARGET_DIR = os.path.join(BASE_DIR, "..", "TargetData")

os.makedirs(TARGET_DIR, exist_ok=True)

# Parametr: ile dni w przód ma przewidywać model
days_forward = 30

for filename in os.listdir(RAW_DIR):
    if filename.endswith(".csv"):
        path = os.path.join(RAW_DIR, filename)
        df = pd.read_csv(path)

        if "Close" in df.columns:
            # Dodaj kolumnę Target (np. Close za 30 dni)
            df["Target"] = df["Close"].shift(-days_forward)
            df.dropna(subset=["Target"], inplace=True)

            # Zapisz do TargetData/
            output_path = os.path.join(TARGET_DIR, filename)
            df.to_csv(output_path, index=False)
            print(f"✅ Zapisano: {filename}")
