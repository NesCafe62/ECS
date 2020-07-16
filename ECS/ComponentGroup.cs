using System.Collections.Generic;

namespace ECS {

	public interface IComponentGroup {
	
		void AddEntity(Entity entity);

		void RemoveEntity(Entity entity);

	}

	public abstract class BaseComponentGroup : IComponentGroup {

		protected List<Entity> Entities = new List<Entity>();

		protected Dictionary<Entity, int> EntitiesIndex = new Dictionary<Entity, int>();

		public IEnumerable<Entity> GetEntities() {
			return Entities;
		}

		public void AddEntity(Entity entity) {
			if (EntitiesIndex.ContainsKey(entity)) {
				return;
			}

			int index = Entities.Count;
			Entities.Add(entity);
			EntitiesIndex.Add(entity, index);
			
			if (OnEntityAdd != null) {
				OnEntityAdd(entity);
			}
		}

		public void RemoveEntity(Entity entity) {
			if (!EntitiesIndex.ContainsKey(entity)) {
				return;
			}

			int index = EntitiesIndex[entity];
			int lastIndex = Entities.Count - 1;
			
			Entity lastEntity = Entities[lastIndex];
			Entities[index] = lastEntity;
			Entities.RemoveAt(lastIndex);

			EntitiesIndex[lastEntity] = index;
			EntitiesIndex.Remove(entity);

			if (OnEntityRemove != null) {
				OnEntityRemove(entity);
			}
		}

		public bool HasEntity(Entity entity) {
			return EntitiesIndex.ContainsKey(entity);
		}

		public delegate void EntityEvent(Entity entity);

		private event EntityEvent OnEntityAdd;

		private event EntityEvent OnEntityRemove;

		public void SubscribeEntityAdd(EntityEvent entityAddEvent) {
			OnEntityAdd += entityAddEvent;
		}

		public void UnSubscribeEntityAdd(EntityEvent entityAddEvent) {
			OnEntityAdd -= entityAddEvent;
		}

		public void SubscribeEntityRemove(EntityEvent entityRemoveEvent) {
			OnEntityRemove += entityRemoveEvent;
		}

		public void UnSubscribeEntityRemove(EntityEvent entityRemoveEvent) {
			OnEntityRemove -= entityRemoveEvent;
		}

	}

	public class ComponentGroup<T> : BaseComponentGroup
		where T : struct, IComponent
	{

		private readonly ComponentsList<T> components;

		public ComponentGroup(EntityManager entityManager) {
			components = entityManager.GetComponentsList<T>();
		}

		public delegate void IterateCallback(Entity entity, ref T component);

		public delegate void SingleCallback(ref T component);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(e, ref components.Elements[e.Id]);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(ref components.Elements[id]);
		}

	}

	public class ComponentGroup<T1, T2> : BaseComponentGroup
		where T1 : struct, IComponent where T2 : struct, IComponent
	{

		private readonly ComponentsList<T1> components1;
		private readonly ComponentsList<T2> components2;

		public ComponentGroup(EntityManager entityManager) {
			components1 = entityManager.GetComponentsList<T1>();
			components2 = entityManager.GetComponentsList<T2>();
		}

		public delegate void IterateCallback(
			Entity entity,
			ref T1 c1, ref T2 c2
		);

		public delegate void SingleCallback(
			ref T1 c1, ref T2 c2
		);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(e, ref components1.Elements[e.Id], ref components2.Elements[e.Id]);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(ref components1.Elements[id], ref components2.Elements[id]);
		}

	}

	public class ComponentGroup<T1, T2, T3> : BaseComponentGroup
		where T1 : struct, IComponent where T2 : struct, IComponent
		where T3 : struct, IComponent
	{

		private readonly ComponentsList<T1> components1;
		private readonly ComponentsList<T2> components2;
		private readonly ComponentsList<T3> components3;

		public ComponentGroup(EntityManager entityManager) {
			components1 = entityManager.GetComponentsList<T1>();
			components2 = entityManager.GetComponentsList<T2>();
			components3 = entityManager.GetComponentsList<T3>();
		}

		public delegate void IterateCallback(
			Entity entity,
			ref T1 c1, ref T2 c2, ref T3 c3
		);

		public delegate void SingleCallback(
			ref T1 c1, ref T2 c2, ref T3 c3
		);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(
					e, ref components1.Elements[e.Id], ref components2.Elements[e.Id],
					ref components3.Elements[e.Id]
				);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(
				ref components1.Elements[id], ref components2.Elements[id],
				ref components3.Elements[id]
			);
		}

	}

	public class ComponentGroup<T1, T2, T3, T4> : BaseComponentGroup
		where T1 : struct, IComponent where T2 : struct, IComponent
		where T3 : struct, IComponent where T4 : struct, IComponent
	{

		private readonly ComponentsList<T1> components1;
		private readonly ComponentsList<T2> components2;
		private readonly ComponentsList<T3> components3;
		private readonly ComponentsList<T4> components4;

		public ComponentGroup(EntityManager entityManager) {
			components1 = entityManager.GetComponentsList<T1>();
			components2 = entityManager.GetComponentsList<T2>();
			components3 = entityManager.GetComponentsList<T3>();
			components4 = entityManager.GetComponentsList<T4>();
		}

		public delegate void IterateCallback(
			Entity entity,
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4
		);

		public delegate void SingleCallback(
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4
		);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(
					e, ref components1.Elements[e.Id], ref components2.Elements[e.Id],
					ref components3.Elements[e.Id], ref components4.Elements[e.Id]
				);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(
				ref components1.Elements[id], ref components2.Elements[id],
				ref components3.Elements[id], ref components4.Elements[id]
			);
		}

	}
	
	public class ComponentGroup<T1, T2, T3, T4, T5> : BaseComponentGroup
		where T1 : struct, IComponent where T2 : struct, IComponent
		where T3 : struct, IComponent where T4 : struct, IComponent
		where T5 : struct, IComponent
	{

		private readonly ComponentsList<T1> components1;
		private readonly ComponentsList<T2> components2;
		private readonly ComponentsList<T3> components3;
		private readonly ComponentsList<T4> components4;
		private readonly ComponentsList<T5> components5;

		public ComponentGroup(EntityManager entityManager) {
			components1 = entityManager.GetComponentsList<T1>();
			components2 = entityManager.GetComponentsList<T2>();
			components3 = entityManager.GetComponentsList<T3>();
			components4 = entityManager.GetComponentsList<T4>();
			components5 = entityManager.GetComponentsList<T5>();
		}

		public delegate void IterateCallback(
			Entity entity,
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4,
			ref T5 c5
		);

		public delegate void SingleCallback(
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4,
			ref T5 c5
		);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(
					e, ref components1.Elements[e.Id], ref components2.Elements[e.Id],
					ref components3.Elements[e.Id], ref components4.Elements[e.Id],
					ref components5.Elements[e.Id]
				);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(
				ref components1.Elements[id], ref components2.Elements[id],
				ref components3.Elements[id], ref components4.Elements[id],
				ref components5.Elements[id]
			);
		}

	}

	public class ComponentGroup<T1, T2, T3, T4, T5, T6> : BaseComponentGroup
		where T1 : struct, IComponent where T2 : struct, IComponent
		where T3 : struct, IComponent where T4 : struct, IComponent
		where T5 : struct, IComponent where T6 : struct, IComponent
	{

		private readonly ComponentsList<T1> components1;
		private readonly ComponentsList<T2> components2;
		private readonly ComponentsList<T3> components3;
		private readonly ComponentsList<T4> components4;
		private readonly ComponentsList<T5> components5;
		private readonly ComponentsList<T6> components6;

		public ComponentGroup(EntityManager entityManager) {
			components1 = entityManager.GetComponentsList<T1>();
			components2 = entityManager.GetComponentsList<T2>();
			components3 = entityManager.GetComponentsList<T3>();
			components4 = entityManager.GetComponentsList<T4>();
			components5 = entityManager.GetComponentsList<T5>();
			components6 = entityManager.GetComponentsList<T6>();
		}

		public delegate void IterateCallback(
			Entity entity,
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4,
			ref T5 c5, ref T6 c6
		);

		public delegate void SingleCallback(
			ref T1 c1, ref T2 c2,
			ref T3 c3, ref T4 c4,
			ref T5 c5, ref T6 c6
		);

		public void Each(IterateCallback func) {
			foreach (var e in Entities) {
				func(
					e, ref components1.Elements[e.Id], ref components2.Elements[e.Id],
					ref components3.Elements[e.Id], ref components4.Elements[e.Id],
					ref components5.Elements[e.Id], ref components6.Elements[e.Id]
				);
			}
		}

		public void One(Entity entity, SingleCallback func) {
			int id = entity.Id;
			func(
				ref components1.Elements[id], ref components2.Elements[id],
				ref components3.Elements[id], ref components4.Elements[id],
				ref components5.Elements[id], ref components6.Elements[id]
			);
		}

	}

}
