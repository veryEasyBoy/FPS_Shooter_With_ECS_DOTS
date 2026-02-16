using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Guns
{
	public class GunAuthoring : MonoBehaviour
	{
		[SerializeField] private float fireCooldown;
		[SerializeField] private float speed;

		[SerializeField] private GameObject bullet;
		[SerializeField] private GameObject muzzle;

		public class Baker : Baker<GunAuthoring>
		{
			public override void Bake(GunAuthoring gunAuthoring)
			{
				Entity gunEntity = GetEntity(TransformUsageFlags.None);

				AddComponent(gunEntity, new GunComponent
				{
					fireCooldown = gunAuthoring.fireCooldown,
					speed = gunAuthoring.speed,
					bullet = GetEntity(gunAuthoring.bullet, TransformUsageFlags.None),
					muzzle = GetEntity(gunAuthoring.muzzle, TransformUsageFlags.Dynamic),

				});
			}
		}
	}

	public struct GunComponent : IComponentData
	{
		public float fireCooldown;
		public float speed;

		public Entity bullet;
		public Entity muzzle;

		public float3 raycastDirection;
	}
}
