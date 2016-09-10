using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float timeDuration = 1.0f;
	public float timer = 0.0f;

	List<int> initiative = new List<int>();
	public List<int> Initiative
	{
		get { return initiative; }
	}

	public bool canAttack = true;

	public KeyCode strong;
	public KeyCode fast;
	public KeyCode tricky;

	int playerNum = 0;

	public float baseDamage = 0.1f;
	public float strongMultiplier = 3.0f;
	public float fastMultiplier = 1.0f;
	public float trickyMultiplier = 0.5f;

	void Start()
	{
		playerNum = AssignPlayerNum();
		initiative = StockInitiative();
	}

	//determine whether this is player 1 or player 2
	//IMPORTANT: this assumes that player names are in the format "Player #"
	int AssignPlayerNum()
	{
		switch(gameObject.name[7])
		{
			case '1':
				return 2;
				break;
			case '2':
				return 1;
				break;
			default:
				Debug.Log("Illegal player number: " + gameObject.name[7]);
				break;
		}

		Debug.Log("Failed to assign player number; " + gameObject.name + " is defaulting to 1");
		return 1;
	}

	//get the correct health dot fill value, based on whether this is player 1 or player 2
	float AssignOpponentHealth()
	{
		switch(playerNum)
		{
			case 1:
				return transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
					.GetComponent<Health>().HealthFill;
				break;
			case 2:
				return transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
					.GetComponent<Health>().HealthFill;
				break;
			default:
				Debug.Log("Illegal player number: " + playerNum);
				break;
		}

		Debug.Log("Failed to assign opponent health; " + gameObject.name + " is defaulting to player 2's health meter");
		return transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
			.GetComponent<Health>().HealthFill;
	}

	//determine initial state of initiative
	List<int> StockInitiative()
	{
		Transform initHierarchy = transform.root.Find("Initiative lists").Find("P" + playerNum.ToString());
		List<int> temp = new List<int>();

		foreach (Transform initiativeMarker in initHierarchy)
		{
			if (initiativeMarker.name.Contains("green")) { temp.Add(1); }
			else if (initiativeMarker.name.Contains("yellow")) { temp.Add(2); }
			else { Debug.Log("Illegal initiative marker: " + initiativeMarker.name); }
		}

		return temp;
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (timer >= timeDuration)
		{
			ResolveNextInitiative();
			timer = 0.0f;
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

			Debug.Log("Removing initiative[0]: " + initiative[0].ToString());
			initiative.RemoveAt(0);
		}
	}

	/*
	 * 
	 * This function manages combat
	 * strong attacks: 3x normal damage, add a "wait" to the initiative list
	 * fast attacks: 1x normal damage, add a "go" to the initiative list
	 * tricky attacks: 0.5x normal damage, add a "go" to your initiative list and a "wait" to the opponent's
	 * 
	 */

	void ListenForInput()
	{
		if (Input.GetKeyDown(strong))
		{
			switch(playerNum)
			{
				case 1:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * strongMultiplier;;
					break;
				case 2:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * fastMultiplier;
					break;
				default:
					Debug.Log("Illegal player number: " + playerNum);
					break;
			}
		
			initiative.Add(2);
			PostAttackReset();
			Debug.Log("strong detected");
		}
		else if (Input.GetKeyDown(fast))
		{
			switch(playerNum)
			{
				case 1:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * fastMultiplier;
					break;
				case 2:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * fastMultiplier;
					break;
				default:
					Debug.Log("Illegal player number: " + playerNum);
					break;
			}
				
			initiative.Add(1);
			PostAttackReset();
		}
		else if (Input.GetKeyDown(tricky))
		{
			switch(playerNum)
			{
				case 1:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * trickyMultiplier;
					transform.root.Find("Players").Find("Player 2").GetComponent<Player>().Initiative.Add(2);
					break;
				case 2:
					transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
						.GetComponent<Health>().HealthFill -= baseDamage * trickyMultiplier;
					transform.root.Find("Players").Find("Player 1").GetComponent<Player>().Initiative.Add(2);
					break;
				default:
					Debug.Log("Illegal player number: " + playerNum);
					break;
			}
				
			initiative.Add(1);
			PostAttackReset();
		}
	}

	void PostAttackReset()
	{
		canAttack = false;

		foreach (int item in initiative) { Debug.Log(item); }
	}
}
