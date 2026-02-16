using UnityEngine;

namespace Assets.Scripts.Health
{
	[CreateAssetMenu(fileName = "HealthSO", order = -1)]
	public class HealthSO : ScriptableObject
	{
		public float MaxHealth;
		public float CurrentHealth;
	}
}
