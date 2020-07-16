using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace ECS {

	public interface IComponent { }

	[AttributeUsage(AttributeTargets.Field)]
	public class InjectAttribute : Attribute {

		public override bool Equals(object obj) {
			return (obj is InjectAttribute);
		}

		public override int GetHashCode() {
			return 0;
		}

	}

	public abstract class ComponentSystem {

		protected EntityManager Manager;

		public virtual void SetEntityManager(EntityManager manager) {
			Manager = manager;

			List<Type> types = new List<Type>();
			Type type = GetType();
			while (type != typeof(object) && type != typeof(ComponentSystem)) {
				types.Add(type);
				type = type.BaseType;
			}
			
			var injectFields = types.SelectMany( t => t
				.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				.Where(field => field.GetCustomAttributes(typeof(InjectAttribute), false).Any())
			);

			var injectGroupMethod = manager.GetType().GetMethod("GetComponentGroup");
			var injectComponentsMethod = manager.GetType().GetMethod("GetComponentsList");

			foreach (var field in injectFields) {
				if (field.FieldType.GetGenericTypeDefinition() == typeof(ComponentsList<>)) {
					field.SetValue(this, injectComponentsMethod.MakeGenericMethod(field.FieldType.GenericTypeArguments[0]).Invoke(manager, new object[] { } ));
				} else {
					field.SetValue(this, injectGroupMethod.MakeGenericMethod(field.FieldType).Invoke(manager, new object[] { } ));
				}
			}

			OnInit();
		}

		protected virtual void OnInit() { }

	}

}
