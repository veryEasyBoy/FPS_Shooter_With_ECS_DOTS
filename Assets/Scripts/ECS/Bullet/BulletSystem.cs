using UnityEngine;
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

		foreach (var (bulletComponent, physicsVelocity, localTransform) in SystemAPI.Query<RefRW<BulletComponent>, RefRW<PhysicsVelocity>, RefRW<LocalTransform>>())
		{
			bulletComponent.ValueRW.timeExistence -= SystemAPI.Time.DeltaTime;

			if (bulletComponent.ValueRO.timeExistence <= 0)
			{
				ecb.DestroyEntity(bulletComponent.ValueRO.bullet);
			}

			PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
			NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

			float3 cast = float3.zero;
			float3 dir = -localTransform.ValueRO.Up();

			Quaternion localRotation = Quaternion.Euler(cast);

			var point1 = localTransform.ValueRW.Position * (dir + (2/2));
			var point2 = localTransform.ValueRW.Position * (-dir + (2 / 2));

			var isGround = physicsWorldSingleton.CapsuleCastAll(
				point1,
				point2,
				0.5f,
				dir,
				0f,
				ref hits,
				new CollisionFilter { BelongsTo = (uint)CollisionLayer.Bullet, CollidesWith = (uint)CollisionLayer.Enemy });

			foreach (ColliderCastHit hit in hits)
			{
				if (hit.Entity != null && !bulletComponent.ValueRW.isHit)
				{
					bulletComponent.ValueRW.isHit = true;
					var health = state.EntityManager.GetComponentData<HealthComponent>(hit.Entity);
					health.health -= 10f;

					ecb.SetComponent(hit.Entity, health);

					Debug.Log(health.health);
					if (health.health <= 0)
						ecb.DestroyEntity(hit.Entity);
				}

			}
		}
		ecb.Playback(state.EntityManager);
	}
}
