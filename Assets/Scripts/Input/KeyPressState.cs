using System;

namespace AssemblyCSharp {
	public static class Extensions {
		public static KeyPressState nextState(this KeyPressState state) {
			return state + 1 < KeyPressState.HOLD
				? state + 1
				: KeyPressState.HOLD;
		}
	}

	public enum KeyPressState {
		OFF = 0,
		FIRST = 1,
		HOLD = 2
	}
}