using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
	[SerializeField] private float timeExistence;
	[SerializeField] private float damage;
	[SerializeField] private float height;
	[SerializeField] private float radius;
	[SerializeField] private float maxDistance;

	public class Baker : Baker<BulletAuthoring>
	{
		public override void Bake(BulletAuthoring authoring)
		{
			Entity bulletEntity = GetEntity(TransformUsageFlags.None);

			AddComponent(bulletEntity, new BulletComponent
			{
				timeExistence = authoring.timeExistence,
				damage = authoring.damage,

			});

			AddComponent(bulletEntity, new BulletCollisionComponent
			{
				radius = authoring.radius,
				height = authoring.height,
				maxDistanceColliderCast = authoring.maxDistance,
			});
		}
	}
}

public struct BulletComponent : IComponentData
{
	public float timeExistence;
	public float damage;

	public Entity bullet;

	public bool isHit;
}

public struct BulletCollisionComponent : IComponentData
{
	public float height;
	public float radius;
	public float maxDistanceColliderCast;
}
