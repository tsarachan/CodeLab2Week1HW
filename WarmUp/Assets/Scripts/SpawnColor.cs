using UnityEngine;
using System.Collections;

public class SpawnColor : MonoBehaviour {

	public Sprite[] sprites;

	void Start()
	{
		InvokeRepeating("Spawn", 1, 1);
	}

	void Spawn()
	{
		GameObject go = Instantiate(Resources.Load("Prefabs/Color")) as GameObject;

		int num = GetComponent<GetColor>().GetSpriteColor();

		go.GetComponent<SpriteRenderer>().sprite = sprites[num];
	}
}
