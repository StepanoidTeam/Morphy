using UnityEngine;

public class GizmoIcon : MonoBehaviour {

	public string iconName;

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, iconName);
	}
}
