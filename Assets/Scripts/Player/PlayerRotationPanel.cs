using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Player
{
	public class PlayerRotationPanel : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private float rotationSpeed = 0.5f;

		[Header("References")]
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private string panelName = "DragPanel";

		// —сылка на камеру (можно через Zenject)
		[SerializeField] private Camera mainCamera;
		[SerializeField] private Transform cameraTransform;

		private PlayerRotationPanelSystemBase cameraRotationPanelSystemBase;

		private void OnEnable()
		{
			if (cameraRotationPanelSystemBase == null)
			{
				cameraRotationPanelSystemBase = World.DefaultGameObjectInjectionWorld.CreateSystemManaged<PlayerRotationPanelSystemBase>();
				Debug.Log("Create cameraRotationPanelSystemBase: " + cameraRotationPanelSystemBase);
				cameraRotationPanelSystemBase.uiDocument = uiDocument;
				cameraRotationPanelSystemBase.rotationSpeed = rotationSpeed;
				cameraRotationPanelSystemBase.mainCamera = mainCamera;
				cameraRotationPanelSystemBase.cameraTransform = cameraTransform;
				cameraRotationPanelSystemBase.panelName = panelName;
				cameraRotationPanelSystemBase.Enabled = true;

				var world = World.DefaultGameObjectInjectionWorld;

				var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
				simulationSystemGroup.AddSystemToUpdateList(cameraRotationPanelSystemBase);

				Debug.Log("System added to SimulationSystemGroup");

			}
			else
				cameraRotationPanelSystemBase.Enabled = true;
		}

		private void OnDestroy()
		{
			cameraRotationPanelSystemBase.Enabled = false;
			Debug.LogWarning("cameraRotationPanelSystemBase.Enabled = false");
		}
	}
}