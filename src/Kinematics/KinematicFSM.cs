using System;
using System.Threading.Tasks;

namespace GuardianSuit.UDB {
    public sealed class KinematicFSM {
        private ModalityProfile _activeProfile;

        public KinematicFSM(ModalityProfile initial) => _activeProfile = initial;

        public async Task SwitchProfile(ModalityProfile next) {
            // 1. Lock joint movement
            // 2. Blend transition
            // 3. Commit new profile
            await Task.Delay(100);
        }
    }
}