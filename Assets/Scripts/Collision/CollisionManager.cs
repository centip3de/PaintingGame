using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class CollisionManager
	{
		private readonly PlayerManager player;
		private readonly List<CollisionObserver> observers;

		private CollisionManager(Builder builder)
		{
			player = builder.player;	
			observers = builder.observers;
		}

		public class Builder {
			public readonly PlayerManager player;
			public readonly List<CollisionObserver> observers = new List<CollisionObserver>();

			public Builder(PlayerManager player) {
				this.player = player;
			}

			public Builder add(CollisionObserver observer) {
				observers.Add (observer);
				return this;
			}

			public CollisionManager build() {
				return new CollisionManager (this);
			}
		}

		public static Builder builder(PlayerManager player) {
			return new Builder(player);
		}

		public void enterNotify(CollisionType type) {
			observers.ForEach (delegate(CollisionObserver observer) {
				if (observer.type().Equals(type)) {
					observer.onCollisionEnter(player);
				}
			});
		}

		public void exitNotify(CollisionType type) {
			observers.ForEach (delegate(CollisionObserver observer) {
				if (observer.type().Equals(type)) {
					observer.onCollisionExit(player);
				}
			});
		}
	}
}