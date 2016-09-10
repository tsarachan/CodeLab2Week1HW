using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float initStepDuration = 1.0f;
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

	GameObject goSymbol;
	GameObject waitSymbol;

	const float INIT_START_X_COORD = 2.75f;
	const float INIT_Y_COORD = 0.0f;
	const float INIT_Z_COORD = 0.0f;

	void Start()
	{
		playerNum = AssignPlayerNum();
		initiative = StockInitiative();
		goSymbol = Resources.Load("Prefabs/Green") as GameObject;
		waitSymbol = Resources.Load("Prefabs/Yellow") as GameObject;
	}

	//determine whether this is player 1 or player 2, set values accordingly
	//IMPORTANT: this assumes that player names are in the format "Player #"
	int AssignPlayerNum()
	{
		switch(gameObject.name[7])
		{
			case '1':
				return 1;
				break;
			case '2':
				return 2;
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
			if (initiativeMarker.name.Contains("Green")) { temp.Add(1); }
			else if (initiativeMarker.name.Contains("Yellow")) { temp.Add(2); }
			else { Debug.Log("Illegal initiative marker: " + initiativeMarker.name); }
		}

		return temp;
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (timer >= initStepDuration)
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
			StrongEffects();
			AddToInitiativeList(2);
			PostAttackReset();
		}
		else if (Input.GetKeyDown(fast))
		{
			FastEffects();	
			PostAttackReset();
		}
		else if (Input.GetKeyDown(tricky))
		{
			TrickyEffects();
			PostAttackReset();
		}
	}

	void StrongEffects()
	{
		switch(playerNum)
		{
			case 1:
				transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
					.GetComponent<Health>().HealthFill -= baseDamage * strongMultiplier;;
				break;
			case 2:
				transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
					.GetComponent<Health>().HealthFill -= baseDamage * strongMultiplier;
				break;
			default:
				Debug.Log("Illegal player number: " + playerNum);
				break;
		}
	}

	void FastEffects()
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
	}

	void TrickyEffects()
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
	}

	void AddToInitiativeList(int whatToAdd)
	{
		initiative.Add(whatToAdd);

		Transform initList = transform.root.Find("Initiative lists").Find("P" + playerNum.ToString());
		GameObject newSymbol = null;

		switch(playerNum)
		{
			case 1:
				if (whatToAdd == 1) { newSymbol = Instantiate(goSymbol,
															  new Vector3(-INIT_START_X_COORD - initList.childCount,
																   		  INIT_Y_COORD,
																		  INIT_Z_COORD),
															  Quaternion.identity) as GameObject; }
				else if (whatToAdd == 2) { newSymbol = Instantiate(waitSymbol,
															  new Vector3(-INIT_START_X_COORD - initList.childCount,
																			   INIT_Y_COORD,
																			   INIT_Z_COORD),
																   Quaternion.identity) as GameObject; }
				else { Debug.Log("Illegal whatToAdd: " + whatToAdd.ToString()); }
				break;
			case 2:
				if (whatToAdd == 1) { newSymbol = Instantiate(goSymbol,
															  new Vector3(INIT_START_X_COORD + initList.childCount,
																		  INIT_Y_COORD,
																		  INIT_Z_COORD),
															  Quaternion.identity) as GameObject; }
				else if (whatToAdd == 2) { newSymbol = Instantiate(waitSymbol,
															  new Vector3(INIT_START_X_COORD + initList.childCount,
																			   INIT_Y_COORD,
																			   INIT_Z_COORD),
																   Quaternion.identity) as GameObject; }
				else { Debug.Log("Illegal whatToAdd: " + whatToAdd.ToString()); }
				break;
			default:
				Debug.Log("Illegal playerNum: " + playerNum.ToString());
				break;

		}

		newSymbol.transform.parent = initList;
	}

	void PostAttackReset()
	{
		canAttack = false;

		foreach (int item in initiative) { Debug.Log(item); }
	}
}
