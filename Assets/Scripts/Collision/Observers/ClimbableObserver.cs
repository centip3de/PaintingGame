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
			Physics2D.gravity = Vector2.zero;
		}

		public void onCollisionExit(PlayerManager player) {
			player.isClimbing = false;
			Physics2D.gravity = player.gravity;
		}
	}
}