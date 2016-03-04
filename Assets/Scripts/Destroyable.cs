using UnityEngine;
using System.Collections;
using System;

public class Destroyable : MonoBehaviour
{
	public float Durability = 1f;
	public float MaxDurability = 1f;
	public Vector3 spawnPosition;

	public float RespawnTimeout = 1f;


	public string[] Hazards;

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
			StartCoroutine(Respawn());
			
		}

	}

	public IEnumerator Respawn()
	{
		gameObject.GetComponent<Renderer>().enabled = false;

		yield return new WaitForSeconds(RespawnTimeout);

		

		this.Durability = MaxDurability;
		//this.transform.position = spawnPosition;

		//this.transform.position.Set(spawnPosition.x, spawnPosition.y,spawnPosition.z); ;
		//this.transform.position.Set(70f, 25f, 0);

		var jelly = this.GetComponent<JellySprite>();
		jelly.SetPosition(spawnPosition, true);

		gameObject.GetComponent<Renderer>().enabled = true;

		Debug.Log("respawned " + Time.time);
	}

	// Update is called once per frame
	void Update()
	{

	}


	void FixedUpdate()
	{

	}


	void OnTriggerEnter2D(Collider2D trigger) {
		if (Array.IndexOf(Hazards, trigger.tag) >= 0)
		{
			var go = trigger.gameObject;
			var damager = go.GetComponent<Damager>();
			if (damager)
			{
				Hit(damager.DamagePerSecond);
			}
		}
	}


	void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D trigger)
	{

		if (Array.IndexOf(Hazards, trigger.Collider2D.tag) >= 0)
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