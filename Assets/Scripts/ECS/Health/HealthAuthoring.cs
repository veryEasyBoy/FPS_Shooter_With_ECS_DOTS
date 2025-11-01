using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
	[SerializeField] private float health;

	public class Baker : Baker<HealthAuthoring>
	{
		public override void Bake(HealthAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);

			AddComponent(entity, new HealthComponent
			{
				health = authoring.health,
			});

		}
	}
}

public struct HealthComponent : IComponentData
{
	public float health;
}
