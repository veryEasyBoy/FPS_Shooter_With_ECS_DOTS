using Assets.Scripts.InputActions;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Player
{
	public partial class PlayerController : SystemBase
	{
		private DesktopInput playerInput;

		public float3 position;

		public quaternion rotation;

		private Transform m_CameraTarget;

		protected override void OnCreate()
		{
			DiContainer container = ProjectContext.Instance.Container;
			playerInput = container.Resolve<DesktopInput>();

			Debug.Log("Create manual");
			Debug.Log(playerInput);
			m_CameraTarget = GameObject.FindWithTag("CameraTarget").transform;
		}

		protected override void OnStartRunning()
		{
			playerInput.SetInputActions();
		}

		protected override void OnStopRunning()
		{
			playerInput.DeactivateInputActions();
		}

		[BurstCompile]
		protected override void OnUpdate()
		{
			foreach (var (localTransform, playerInputComponent, playerControllerComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInputComponent>, RefRW<PlayerControllerComponent>>())
			{
				playerInputComponent.ValueRW.directionInput = playerInput.DirectionInput;
				playerInputComponent.ValueRW.jumpInput = playerInput.JumpInput;
				playerInputComponent.ValueRW.lmbInput = playerInput.LMBInput;
				//if(m_CameraTarget == null)

				//localTransform.ValueRW.Rotation.value = rotation.value;

				float3 eulerAngles = math.degrees(math.Euler(rotation.value));

				//Ограничиваем вращение по Y, например, ставим его в 0 или ограничиваем диапазон
				//eulerAngles.xz = 0f; // или math.clamp(eulerAngles.y, minY, maxY);

				//Обратное преобразование
				//	localTransform.ValueRW.Rotation = quaternion.Euler(math.radians(eulerAngles));

				//position = playerControllerComponent.ValueRO.localPosition;
				m_CameraTarget.position = localTransform.ValueRO.Position;
				localTransform.ValueRW.Rotation = m_CameraTarget.rotation;
			}
		}
	}
}

