using UnityEngine;
using System.Collections;

public class TitleSequencing : MonoBehaviour {

	public GameObject[] characterIcons;


	// Use this for initialization
	void Start () {
		StartCoroutine( "OuterLoop");
	}
	
	IEnumerator OuterLoop()
	{
		while(true)  // keep the game running forever ( i guess we use Application.Exit() to quit sometime in the future)
		{
			HideCharacterIcons();

			yield return StartCoroutine(ShowTitleUntilExit());	// show a title screen until the title screen is exited

			ShowCharacterIcons();

			string[] menuChoices = {"Wizard", "Knight", "Cleric"};
			yield return StartCoroutine( GenericSelectionMenu( "Select your character:", menuChoices));
			int selectedCharacter = genericMenuSelection;

			// hide all but the one we selected
			foreach( GameObject go in characterIcons)
				if( go != characterIcons[selectedCharacter])
					go.SetActive( false);
			yield return new WaitForSeconds(1f);	// allow user to see their selection

			HideCharacterIcons();

			string[] levelChoices = {"Level 1", "Level 2", "Level 3", "Level 4", "Level 5"};
			yield return StartCoroutine( GenericSelectionMenu( "Select a battle:", levelChoices));
			int selectedLevel = genericMenuSelection;

			// create a 'battle configuration' object with the selected character and level
			BattleConfig bc = BattleConfig.GetSingleton();
			bc.playerCharacter = selectedCharacter;
			bc.level = selectedLevel + 1;
			switch( selectedCharacter)
			{
			case 0:
			default:
				bc.PCStats = new CharacterStats(0, 0, 1, 1);	// WIZ	str def mag agi
				break;
			case 1:
				bc.PCStats = new CharacterStats(1, 1, 0, 0); // KNI
				break;
			case 2:
				bc.PCStats = new CharacterStats(0, 1, 1, 0); // CLR
				break;
			}

			Application.LoadLevel ("leveling"); // allow player to customize their character right off
			//Application.LoadLevel("battle"); // switch to battle scene, which will use battleConfig to populate the monsters and stuff
		}
	}

	void ShowCharacterIcons()
	{
		foreach( GameObject go in characterIcons)
			go.SetActive(true);
	}

	void HideCharacterIcons()
	{
		foreach( GameObject go in characterIcons)
			go.SetActive(false);
	}

	// copied from Sequencing.cs
	IEnumerator ShowTitleUntilExit()
	{
		guiText.text = "RPG Battler\nby Ed P\nPress SPACEBAR to start";
		while( !Input.GetKeyDown(KeyCode.Space) )
			yield return null;
	}

	// copied from Sequencing.cs
	int genericMenuSelection;
	IEnumerator GenericSelectionMenu( string title, string[] options)
	{
		genericMenuSelection = 0;
		int menuSize = options.Length;
		while( !Input.GetKeyDown(KeyCode.Return) ) 
		{
			// draw the menu with a selector arrow on the current option
			string menuText = title.Clone () as string;
			for(int i=0; i<menuSize; i+=1)
			{
				if(genericMenuSelection == i)
					menuText += "\n> ";
				else 
					menuText += "\n   ";
				menuText += options[i];
			}
			guiText.text = menuText;
			
			// move arrow if UP/DOWN key pressed
			if( Input.GetKeyDown(KeyCode.UpArrow) )
			{
				genericMenuSelection -= 1;
				if( genericMenuSelection < 0)
					genericMenuSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
				genericMenuSelection = (genericMenuSelection + 1) % menuSize;
			yield return null;
		}
		Debug.Log("Generic menu selection = " + genericMenuSelection);
	}
}
