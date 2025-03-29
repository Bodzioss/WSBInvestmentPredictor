import os

input_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'TargetData'))

for filename in os.listdir(input_dir):
    if filename.endswith(".csv"):
        path = os.path.join(input_dir, filename)

        with open(path, "r", encoding="utf-8") as f:
            lines = f.readlines()

        # Usuń linie 2 i 3
        cleaned = [lines[0]] + lines[3:]

        with open(path, "w", encoding="utf-8") as f:
            f.writelines(cleaned)

        print(f"✅ Wyczyszczono: {filename}")
