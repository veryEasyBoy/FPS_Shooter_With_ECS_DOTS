using Assets.Scripts.InputActions;
using UnityEngine;
using Zenject;

public class PlayerMovementTest : MonoBehaviour
{
	Rigidbody rb;

	private DesktopInput inputActions;

	[SerializeField] private Vector2 vet;

	[Inject]
	private void Construct(DesktopInput inputActions)
	{
		Debug.Log("Create PlayerMovementTest OOP");
		Debug.Log(inputActions);
		this.inputActions = inputActions;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

	}

	public void OnEnable()
	{
		DiContainer container = ProjectContext.Instance.Container;
		inputActions = container.Resolve<DesktopInput>();
		inputActions.SetInputActions();
	}

	private void FixedUpdate()
	{
		Vector3 move = new Vector3(inputActions.DirectionInput.x, 0, inputActions.DirectionInput.y);
		transform.Translate(move * 10f * Time.deltaTime);
	}
}
