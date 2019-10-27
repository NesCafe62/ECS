using System;

namespace ECS {

	public struct Entity : IEquatable<Entity> {
		
		public readonly int Id;

		public Entity(int id) {
			Id = id;
		}

		public override int GetHashCode() {
			return Id;
		}

		public override bool Equals(object obj) {
			return (obj is Entity) && Equals((Entity) obj);
		}

		public bool Equals(Entity entity) {
			return Id == entity.Id;
		}

		public static bool operator == (Entity a, Entity b) {
			return a.Equals(b);
		}

		public static bool operator != (Entity a, Entity b) {
			return !a.Equals(b);
		}

		public static readonly Entity None = new Entity(0);
		
		public bool IsNone {
			get { return Id == 0; }
		}

		public override string ToString() {
			return String.Format("[Entity #{0}]", Id);
		}

	}

}
