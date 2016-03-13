using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class BlobStateSwitcher : MonoBehaviour
{

	[Serializable]
	public class BlobStateBehavior
	{

		public string stateName;
		public KeyCode key;
		public Texture2D texture;

		public float jellyStiffness;
		public float jellyMass;

	}


	BlobStateBehavior сurrentState;
    public BlobStateBehavior GetCurrentState() {
		return сurrentState;
	}

	public BlobStateBehavior[] BlobStates;

	Renderer m_Renderer;

	UnityJellySprite jellySprite;

	void Start()
	{
		m_Renderer = GetComponent<Renderer>();
		jellySprite = GetComponent<UnityJellySprite>();

		SetBlobState(BlobStates.First());
	}

	// Update is called once per frame
	void Update()
	{


	}
	

	void FixedUpdate()
	{

		CheckKeys();

	}


	void CheckKeys()
	{
		foreach (var state in BlobStates)
		{
			if (Input.GetKeyDown(state.key))
			{
				SetBlobState(state);
			}
		}
	}



	void SetBlobState(string stateName)
	{
		var blobState = Array.Find(BlobStates, x => x.stateName == stateName);
		SetBlobState(blobState);
	}

	void SetBlobState(BlobStateBehavior state)
	{
		сurrentState = state;

		m_Renderer.material.mainTexture = state.texture;

		//GetComponent<Damager>().Tags



		//jelly params
		jellySprite.m_Stiffness = state.jellyStiffness;
		jellySprite.m_Mass = state.jellyMass;
    }

}
