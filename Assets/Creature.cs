using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{

	public Vector3 spawnPosition;


	// Use this for initialization
	void Start()
	{

		spawnPosition = transform.position;

	}

	// Update is called once per frame
	void Update()
	{

	}


	void FixedUpdate()
	{

	}

	void OnJellyTriggerEnter2D(JellySprite.JellyCollider2D trigger) { 
		Debug.Log(trigger.Collider2D.tag);

	}

}
