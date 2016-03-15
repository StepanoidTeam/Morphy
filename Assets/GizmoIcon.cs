using UnityEngine;
using System.Collections;

public class GizmoIcon : MonoBehaviour {

	public string iconName;

	void OnDrawGizmos()
	{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(transform.position, 1.0f);

		Gizmos.DrawIcon(transform.position, iconName);
	}
}
