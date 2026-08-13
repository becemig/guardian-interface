from pathlib import Path
from io import StringIO
import gzip
import pandas as pd

accession = "GSE123456"
input_path = Path(f"data/raw/geo/{accession}_series_matrix.txt.gz")
out_dir = Path("data/processed")
out_dir.mkdir(parents=True, exist_ok=True)

if not input_path.exists():
    raise FileNotFoundError(f"Missing source file: {input_path}")

table_lines = []
inside_table = False

with gzip.open(input_path, "rt", encoding="utf-8", errors="replace") as handle:
    for line in handle:
        if line.startswith("!series_matrix_table_begin"):
            inside_table = True
            continue

        if line.startswith("!series_matrix_table_end"):
            break

        if inside_table:
            table_lines.append(line)

if not table_lines:
    raise ValueError("Could not locate the series matrix table.")

expression = pd.read_csv(
    StringIO("".join(table_lines)),
    sep="\t",
    quotechar='"',
    index_col=0,
)

expression.index.name = "probe_id"

for column in expression.columns:
    expression[column] = pd.to_numeric(expression[column], errors="coerce")

output_path = out_dir / f"{accession}_probe_expression.csv"
expression.to_csv(output_path)

print("Expression matrix shape:", expression.shape)
print("\nSample columns:")
print(expression.columns.tolist())

print("\nFirst five probes:")
print(expression.head().to_string())

print("\nPer-sample summary:")
print(expression.describe().T[["mean", "std", "min", "max"]].to_string())

print("\nMissing values by sample:")
print(expression.isna().sum().to_string())

print(f"\nSaved expression matrix: {output_path}")
