using UnityEngine;
using System.Collections;

public class TitleSequencing : MonoBehaviour {

	public GameObject[] characterIcons;
	public AudioClip backgroundMusic;
	public AudioClip menuSelect;
	public AudioClip menuAccept;
	SfxManager sfx;

	// Use this for initialization
	void Start () {
		sfx = SfxManager.GetSingleton();
		StartCoroutine( "OuterLoop");
	}
	
	IEnumerator OuterLoop()
	{
		while(true)  // keep the game running forever ( i guess we use Application.Exit() to quit sometime in the future)
		{
			HideCharacterIcons();

			BackgroundMusic bm = BackgroundMusic.GetSingleton();
			bm.Play( backgroundMusic);

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

			string[] levelChoices = {"Easy", "Normal", "Hard"};
			yield return StartCoroutine( GenericSelectionMenu( "Select a difficulty:", levelChoices));
			int selectedLevel = genericMenuSelection;

			// create a 'battle configuration' object with the selected character and level
			BattleConfig bc = BattleConfig.GetSingleton();
			bc.playerCharacter = selectedCharacter;
			bc.level = (selectedLevel * 2) + 1;
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
		{
			go.SetActive(true);
		}
		characterIcons[0].GetComponent<SpriteAnimator>().Play("cast");	// wizard
		characterIcons[1].GetComponent<SpriteAnimator>().Play("idle");	// knight
		characterIcons[2].GetComponent<SpriteAnimator>().Play("defend");	// cleric
	}

	void HideCharacterIcons()
	{
		foreach( GameObject go in characterIcons)
			go.SetActive(false);
	}

	// copied from Sequencing.cs
	IEnumerator ShowTitleUntilExit()
	{
		string credits = "\n\n\n\n\n\n\n" +
						 "Programming: Ed Paradis\n" +
						 "        edparadis.com\n" +
						 "        github.com/eparadis\n" +
					     "\n" +
						 "Music: 'Subsidized'\n" +
						 "       by Yubatake\n" +
						 "       CC-By-3.0\n" +
						 "       opengameart.org\n" +
						 "Sfx: Ed Paradis\n" +
						 "     with bfxr.net\n" +
						 "Programmer Art: Ed P.";
		guiText.text = "RPG Battler\n\n** Press any key to start! **\n" + credits;;
		while( !Input.anyKeyDown )
			yield return null;
		sfx.Play( menuAccept);
		yield return new WaitForEndOfFrame();
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
				sfx.Play(menuSelect);
				genericMenuSelection -= 1;
				if( genericMenuSelection < 0)
					genericMenuSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
			{
				sfx.Play(menuSelect);
				genericMenuSelection = (genericMenuSelection + 1) % menuSize;
			}
			yield return null;
		}
		sfx.Play(menuAccept);
		//Debug.Log("Generic menu selection = " + genericMenuSelection);
	}
}
