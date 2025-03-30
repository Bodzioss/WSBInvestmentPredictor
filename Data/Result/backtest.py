import pandas as pd
import matplotlib.pyplot as plt

# Wczytaj predykcje
df = pd.read_csv("predictions.csv", parse_dates=["Date"])
df = df.sort_values("Date")

# Parametry strategii
threshold = 0.05  # prognoza zwrotu > 5%

# Decyzja: kup jeśli prognoza > próg
df["Decision"] = df["Prediction"] > threshold

# Zysk: jeśli kupiliśmy → Target, inaczej 0
df["Profit"] = df["Target"].where(df["Decision"], 0)

# Kapitał początkowy i krzywa kapitału
df["Equity"] = (1 + df["Profit"]).cumprod()

# Wyniki
total_return = df["Equity"].iloc[-1] - 1
win_rate = df[df["Decision"]]["Profit"].gt(0).mean()
num_trades = df["Decision"].sum()

print("\n===== WYNIKI BACKTESTU =====")
print(f"Liczba transakcji: {num_trades}")
print(f"Skuteczność (trafność): {win_rate:.2%}")
print(f"Zysk końcowy: {total_return:.2%}")
print("============================")

# Wykres
plt.figure(figsize=(10, 4))
plt.plot(df["Date"], df["Equity"], label="Kapitał")
plt.title("Equity Curve - Strategia na bazie AI")
plt.xlabel("Data")
plt.ylabel("Kapitał")
plt.grid()
plt.legend()
plt.tight_layout()
plt.show()
