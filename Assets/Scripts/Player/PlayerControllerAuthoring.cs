using Assets.Scripts.InputActions;
using Assets.Scripts.Physics;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player
{
	internal class PlayerControllerAuthoring : MonoBehaviour
	{
		[SerializeField] private float speed;
		[SerializeField] private float maxDistanceColliderCast;
		[SerializeField] private float cdJump;
		[SerializeField] private float jumpPower;
		[SerializeField] private float height;
		[SerializeField] private float radius;


		private class Baker : Baker<PlayerControllerAuthoring>
		{
			public override void Bake(PlayerControllerAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);

				AddComponent(entity, new PlayerControllerComponent()
				{
					speed = authoring.speed,
					cdJump = authoring.cdJump,
					jumpPower = authoring.jumpPower,
				});

				AddComponent(entity, new PlayerInputComponent()
				{

				});

				AddComponent(entity, new PlayerCollisionComponent()
				{
					maxDistanceColliderCast = authoring.maxDistanceColliderCast,
					height = authoring.height,
					radius = authoring.radius,
				});
			}
		}
	}

	public struct PlayerControllerComponent : IComponentData
	{
		public float3 speed;
		public float3 localPosition;

		public float3 transform;

		public Entity player;

		public float cdJump;
		public float jumpPower;
	}
}

