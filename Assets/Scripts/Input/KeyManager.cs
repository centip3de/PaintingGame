using System;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class KeyManager : MonoBehaviour {

	public event EventHandler<KeyDownEvent> onKeyDown;
	public event EventHandler<KeyPressEvent> onKeyPress;

	private readonly Dictionary<KeyCode, KeyPressState> stateMap = new Dictionary<KeyCode, KeyPressState>();

	public void FixedUpdate() {
		foreach (int code in Enum.GetValues(typeof (KeyCode))) {
			KeyCode key = (KeyCode)code;

			if (Input.GetKey (key)) {
				keyDown (key);

				KeyPressState state = stateMap.ContainsKey (key)
					? stateMap [key]
					: KeyPressState.OFF;

				KeyPressState nextState = state.nextState ();
				
				if (state == KeyPressState.OFF) {
					keyPressed (key);
				}

				stateMap [key] = nextState;
			} else {
				stateMap [key] = KeyPressState.OFF;
			}
		}
	}

	private void keyPressed (KeyCode keyCode) {
		EventHandler<KeyPressEvent> handler = onKeyPress;

		if (handler != null) {
			KeyPressEvent ev = new KeyPressEvent (keyCode);
			handler (this, ev);
		}
	}

	private void keyDown (KeyCode keyCode) {
		EventHandler<KeyDownEvent> handler = onKeyDown;

		if (handler != null) {
			KeyDownEvent ev = new KeyDownEvent (keyCode);
			handler (this, ev);
		}
	}
}