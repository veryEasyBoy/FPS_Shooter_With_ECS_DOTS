using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthManager : MonoBehaviour
{
	[SerializeField] private HealthSO progressData;

	private UIDocument uiDocument;

	private HealthSystem healthSystem;

	private ProgressBar progressBar;

	private void Awake()
	{
		uiDocument = GetComponent<UIDocument>();

		var root = uiDocument.rootVisualElement;

		progressBar = root.Q<ProgressBar>("progressBar");
		progressBar.visible = false;

		healthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HealthSystem>();
	}

	public void OnEnable()
	{
		healthSystem.UpdateCurrentHealth += DealDamageUi;
	}

	public void OnDisable()
	{
		healthSystem.UpdateCurrentHealth -= DealDamageUi;
	}

	private void DealDamageUi(float maxValue, float currentHealth, bool can)
	{
		if (can)
		{
			progressBar.visible = can;

			progressBar.highValue = maxValue;
			progressBar.value = currentHealth;
		}
		else
		{
			progressBar.visible = can;
		}
	}
}
