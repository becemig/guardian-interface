from pathlib import Path
import pandas as pd

accession = "GSE123456"

input_path = Path(
    f"data/processed/{accession}_probe_expression.csv"
)

output_path = Path(
    f"data/processed/{accession}_biological_transcript_probes.csv"
)

if not input_path.exists():
    raise FileNotFoundError(
        f"Missing input file: {input_path}\n"
        "Run: python scripts/03_inspect_expression_matrix.py"
    )

expression = pd.read_csv(input_path, index_col=0)
expression.index = expression.index.astype(str)

is_affx_control = expression.index.str.startswith("AFFX-")

is_negative_control = expression.index.str.contains(
    r"(?:^|[-_])neg(?:[-_]|$)",
    case=False,
    regex=True,
)

is_transcript_cluster = expression.index.str.match(
    r"^TC\d+\.hg\.\d+$",
    case=False,
)

keep_biological_transcripts = (
    ~is_affx_control
    & ~is_negative_control
    & is_transcript_cluster
)

filtered = expression.loc[keep_biological_transcripts].copy()

removed_affx = int(is_affx_control.sum())
removed_negative = int(is_negative_control.sum())
removed_non_transcript = int((~is_transcript_cluster).sum())
removed_total = len(expression) - len(filtered)

filtered.to_csv(output_path)

print(f"Input probes: {len(expression):,}")
print(f"Retained transcript-cluster probes: {len(filtered):,}")
print(f"Removed total: {removed_total:,}")
print(f"  AFFX background/control probes: {removed_affx:,}")
print(f"  Negative-control probes: {removed_negative:,}")
print(f"  Non-transcript-cluster probes: {removed_non_transcript:,}")
print(f"Saved filtered matrix: {output_path}")

print("\nFirst five retained probe IDs:")
for probe_id in filtered.index[:5]:
    print(f"- {probe_id}")
