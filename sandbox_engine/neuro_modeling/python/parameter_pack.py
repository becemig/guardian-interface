from __future__ import annotations

from dataclasses import asdict
from pathlib import Path
import json

from continuous_time_rnn_numpy import ContinuousTimeRnnConfig


def save_parameter_pack(
    config: ContinuousTimeRnnConfig,
    path: Path,
) -> None:
    """
    Serialize a ContinuousTimeRnnConfig to JSON without weights.
    """
    payload = {
        "input_size": config.input_size,
        "hidden_size": config.hidden_size,
        "output_size": config.output_size,
        "tau": config.tau,
        "integration_method": config.integration_method,
        "seed": config.seed,
    }

    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        json.dumps(payload, indent=2),
        encoding="utf-8",
    )


def load_parameter_pack(path: Path) -> ContinuousTimeRnnConfig:
    """
    Load a ContinuousTimeRnnConfig from a JSON parameter pack.
    """
    data = json.loads(path.read_text(encoding="utf-8"))

    return ContinuousTimeRnnConfig(
        input_size=int(data["input_size"]),
        hidden_size=int(data["hidden_size"]),
        output_size=int(data["output_size"]),
        tau=float(data["tau"]),
        integration_method=str(data["integration_method"]),
        seed=int(data["seed"]),
    )
