using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Player
{
	internal class PlayerIntoController : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera virtualCamera;

		private void OnEnable()
		{
			Debug.Log("OnEnable");
			virtualCamera.Follow = gameObject.transform;
			virtualCamera.LookAt = gameObject.transform;
		}
	}
}
