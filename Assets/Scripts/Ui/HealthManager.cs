using Assets.Scripts.Health;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

internal class HealthManager : MonoBehaviour
{
	[SerializeField] private HealthSO progressData;

	private UIDocument uiDocument;

	private HealthSystem healthSystem;

	private ProgressBar progressBar;

	private float maxHealthLast;

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

	private void DealDamageUi(float currentHealth, bool can)
	{
		if (can)
		{
			progressBar.visible = can;

			if (maxHealthLast != progressBar.highValue)
			{
				progressBar.highValue = currentHealth;
				maxHealthLast = progressBar.highValue;
			}

			progressBar.value = currentHealth;
		}

		else
		{
			progressBar.visible = can;
		}
	}
}
