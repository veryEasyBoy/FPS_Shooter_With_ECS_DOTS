using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
	[BurstCompile]
	private void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		foreach (var (bulletComponent, bulletCollision, localTransform) 
			in SystemAPI.Query<RefRW<BulletComponent>, 
			RefRO<BulletCollisionComponent>, 
			RefRO<LocalTransform>>())
		{
			bulletComponent.ValueRW.timeExistence -= SystemAPI.Time.DeltaTime;

			if (bulletComponent.ValueRO.timeExistence <= 0)
			{
				ecb.DestroyEntity(bulletComponent.ValueRO.bullet);
			}

			PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
			NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

			float3 cast = float3.zero;
			float3 dir = localTransform.ValueRO.Up();

			var point1 = localTransform.ValueRO.Position + new float3(0,0,0) + dir * -bulletCollision.ValueRO.height * 0.5f;
			var point2 = point1 + dir * bulletCollision.ValueRO.height;

			var isHit = physicsWorldSingleton.CapsuleCastAll(
				point1,
				point2,
				bulletCollision.ValueRO.radius,
				dir,
				bulletCollision.ValueRO.maxDistanceColliderCast,
				ref hits,
				new CollisionFilter { BelongsTo = (uint)CollisionLayer.Bullet, CollidesWith = (uint)CollisionLayer.Enemy });

			foreach (ColliderCastHit hit in hits)
			{
				if (!bulletComponent.ValueRO.isHit)
				{
					if (state.EntityManager.Exists(hit.Entity))
					{
						bulletComponent.ValueRW.isHit = true;

						var health = state.EntityManager.GetComponentData<HealthComponent>(hit.Entity);
						var healthUi = state.EntityManager.GetComponentData<HealthUiComponent>(hit.Entity);

						health.currentHealth -= bulletComponent.ValueRO.damage;
						healthUi.canUpdateUi = true;

						ecb.SetComponent(hit.Entity, health);
						ecb.SetComponent(hit.Entity, healthUi);
					}
				}
			}
			hits.Clear();
		}
		ecb.Playback(state.EntityManager);
	}
}
