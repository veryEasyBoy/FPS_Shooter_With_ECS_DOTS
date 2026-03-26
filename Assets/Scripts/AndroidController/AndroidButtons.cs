using Assets.Scripts.InputActions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class AndroidButtons : MonoBehaviour
{
	private Button fireButton;
	private Button jumpButton;

	private UIDocument root;

	private bool isDraggingFire = false;
	private bool isDraggingJump = false;
	private bool isFire = false;

	private DesktopInput playerInput;

	void OnEnable()
	{
		root = GetComponent<UIDocument>();

		Debug.Log("UiDocument Check: " + root);

		DiContainer container = ProjectContext.Instance.Container;
		playerInput = container.Resolve<DesktopInput>();

		fireButton = root.rootVisualElement.Q<Button>("fire");
		jumpButton = root.rootVisualElement.Q<VisualElement>("jump") as Button;

		// Для Fire - используем PointerDown и PointerUp
		fireButton.RegisterCallback<PointerDownEvent>(OnFireDown, TrickleDown.TrickleDown);
		fireButton.RegisterCallback<PointerUpEvent>(OnFireUp);
		fireButton.RegisterCallback<PointerLeaveEvent>(OnFireLeave); // Добавляем на случай, если палец уйдет с кнопки

		// Для Jump - используем PointerDown и PointerUp (без ClickEvent)
		jumpButton.RegisterCallback<PointerDownEvent>(OnJumpDown, TrickleDown.TrickleDown);
		jumpButton.RegisterCallback<PointerUpEvent>(OnJumpUp);
		jumpButton.RegisterCallback<PointerLeaveEvent>(OnJumpLeave); // Добавляем на случай, если палец уйдет с кнопки
	}

	private void OnFireDown(PointerDownEvent evt)
	{
		Debug.Log("Fire Down - LMBInput: " + playerInput.LMBInput);
		isDraggingFire = true;
		playerInput.LMBInput = true; // Устанавливаем сразу при нажатии
		evt.StopPropagation();
	}

	private void OnFireUp(PointerUpEvent evt)
	{
		Debug.Log("Fire Up");
		isDraggingFire = false;
		playerInput.LMBInput = false;
		evt.StopPropagation();
	}

	private void OnFireLeave(PointerLeaveEvent evt)
	{
		// Если палец ушел с кнопки, но еще не отпущен - ничего не делаем
		// или можно сбросить, если нужно
		Debug.Log("Fire Leave");
		isDraggingFire = false;
		playerInput.LMBInput = false;
		evt.StopPropagation();
	}

	private void OnJumpDown(PointerDownEvent evt)
	{
		Debug.Log("Jump Down - устанавливаем JumpInput = true");
		isDraggingJump = true;
		playerInput.JumpInput = true; // Устанавливаем сразу при нажатии
		evt.StopPropagation();
	}

	private void OnJumpUp(PointerUpEvent evt)
	{
		Debug.Log("Jump Up - устанавливаем JumpInput = false");
		isDraggingJump = false;
		playerInput.JumpInput = false;
		evt.StopPropagation();
	}

	private void OnJumpLeave(PointerLeaveEvent evt)
	{
		Debug.Log("Jump Leave");
		isDraggingJump = false;
		playerInput.JumpInput = false;
		evt.StopPropagation();
	}
}