using System;

namespace ECS {

	public interface IComponentsList {
	
		void SetComponent(Entity entity, IComponent component);
		
		IComponent GetComponent(Entity entity);
	
	}

	public class ComponentsList<TComponent> : IComponentsList
		where TComponent : struct, IComponent
	{

		private const int StartSize = 64;

		public TComponent[] Elements = new TComponent[StartSize];

		private int size = StartSize;

		public void SetComponent(Entity entity, IComponent component) {
			int id = entity.Id;
			if (id >= size) {
				size = Utils.CeilPower2(id + 1);
				Array.Resize(ref Elements, size);
			}
			Elements[id] = (TComponent) component;
		}
		
		public IComponent GetComponent(Entity entity) {
			return (IComponent) Elements[entity.Id];
		}

	}

}
