using UnityEngine;
using System.Collections;

public class BlobMover : MonoBehaviour
{

	public float m_JumpForce = 10.0f;
	public float m_Speed = 10.0f;

	public LayerMask m_GroundLayer;

	JellySprite m_JellySprite;



	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		m_JellySprite = GetComponent<JellySprite>();
	}

	public float m_GroundRayOffset = 3.0f;
	public float m_GroundRayLength = 3.0f;

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
		var Axes = new Vector3(Input.GetAxis("Horizontal"), 0/*, Input.GetAxis("Vertical")*/);

		if (Input.GetButton("Jump") /*&& CheckIsGrounded() */&& m_JellySprite.IsGrounded(m_GroundLayer, 1))
		{
			m_JellySprite.AddForce(m_JumpForce * Vector2.up);
			//body.AddForce(m_JumpForce * Vector2.up);
			//Debug.Log("jumped Jellly");
		}

		m_JellySprite.AddForce(Axes * m_Speed);
		//body.AddForce(Axes * m_Speed);
	}


}
