using UnityEngine;

[CreateAssetMenu(fileName = "HealthSO", order = -1)]
public class HealthSO : ScriptableObject
{
	public float MaxHealth;
	public float CurrentHealth;
}
