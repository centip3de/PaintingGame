using System;
using UnityEngine;

namespace AssemblyCSharp {
	
	public class PaintingObserver : CollisionObserver {
		public CollisionType type() {
			return CollisionType.PAINTING;
		}

		public void onCollisionEnter(PlayerManager player, Collider2D coll) {
			string paintingName = coll.gameObject.name.Replace(" Painting Physic", "");
			GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().nextLevel(paintingName);
		}

		public void onCollisionStay(PlayerManager player, Collider2D coll) {
		}

		public void onCollisionExit(PlayerManager player, Collider2D coll) {
		}
	}
}
