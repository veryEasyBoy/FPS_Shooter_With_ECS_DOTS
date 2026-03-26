using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Player
{
	internal class PlayerIntoController : MonoBehaviour
	{
		private Camera cameraPlayer;
		private PlayerController playerController;

		private void OnEnable()
		{
			cameraPlayer = GetComponent<Camera>();

			playerController = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlayerController>();
			playerController.m_CameraTarget = cameraPlayer.transform;
		}
	}
}
