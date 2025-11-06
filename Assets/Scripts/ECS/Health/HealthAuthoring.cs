using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
	[SerializeField] private HealthSO health;

	public class Baker : Baker<HealthAuthoring>
	{
		public override void Bake(HealthAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);

			AddComponent(entity, new HealthComponent
			{
				currentHealth = authoring.health.MaxHealth,
				maxHealth = authoring.health.MaxHealth,
			});

			AddComponent(entity, new HealthUiComponent { });

		}
	}
}

public struct HealthComponent : IComponentData
{
	public float currentHealth;
	public float maxHealth;
}

public struct HealthUiComponent : IComponentData
{
	public bool canUpdateUi;
}
