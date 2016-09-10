using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

	Image healthImage;

	public float gameEndDelay = 2.0f;

	float healthFill = 1.0f;
	public float HealthFill
	{
		get { return healthFill; }
		set
		{
			healthFill = value;
			healthImage.fillAmount = healthFill;
			Debug.Log(healthFill);
			if (healthFill <= 0.0f) { LoseGame(); }
		}
	}

	void Start()
	{
		healthImage = GetComponent<Image>();
	}

	void LoseGame()
	{
		transform.root.GetComponent<InitiativeSystem>().GameOver = true;

		StartCoroutine(GameEnd());
	}

	IEnumerator GameEnd()
	{
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(gameEndDelay));

		SceneManager.LoadScene(0);
	}
}
