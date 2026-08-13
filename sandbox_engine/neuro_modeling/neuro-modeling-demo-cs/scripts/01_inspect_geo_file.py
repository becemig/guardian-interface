from pathlib import Path
import gzip

path = Path("data/raw/geo/GSE123456_series_matrix.txt.gz")

if not path.exists():
    raise FileNotFoundError(f"File not found: {path}")

with gzip.open(path, "rt", encoding="utf-8", errors="replace") as handle:
    for index, line in enumerate(handle):
        print(line.rstrip())
        if index >= 79:
            break
