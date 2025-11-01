using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GunAuthoring : MonoBehaviour
{
	[SerializeField] private float fireRate;
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
				fireRate = gunAuthoring.fireRate,
				speed = gunAuthoring.speed,
				bullet = GetEntity(gunAuthoring.bullet, TransformUsageFlags.None),
				muzzle = GetEntity(gunAuthoring.muzzle, TransformUsageFlags.Dynamic),
				
			});
		}
	}
}

public struct GunComponent : IComponentData
{
	public float fireRate;
	public float speed;

	public Entity bullet;
	public Entity muzzle;

	public float3 raycastDirection;

}
