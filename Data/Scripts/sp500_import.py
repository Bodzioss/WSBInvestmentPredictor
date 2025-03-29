import yfinance as yf
import pandas as pd
import os
import time

# Ścieżki względem katalogu skryptu
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
RAW_DIR = os.path.join(BASE_DIR, "..", "RawData")
os.makedirs(RAW_DIR, exist_ok=True)

# Pobierz listę spółek z S&P 500
def get_sp500_tickers():
    url = "https://en.wikipedia.org/wiki/List_of_S%26P_500_companies"
    tables = pd.read_html(url)
    tickers = tables[0]["Symbol"].tolist()
    tickers = [t.replace(".", "-") for t in tickers]  # np. BRK.B → BRK-B
    return tickers

tickers = get_sp500_tickers()
print(f"🔎 Znaleziono {len(tickers)} spółek w indeksie S&P 500.")

# Zakres dat – 20 lat
start_date = "2004-01-01"
end_date = "2024-12-31"

# Pobierz dane dla każdej spółki
for i, ticker in enumerate(tickers, 1):
    try:
        print(f"[{i}/{len(tickers)}] Pobieram dane dla {ticker}...")
        df = yf.download(ticker, start=start_date, end=end_date)

        if not df.empty:
            output_path = os.path.join(RAW_DIR, f"{ticker}.csv")
            df.to_csv(output_path)
        else:
            print(f"⚠️ Brak danych: {ticker}")

        time.sleep(1)  # zabezpieczenie przed limitami API
    except Exception as e:
        print(f"❌ Błąd przy {ticker}: {e}")
