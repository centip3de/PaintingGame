using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ClimbableObserver : CollisionObserver
	{
		public ClimbableObserver ()
		{
		}

		public CollisionType type() {
			return CollisionType.CLIMBABLE;
		}

		public void onCollisionEnter(PlayerManager player) {
			player.isClimbing = true;

			Rigidbody2D body = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D> ();
			body.gravityScale = 0;
		}

		public void onCollisionExit(PlayerManager player) {
			player.isClimbing = false;

			Rigidbody2D body = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D> ();
			body.gravityScale = 1;
		}
	}
}