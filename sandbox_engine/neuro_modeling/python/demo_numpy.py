from __future__ import annotations

import math
from pathlib import Path
import sys

import numpy as np


CURRENT_DIR = Path(__file__).resolve().parent
sys.path.insert(0, str(CURRENT_DIR))

from continuous_time_rnn_numpy import (
    ContinuousTimeRnn,
    ContinuousTimeRnnConfig,
)


def create_input(time_seconds: float) -> np.ndarray:
    """
    Example three-channel input:

    0: sinusoidal motion-like signal
    1: slower respiration-like signal
    2: temporary stimulus pulse
    """

    motion_signal = math.sin(
        2.0 * math.pi * 0.5 * time_seconds
    )

    respiration_signal = math.sin(
        2.0 * math.pi * 0.2 * time_seconds
    )

    pulse_signal = (
        1.0
        if 2.0 <= time_seconds <= 4.0
        else 0.0
    )

    return np.array(
        [
            motion_signal,
            respiration_signal,
            pulse_signal,
        ],
        dtype=np.float64,
    )


def main() -> None:
    config = ContinuousTimeRnnConfig(
        input_size=3,
        hidden_size=8,
        output_size=2,
        tau=0.5,
        integration_method="rk4",
        seed=42,
    )

    model = ContinuousTimeRnn(config)

    dt = 0.02
    duration_seconds = 8.0
    number_of_steps = int(duration_seconds / dt)

    print("Continuous-time RNN demonstration")
    print("=================================")
    print(f"dt: {dt}")
    print(f"tau: {config.tau}")
    print(f"integration: {config.integration_method}")
    print()

    for step_index in range(number_of_steps):
        time_seconds = step_index * dt
        input_vector = create_input(time_seconds)

        output = model.step(
            input_vector=input_vector,
            dt=dt,
        )

        if step_index % 25 == 0:
            print(
                f"time={time_seconds:5.2f} "
                f"input={np.round(input_vector, 3)} "
                f"output={np.round(output, 3)} "
                f"state_norm={model.state_norm():.4f}"
            )


if __name__ == "__main__":
    main()
