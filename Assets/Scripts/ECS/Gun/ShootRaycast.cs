using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class ShootRaycast : SystemBase
{
	public Camera Camera;

	[BurstCompile]
	protected override void OnUpdate()
	{
		foreach (var (localTransform, gunComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<GunComponent>>())
		{
			Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
			gunComponent.ValueRW.raycastDirection = ray.direction;
		}
	}
}
