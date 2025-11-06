using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial class ShootRaycast : SystemBase
{
	public Camera Camera;

	[BurstCompile]
	protected override void OnUpdate()
	{
		foreach (var gunComponent in SystemAPI.Query<RefRW<GunComponent>>())
		{
			Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
			gunComponent.ValueRW.raycastDirection = ray.direction;
		}
	}
}
