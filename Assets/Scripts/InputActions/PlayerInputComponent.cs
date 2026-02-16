using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.InputActions
{
	public struct PlayerInputComponent : IComponentData
	{
		public float2 directionInput;

		public bool jumpInput;
		public bool lmbInput;
	}
}
