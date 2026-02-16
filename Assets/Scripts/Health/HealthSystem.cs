using System;
using Unity.Collections;
using Unity.Entities;

namespace Assets.Scripts.Health
{
	public partial class HealthSystem : SystemBase
	{
		private float timer;

		public Action<float, bool> UpdateCurrentHealth;

		protected override void OnCreate()
		{
			timer = 4;
		}

		protected override void OnUpdate()
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (health, entity)
				in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
			{

				if (health.ValueRO.currentHealth <= 0)
				{
					UpdateCurrentHealth?.Invoke(health.ValueRO.currentHealth, false);
					timer = 4;
					ecb.AddComponent<Disabled>(entity);
				}

				if (health.ValueRO.isHit && health.ValueRO.currentHealth > 0)
				{
					UpdateCurrentHealth?.Invoke(health.ValueRO.currentHealth, true);
					timer = 4;
					health.ValueRW.isHit = false;
				}

				else
				{
					timer -= SystemAPI.Time.DeltaTime;
					if (timer <= 0)
					{
						UpdateCurrentHealth?.Invoke(health.ValueRO.currentHealth, false);
						timer = 4;
					}
				}
			}
			ecb.Playback(EntityManager);
		}
	}
}
