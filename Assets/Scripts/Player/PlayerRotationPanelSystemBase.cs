using Assets.Scripts.InputActions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Player
{
	[DisableAutoCreation]
	public partial class PlayerRotationPanelSystemBase : SystemBase
	{
		public Camera mainCamera;
		public Transform cameraTransform;
		public UIDocument uiDocument;

		public string panelName = "DragPanel";

		public float rotationSpeed = 0.5f;
		
		private VisualElement dragPanel;
		private float2 rotationInput;
		private Vector3 lastPointerPosition;
		private bool isDragging = false;

		protected override void OnStartRunning()
		{
			InitializeDragPanel();
		}

		protected override void OnStopRunning()
		{
			UnregisterCallback();
		}

		protected override void OnUpdate()
		{
			foreach (var (localTransform, playerInputComponent, playerControllerComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInputComponent>, RefRW<PlayerControllerComponent>>())
			{
				playerInputComponent.ValueRW.rotationInput = rotationInput;
			}
		}

		private void InitializeDragPanel()
		{
			if (uiDocument == null || uiDocument.rootVisualElement == null)
			{
				Debug.LogError("UIDocument not found!");
				return;
			}

			// Находим панель
			dragPanel = uiDocument.rootVisualElement.Q<VisualElement>(panelName);

			if (dragPanel == null)
			{
				Debug.LogError("dragPanel Not Found");
				// Если панель не найдена, создаем ее программно
				dragPanel = new VisualElement();
				dragPanel.name = panelName;
				dragPanel.style.flexGrow = 1;
				dragPanel.style.position = Position.Absolute;
				dragPanel.style.left = 0;
				dragPanel.style.right = 0;
				dragPanel.style.top = 0;
				dragPanel.style.bottom = 0;
				dragPanel.style.backgroundColor = new Color(0, 0, 0, 0); // Прозрачный

				uiDocument.rootVisualElement.Add(dragPanel);
			}

			// Регистрируем события для мыши/касания
			RegisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			dragPanel.RegisterCallback<PointerDownEvent>(OnPointerDown);
			dragPanel.RegisterCallback<PointerMoveEvent>(OnPointerMove);
			dragPanel.RegisterCallback<PointerUpEvent>(OnPointerUp);
			dragPanel.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			isDragging = true;
			lastPointerPosition = evt.position;
			dragPanel.CapturePointer(evt.pointerId);
			evt.StopPropagation();

			Debug.Log("Camera rotation started ECS DOOOTSSSS");
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			if (!isDragging) return;

			Vector2 delta = evt.position - lastPointerPosition;

			rotationInput = delta;

			lastPointerPosition = evt.position;
			evt.StopPropagation();
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			if (!isDragging) return;

			isDragging = false;
			dragPanel.ReleasePointer(evt.pointerId);
			rotationInput = float2.zero;
			evt.StopPropagation();

			Debug.Log("Camera rotation ended");
		}

		private void OnPointerLeave(PointerLeaveEvent evt)
		{
			if (isDragging)
			{
				rotationInput = float2.zero;
				isDragging = false;
				dragPanel.ReleasePointer(evt.pointerId);
			}
		}

		private void UnregisterCallback()
		{
			// Очищаем callback'и при отключении
			if (dragPanel != null)
			{
				dragPanel.UnregisterCallback<PointerDownEvent>(OnPointerDown);
				dragPanel.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
				dragPanel.UnregisterCallback<PointerUpEvent>(OnPointerUp);
				dragPanel.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
			}
		}
	}
}
