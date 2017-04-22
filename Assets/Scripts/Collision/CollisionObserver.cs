using System;

namespace AssemblyCSharp
{
	public interface CollisionObserver
	{
		CollisionType type();
		void onCollisionEnter();
		void onCollisionExit();
	}
}