using System;

namespace AssemblyCSharp
{
	public class CollisionManager
	{
		private readonly List<CollisionObserver> observers;

		private CollisionManager(Builder builder)
		{
			observers = builder.observers;
		}

		public static class Builder {
			private const List<CollisionObserver> observers = new List<>();

			private Builder() {
			}

			public Builder add(CollisionObserver observer) {
				observers.add (observer);
				return this;
			}

			public CollisionManager build() {
				return new CollisionManager (this);
			}
		}

		public static Builder builder() {
			return new Builder();
		}

		public void enterNotify(CollisionType type) {
			observers.ForEach (delegate(CollisionObserver observer) {
				if (observer.type().Equals(type)) {
					observer.onCollisionEnter();
				}
			});
		}

		public void exitNotify(CollisionType type) {
			observers.ForEach (delegate(CollisionObserver observer) {
				if (observer.type().Equals(type)) {
					observer.onCollisionEnter();
				}
			});
		}
	}
}