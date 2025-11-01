using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerAuthoring : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private float maxDistanceColliderCast;
	[SerializeField] private float cdJump;
	[SerializeField] private float jumpPower;

	private class Baker : Baker<PlayerControllerAuthoring>
	{
		public override void Bake(PlayerControllerAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);

			AddComponent(entity, new PlayerControllerComponent()
			{
				speed = authoring.speed,
				maxDistanceColliderCast = authoring.maxDistanceColliderCast,
				cdJump = authoring.cdJump,
				jumpPower = authoring.jumpPower,
			});

			AddComponent(entity, new PlayerInputComponent()
			{

			});
		}
	}
}

public struct PlayerControllerComponent : IComponentData
{
	public float3 speed;
	public float3 localPosition;

	public float maxDistanceColliderCast;
	public float cdJump;
	public float jumpPower;
}

public struct PlayerInputComponent : IComponentData
{
	public float2 directionInput;

	public bool jumpInput;
	public bool lmbInput;
}

