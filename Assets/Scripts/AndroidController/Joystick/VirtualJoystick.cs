using Assets.Scripts.InputActions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class VirtualJoystick : MonoBehaviour
{
	[Header("UI Toolkit")]
	[SerializeField] private VisualTreeAsset joystickUXML; // Joystick.uxml file
	[SerializeField] private StyleSheet joystickUSS; // Joystick.uss file

	[Header("Joystick Settings")]
	[SerializeField] private float size = 60; // size(width and height) of joystick element, modify it if you want
	[SerializeField] private float sensitivity = 50; // the higher, the more sensitive. 0 means sudden switches between directions(no sensitivity)

	[Header("Positioning")]
	[SerializeField] private Vector2 startPos;

	// ===== PRIVATE FIELDS =====

	// UI Toolkit Elements
	private VisualElement joystickElement; // joystick itself (parent joystick element) it will be used to show and hide joystick by changing its style (display: none | flex)
	private VisualElement joystickKnob; // inner circle of joystick, dynamic moving part

	private DesktopInput joystickInput;

	// State
	private Vector2 input = Vector2.zero; // it is base input to publish it anywhere to use for know which direction and how strenght I move my finger.
	private Vector3 currentPos;

	// Flags
	private bool detectJoystickMovement = false; // you can assume it as a bug fixer flag. App triggers <PointerMoveEvent> for once at the ver first frame of app for a reason I dont know why. So this flag is to prevent it happen

	private void OnEnable()
	{
		DiContainer container = ProjectContext.Instance.Container;
		joystickInput = container.Resolve<DesktopInput>();

		Initialize();
	}

	private void Initialize()
	{
		VisualElement root = GetComponent<UIDocument>().rootVisualElement;
		VisualElement joystickUI = root;
		VisualElement joystickTouchArea = joystickUI.Q<VisualElement>("JoystickTouchArea");
		joystickElement = joystickUI.Q("JoystickOuterBorder"); // There is a parent node named "JoystickOuterBorder" in Joystick.uxml file, just leave it as it is, you will need this variable to show/hide joystick later
		joystickKnob = joystickElement.Q("JoystickKnob"); // There is a child node named "JoystickKnob" in Joystick.uxml file, just leave it as it is, you will need this variable to move the little circle on the middle of the joystick later

		joystickElement.style.width = size; // applying width of joystick
		joystickElement.style.height = size; // applying height of joystick

		joystickKnob.style.transformOrigin = new TransformOrigin(Length.Percent(100), 0, 0);

		root.styleSheets.Add(joystickUSS); // add joystick uss file to root, it is needed to apply joystick styles
		root.Add(joystickTouchArea); // add joystick touchable node to root

		Debug.Log("joystickTouchArea: " + joystickTouchArea);

		joystickTouchArea.RegisterCallback<PointerDownEvent>((ev) =>
		{
			Debug.Log(joystickUI.style.width);
			Debug.Log(joystickUI.style.height);
			Debug.Log("RegisterCallback<PointerDownEvent>");
			ShowJoystick(ev);
			
		});

		joystickTouchArea.RegisterCallback<PointerMoveEvent>((ev) =>
		{
			UpdateJoystick(ev);
		});

		joystickTouchArea.RegisterCallback<PointerUpEvent>((ev) =>
		{
			HideJoystick(ev);
		});

		joystickTouchArea.RegisterCallback<PointerLeaveEvent>((ev) =>
		{
			HideJoystick(ev);
		});

		Debug.Log("Initialize Joystick");
		joystickElement.style.left = startPos.x;
		joystickElement.style.top = startPos.y;
		joystickElement.style.display = DisplayStyle.Flex;

	}

	private void ShowJoystick(PointerDownEvent _ev)
	{
		Debug.Log("PointerDown");
		detectJoystickMovement = true;
		currentPos = _ev.position;
		joystickElement.style.left = _ev.position.x - size / 2;
		joystickElement.style.top = _ev.position.y - size / 2;
		joystickElement.style.display = DisplayStyle.Flex;
	}

	private void UpdateJoystick(PointerMoveEvent _ev)
	{
		if (detectJoystickMovement)
		{
			float deltaX = _ev.position.x - currentPos.x;
			float deltaY = currentPos.y - _ev.position.y;

			input = new Vector2(deltaX, deltaY);
			input = input.normalized;

			ApplySensitivity(ref input, deltaX, deltaY, sensitivity);

			joystickInput.DirectionInput = input;

			joystickKnob.style.translate = new StyleTranslate(new Translate(new Length(input.x * size / 2, LengthUnit.Pixel), new Length(-input.y * size / 2, LengthUnit.Pixel)));
		}
	}

	private void HideJoystick(PointerUpEvent _ev)
	{
		Debug.Log("HideJoystick");
		input = Vector3.zero;
		detectJoystickMovement = false;
		joystickElement.style.left = startPos.x;
		joystickElement.style.top = startPos.y;
		joystickInput.DirectionInput = input;
		joystickKnob.style.translate = new StyleTranslate(new Translate(new Length(0, LengthUnit.Pixel), new Length(0, LengthUnit.Pixel)));
	}

	private void HideJoystick(PointerLeaveEvent _ev)
	{
		Debug.Log("HideJoystick");
		input = Vector3.zero;
		detectJoystickMovement = false;
		joystickElement.style.left = startPos.x;
		joystickElement.style.top = startPos.y;
		joystickInput.DirectionInput = input;
		joystickKnob.style.translate = new StyleTranslate(new Translate(new Length(0, LengthUnit.Pixel), new Length(0, LengthUnit.Pixel)));
	}

	private static void ApplySensitivity(ref Vector2 input, float _deltaX, float _deltaY, float sensitivity)
	{
		if (Mathf.Abs(_deltaX) >= sensitivity || Mathf.Abs(_deltaY) >= sensitivity) { return; } // it is to avoid stuttering when one of directions is above sensitivity limit, you can assume it as a bug fixer line

		if (_deltaX > 0) // if finger movement is towards right
		{
			input.x = (_deltaX >= sensitivity) ? input.x : Mathf.Lerp(0f, 1f, _deltaX / sensitivity);
		}
		else // if finger movement is towards left
		{
			input.x = (_deltaX <= -sensitivity) ? input.x : Mathf.Lerp(0f, -1f, _deltaX / -sensitivity);
		}

		if (_deltaY > 0) // if finger movement is towards up
		{
			input.y = (_deltaY >= sensitivity) ? input.y : Mathf.Lerp(0f, 1f, _deltaY / sensitivity);
		}
		else // if finger movement is towards down
		{
			input.y = (_deltaY <= -sensitivity) ? input.y : Mathf.Lerp(0f, -1f, _deltaY / -sensitivity);
		}
	}
}