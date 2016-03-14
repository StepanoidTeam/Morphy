using UnityEngine;
using System.Collections;
using System;
using System.Linq;

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
		return Hazards.Contains(tag);
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
		else
		{
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


	void OnTriggerEnter2D(Collider2D trigger)
	{
		GetDamagerAndDoStuff(trigger.gameObject);
	}


	void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D trigger)
	{
		GetDamagerAndDoStuff(trigger.Collider2D.gameObject);
	}


	void GetDamagerAndDoStuff(GameObject go)
	{
		var damager = go.GetComponent<Damager>();
		if (damager)
		{
			foreach (var h in Hazards)
			{
				if (damager.HasTag(h))
				{
					Hit(damager.DamagePerSecond);
					//coll.gameObject.SendMessage("ApplyDamage", 10);
					break;
				}
			}
		}

	}

}