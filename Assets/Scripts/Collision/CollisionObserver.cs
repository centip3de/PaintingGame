using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public interface CollisionObserver
	{
		CollisionType type();

		void onCollisionEnter(PlayerManager player, Collider2D coll);
		void onCollisionStay(PlayerManager player, Collider2D coll);
		void onCollisionExit(PlayerManager player, Collider2D coll);
	}
}