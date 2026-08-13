from __future__ import annotations

from dataclasses import dataclass
from typing import Literal

import numpy as np
import numpy.typing as npt


Array = npt.NDArray[np.float64]


@dataclass
class ContinuousTimeRnnConfig:
    input_size: int
    hidden_size: int
    output_size: int
    tau: float = 1.0
    integration_method: Literal["euler", "rk4"] = "euler"
    seed: int = 42


class ContinuousTimeRnn:
    """
    Minimal NumPy continuous-time recurrent neural network.

    State equation:

        da/dt =
            (
                -a
                + W_rec tanh(a)
                + W_in x
                + b_hidden
            ) / tau

    Output equation:

        y = W_out tanh(a) + b_out
    """

    def __init__(self, config: ContinuousTimeRnnConfig) -> None:
        self._validate_config(config)

        self.config = config
        rng = np.random.default_rng(config.seed)

        input_scale = 1.0 / np.sqrt(config.input_size)
        recurrent_scale = 0.5 / np.sqrt(config.hidden_size)

        self.w_in: Array = rng.normal(
            loc=0.0,
            scale=input_scale,
            size=(config.hidden_size, config.input_size),
        )

        self.w_rec: Array = rng.normal(
            loc=0.0,
            scale=recurrent_scale,
            size=(config.hidden_size, config.hidden_size),
        )

        self.b_hidden: Array = np.zeros(
            config.hidden_size,
            dtype=np.float64,
        )

        self.w_out: Array = rng.normal(
            loc=0.0,
            scale=1.0 / np.sqrt(config.hidden_size),
            size=(config.output_size, config.hidden_size),
        )

        self.b_out: Array = np.zeros(
            config.output_size,
            dtype=np.float64,
        )

        self.state: Array = np.zeros(
            config.hidden_size,
            dtype=np.float64,
        )

    @staticmethod
    def _validate_config(config: ContinuousTimeRnnConfig) -> None:
        if config.input_size <= 0:
            raise ValueError("input_size must be greater than zero")

        if config.hidden_size <= 0:
            raise ValueError("hidden_size must be greater than zero")

        if config.output_size <= 0:
            raise ValueError("output_size must be greater than zero")

        if config.tau <= 0.0:
            raise ValueError("tau must be greater than zero")

        if config.integration_method not in {"euler", "rk4"}:
            raise ValueError(
                "integration_method must be 'euler' or 'rk4'"
            )

    def reset_state(self, state: Array | None = None) -> None:
        if state is None:
            self.state = np.zeros(
                self.config.hidden_size,
                dtype=np.float64,
            )
            return

        candidate = np.asarray(state, dtype=np.float64)

        expected_shape = (self.config.hidden_size,)

        if candidate.shape != expected_shape:
            raise ValueError(
                f"Expected state shape {expected_shape}, "
                f"received {candidate.shape}"
            )

        if not np.all(np.isfinite(candidate)):
            raise ValueError("state contains non-finite values")

        self.state = candidate.copy()

    @staticmethod
    def activation(value: Array) -> Array:
        return np.tanh(value)

    def derivative(
        self,
        state: Array,
        input_vector: Array,
    ) -> Array:
        recurrent_signal = (
            self.w_rec @ self.activation(state)
        )

        input_signal = self.w_in @ input_vector

        derivative = (
            -state
            + recurrent_signal
            + input_signal
            + self.b_hidden
        ) / self.config.tau

        return derivative

    def integrate_euler(
        self,
        state: Array,
        input_vector: Array,
        dt: float,
    ) -> Array:
        return (
            state
            + dt * self.derivative(state, input_vector)
        )

    def integrate_rk4(
        self,
        state: Array,
        input_vector: Array,
        dt: float,
    ) -> Array:
        k1 = self.derivative(state, input_vector)

        k2 = self.derivative(
            state + 0.5 * dt * k1,
            input_vector,
        )

        k3 = self.derivative(
            state + 0.5 * dt * k2,
            input_vector,
        )

        k4 = self.derivative(
            state + dt * k3,
            input_vector,
        )

        return state + (dt / 6.0) * (
            k1
            + 2.0 * k2
            + 2.0 * k3
            + k4
        )

    def step(
        self,
        input_vector: Array,
        dt: float,
    ) -> Array:
        if dt <= 0.0:
            raise ValueError("dt must be greater than zero")

        input_array = np.asarray(
            input_vector,
            dtype=np.float64,
        )

        expected_shape = (self.config.input_size,)

        if input_array.shape != expected_shape:
            raise ValueError(
                f"Expected input shape {expected_shape}, "
                f"received {input_array.shape}"
            )

        if not np.all(np.isfinite(input_array)):
            raise ValueError(
                "input_vector contains non-finite values"
            )

        if self.config.integration_method == "rk4":
            next_state = self.integrate_rk4(
                self.state,
                input_array,
                dt,
            )
        else:
            next_state = self.integrate_euler(
                self.state,
                input_array,
                dt,
            )

        if not np.all(np.isfinite(next_state)):
            raise FloatingPointError(
                "Continuous-time RNN state became non-finite"
            )

        self.state = next_state

        return self.output()

    def output(self) -> Array:
        output = (
            self.w_out @ self.activation(self.state)
            + self.b_out
        )

        if not np.all(np.isfinite(output)):
            raise FloatingPointError(
                "Continuous-time RNN output became non-finite"
            )

        return output

    def state_norm(self) -> float:
        return float(np.linalg.norm(self.state))
