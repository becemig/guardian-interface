from pathlib import Path
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

accession = "GSE123456"
input_path = Path(f"data/processed/{accession}_biological_transcript_probes.csv")
out_dir = Path("reports")
figure_dir = out_dir / "figures"

if not input_path.exists():
    raise FileNotFoundError(
        f"Missing {input_path}. Run scripts/06_filter_technical_controls.py first."
    )

out_dir.mkdir(parents=True, exist_ok=True)
figure_dir.mkdir(parents=True, exist_ok=True)

expression = pd.read_csv(input_path, index_col=0)

control = ["GSM3504366", "GSM3504367"]
overexpression = ["GSM3504368", "GSM3504369"]

result = pd.DataFrame(index=expression.index)
result["control_mean"] = expression[control].mean(axis=1)
result["overexpression_mean"] = expression[overexpression].mean(axis=1)
result["mean_difference"] = (
    result["overexpression_mean"] - result["control_mean"]
)
result["control_sd"] = expression[control].std(axis=1)
result["overexpression_sd"] = expression[overexpression].std(axis=1)
result["absolute_mean_difference"] = result["mean_difference"].abs()

ranked = result.sort_values("absolute_mean_difference", ascending=False)

table_path = out_dir / f"{accession}_biological_transcript_differences.csv"
ranked.to_csv(table_path)

print("Top 20 retained transcript-cluster probes:")
print(ranked.head(20).to_string())

top = ranked.head(20).copy()
top["transcript_cluster_id"] = top.index
top = top.sort_values("mean_difference")

plt.figure(figsize=(10, 7))
sns.barplot(
    data=top,
    x="mean_difference",
    y="transcript_cluster_id",
    hue="mean_difference",
    palette="vlag",
    legend=False,
)
plt.axvline(0, color="black", linewidth=0.8)
plt.xlabel("Mean expression difference: C6orf141 OE − control")
plt.ylabel("Clariom D transcript-cluster ID")
plt.title("GSE123456: largest descriptive biological transcript differences")
plt.tight_layout()

figure_path = figure_dir / f"{accession}_biological_transcript_differences.png"
plt.savefig(figure_path, dpi=200)
plt.close()

print(f"\nSaved table: {table_path}")
print(f"Saved figure: {figure_path}")
