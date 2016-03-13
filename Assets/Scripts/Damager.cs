using UnityEngine;
using System.Linq;

public class Damager : MonoBehaviour
{

	public float DamagePerSecond = 1f;

	public string[] Tags;

	public bool HasTag(string tag)
	{
		return Tags.Contains(tag);
	}

}
