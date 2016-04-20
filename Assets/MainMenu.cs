using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	public string[] LevelScenes;
	public GameObject LevelContainer;
	public GameObject LevelButtonPrefab;



	// Use this for initialization
	void Start()
	{

		//clear all old levels
		foreach (var t in LevelContainer.transform.OfType<Transform>().ToList())
		{
			Destroy(t.gameObject);
		}


		foreach (var levelSceneName in LevelScenes) {

			var go = Instantiate(LevelButtonPrefab);
			var lsb = go.GetComponent<LevelSelectButton>();

			lsb.Init(levelSceneName);

			go.transform.SetParent(LevelContainer.transform);
		}

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ShowLevelSelectionMenu()
	{

	}

	public void StartGame()
	{
		SceneManager.LoadScene("Morphy Scene");
	}


	public void LoadLevel(string sceneName)
	{

	}

	public void ExitGame()
	{
		Application.Quit();
	}

}
