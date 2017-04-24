using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ClimbableObserver : CollisionObserver
	{
        private float playerGravScale;

		public ClimbableObserver ()
        {
		}

		public CollisionType type() {
			return CollisionType.CLIMBABLE;
		}

		public void onCollisionEnter(PlayerManager player, Collider2D coll) {
			player.isClimbing = true;

			Rigidbody2D body = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D> ();

			body.velocity = Vector2.zero;
            playerGravScale = body.gravityScale;
			body.gravityScale = 0;
		}

		public void onCollisionStay(PlayerManager player, Collider2D coll) {
		}

		public void onCollisionExit(PlayerManager player, Collider2D coll) {
			player.isClimbing = false;

			Rigidbody2D body = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D> ();
            body.gravityScale = playerGravScale;
		}
	}
}