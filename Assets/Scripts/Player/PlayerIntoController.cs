using Assets.Scripts.Guns;
using Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerIntoController : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera virtualCamera;
		[SerializeField] private Camera cam;

		[SerializeField] private float pos;

		private ShootRaycastSystem raycastSpawn;

		private void OnEnable()
		{
			Debug.Log("OnEnable");
			virtualCamera.Follow = gameObject.transform;
			virtualCamera.LookAt = gameObject.transform;

			raycastSpawn = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShootRaycastSystem>();
			raycastSpawn.Camera = cam;
		}
	}
}
