import os
import pandas as pd
import matplotlib.pyplot as plt

# === ŚCIEŻKA DO PLIKU CSV ===
base_dir = os.path.dirname(__file__)
csv_path = os.path.join(base_dir, "..", "Result", "predictions.csv")

# === WCZYTANIE DANYCH ===
df = pd.read_csv(csv_path, parse_dates=["Date"])
df = df.dropna()
df = df[df["Prediction"] != 0]

# === METRYKI OGÓLNE ===
print("📊 ROZKŁAD PREDYKCJI I TARGETU")
print(df[["Prediction", "Target"]].describe())
print()

# === WYLICZENIE BŁĘDU ===
df["Error"] = df["Prediction"] - df["Target"]

# === WYKRES LINIOWY: PREDYKCJA vs TARGET vs BŁĄD ===
plt.figure(figsize=(14, 6))
plt.plot(df["Date"], df["Prediction"], label="Prediction", color="blue", linewidth=1)
plt.plot(df["Date"], df["Target"], label="Target", color="red", linewidth=1)
plt.plot(df["Date"], df["Error"], label="Prediction Error", color="gray", linestyle="--", linewidth=0.8)

plt.title("Porównanie predykcji, rzeczywistego zwrotu i błędu")
plt.xlabel("Data")
plt.ylabel("Zwrot (%)")
plt.legend()
plt.grid(True)
plt.tight_layout()
plt.show()
