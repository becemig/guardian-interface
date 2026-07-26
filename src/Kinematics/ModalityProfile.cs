using System.Collections.Generic;

namespace GuardianSuit.UDB {
    public sealed class ModalityProfile {
        public string ProfileId { get; init; } = "";
        public Dictionary<string, PidParameters> JointPidMap { get; init; } = new();
        public float TransitionBlendMs { get; init; } = 300f;
    }

    public sealed class PidParameters {
        public float Kp { get; init; }
        public float Ki { get; init; }
        public float Kd { get; init; }
    }
}