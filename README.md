# ECS
Simple entity component system library wtitten in C#

# Usage

## Define components
```cs
public struct Position : IComponent {

	public float X;
	public float Y;
	
	public Position(float x, float y) {
		X = x;
		Y = y;
	}

}

public struct Velocity : IComponent {

	public float X;
	public float Y;
	
	public Position(float x, float y) {
		X = x;
		Y = y;
	}

}

public struct Decay : IComponent {
	
	public double Time;
	
	public Position(double time) {
		Time = time;
	}

}
```

## Define Systems

You can inject multiple ComponentGroup's. Also injecting of ComponentsList's is available.

Remember that your components are struct's so any modification should be performed on `ComponentsList.Elements` array directly, or by ref in delegate functions. (There are ref variables implemented since .Net 7 which could help. But I wanted to have compatibility with older .Net versions)

ComponentGroup's can consist of up to 6 components.
They will contain all entities that have components of given types (same time entity can have more components of other types).
ComponentGroup's are updated automatically whenever entities for EntityManager get added/removed or entity components added/removed.

There are also entity added/removed events for ComponentGroup
```cs
public class MovementSystem : ComponentSystem {
	
	[Inject]
	protected ComponentGroup<Position, Velocity> Entities;
	
	[Inject]
	protected ComponentsList<Position> Positions;
	
	public void Update() {
		Entities.Each(( Entity entity, ref Position position, ref Velocity velocity) => {
			position.X += Velocity.X;
			position.Y += Velocity.Y;
		});
	}
	
	public Position GetPosition(Entity entity) {
		if (!Entities.HasEntity(entity)) {
			// or !Manager.HasComponent<Position>(entity) if you have no ComponentGroup with that component
			throw new Exception("Entity \"Position\" component doesn't exist");
		}
		return Positions.Elements[entity.Id];
	}

}

public class DecaySystem : ComponentSystem {
	
	[Inject]
	protected ComponentGroup<Decay> Entities;
	
	private double time;
	
	protected override void OnInit() {
		Entities.SubscribeEntityAdd(OnEntityAdd);
		Entities.SubscribeEntityRemove(OnEntityRemove);
	}
	
	protected void OnEntityAdd(Entity entity) {
		// Entity with Decay component added
	}
	
	protected void OnEntityRemove(Entity entity) {
		// Entity with Decay component removed
	}
	
	public void Update(double dTime) {
		time += dTime;
		Entities.Each(( Entity entity, ref Decay decay) => {
			if (time > decay.Time) {
				Manager.Destroy(entity);
			}
		});
	}

}
```

## Create systems and entities
```cs
public class World {
	
	private EntityManager manager = new EntityManager();

	private MovementSystem movementSystem;
	private DecaySystem decaySystem;

	public World() {
		movementSystem = manager.AddSystem(new MovementSystem());
		decaySystem = manager.AddSystem(new DecaySystem());
		
		var entity1 = CreateEntity(5, 5, 0, 0);
		var entity2 = CreateEntity(5, 10, 1.5f, -0.5f);
		manager.AddComponent(entity2, new Decay(10));

		// Add multiple components
		// manager.AddComponents(entity1, new Decay(10), new OtherComponent());

		// Remove component
		// manager.RemoveComponent<Decay>(entity2);
		
		// Check if entity has a component. Entity can have only one component of each type
		// manager.HasComponent<Decay>(entity2);
		
		// Get entity component. But faster way is to retreive via injected ComponentsList
		// var position = manager.GetComponent<Position>(entity2);

		// Add entity to destroy queue. Need later perform deletion by calling manager.UpdateDestroyed()
		// manager.Destroy(entity2);
		
		// Destroy entity immediately. But queueing delete cause less ComponentGroup's updates
		// manager.DestroyImmediate(entity2);
	}
	
	public Entity CreateEntity(float posX, float posY, float velocityX = 0, float velocityY = 0) {
		return manager.Create(
			new Position(posX, posY),
			new Veclocity(velocityX, velocityY)
		);
	}

	public void Update(double dTime) {
		movementSystem.Update();
		decaySystem.Update(dTime);
		manager.UpdateDestroyed();
	}

}
```
