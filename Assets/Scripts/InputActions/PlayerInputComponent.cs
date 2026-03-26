using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.InputActions
{
	public struct PlayerInputComponent : IComponentData
	{
		public float2 directionInput;
		public float2 rotationInput;

		public bool jumpInput;
		public bool lmbInput;
	}
}
