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

		private void Start()
		{
			cameraRotationPanelSystemBase = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlayerRotationPanelSystemBase>();
			cameraRotationPanelSystemBase.uiDocument = uiDocument;
			cameraRotationPanelSystemBase.rotationSpeed = rotationSpeed;
			cameraRotationPanelSystemBase.mainCamera = mainCamera;
			cameraRotationPanelSystemBase.cameraTransform = cameraTransform;
			cameraRotationPanelSystemBase.panelName = panelName;
		}
	}
}