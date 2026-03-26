using Assets.Scripts.InputActions;
using Assets.Scripts.Physics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.Scripts.Player
{
	[BurstCompile]
	public partial struct PlayerMovementSystem : ISystem
	{
		private bool isJump;
		private bool isGround;

		private float timer;

		private float2 lastDelta;

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
				in SystemAPI.Query<RefRW<LocalTransform>,
					RefRO<PlayerCollisionComponent>,
					RefRO<PlayerInputComponent>,
					RefRW<PlayerControllerComponent>,
					RefRW<PhysicsVelocity>>())
			{
				this.localTransform = localTransform.ValueRW;
				this.playerInputComponent = playerInputComponent.ValueRO;
				this.playerCollisionComponent = playerCollisionComponent.ValueRO;

				physicsVelocity.ValueRW.Angular.xyz = 0;

				RotatePlayer(playerInputComponent, localTransform, SystemAPI.Time.DeltaTime);

				MovementController(playerControllerComponent, physicsVelocity, SystemAPI.Time.DeltaTime);

				CheckGround();

				Jump(playerControllerComponent, physicsVelocity, SystemAPI.Time.DeltaTime);
			}
		}

		[BurstCompile]
		private void RotatePlayer(RefRO<PlayerInputComponent> playerInputComponent, RefRW<LocalTransform> localTransform, float time)
		{
			if (playerInputComponent.ValueRO.rotationInput.y != 0f || playerInputComponent.ValueRO.rotationInput.x != 0)
			{
				if (lastDelta.x != playerInputComponent.ValueRO.rotationInput.x || lastDelta.y != playerInputComponent.ValueRO.rotationInput.y)
				{
					// Получаем ввод
					float pitchDelta = playerInputComponent.ValueRO.rotationInput.y * 0.5f * -1; // Вверх-вниз
					float yawDelta = playerInputComponent.ValueRO.rotationInput.x * 0.5f;   // Влево-вправо

					// Получаем текущие углы
					float3 currentEuler = math.degrees(math.Euler(localTransform.ValueRO.Rotation));

					// Вычисляем новые углы
					float newPitch = currentEuler.x - (pitchDelta * 10 * time);
					float newYaw = currentEuler.y + (yawDelta * 10 * time);

					// Ограничиваем Pitch
					newPitch = math.clamp(newPitch, -45f, 45f);

					// Применяем новое вращение
					localTransform.ValueRW.Rotation = quaternion.Euler(math.radians(new float3(newPitch, newYaw, 0)));
					lastDelta = playerInputComponent.ValueRO.rotationInput;
				}
			}
		}

		[BurstCompile]
		private float3 StartMove(RefRW<PlayerControllerComponent> playerControllerComponent, float deltaTime, float2 input, RefRW<PhysicsVelocity> physicsVelocity)
		{

			return (localTransform.Right() * input.x + localTransform.Forward() * input.y)
			* playerControllerComponent.ValueRO.speed * deltaTime;

		}

		[BurstCompile]
		private void MovementController(RefRW<PlayerControllerComponent> playerControllerComponent, RefRW<PhysicsVelocity> physicsVelocity, float deltaTime)
		{
			
			var inputDirection = playerInputComponent.directionInput;
			if (inputDirection.x != 0 || inputDirection.y != 0)
				physicsVelocity.ValueRW.Linear.xz = StartMove(playerControllerComponent, deltaTime, inputDirection, physicsVelocity).xz;

			else
			{
				physicsVelocity.ValueRW.Linear.xz = 0;
			}

			playerControllerComponent.ValueRW.localPosition = localTransform.Position;
		}

		[BurstCompile]
		private void CheckGround()
		{
			PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

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
}
