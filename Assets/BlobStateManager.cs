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

		public float m_jellyStiffness;
		public float m_jellyMass;

		public float m_AngularDrag;
		public float m_DampingRatio;
		public float m_Drag;
		public float m_GravityScale;

		public bool m_LockRotation;
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
			bsi.blobStateManager = this;

			bsi.Init(state.texture, state.stateName, state.key);

			go.transform.SetParent(BlobStateContainer.transform);
		}
	}

	public void SetBlobState(string stateName)
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


		//jelly params
		Player.m_Stiffness = state.m_jellyStiffness;
		Player.m_Mass = state.m_jellyMass;

		Player.m_AngularDrag = state.m_AngularDrag;
		Player.m_DampingRatio = state.m_DampingRatio;
		Player.m_Drag = state.m_Drag;
		Player.m_GravityScale = state.m_GravityScale;
		Player.m_LockRotation = state.m_LockRotation;

		Player.UpdateJoints();
		Player.WakeUp();
	}
}
