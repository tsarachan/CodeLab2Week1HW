﻿/*
 * This script contains the main game logic
 * Players are either in the "can act" or "can't act" state
 * When a player can act, the game listens for her inputs
 * Upon input, three things happen:
 * 1. The player enters the "can't act" state
 * 2. Any damage caused by the attack is inflicted on the other player
 * 3. The initiative list is updated
 * 
 * Whichever player is next in the initiative list is then put into the "can act" state
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InitiativeSystem : MonoBehaviour {

	public float initStepDuration = 1.0f;
	float timer = 0.0f;
	Transform initList;
	GameObject p1InitSymbol;
	GameObject p2InitSymbol;

	const float INIT_X_POS = 0.0f;
	const float INIT_Y_START_POS = -3.15f;
	const float INIT_Z_POS = 0.0f;

	bool p1CanAct = true;
	bool p2CanAct = true;
	Image p1ReadyImage;
	Image p2ReadyImage;

	Health p1Health;
	Health p2Health;


	public KeyCode p1Strong = KeyCode.Q;
	public KeyCode p1Fast = KeyCode.W;
	public KeyCode p1Tricky = KeyCode.E;
	public KeyCode p2Strong = KeyCode.I;
	public KeyCode p2Fast = KeyCode.O;
	public KeyCode p2Tricky = KeyCode.P;

	public float baseDamage = 0.1f;
	public float strongMultiplier = 3.0f;
	public float fastMultiplier = 1.0f;
	public float trickyMultiplier = 0.0f;

	bool gameOver = false;
	public bool GameOver { get; set; }

	AudioManager audioManager;

	void Start()
	{
		p1InitSymbol = Resources.Load("Prefabs/Green") as GameObject;
		p2InitSymbol = Resources.Load("Prefabs/Yellow") as GameObject;

		p1ReadyImage = transform.root.Find("Players").Find("Player 1").Find("Health meter").Find("Ready image")
			.GetComponent<Image>();
		p2ReadyImage = transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Ready image")
			.GetComponent<Image>();

		p1Health = transform.root.Find("Players").Find("Player 1").Find("Health meter").Find("Health image")
			.GetComponent<Health>();
		p2Health = transform.root.Find("Players").Find("Player 2").Find("Health meter").Find("Health image")
			.GetComponent<Health>();

		initList = transform.root.Find("Initiative list");

		audioManager = GetComponent<AudioManager>();

		SetReadiness(2, false);
	}

	void Update()
	{
		if (!GameOver)
		{
			if (p1CanAct) { ListenForInput(1); }
			if (p2CanAct) { ListenForInput(2); }
		}
	}

	void ListenForInput(int playerNum)
	{
		switch (playerNum)
		{
			case 1:
				if (Input.GetKeyDown(p1Strong))
				{
					InflictDamage(1, 2, strongMultiplier);
					AddInitSymbol(1, "Strong");
					SetReadiness(1, false);
					audioManager.PlayClip("Strong");
					ResolveNextInit();
				}
				else if (Input.GetKeyDown(p1Fast))
				{
					InflictDamage(1, 2, fastMultiplier);
					AddInitSymbol(1 , "Fast");
					SetReadiness(1, false);
					audioManager.PlayClip("Fast");
					ResolveNextInit();
				}
				else if (Input.GetKeyDown(p1Tricky))
				{
					TrickyEffects(1, 2, trickyMultiplier);
					AddInitSymbol(1, "Tricky");
					SetReadiness(1, false);
					audioManager.PlayClip("Tricky");
					ResolveNextInit();
				}
				break;
			case 2:
				if (Input.GetKeyDown(p2Strong))
				{
					InflictDamage(2, 1, strongMultiplier);
					AddInitSymbol(2, "Strong");
					SetReadiness(2, false);
					audioManager.PlayClip("Strong");
					ResolveNextInit();
				}
				else if (Input.GetKeyDown(p2Fast))
				{
					InflictDamage(2, 1, fastMultiplier);
					AddInitSymbol(2, "Fast");
					SetReadiness(2, false);
					audioManager.PlayClip("Fast");
					ResolveNextInit();
				}
				else if (Input.GetKeyDown(p2Tricky))
				{
					TrickyEffects(2, 1, trickyMultiplier);
					AddInitSymbol(2, "Tricky");
					SetReadiness(2, false);
					audioManager.PlayClip("Tricky");
					ResolveNextInit();
				}
				break;
			default:
				Debug.Log("Illegal playerNum: " + playerNum);
				break;
		}
	}

	void InflictDamage(int attacker, int defender, float multiplier)
	{
		switch (attacker)
		{
			case 1:
				p2Health.HealthFill -= baseDamage * multiplier;
				SetReadiness(1, false);
				break;
			case 2:
				p1Health.HealthFill -= baseDamage * multiplier;
				SetReadiness(2, false);
				break;
			default:
				Debug.Log("Illegal attacker: " + attacker);
				break;
		}
	}
		
	void TrickyEffects(int attacker, int defender, float multiplier)
	{
		switch (attacker)
		{
			case 1:
				SetReadiness(1, false);
				//GoToEndOfLine("Yellow");
				SwapInitSymbols("Yellow");
				break;
			case 2:
				SetReadiness(2, false);
				//GoToEndOfLine("Green");
				SwapInitSymbols("Green");
				break;
			default:
				Debug.Log("Illegal attacker: " + attacker);
				break;
		}
	}

	void SwapInitSymbols(string target)
	{
		foreach (Transform symbol in initList)
		{
			if (symbol.name.Contains(target))
			{
				if (symbol.GetSiblingIndex() < initList.childCount - 1) //if this isn't the last item in the hierarchy
				{
					int temp = symbol.GetSiblingIndex();
					initList.GetChild(temp + 1).SetSiblingIndex(temp);
					symbol.SetSiblingIndex(temp + 1);

					Debug.Log(symbol.name + " is at " + symbol.GetSiblingIndex());
					RepositionSymbols();
					break;
				}
			}
		}
	}

	void GoToEndOfLine(string target)
	{
		foreach (Transform symbol in initList)
		{
			if (symbol.name.Contains(target))
			{
				symbol.SetAsLastSibling();
				RepositionSymbols();
			}
		}
	}

	/*
	 * 
	 * Strong attacks add one of a player's symbols to the initiative list
	 * Fast attacks add two
	 * Tricky attacks add none (but note the TrickyEffects() method)
	 * 
	 */
	void AddInitSymbol(int player, string attackType)
	{
		switch(attackType)
		{
			case "Strong":
				if (player == 1) { CreateInitSymbol(1); }
				else if (player == 2) { CreateInitSymbol(2); }
				else { Debug.Log("Illegal player: " + player); }
				break;
			case "Fast":
				if (player == 1) { CreateInitSymbol(1); CreateInitSymbol(1); }
				else if (player == 2) { CreateInitSymbol(2); CreateInitSymbol(2); }
				else { Debug.Log("Illegal player: " + player); }
				break;
			case "Tricky":
				break;
			default:
				Debug.Log("Illegal attackType: " + attackType);
				break;
		}

		RepositionSymbols();
	}

	void CreateInitSymbol(int playerNum)
	{
		GameObject newSymbol;

		switch (playerNum)
		{
			case 1:
				newSymbol = Instantiate(p1InitSymbol, initList) as GameObject;
				break;
			case 2:
				newSymbol = Instantiate(p2InitSymbol, initList) as GameObject;
				break;
			default:
				Debug.Log("Illegal playernum: " + playerNum);
				break;
		}
	}

	void RepositionSymbols()
	{
		for (int i = 0; i < initList.childCount; i++)
		{
			//the awkward y position here reflects the fact that the to-be-destroyed symbol will still be present
			//until the end of the frame; the -1 corrects for it
			initList.GetChild(i).transform.localPosition = new Vector3(INIT_X_POS,
																	   INIT_Y_START_POS + i - 1,
																	   INIT_Z_POS);
		}
	}

	//allow either p1 or p2 to act, depending on who's symbol is next in the initiative list
	//then clear the symbol
	void ResolveNextInit()
	{
		if (initList.childCount > 0) { Destroy(initList.GetChild(0).gameObject); }

		if (initList.childCount > 1) //the Destroy() method resolved at end of frame, so previous symbol isn't gone yet
		{
			if (initList.GetChild(1).name.Contains("Green")) { SetReadiness(1, true); }
			else if (initList.GetChild(1).name.Contains("Yellow")) { SetReadiness(2, true); }
		}

		RepositionSymbols();
	}

	void SetReadiness(int player, bool state)
	{
		switch (player)
		{
			case 1:
				p1CanAct = state;
				p1ReadyImage.enabled = state;
				break;
			case 2:
				p2CanAct = state;
				p2ReadyImage.enabled = state;
				break;
			default:
				Debug.Log("Illegal player: " + player);
				break;
		}
	}
}
