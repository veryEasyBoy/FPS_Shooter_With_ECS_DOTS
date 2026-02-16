using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Health
{
	internal class HealthAuthoring : MonoBehaviour
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
				});
			}
		}
	}

	public struct HealthComponent : IComponentData
	{
		public float currentHealth;
		public bool isHit;
	}
}

