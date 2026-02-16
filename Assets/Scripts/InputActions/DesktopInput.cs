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
		public float2 DirectionInput { get; private set; }
		public bool JumpInput { get; private set; }
		public bool LMBInput { get; private set; }

		[Inject]
		public void GetInputActions(PlayerInputActions inputActions)
		{
			this.inputActions = inputActions;

			Debug.Log("Create PlayerInputActions");
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

			inputActions.Enable();
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
