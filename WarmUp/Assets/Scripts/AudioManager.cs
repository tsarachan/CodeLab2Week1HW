using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	AudioSource audioSource;
	AudioClip coin;
	AudioClip bump;
	AudioClip stomp;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();

		coin = Resources.Load("sound/coin") as AudioClip;
		bump = Resources.Load("sound/bumpwall") as AudioClip;
		stomp = Resources.Load("sound/stomp") as AudioClip;
	}

	public void PlayClip(string attack)
	{
		switch (attack)
		{
			case "Strong":
				audioSource.PlayOneShot(stomp);
				break;
			case "Fast":
				audioSource.PlayOneShot(coin);
				break;
			case "Tricky":
				audioSource.PlayOneShot(bump);
				break;
			default:
				Debug.Log("Illegal attack: " + attack);
				break;
		}
	}
}
