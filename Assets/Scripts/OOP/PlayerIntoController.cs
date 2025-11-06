using Cinemachine;
using Unity.Entities;
using UnityEngine;

public class PlayerIntoController : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera;
	[SerializeField] private Camera cam;

	[SerializeField] private float pos;

	private PlayerController playerTransform;

	private ShootRaycast raycastSpawn;

	private void OnEnable()
	{
		virtualCamera.Follow = gameObject.transform;
		virtualCamera.LookAt = gameObject.transform;

		playerTransform = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerController>();
		raycastSpawn = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShootRaycast>();
	}

	private void Update()
	{
		raycastSpawn.Camera = cam;

		transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + pos, playerTransform.position.z);

		playerTransform.rotation = gameObject.transform.rotation;
	}
}
