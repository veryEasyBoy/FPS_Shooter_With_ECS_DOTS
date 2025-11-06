using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
	private bool isJump;
	private bool isGround;

	private float timer;

	private LocalTransform localTransform;
	private PlayerInputComponent playerInputComponent;
	private PlayerCollisionComponent playerCollisionComponent;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		isJump = true;
	}

	[BurstCompile]
	private void OnUpdate(ref SystemState state)
	{
		foreach (var (localTransform, playerCollisionComponent, playerInputComponent, playerControllerComponent, physicsVelocity)
			in SystemAPI.Query<RefRO<LocalTransform>,
				RefRO<PlayerCollisionComponent>,
				RefRO<PlayerInputComponent>,
				RefRW<PlayerControllerComponent>,
				RefRW<PhysicsVelocity>>())
		{
			this.localTransform = localTransform.ValueRO;
			this.playerInputComponent = playerInputComponent.ValueRO;
			this.playerCollisionComponent = playerCollisionComponent.ValueRO;

			MovementController(playerControllerComponent, physicsVelocity, SystemAPI.Time.DeltaTime);

			CheckGround();

			Jump(playerControllerComponent, physicsVelocity, SystemAPI.Time.DeltaTime);
		}
	}

	[BurstCompile]
	private float3 StartMove(RefRW<PlayerControllerComponent> playerControllerComponent, float deltaTime, float2 input)
	{
		return (localTransform.Right() * input.x + localTransform.Forward() * input.y)
		* playerControllerComponent.ValueRO.speed * deltaTime;
	}

	[BurstCompile]
	private void MovementController(RefRW<PlayerControllerComponent> playerControllerComponent, RefRW<PhysicsVelocity> physicsVelocity, float deltaTime)
	{
		var inputDirection = playerInputComponent.directionInput;
		if (inputDirection.x != 0 || inputDirection.y != 0)
			physicsVelocity.ValueRW.Linear.xz = StartMove(playerControllerComponent, deltaTime, inputDirection).xz;

		else
		{
			physicsVelocity.ValueRW.Angular.xz = 0;
			physicsVelocity.ValueRW.Linear.xz = 0;
		}

		playerControllerComponent.ValueRW.localPosition = localTransform.Position;
	}

	[BurstCompile]
	private void CheckGround()
	{
		PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
		NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

		float3 cast = float3.zero;
		float3 dir = -localTransform.Up();

		var point1 = localTransform.Position + new float3(0, 0, 0) + dir * -playerCollisionComponent.height * 0.5f;
		var point2 = point1 + dir * playerCollisionComponent.height;

		isGround = physicsWorldSingleton.CapsuleCast(
			point1,
			point2,
			playerCollisionComponent.radius,
			dir,
			playerCollisionComponent.maxDistanceColliderCast,
			new CollisionFilter { BelongsTo = (uint)CollisionLayer.Player, CollidesWith = (uint)CollisionLayer.Collectable });
	}

	[BurstCompile]
	private void Jump(RefRW<PlayerControllerComponent> playerControllerComponent, RefRW<PhysicsVelocity> physicsVelocity, float deltaTime)
	{
		if (playerInputComponent.jumpInput && isGround && isJump)
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
