using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


	public Camera m_Camera;

	public Transform m_PlayerPosition;

	public Vector3 spawnPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void Respawn() {
		

		var jelly = m_PlayerPosition.GetComponent<JellySprite>();
		if (jelly)
		{
			jelly.SetPosition(spawnPosition, true);
		}
	}
}
