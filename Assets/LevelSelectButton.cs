using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour {


	public string LevelScene;

	public UnityEngine.UI.Text ButtonText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadLevel() {
		SceneManager.LoadScene("DesertBgLayers", LoadSceneMode.Single);
		SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
		SceneManager.LoadSceneAsync(LevelScene, LoadSceneMode.Additive);
	}


	public void Init(string sceneName) {

		LevelScene = sceneName;

		ButtonText.text = LevelScene;

	}
}
