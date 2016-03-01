using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{
	public float Durability = 1f;
	public float MaxDurability = 1f;
	public Vector3 spawnPosition;


	// Use this for initialization
	void Start()
	{

		spawnPosition = transform.position;

	}


	public void Hit(float hitRate)
	{

		Durability -= hitRate;

		if (Durability <= 0)
		{
			Respawn();
		}

	}

	public void Respawn()
	{

		this.Durability = MaxDurability;
		//this.transform.position = spawnPosition;

		//this.transform.position.Set(spawnPosition.x, spawnPosition.y,spawnPosition.z); ;
		this.transform.position.Set(70f, 25f, 0);
		Debug.Log("respawned " + Time.time);

	}

	// Update is called once per frame
	void Update()
	{

	}


	void FixedUpdate()
	{

	}

	void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D trigger)
	{
		if (trigger.Collider2D.tag == "Damager")
		{
			var go = trigger.Collider2D.gameObject;
			var damager = go.GetComponent<Damager>();
			if (damager)
			{
				Hit(damager.DamagePerSecond);
			}
		}
	}

}
