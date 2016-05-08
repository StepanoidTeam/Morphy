using UnityEngine;

public class BlobMover : MonoBehaviour
{

	public float JumpForce = 10.0f;
	public float Speed = 10.0f;

	public int groundedBodies = 1;
	public float onAirSpeedPenalty = 4f;

	public LayerMask m_GroundLayer;

	JellySprite m_JellySprite;

	Animator m_Animator;


	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		m_JellySprite = GetComponent<JellySprite>();

		m_Animator = GetComponentInChildren<Animator>();
	}


	bool CheckIsGrounded()
	{
		return m_JellySprite.IsGrounded(m_GroundLayer, groundedBodies);
	}


	public void Jump()
	{
		if (CheckIsGrounded())
		{
			m_JellySprite.AddForce(JumpForce * Vector2.up);
			direction.y = 1;
		}
	}

	public void MoveLeft()
	{
		Move(-1);
	}

	public void MoveRight()
	{
		Move(1);
	}


	Vector2 direction = Vector2.zero;

	void FixedUpdate()
	{
		direction = Vector2.Lerp(direction, Vector2.zero, Time.deltaTime);
		m_Animator.SetFloat("HDirection", direction.x);
		m_Animator.SetFloat("VDirection", direction.y);
	}

	public void Move(float hDirection)
	{

		direction.x = hDirection;

		m_JellySprite.AddForce(Vector2.right * direction.x * Speed * (CheckIsGrounded() ? 1 : 1 / onAirSpeedPenalty));
	}
}
