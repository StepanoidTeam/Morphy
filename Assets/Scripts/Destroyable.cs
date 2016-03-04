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

	bool IsHazardous(string tag)
	{
		return Array.IndexOf(Hazards, tag) >= 0;
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
		var renderer = gameObject.GetComponent<Renderer>();
		if (renderer)
		{

			renderer.enabled = false;
		}
		else {
			this.gameObject.SetActive(false);
		}

		yield return new WaitForSeconds(RespawnTimeout);



		this.Durability = MaxDurability;
		//this.transform.position = spawnPosition;

		//this.transform.position.Set(spawnPosition.x, spawnPosition.y,spawnPosition.z); ;
		//this.transform.position.Set(70f, 25f, 0);

		var jelly = this.GetComponent<JellySprite>();
		if (jelly)
		{
			jelly.SetPosition(spawnPosition, true);
		}
		else
		{

			this.gameObject.transform.position = spawnPosition;
		}

		if (renderer) renderer.enabled = true;

		Debug.Log("respawned " + Time.time);
	}

	// Update is called once per frame
	void Update()
	{

	}


	void FixedUpdate()
	{

	}
	
	void OnTriggerEnter2D(Collider2D trigger)
	{

		if (IsHazardous(trigger.tag))
		{
			var go = trigger.gameObject;
			var damager = go.GetComponent<Damager>();
			if (damager)
			{
				Hit(damager.DamagePerSecond);
				//coll.gameObject.SendMessage("ApplyDamage", 10);
			}
		}
	}


	void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D trigger)
	{

		if (IsHazardous(trigger.Collider2D.tag))
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