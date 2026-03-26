using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.InputActions
{
	public class DesktopInput
	{
		private PlayerInputActions inputActions;
		public float2 DirectionInput { get; set; }
		public bool JumpInput { get; set; }
		public bool LMBInput { get; set; }

		[Inject]
		public void GetInputActions(PlayerInputActions inputActions)
		{
			this.inputActions = inputActions;

			Debug.Log("GetInputActions DesktopInput");
			Debug.Log(inputActions);
		}

		public void SetInputActions()
		{
			inputActions.Move.WASD.performed += ctx => DirectionInput = ctx.ReadValue<Vector2>();
			inputActions.Move.WASD.canceled += ctx => DirectionInput = Vector2.zero;

			inputActions.Move.Jump.performed += ctx => OnJump(ctx);
			inputActions.Move.Jump.canceled += ctx => OffJump(ctx);

			inputActions.Move.LMB.performed += ctx => LMBInput = ctx.ReadValueAsButton();
			inputActions.Move.LMB.canceled += ctx => LMBInput = false;

			inputActions.Move.Joystick.performed += ctx => DirectionInput = ctx.ReadValue<Vector2>();
			inputActions.Move.Joystick.canceled += ctx => DirectionInput = Vector2.zero;

			inputActions.Enable();

			Debug.Log("Enable DesktopInput");
		}

		public void DeactivateInputActions()
		{
			inputActions.Disable();
		}

		private Action<InputAction.CallbackContext> OnJump(InputAction.CallbackContext context)
		{
			JumpInput = true;
			Debug.Log(JumpInput);
			return context => JumpInput = true;
		}

		private Action<InputAction.CallbackContext> OffJump(InputAction.CallbackContext context)
		{
			JumpInput = false;
			Debug.Log(JumpInput);
			return context => JumpInput = false;
		}
	}
}
