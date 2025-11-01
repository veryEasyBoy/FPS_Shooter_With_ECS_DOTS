using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
	private bool isJump;
	private bool isGround;

	private float timer;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		isJump = true;		
	}

	[BurstCompile]
	private void OnUpdate(ref SystemState state)
	{
		foreach (var (localTransform, playerInputComponent, playerControllerComponent, physicsVelocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInputComponent>, RefRW<PlayerControllerComponent>, RefRW<PhysicsVelocity>>())
		{
			MovementController(localTransform, playerInputComponent, playerControllerComponent, physicsVelocity, SystemAPI.Time.DeltaTime);

			CheckGround(localTransform, playerControllerComponent);

			Jump(playerControllerComponent, playerInputComponent, physicsVelocity, SystemAPI.Time.DeltaTime);
		}
	}

	[BurstCompile]
	private float3 StartMove(RefRW<LocalTransform> localTransform, RefRW<PlayerControllerComponent> playerControllerComponent, float deltaTime, float2 input)
	{
		return (localTransform.ValueRO.Right() * input.x + localTransform.ValueRO.Forward() * input.y)
		* playerControllerComponent.ValueRO.speed * deltaTime;
	}

	[BurstCompile]
	private void MovementController(RefRW<LocalTransform> localTransform, RefRW<PlayerInputComponent> playerInputComponent, RefRW<PlayerControllerComponent> playerControllerComponent, RefRW<PhysicsVelocity> physicsVelocity, float deltaTime)
	{
		var inputDirection = playerInputComponent.ValueRO.directionInput;

		if (inputDirection.x != 0 || inputDirection.y != 0)
			physicsVelocity.ValueRW.Linear.xz = StartMove(localTransform, playerControllerComponent, deltaTime, inputDirection).xz;

		else
		{
			 physicsVelocity.ValueRW.Angular.xz = 0;
			 physicsVelocity.ValueRW.Linear.xz = 0;
		}
		playerControllerComponent.ValueRW.localPosition = localTransform.ValueRO.Position;
	}

	[BurstCompile]
	private void CheckGround(RefRW<LocalTransform> localTransform, RefRW<PlayerControllerComponent> playerControllerComponent)
	{
		PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
		NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

		float3 cast = float3.zero;
		float3 dir = -localTransform.ValueRO.Up();

		Quaternion localRotation = Quaternion.Euler(cast);

		var point1 = localTransform.ValueRW.Position;
		var point2 = localTransform.ValueRW.Position;

		isGround = physicsWorldSingleton.CapsuleCast(
			point1,
			point2,
			0.5f,
			dir,
			playerControllerComponent.ValueRO.maxDistanceColliderCast,
			new CollisionFilter { BelongsTo = (uint)CollisionLayer.Player, CollidesWith = (uint)CollisionLayer.Collectable });
	}

	[BurstCompile]
	private void Jump(RefRW<PlayerControllerComponent> playerControllerComponent, RefRW<PlayerInputComponent> playerInputComponent, RefRW<PhysicsVelocity> physicsVelocity, float deltaTime)
	{
		if (playerInputComponent.ValueRO.jumpInput && isGround && isJump)
		{
			timer = playerControllerComponent.ValueRO.cdJump;

			isJump = false;

			physicsVelocity.ValueRW.Linear.y = playerControllerComponent.ValueRO.jumpPower;
		}

		if (!isJump)
		{
			timer -= deltaTime;

			if (timer <= 0)
			{
				timer = playerControllerComponent.ValueRO.cdJump;
				isJump = true;
			}
		}
	}
}

public enum CollisionLayer
{
	Player = 1 << 6,
	Collectable = 1 << 7,
	Bullet = 1 << 8,
	Enemy = 1 << 9,
}
