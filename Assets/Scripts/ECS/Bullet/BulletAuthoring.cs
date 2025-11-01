using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
	[SerializeField] private float timeExistence;

	public class Baker : Baker<BulletAuthoring>
	{
		public override void Bake(BulletAuthoring authoring)
		{
			Entity bulletEntity = GetEntity(TransformUsageFlags.None);

			AddComponent(bulletEntity, new BulletComponent
			{
				timeExistence = authoring.timeExistence,
			});
		}
	}
}

public struct BulletComponent : IComponentData
{
	public float timeExistence;

	public Entity bullet;

	public bool isHit;
}
