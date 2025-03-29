import os

RAW_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "RawData"))
CLEAN_DIR = os.path.join(RAW_DIR, "Clean")
os.makedirs(CLEAN_DIR, exist_ok=True)

# Nagłówki docelowe
new_header = "Date,Close,High,Low,Open,Volume\n"

for file in os.listdir(RAW_DIR):
    path = os.path.join(RAW_DIR, file)

    # pomiń folder Clean i inne nie-CSV
    if not file.endswith(".csv") or not os.path.isfile(path):
        continue

    with open(path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    if len(lines) < 4:
        print(f"⛔ Plik za krótki: {file}")
        continue

    # usuń 2 pierwsze linie (Price,... i Ticker,...), pomiń też "Date,,,,,"
    fixed_lines = [new_header] + lines[3:]  # linia 0,1,2 → skip; 3+ → dane

    # zapisz do Clean/
    output_path = os.path.join(CLEAN_DIR, file)
    with open(output_path, "w", encoding="utf-8") as f:
        f.writelines(fixed_lines)

    print(f"✅ Naprawiono: {file}")
