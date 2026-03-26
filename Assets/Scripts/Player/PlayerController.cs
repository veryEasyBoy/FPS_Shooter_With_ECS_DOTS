using Assets.Scripts.InputActions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Player
{
	//[DisableAutoCreation]
	public partial class PlayerController : SystemBase
	{
		private DesktopInput playerInput;

		public Transform m_CameraTarget;

		private float smoothSpeed = 0.1f; // Скорость сглаживания

		protected override void OnCreate()
		{
			Debug.Log("Create PlayerController");
			DiContainer container = ProjectContext.Instance.Container;
			playerInput = container.Resolve<DesktopInput>();

			Debug.Log(playerInput);
			//m_CameraTarget = GameObject.FindWithTag("CameraTarget").transform;
		}

		protected override void OnStartRunning()
		{
			playerInput.SetInputActions();

		}

		protected override void OnStopRunning()
		{
			playerInput.DeactivateInputActions();
		}

		protected override void OnUpdate()
		{
			foreach (var (localTransform, playerInputComponent, playerControllerComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInputComponent>, RefRW<PlayerControllerComponent>>())
			{
				Move(playerInputComponent, localTransform);
			}
		}

		private void Move(RefRW<PlayerInputComponent> playerInputComponent, RefRW<LocalTransform> localTransform)
		{

			playerInputComponent.ValueRW.directionInput = new float2(playerInput.DirectionInput.x, playerInput.DirectionInput.y);
			playerInputComponent.ValueRW.jumpInput = playerInput.JumpInput;
			playerInputComponent.ValueRW.lmbInput = playerInput.LMBInput;

			m_CameraTarget.position = localTransform.ValueRO.Position;

			//m_CameraTarget.position = Vector3.Lerp(m_CameraTarget.position, localTransform.ValueRO.Position, smoothSpeed + SystemAPI.Time.DeltaTime);

			m_CameraTarget.rotation = Quaternion.Lerp(m_CameraTarget.rotation, localTransform.ValueRO.Rotation, smoothSpeed + SystemAPI.Time.DeltaTime);
		}
	}
}

