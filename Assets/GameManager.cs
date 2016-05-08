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
	BlobMover currentPlayerMover;


	public void BackToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}


	void FixedUpdate()
	{
		if (currentPlayer == null)
		{
			Respawn();
		}

		//todo: move to buttons
		//if (Input.GetKey(KeyCode.Tab))
		//{
		//	Respawn();
		//}

		
	}

	public void PlayerJump()
	{
		currentPlayerMover.Jump();
	}


	public void PlayerMove(float direction)
	{
		currentPlayerMover.Move(direction);
	}



	public void Respawn()
	{
		Destroy(currentPlayer);

		currentPlayer = Instantiate(PlayerPrefab);
		currentPlayer.transform.position = SpawnPosition.position;

		var jelly = currentPlayer.GetComponent<UnityJellySprite>();
		//jelly.SetPosition(SpawnPosition.position, true);


		cameraFollow.Target = jelly.transform;
		//cameraFollow.SetupCamera();

		blobStateManager.Player = jelly;
		blobStateManager.Setup();

		currentPlayerMover = currentPlayer.GetComponent<BlobMover>();
	}
}
