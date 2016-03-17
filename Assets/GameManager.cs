using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject PlayerPrefab;

	public Transform SpawnPosition;

	public CameraFollow cameraFollow;
	public BlobStateManager blobStateManager;


	GameObject currentPlayer;


	void Start()
	{

		
    }

	// Update is called once per frame
	void Update()
	{

	}

	void FixedUpdate()
	{
		if (currentPlayer == null)
		{
			Respawn();
		}
	}


	void Respawn()
	{
		currentPlayer = Instantiate(PlayerPrefab);
		var jelly = currentPlayer.GetComponent<UnityJellySprite>();
		//jelly.SetPosition(SpawnPosition.position, true);


		cameraFollow.Target = jelly.transform;
		cameraFollow.SetupCamera();

		blobStateManager.Player = jelly;
		blobStateManager.Setup();
	}



}
