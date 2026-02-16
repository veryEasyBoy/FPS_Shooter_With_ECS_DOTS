using Unity.Entities;

namespace Assets.Scripts.Physics
{
	public struct BulletCollisionComponent : IComponentData
	{
		public float height;
		public float radius;
		public float maxDistanceColliderCast;
	}
}
