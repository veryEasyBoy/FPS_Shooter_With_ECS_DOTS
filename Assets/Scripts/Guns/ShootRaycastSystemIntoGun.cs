using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Guns
{
	internal class ShootRaycastSystemIntoGun : MonoBehaviour
	{
		[SerializeField] private Camera cam;

		private ShootRaycastSystem raycastSpawn;

		private void OnEnable()
		{
			raycastSpawn = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShootRaycastSystem>();
			raycastSpawn.Camera = cam;
		}
	}
}
