using System;
using Unity.Collections;
using Unity.Entities;

public partial class HealthSystem : SystemBase
{
	private float timer;

	private bool startTimer;

	public Action<float, float, bool> UpdateCurrentHealth;

	protected override void OnCreate()
	{
		timer = 4;
	}

	protected override void OnUpdate()
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		foreach (var (health, healthUi, entity)
			in SystemAPI.Query<RefRW<HealthComponent>, RefRW<HealthUiComponent>>().WithEntityAccess())
		{

			if (health.ValueRO.currentHealth <= 0)
			{
				UpdateCurrentHealth?.Invoke( health.ValueRO.maxHealth, health.ValueRO.currentHealth, false);
				timer = 4;
				ecb.DestroyEntity(entity);
			}

			if (healthUi.ValueRO.canUpdateUi && health.ValueRO.currentHealth > 0)
			{
				UpdateCurrentHealth?.Invoke(health.ValueRO.maxHealth, health.ValueRO.currentHealth, true);
				timer = 4;
				startTimer = true;
				healthUi.ValueRW.canUpdateUi = false;
			}

			if (!healthUi.ValueRO.canUpdateUi && startTimer)
			{
				timer -= SystemAPI.Time.DeltaTime;
				if (timer <= 0)
				{
					UpdateCurrentHealth?.Invoke(health.ValueRO.maxHealth, health.ValueRO.currentHealth, false);
					startTimer = false;
					timer = 4;
				}
			}
		}
		ecb.Playback(EntityManager);
	}
}
