using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class BlobStateIndicator : MonoBehaviour
{
	public BlobStateManager blobStateManager;

	public Sprite StateImage;

	public string Name;
	public KeyCode Hotkey;

	Image m_Image;
	Text m_Text;


	public void Init(Texture2D tex, string name, KeyCode hotkey)
	{
		StateImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one);
		Hotkey = hotkey;
		Name = name;
	}


	// Use this for initialization
	void Start()
	{

		m_Image = GetComponent<Image>();
		m_Text = GetComponentInChildren<Text>();


		m_Image.sprite = StateImage;
		m_Text.text = Hotkey.ToString();

		
	}

	void FixedUpdate()
	{
		if (Input.GetKeyDown(Hotkey))
		{
			SetCurrentState();
		}
	}


	public void SetCurrentState()
	{
		blobStateManager.SetBlobState(Name);
	}
}
