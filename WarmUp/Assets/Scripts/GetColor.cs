using UnityEngine;
using System.Collections;

public class GetColor : MonoBehaviour {

	public virtual int GetSpriteColor()
	{
		return Random.Range(0, 3);
	}
}
