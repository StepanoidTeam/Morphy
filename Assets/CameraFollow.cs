using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Camera Camera;
	public Transform Target;            // The position that that camera will be following.
	public float smoothing = 5f;        // The speed with which the camera will be following.

	Vector3 offset;                     // The initial offset from the target.

	void Start()
	{
		SetupCamera();
	}

	bool setupOk = false;

	public void SetupCamera()
	{
		if (Target && !setupOk)
		{
			setupOk = true;
			// Calculate the initial offset.
			offset = Camera.transform.position - Target.position;
		}
	}

	void FixedUpdate()
	{
		if (Target)
		{
			Vector3 targetCamPos = Target.position + offset;

			// Smoothly interpolate between the camera's current position and it's target position.
			Camera.transform.position = Vector3.Lerp(Camera.transform.position, targetCamPos, smoothing * Time.deltaTime);
		}
	}
}