using UnityEngine;

public class Hotkey : MonoBehaviour
{

	public KeyCode key;
	public bool autoFire;

	bool isButtonPressed;

	UnityEngine.UI.Button button;

	// Use this for initialization
	void Start()
	{
		button = GetComponent<UnityEngine.UI.Button>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var isKeyPressed = autoFire ? Input.GetKey(key) : Input.GetKeyDown(key);

		if ((autoFire ? isButtonPressed : false) || isKeyPressed)
		{
			button.onClick.Invoke();
		}
	}

	public void ButtonDown()
	{
		isButtonPressed = true;

	}

	public void ButtonUp()
	{
		isButtonPressed = false;
	}

}
