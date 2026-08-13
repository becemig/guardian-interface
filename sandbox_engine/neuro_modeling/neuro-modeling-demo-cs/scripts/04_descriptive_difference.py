from pathlib import Path
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

accession = "GSE123456"
input_path = Path(f"data/processed/{accession}_probe_expression.csv")
out_dir = Path("reports")
figure_dir = Path("reports/figures")

out_dir.mkdir(parents=True, exist_ok=True)
figure_dir.mkdir(parents=True, exist_ok=True)

expression = pd.read_csv(input_path, index_col=0)

control = ["GSM3504366", "GSM3504367"]
overexpression = ["GSM3504368", "GSM3504369"]

missing = [sample for sample in control + overexpression if sample not in expression.columns]
if missing:
    raise ValueError(f"Expected sample columns not found: {missing}")

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
ranked.to_csv(out_dir / f"{accession}_descriptive_probe_differences.csv")

print("Top 20 probes by absolute mean difference:")
print(ranked.head(20).to_string())

top = ranked.head(20).copy()
top["probe"] = top.index.astype(str)
top = top.sort_values("mean_difference")

plt.figure(figsize=(10, 7))
sns.barplot(
    data=top,
    x="mean_difference",
    y="probe",
    hue="mean_difference",
    palette="vlag",
    legend=False,
)
plt.axvline(0, color="black", linewidth=0.8)
plt.xlabel("Mean expression difference: overexpression − control")
plt.ylabel("Microarray probe ID")
plt.title("GSE123456: largest descriptive probe-level differences")
plt.tight_layout()
plt.savefig(
    figure_dir / f"{accession}_top_probe_differences.png",
    dpi=200,
)
plt.close()
