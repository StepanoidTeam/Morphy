using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
		if (Input.GetButton("Fire1"))
		{
			Destroy(currentPlayer);
		}

		if (Input.GetButton("Cancel"))
		{
			SceneManager.LoadScene("MainMenu");
		}


		if (currentPlayer == null)
		{
			Respawn();
		}
	}


	void Respawn()
	{
		currentPlayer = Instantiate(PlayerPrefab);
		currentPlayer.transform.position = SpawnPosition.position;

		var jelly = currentPlayer.GetComponent<UnityJellySprite>();
		//jelly.SetPosition(SpawnPosition.position, true);


		cameraFollow.Target = jelly.transform;
		//cameraFollow.SetupCamera();

		blobStateManager.Player = jelly;
		blobStateManager.Setup();
	}



}
