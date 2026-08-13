from pathlib import Path
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

accession = "GSE123456"
input_path = Path(f"data/processed/{accession}_probe_expression.csv")
figure_dir = Path("reports/figures")
figure_dir.mkdir(parents=True, exist_ok=True)

expression = pd.read_csv(input_path, index_col=0)

sample_labels = {
    "GSM3504366": "Control 1",
    "GSM3504367": "Control 2",
    "GSM3504368": "C6orf141 OE 1",
    "GSM3504369": "C6orf141 OE 2",
}

plot_data = expression.rename(columns=sample_labels)

plt.figure(figsize=(10, 6))
sns.boxplot(data=plot_data, palette=["#4C78A8", "#4C78A8", "#E45756", "#E45756"])
sns.stripplot(
    data=plot_data.sample(n=min(5000, len(plot_data)), random_state=42),
    color="black",
    alpha=0.08,
    size=1,
)
plt.ylabel("Processed microarray expression value")
plt.xlabel("")
plt.title("GSE123456: distribution of probe-level expression by sample")
plt.tight_layout()

output_path = figure_dir / f"{accession}_expression_distributions.png"
plt.savefig(output_path, dpi=200)
plt.close()

print(f"Saved QC figure: {output_path}")
