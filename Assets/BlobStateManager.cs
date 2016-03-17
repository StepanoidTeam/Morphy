using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class BlobStateManager : MonoBehaviour
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


	public UnityJellySprite Player;

	Damager playerDamager;

	public UnityEngine.UI.LayoutGroup BlobStateContainer;
	public GameObject BlobStateIndicatorPrefab;


	public BlobStateBehavior сurrentState;
	public BlobStateBehavior GetCurrentState()
	{
		return сurrentState;
	}

	public BlobStateBehavior[] BlobStates;

	Renderer playerRenderer;



	void Start()
	{
		Setup();
	}



	public void Setup()
	{
		if (Player)
		{
			playerRenderer = Player.GetComponent<Renderer>();
			playerDamager = Player.GetComponent<Damager>();


			InitIndicators();
			SetBlobState(BlobStates.First());
		}
	}


	void InitIndicators()
	{

		//clear all old states
		foreach (var t in BlobStateContainer.transform.OfType<Transform>().ToList())
		{
			Destroy(t.gameObject);
		}


		foreach (var state in BlobStates)
		{

			var go = Instantiate(BlobStateIndicatorPrefab);
			var bsi = go.GetComponent<BlobStateIndicator>();

			bsi.Init(state.texture, state.key.ToString());

			go.transform.SetParent(BlobStateContainer.transform);
		}
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

		playerRenderer.material.mainTexture = state.texture;


		//GetComponent<Damager>().Tags
		//todo: set tags

		playerDamager.Tags = new string[] { "Player", state.stateName };


		//if (Player)
		{
			//jelly params
			Player.m_Stiffness = state.jellyStiffness;
			Player.m_Mass = state.jellyMass;
		}
	}
}
