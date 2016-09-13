using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	const string GAME_SCENE = "EmptyScene";
	const string TITLE_SCENE = "Title scene";

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (SceneManager.GetActiveScene().name == TITLE_SCENE) { SceneManager.LoadScene(GAME_SCENE); }
		}
	}
}
