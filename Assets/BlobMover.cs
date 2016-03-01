using UnityEngine;
using System.Collections;

public class BlobMover : MonoBehaviour
{

	public float JumpForce = 10.0f;
	public float Speed = 10.0f;

	public int groundedBodies = 1;
	public float onAirSpeedPenalty = 4f;

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
		return m_JellySprite.IsGrounded(m_GroundLayer, groundedBodies);


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
		var isGrounded = CheckIsGrounded();

		var axes = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (Input.GetButton("Jump") && isGrounded)
		{
			m_JellySprite.AddForce(JumpForce * Vector2.up);
		}

		m_JellySprite.AddForce(Vector2.right * axes.x * Speed * (isGrounded ? 1 : 1 / onAirSpeedPenalty));

		if (!isGrounded && axes.z < 0)
		{
			m_JellySprite.AddForce(Vector2.up * axes.z * Speed);
		}
	}


}
