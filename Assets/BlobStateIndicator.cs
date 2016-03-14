using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class BlobStateIndicator : MonoBehaviour
{


	public Sprite StateImage;

	bool isActive;

	public string Hotkey;

	Color colorActive = new Color(1, 1, 1, 1);
	Color colorInactive = new Color(1, 1, 1, 1);

	Image m_Image;
	Text m_Text;


	public void Init(Texture2D tex, string hotkey){
		StateImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.one);
		isActive = false;
		Hotkey = hotkey;
	}


	// Use this for initialization
	void Start()
	{

		m_Image = GetComponent<Image>();
		m_Text = GetComponentInChildren<Text>();


		m_Image.sprite = StateImage;
		m_Text.text = Hotkey;

		SetState(isActive);
	}



	public void SetState(bool value)
	{
		isActive = value;
		//m_Image.material.SetColor("_Color", isActive ? colorActive : colorInactive);
	}
}
