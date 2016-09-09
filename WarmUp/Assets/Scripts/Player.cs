using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float timeDuration = 1.0f;
	float timer = 0.0f;

	List<int> initiative = new List<int>();

	bool canAttack = false;

	public KeyCode strong;
	public KeyCode fast;
	public KeyCode tricky;

	void Update()
	{
		timer += Time.deltaTime;

		if (timer >= timeDuration)
		{
			ResolveNextInitiative();
		}

		if (canAttack) { ListenForInput(); }
	}

	//Take the next action in each player's initiative list
	//if it's 1 (for "go"), the player gets to act
	//if it's 2 (for "wait"), the player can't go yet.
	void ResolveNextInitiative()
	{
		if (initiative.Count > 0)
		{
			if (initiative[0] == 1) { canAttack = true; }

			initiative.Remove(0);
		}
	}

	void ListenForInput()
	{
		if (Input.GetKeyDown(strong)) { }
	}
}
