using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
	void Update()
	{
		RaycastHit[] hits;
		CharacterController charCtrl = GetComponent<CharacterController>();
		Vector3 p1 = transform.position + charCtrl.center + transform.forward * -charCtrl.height * 0.5F;
		Vector3 p2 = p1 + transform.forward * charCtrl.height;

		// Cast character controller shape 10 meters forward, to see if it is about to hit anything
		hits = Physics.CapsuleCastAll(p1, p2, charCtrl.radius, transform.forward, 10);
		Debug.Log(transform.forward);
		// Change the material of all hit colliders
		// to use a transparent Shader
		for (int i = 0; i < hits.Length; i++)
		{
			
			//Debug.Log(transform.up);
			//Debug.Log(transform.right);
			RaycastHit hit = hits[i];
			Renderer rend = hit.transform.GetComponent<Renderer>();

			if (rend)
			{
				rend.material.shader = Shader.Find("Transparent/Diffuse");
				Color tempColor = rend.material.color;
				tempColor.a = 0.3F;
				rend.material.color = tempColor;
			}
		}
	}
}