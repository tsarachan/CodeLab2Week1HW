using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

	Image healthImage;

	float healthFill = 1.0f;
	public float HealthFill
	{
		get { return healthFill; }
		set
		{
			healthFill = value;
			healthImage.fillAmount = healthFill;
			if (healthFill == 0.0f) { LoseGame(); }
		}
	}

	void Start()
	{
		healthImage = GetComponent<Image>();
	}

	void LoseGame()
	{
		//do something
	}
}
