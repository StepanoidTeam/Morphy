using UnityEngine;
using System.Collections;

public class mover : MonoBehaviour
{


	public float m_JumpForce = 100.0f;
	public float m_Speed = 100.0f;

	public float m_GroundRayOffset = 3.0f;
	public float m_GroundRayLength = 3.0f;

	public LayerMask m_GroundLayer;

	Rigidbody2D body;


	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		body = GetComponent<Rigidbody2D>();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawLine(transform.position + Vector3.down * m_GroundRayOffset, transform.position + Vector3.down * (m_GroundRayLength + m_GroundRayOffset));

	}

	bool CheckIsGrounded()
	{

		RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * m_GroundRayOffset, Vector2.down, m_GroundRayLength);
		if (hit.collider != null)
		{
			float distance = Mathf.Abs(hit.point.y - transform.position.y);

			Debug.Log(distance);

			return true;
		}


		return false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var Axes = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (Input.GetButton("Jump") && CheckIsGrounded() /* && body.IsGrounded(m_GroundLayer, 2)*/)
		{
			body.AddForce(m_JumpForce * Vector2.up);
			//Debug.Log("jumped2d");
		}

		body.AddForce(Axes * m_Speed);

	}
}
