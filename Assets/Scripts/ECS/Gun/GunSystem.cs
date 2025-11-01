using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct GunSystem : ISystem
{
	private float fireRate;

	private Entity playerInputComponent;

	private PlayerInputComponent LMBInput;

	[BurstCompile]
	private void OnStartRunning(ref SystemState state)
	{
		foreach (var gunComponent in SystemAPI.Query<RefRW<GunComponent>>())
			fireRate = gunComponent.ValueRO.fireRate;
	}

	[BurstCompile]
	private void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (transformToWorld, gunComponent) in SystemAPI.Query<RefRW<LocalToWorld>, RefRW<GunComponent>>())
		{
			playerInputComponent = SystemAPI.GetSingletonEntity<PlayerInputComponent>();

			LMBInput = state.EntityManager.GetComponentData<PlayerInputComponent>(playerInputComponent);

			if (gunComponent.ValueRO.fireRate == fireRate)
			{
				if (LMBInput.lmbInput)
				{
					fireRate -= SystemAPI.Time.DeltaTime;

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
				fireRate -= SystemAPI.Time.DeltaTime;

			if (fireRate <= 0)
				fireRate = gunComponent.ValueRO.fireRate;
		}

		ecb.Playback(state.EntityManager);
	}
}
