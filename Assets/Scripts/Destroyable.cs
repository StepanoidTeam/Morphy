using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class Destroyable : MonoBehaviour
{
	public float Durability = 1f;
	public float MaxDurability = 1f;

	public string[] Hazards;

	// Use this for initialization
	void Start()
	{

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
			Destroy(this.gameObject);
		}

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