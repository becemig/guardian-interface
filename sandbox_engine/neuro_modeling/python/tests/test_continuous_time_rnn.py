from __future__ import annotations

from pathlib import Path
import sys
import unittest

import numpy as np


PYTHON_DIR = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PYTHON_DIR))

from continuous_time_rnn_numpy import (
    ContinuousTimeRnn,
    ContinuousTimeRnnConfig,
)


class TestContinuousTimeRnn(unittest.TestCase):
    def create_model(
        self,
        integration_method: str = "euler",
    ) -> ContinuousTimeRnn:
        config = ContinuousTimeRnnConfig(
            input_size=3,
            hidden_size=6,
            output_size=2,
            tau=0.5,
            integration_method=integration_method,
            seed=42,
        )

        return ContinuousTimeRnn(config)

    def test_initial_state_is_zero(self) -> None:
        model = self.create_model()

        np.testing.assert_allclose(
            model.state,
            np.zeros(6, dtype=np.float64),
        )

    def test_step_returns_correct_output_shape(self) -> None:
        model = self.create_model()

        output = model.step(
            np.array(
                [1.0, 0.0, -1.0],
                dtype=np.float64,
            ),
            dt=0.02,
        )

        self.assertEqual(output.shape, (2,))

    def test_reset_state_returns_to_zero(self) -> None:
        model = self.create_model()

        model.step(
            np.array(
                [1.0, 1.0, 1.0],
                dtype=np.float64,
            ),
            dt=0.02,
        )

        model.reset_state()

        np.testing.assert_allclose(
            model.state,
            np.zeros(6, dtype=np.float64),
        )

    def test_invalid_input_shape_raises_value_error(self) -> None:
        model = self.create_model()

        with self.assertRaises(ValueError):
            model.step(
                np.array(
                    [1.0, 2.0],
                    dtype=np.float64,
                ),
                dt=0.02,
            )

    def test_invalid_timestep_raises_value_error(self) -> None:
        model = self.create_model()

        with self.assertRaises(ValueError):
            model.step(
                np.zeros(3, dtype=np.float64),
                dt=0.0,
            )

    def test_euler_state_remains_finite(self) -> None:
        model = self.create_model("euler")

        input_vector = np.array(
            [0.5, -0.25, 0.75],
            dtype=np.float64,
        )

        for _ in range(1000):
            model.step(input_vector, dt=0.01)

        self.assertTrue(
            np.all(np.isfinite(model.state))
        )

    def test_rk4_state_remains_finite(self) -> None:
        model = self.create_model("rk4")

        input_vector = np.array(
            [0.5, -0.25, 0.75],
            dtype=np.float64,
        )

        for _ in range(1000):
            model.step(input_vector, dt=0.01)

        self.assertTrue(
            np.all(np.isfinite(model.state))
        )

    def test_zero_input_preserves_zero_state(self) -> None:
        model = self.create_model("rk4")

        output = model.step(
            np.zeros(3, dtype=np.float64),
            dt=0.02,
        )

        np.testing.assert_allclose(
            model.state,
            np.zeros(6, dtype=np.float64),
            atol=1e-12,
        )

        np.testing.assert_allclose(
            output,
            np.zeros(2, dtype=np.float64),
            atol=1e-12,
        )


if __name__ == "__main__":
    unittest.main(verbosity=2)
