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


	bool _isSelected = false;
	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			if (m_Image != null)
			{
				m_Image.color = new Color(1f, 1f, 1f, value ? 1 : 0.3f);
			}
		}
	}

	public void SetCurrentState()
	{
		blobStateManager.SetBlobState(Name);
	}
}
