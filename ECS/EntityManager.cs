using System;
using System.Collections.Generic;

namespace ECS {

	public struct ComponentGroupKey {
		
		public Type[] Types;
		
		private readonly int hash;

		private static int GenerateHash(Type[] types) {
			unchecked {
				int hash = 5381;
				foreach (var type in types) {
					hash = hash * 33 + type.GetHashCode();
				}
				return hash;
			}
		}

		public ComponentGroupKey(params Type[] types) {
			Types = types;
			hash = GenerateHash(types);
		}

		public override int GetHashCode() {
			return hash;
		}

		public override bool Equals(object obj) {
			return (obj is ComponentGroupKey) && hash == obj.GetHashCode();
		}

		public static bool operator == (ComponentGroupKey a, ComponentGroupKey b) {
			return a.hash == b.hash;
		}

		public static bool operator != (ComponentGroupKey a, ComponentGroupKey b) {
			return a.hash != b.hash;
		}

	}

	public class EntityManager {

		private readonly HashSet<Entity> entities = new HashSet<Entity>();

		private readonly Dictionary<Type, IComponentsList> componentLists = new Dictionary<Type, IComponentsList>();

		private readonly Dictionary<Entity, HashSet<Type>> entityComponentTypes = new Dictionary<Entity, HashSet<Type>>();

		private readonly Dictionary<ComponentGroupKey, IComponentGroup> componentGroups = new Dictionary<ComponentGroupKey, IComponentGroup>();


		private readonly List<Entity> destroyEntities = new List<Entity>();

		private int nextId = 1;

		public TComponentSystem AddSystem<TComponentSystem>(TComponentSystem system) where TComponentSystem : ComponentSystem {
			system.SetEntityManager(this);
			return system;
		}

		// todo: re-index entities Id when nextId is close to limit
		public Entity Create() {
			if (nextId >= int.MaxValue) {
				throw new InvalidOperationException(String.Format("Maximum entities Id \"{0}\" exceeded", int.MaxValue));
			}
			Entity entity = new Entity(nextId++);
			entityComponentTypes.Add(entity, new HashSet<Type>());
			entities.Add(entity);
			return entity;
		}

		public Entity Create(params IComponent[] components) {
			Entity entity = Create();
			AddComponents(entity, components);
			return entity;
		}

		public void Destroy(Entity entity) {
			if (entities.Contains(entity)) {
				destroyEntities.Add(entity);
			}
		}

		public void DestroyImmediate(Entity entity) {
			if (!entities.Contains(entity)) {
				return;
			}
			
			HashSet<Type> entityComponents = entityComponentTypes[entity];

			entities.Remove(entity);
			entityComponentTypes.Remove(entity);

			foreach (var group in componentGroups) {
				if (IsComponentTypesMatchGroup(entityComponents, group.Key)) {
					group.Value.RemoveEntity(entity);
				}
			}

			foreach (Type componentType in entityComponents) {
				componentLists[componentType].SetComponent(entity, Activator.CreateInstance(componentType) as IComponent);
			}
		}

		public void AddComponents(Entity entity, params IComponent[] components) {
			HashSet<Type> entityComponents = entityComponentTypes[entity];

			foreach (var component in components) {
				Type componentType = component.GetType();

				if (entityComponents.Contains(componentType)) {
					// component type duplicate
					continue;
				}
				entityComponents.Add(componentType);

				IComponentsList componentsList;
				if (!componentLists.TryGetValue(componentType, out componentsList)) {
					componentsList = (IComponentsList) Activator.CreateInstance(
						typeof(ComponentsList<>).MakeGenericType(componentType)
					);
					componentLists.Add(componentType, componentsList);
				}
				
				componentsList.SetComponent(entity, component);
			}

			foreach (var group in componentGroups) {
				if (IsComponentTypesMatchGroup(entityComponents, group.Key)) {
					group.Value.AddEntity(entity);
				}
			}
		}

		public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);

			HashSet<Type> entityComponents = entityComponentTypes[entity];
			if (entityComponents.Contains(componentType)) {
				return;
			}
			entityComponents.Add(componentType);
			
			IComponentsList componentsList;
			if (!componentLists.TryGetValue(componentType, out componentsList)) {
				componentsList = new ComponentsList<TComponent>();
				componentLists.Add(componentType, componentsList);
			}
			componentsList.SetComponent(entity, component);
			
			foreach (var group in componentGroups) {
				if (IsComponentTypesMatchGroup(entityComponents, group.Key)) {
					group.Value.AddEntity(entity);
				}
			}
		}

		public void RemoveComponent<TComponent>(Entity entity) where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);

			HashSet<Type> entityComponents = entityComponentTypes[entity];

			if (entityComponents.Contains(componentType)) {
				entityComponents.Remove(componentType);

				// clear component data
				componentLists[componentType].SetComponent(entity, (IComponent) Activator.CreateInstance(componentType));
				
				foreach (var group in componentGroups) {
					if (!IsComponentTypesMatchGroup(entityComponents, group.Key)) {
						group.Value.RemoveEntity(entity);
					}
				}
			}

			/* if (destroyComponents.Contains(componentsKey)) {
				return;
			} */
			// destroyComponents.Add(componentsKey);
		}

		public bool HasComponent<TComponent>(Entity entity) where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);
			return entityComponentTypes[entity].Contains(componentType);
		}

		public TComponent GetComponent<TComponent>(Entity entity) where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);
			
			if (!entityComponentTypes[entity].Contains(componentType)) {
				throw new KeyNotFoundException(String.Format("Entity \"{0}\" component doesn't exist", componentType));
			}

			var componentsList = (ComponentsList<TComponent>) componentLists[componentType];
			return componentsList.Elements[entity.Id];
		}

		public void SetComponent<TComponent>(Entity entity, TComponent component) where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);

			if (!entityComponentTypes[entity].Contains(componentType)) {
				throw new KeyNotFoundException(String.Format("Entity \"{0}\" component doesn't exist", componentType));
			}

			var componentsList = (ComponentsList<TComponent>) componentLists[componentType];
			componentsList.Elements[entity.Id] = component;
		}

		public IComponent[] GetComponents(Entity entity) {
			return entityComponentTypes[entity]
				.Select( componentType => componentLists[componentType].GetComponent(entity) )
				.ToArray();
		}

		private bool IsComponentTypesMatchGroup(HashSet<Type> componentTypes, ComponentGroupKey groupKey) {
			foreach (Type componentType in groupKey.Types) {
				if (!componentTypes.Contains(componentType)) {
					return false;
				}
			}
			return true;
		}

		// private void DestroyComponents(Entity entity) {
			/* foreach (Type componentType in entityComponentTypes[entity]) {
				componentsMap.Remove(new EntityComponentsKey(entity, componentType));
			} */
            
			// entityComponentTypes.Remove(entity);

			// // freeEntityIds.Push(entityId);

			// InspectComponentGroups(entity);
		// }

		public void UpdateDestroyed() {
			/* foreach (var componentsKey in destroyComponents) {
				if (componentsMap.ContainsKey(componentsKey)) {
					Entity entity = componentsKey.Key;
					Type componentType = componentsKey.Value;
					componentsMap.Remove(new EntityComponentsKey(entity, componentType));
					entityComponentTypes[entity].Remove(componentType);
					InspectComponentGroups(entity);
				}
			}
			destroyComponents.Clear(); */

			foreach (var entity in destroyEntities) {
				DestroyImmediate(entity);
			}
			destroyEntities.Clear();
		}

		public TComponentGroup GetComponentGroup<TComponentGroup>() where TComponentGroup : class, IComponentGroup {
			ComponentGroupKey groupKey = new ComponentGroupKey(typeof(TComponentGroup).GetGenericArguments());
	
			IComponentGroup group;
			if (componentGroups.TryGetValue(groupKey, out group)) {
				return (TComponentGroup) group;
			}

			TComponentGroup componentGroup = (TComponentGroup) Activator.CreateInstance(typeof(TComponentGroup), this);
			foreach (var componentTypes in entityComponentTypes) {
				Entity entity = componentTypes.Key;
				HashSet<Type> entityComponents = componentTypes.Value;
				if (IsComponentTypesMatchGroup(entityComponents, groupKey)) {
					componentGroup.AddEntity(entity);
				}
			}

			componentGroups.Add(groupKey, componentGroup);
			return componentGroup;
		}

		public ComponentsList<TComponent> GetComponentsList<TComponent>() where TComponent : struct, IComponent {
			Type componentType = typeof(TComponent);

			IComponentsList components;
			if (componentLists.TryGetValue(componentType, out components)) {
				return (ComponentsList<TComponent>) components;
			}

			var componentsList = new ComponentsList<TComponent>();
			componentLists.Add(componentType, componentsList);
			return componentsList;
		}

	}

}
