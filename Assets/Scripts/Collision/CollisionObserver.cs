using System;

namespace AssemblyCSharp
{
	public interface CollisionObserver
	{
		CollisionType type();
		void onCollisionEnter(PlayerManager player);
		void onCollisionExit(PlayerManager player);
	}
}