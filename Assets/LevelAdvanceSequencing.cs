using UnityEngine;
using System.Collections;

public class LevelAdvanceSequencing : MonoBehaviour {

	BattleConfig bc;
	public GameObject[] characterIcons;
	private GameObject player;
	SfxManager sfx;
	public AudioClip menuSelect;
	public AudioClip menuAccept;

	// Use this for initialization
	void Start () {
		sfx = SfxManager.GetSingleton();
		bc = BattleConfig.GetSingleton();
		player = characterIcons[bc.playerCharacter];
		player.SetActive( true); // show the character we're using
		StartCoroutine( "OuterLoop");
	}
	
	IEnumerator OuterLoop()
	{
		int statPointsRemaining = 4;	// assumes 4 points per level

		while(true)  // keep the game running forever ( i guess we use Application.Exit() to quit sometime in the future)
		{
			if( statPointsRemaining == 0)
				Application.LoadLevel( "battle");

			string[] menuChoices = {
				"STR " + bc.PCStats.STR + " + 1 = " + (bc.PCStats.STR+1),
				"DEF " + bc.PCStats.DEF + " + 1 = " + (bc.PCStats.DEF+1),
				"MAG " + bc.PCStats.MAG + " + 1 = " + (bc.PCStats.MAG+1),
				"AGI " + bc.PCStats.AGI + " + 1 = " + (bc.PCStats.AGI+1) };
			yield return StartCoroutine( GenericSelectionMenu( "What stat do you want to\nlevel? You have " + statPointsRemaining + " stat\npoints remaning.", menuChoices));
			int selectedStat = genericMenuSelection;

			switch( selectedStat)
			{
			case 0:
				bc.PCStats.STR += 1;
				break;
			case 1:
				bc.PCStats.DEF += 1;
				break;
			case 2:
				bc.PCStats.MAG += 1;
				break;
			case 3:
				bc.PCStats.AGI += 1;
				break;
			}

			statPointsRemaining -= 1;

			// we need to have a Character with an assigned gfx to be able to call Character.AttactSparklies( Color.yellow) or something
			yield return new WaitForSeconds( 0.5f);  // TODO play levelling up animation
		}
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
				sfx.Play( menuSelect);
				genericMenuSelection -= 1;
				if( genericMenuSelection < 0)
					genericMenuSelection = menuSize - 1;
			}
			if( Input.GetKeyDown(KeyCode.DownArrow) )
			{
				sfx.Play( menuSelect);
				genericMenuSelection = (genericMenuSelection + 1) % menuSize;
			}
			yield return null;
		}
		sfx.Play( menuAccept);
		Debug.Log("Generic menu selection = " + genericMenuSelection);
	}
}
