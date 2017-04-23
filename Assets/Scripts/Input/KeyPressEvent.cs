using System;
using UnityEngine;

namespace AssemblyCSharp {
	
	public class KeyPressEvent : EventArgs {
		
		public KeyPressEvent (KeyCode keyCode) {
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