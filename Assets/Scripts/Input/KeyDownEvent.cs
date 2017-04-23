using System;
using UnityEngine;

namespace AssemblyCSharp {

	public class KeyDownEvent : EventArgs {

		public KeyDownEvent (KeyCode keyCode) {
			code = keyCode;
		}

		private KeyCode code;

		public KeyCode keyCode {
			get {
				return code;
			}
		}
	}
}