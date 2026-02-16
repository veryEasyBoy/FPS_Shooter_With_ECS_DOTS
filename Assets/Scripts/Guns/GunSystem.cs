using Assets.Scripts.Bullets;
using Assets.Scripts.InputActions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Guns
{
	public partial class ShootRaycastSystem : SystemBase
	{
		public Camera Camera;

		[BurstCompile]
		protected override void OnUpdate()
		{
			foreach (var gunComponent in SystemAPI.Query<RefRW<GunComponent>>())
			{
				UnityEngine.Ray ray = Camera.ViewportPointToRay(new float3(0.5f, 0.5f, 0));
				gunComponent.ValueRW.raycastDirection = ray.direction;
			}
		}
	}

	[BurstCompile]
	public partial struct GunSystem : ISystem
	{
		private float fireCooldown;

		private Entity playerInputComponent;

		private PlayerInputComponent LMBInput;

		[BurstCompile]
		private void OnStartRunning(ref SystemState state)
		{
			foreach (var gunComponent in SystemAPI.Query<RefRW<GunComponent>>())
				fireCooldown = gunComponent.ValueRO.fireCooldown;
		}

		[BurstCompile]
		private void OnUpdate(ref SystemState state)
		{
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (transformToWorld, gunComponent) in SystemAPI.Query<RefRW<LocalToWorld>, RefRW<GunComponent>>())
			{
				playerInputComponent = SystemAPI.GetSingletonEntity<PlayerInputComponent>();

				LMBInput = state.EntityManager.GetComponentData<PlayerInputComponent>(playerInputComponent);

				if (gunComponent.ValueRO.fireCooldown == fireCooldown)
				{
					if (LMBInput.lmbInput)
					{
						fireCooldown -= SystemAPI.Time.DeltaTime;

						var bullet = state.EntityManager.Instantiate(gunComponent.ValueRO.bullet);

						var bulletTransform = state.EntityManager.GetComponentData<LocalTransform>(bullet);
						var bulletPhysics = state.EntityManager.GetComponentData<PhysicsVelocity>(bullet);
						var bulletComponent = state.EntityManager.GetComponentData<BulletComponent>(bullet);

						var muzzleRotation = state.EntityManager.GetComponentData<LocalToWorld>(gunComponent.ValueRO.muzzle).Value.Rotation();
						var muzzlePosition = state.EntityManager.GetComponentData<LocalToWorld>(gunComponent.ValueRO.muzzle).Position;

						bulletTransform.Position = muzzlePosition;
						bulletTransform.Rotation = muzzleRotation;

						bulletPhysics.Linear = gunComponent.ValueRO.raycastDirection * gunComponent.ValueRO.speed;

						bulletComponent.bullet = bullet;

						ecb.SetComponent(bullet, bulletTransform);
						ecb.SetComponent(bullet, bulletPhysics);
						ecb.SetComponent(bullet, bulletComponent);

					}
				}

				else
					fireCooldown -= SystemAPI.Time.DeltaTime;

				if (fireCooldown <= 0)
					fireCooldown = gunComponent.ValueRO.fireCooldown;
			}

			ecb.Playback(state.EntityManager);
		}
	}
}
