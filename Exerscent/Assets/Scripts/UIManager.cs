﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[System.Serializable]
//UI state enumerator
public enum UIState {
	waitingForArduino,
	loginMenu, 
	enterLogin, 
	welcome, 
	openMenu,
	closeMenu,
	waitingForScent, 
	waitingForAttempt, 
	endGame
	};

public enum menuState {
	open,
	close,
	about,
	exit,
	admin,
	quit
};

public enum languageSettings {
	english,
	swedish
}

public class UIManager : MonoBehaviour {
	public languageSettings language;
	public GameObject canvas;
	public Vector2 screenScaled;
	public float transitionSpeed = 0.35f;
	public float menuSpeed = .2f;
	//References to UI elements
	public GameObject title;
	public GameObject menuButton;
	public GameObject mainMenu;
	public GameObject realMainMenu;
	public GameObject menuBackground;
	public GameObject about;
	public GameObject exitSession;
	public GameObject admin;
	public GameObject quit;
	public GameObject aboutWindow;
	public GameObject exitWindow;
	public GameObject adminWindow;
	public GameObject quitWindow;
	public GameObject restartWindow;
	public GameObject scentsParent;
	public GameObject welcomeScreen; 
	public GameObject sixOptions;
	public GameObject tenOptions;
	public GameObject playAgainButton;
	public TextMeshProUGUI infoText;
	public GameObject progressBar;
	public bool windowOpen = false;
	public Image background;
	public TextMeshProUGUI scoreNumber;
	public TextMeshProUGUI WelcomeText;


	public GameObject endScreen;
	//Game manager reference
	public gameSystemLogic manager;
	//Current state of the UI
	public UIState currentState;
	public menuState currentMenuState;
	public bool menuOpen = false;
	bool aboutOpen = false;
	bool adminOpen = false;
	bool quitOpen = false;
	bool restartOpen = false;
	bool exitOpen = false;
	bool sizeSelected = true;
	
	// Use this for initialization
	void Start () {
		manager = GameObject.FindObjectOfType<gameSystemLogic>();
		updateUIState(UIState.waitingForArduino);
		DOTween.defaultEaseType = Ease.OutBack;
        //Set initial menu object positions
		aboutWindow.transform.localPosition = new Vector3(200, 700, 0);
		quitWindow.transform.localPosition = new Vector3(200, 700, 0);
		adminWindow.transform.localPosition = new Vector3(200, 700, 0);
		restartWindow.transform.localPosition = new Vector3(200, 700, 0);
		exitWindow.transform.localPosition = new Vector3(200, 700, 0);
		screenScaled = new Vector2(
			canvas.GetComponent<RectTransform>().rect.width / Screen.width,
			canvas.GetComponent<RectTransform>().rect.height / Screen.height
		);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Switch UI state and animate elements accordingly
	public void updateUIState(UIState newState) {
		currentState = newState;
		Debug.Log(currentState.ToString());
		switch (newState) {
			case UIState.waitingForArduino:
				Sequence titleSequence = DOTween.Sequence();
				titleSequence.Append(title.GetComponentInChildren<Image>().DOFillAmount(1, 10f).SetEase(Ease.InOutSine));
				break;
                //Login menu currently not used
			case UIState.loginMenu:
				break;
			case UIState.enterLogin:
				StartCoroutine(enterWait());
				break;
                //Show welcome screen
			case UIState.welcome:
				welcomeScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				title.GetComponentInChildren<Image>().DOFillAmount(0, .1f).SetEase(Ease.InSine);
				StartCoroutine(switchInfoText("Pick a scent sample and smell it, then place it on the reader platform.", true));
				enterMain();
				break;
                //Show prompt asking player to put a new scent on the reader
			case UIState.waitingForScent:
				welcomeScreen.transform.DOLocalMove(new Vector3(1500,0, 0), transitionSpeed);
				StartCoroutine(switchInfoText("Pick a scent sample and smell it, then place it on the reader platform.", true));
				progressBar.transform.DOLocalMove(new Vector3(0, 300, 0), transitionSpeed);
				break;
			case UIState.waitingForAttempt:
				welcomeScreen.transform.DOLocalMove(new Vector3(0,-Screen.height * 2, 0), transitionSpeed);
				StartCoroutine(switchInfoText("", false));
				progressBar.transform.DOLocalMove(new Vector3(0, 300, 0), transitionSpeed);
				break;
			case UIState.endGame:
				Debug.Log(manager.allResults.Count);
				Invoke("endGameScript", 4.5f);
				//            *********NOW INSIDE A IEnumerator function instead *************
				// float totalScore = manager.totalScore;
				// float gameLength = manager.gameLength;
				// float procent = (totalScore / gameLength) *100;
				// endScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				// TextMeshProUGUI endScore = GameObject.Find("EndText").GetComponent<TextMeshProUGUI>();
				// playAgainButton.transform.DOLocalMove(new Vector3(0, -290, 0), transitionSpeed);
				// if(procent < 1){
				// 	endScore.text = "You've completed the session. You should see a doctor. Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
				// } else if(procent > 1 && procent < 33){
				// 	endScore.text = "You've completed the session. Just getting started? Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
				// } else if(procent > 33 && procent < 66){
				// 	endScore.text = "You've completed the session. It seems like you could use a bit more practice. Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
				// } else if (procent > 66 && procent < 99) {
				// 	endScore.text = "You've completed the session. A few more days and you'll get them all! Your sore was "+ manager.totalScore + " out of " + manager.gameLength + ".";
				// } else if (procent > 99){
				// 	endScore.text = "You've completed the session. You're an expert! Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
				// }
				// Debug.Log("The player scored: " + procent + "%.");
				// endScore.text = "You’ve completed the session. Well done! " + manager.totalScore + " out of " + manager.gameLength + " scents were correctly identified in this session.";
				break;
			case UIState.openMenu:
				Sequence openSequence = DOTween.Sequence();
				break;
			case UIState.closeMenu:
				Sequence closeSequence = DOTween.Sequence();
				closeSequence.Insert(.3f, menuBackground.transform.DOLocalMove(new Vector3(250, 0, 0), .5f).SetEase(Ease.InBack));
				break;
			default:
				break;
		}
	}

	public void endGameScript(){
		float totalScore = manager.totalScore;
		float gameLength = manager.gameLength;
		float procent = (totalScore / gameLength) *100;
		endScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
		TextMeshProUGUI endScore = GameObject.Find("EndText").GetComponent<TextMeshProUGUI>();
		playAgainButton.transform.DOLocalMove(new Vector3(0, -290, 0), transitionSpeed);
		if(procent < 1){
			endScore.text = "You've completed the session. You should see a doctor. Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
		} else if(procent > 1 && procent < 33){
			endScore.text = "You've completed the session. Just getting started? Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
		} else if(procent > 33 && procent < 66){
			endScore.text = "You've completed the session. It seems like you could use a bit more practice. Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		} else if (procent > 66 && procent < 99) {
			endScore.text = "You've completed the session. A few more days and you'll get them all! Your sore was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		} else if (procent > 99){
			endScore.text = "You've completed the session. You're an expert! Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		}
		Debug.Log("The player scored: " + procent + "%.");
	}

    //Only used for opening and closing menu, can probably safely delete the other cases
	public void updateMenuState(GameObject caller) {
		Debug.Log(caller.name);
		switch(caller.gameObject.name) {
			case "MenuButton":
				if(!menuOpen) {
					menuOpen = true;
					Debug.Log("opening");
					Sequence openSequence = DOTween.Sequence();
					openSequence.Append(mainMenu.transform.DOLocalMove(new Vector3(-570, 145, 0), menuSpeed)).SetEase(Ease.InOutSine);
					openSequence.Insert(0, background.DOFade(1, menuSpeed));
					openSequence.Insert(0, menuButton.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f));
					menuButton.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
					background.raycastTarget = true;
				}
				else {
					menuOpen = false;
					Debug.Log("closing");
					hideAll();
					Sequence closeSequence = DOTween.Sequence();
					closeSequence.Append(mainMenu.transform.DOLocalMove(new Vector3(0-1400, 145, 0), menuSpeed)).SetEase(Ease.InSine);
					closeSequence.Insert(0, background.DOFade(0, menuSpeed));
					closeSequence.Insert(0, menuButton.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f));
					menuButton.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
					background.raycastTarget = false;
				}
				break;
			case "About":
				showAbout();
				break;
			case "ExitSession":
				break;
			case "Admin":
				break;
			case "Quit":
				Application.Quit();
				break;
			default:
				break;
		}
	}

	public IEnumerator enterWait() {
		Sequence enterSequence = DOTween.Sequence();
		enterSequence.Append(title.GetComponentInChildren<Image>().DOFillAmount(1, 1f).SetEase(Ease.InSine));
		enterSequence.Append(menuButton.transform.DOLocalMove(new Vector3(-513, 324, 0), menuSpeed)).SetEase(Ease.InOutSine);
		enterSequence.Insert(1, title.transform.DOMove((new Vector3(GameObject.Find("TitleStop").transform.position.x, GameObject.Find("TitleStop").transform.position.y, GameObject.Find("TitleStop").transform.position.z)), 0.5f).SetEase(Ease.InOutBack));
		enterSequence.Insert(1, title.transform.DOScale(new Vector3(.6f, .6f, .6f), 0.5f).SetEase(Ease.OutBack));
		yield return enterSequence.WaitForCompletion();
		StartCoroutine(switchInfoText("Place your tag on the reader to log in", true));
	}

	public void showAbout() {
		if(!aboutOpen) {
			hideAll();
			aboutOpen = true;
			aboutWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence aboutSequence = DOTween.Sequence();
			aboutSequence.Append(aboutWindow.transform.DOLocalMove(new Vector3(220, -50, 0), menuSpeed)).SetEase(Ease.InOutSine);
			about.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			about.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		}
	}

	public void showAdmin() {
		if(!adminOpen) {
			hideAll();
			adminOpen = true;
			adminWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence adminSequence = DOTween.Sequence();
			adminSequence.Append(adminWindow.transform.DOLocalMove(new Vector3(250,-100, 0), menuSpeed)).SetEase(Ease.InOutSine);
			admin.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			admin.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		}		
	}

	public void hideAbout() {
		Sequence closeAbout = DOTween.Sequence();
		closeAbout.Append(aboutWindow.transform.DOLocalMove(new Vector3(220, -1000, 0), menuSpeed)).SetEase(Ease.InOutSine);
		about.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
		about.GetComponent<TextMeshProUGUI>().DOColor(new Color32(223, 139, 25, 255), .3f);
	}

	public void showQuit() {
		if(!quitOpen) {
			hideAll();
			quitOpen = true;
			quitWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence quitSequence = DOTween.Sequence();
			quitSequence.Append(quitWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideQuit() {
		Sequence closeQuit = DOTween.Sequence();
		closeQuit.Append(quitWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		quitOpen = false;
	}

	public void showRestart() {
		if(!restartOpen) {
			hideAll();
			restartOpen = true;
			restartWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence restartSequence = DOTween.Sequence();
			restartSequence.Append(restartWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideRestart() {
		Sequence closeRestart = DOTween.Sequence();
		closeRestart.Append(restartWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		restartOpen = false;
	}

	public void showExit() {
		if(!exitOpen) {
			hideAll();
			exitOpen = true;
			exitWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence exitSequence = DOTween.Sequence();
			exitSequence.Append(exitWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideExit() {
		Sequence closeExit = DOTween.Sequence();
		closeExit.Append(exitWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		exitOpen = false;
	}

	public void hideAdmin() {
		Sequence closeAdmin = DOTween.Sequence();
		admin.GetComponent<TextMeshProUGUI>().DOColor(new Color32(224, 175, 29, 255), .3f);
		admin.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
		closeAdmin.Append(adminWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
	}

	public void hideAll() {
		if(aboutOpen) {
			hideAbout();
			aboutOpen = false;
		}
		if(quitOpen) {
			hideQuit();
			quitOpen = false;
		}
		if(restartOpen){
			hideRestart();
			restartOpen = false;
		}
		if(adminOpen) {
			hideAdmin();
			adminOpen = false;
		}
		
	}

	//public void toggleMenu
	public void loginPressed() {
		updateUIState(UIState.enterLogin);
	}

	//Quit
	public void quitPressed() {
		manager.quitGame();
	}

	//Scale and change info text
	public IEnumerator switchInfoText(string newText, bool reappear) {
		Tween shrinkText = infoText.gameObject.transform.DOLocalMove(new Vector3(1000, 0, 0), .3f).SetEase(Ease.InOutBack);
		yield return shrinkText.WaitForCompletion();
		infoText.gameObject.transform.localPosition = new Vector3(-1000, 0, 0);
		infoText.text = newText;
		if(reappear) infoText.gameObject.transform.DOLocalMove(new Vector3(0, 0, 0), .3f).SetEase(Ease.OutBack);
	}

	//Set welcome text and show old data
	public void enterMain() {
		WelcomeText.text = "Hello, " + manager.playerName.ToUpper() + "!";
	}

	public void startPressed() {
		StartCoroutine(startAnim());
	}

	public IEnumerator startAnim() {
		GameObject startButton = GameObject.Find("Start");
		Tween buttonAnim = startButton.transform.DOPunchPosition(new Vector3(15, 15, 15), .7f, 10, 10);
		yield return buttonAnim.WaitForCompletion();
		updateUIState(UIState.waitingForScent);
	}

    //Highlight card on hover
	public IEnumerator focusCard(GameObject card, bool success) {
		Sequence cardSequence = DOTween.Sequence();
		cardSequence.Append(card.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), .5f));
		cardSequence.Insert(0, card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f));
		yield return null;
	}

	//Animate progress bar
	public void animateBar(bool correct, GameObject scentObject) {
		Image circleBar = progressBar.GetComponentInChildren<Image>();
		if(correct) {
			title.GetComponentInChildren<Image>().DOFillAmount((float)manager.totalAttempts / (manager.gameLength), 1f).SetEase(Ease.OutSine);
		}
		else {
			title.GetComponentInChildren<Image>().DOFillAmount((float)manager.totalAttempts / (manager.gameLength), 1f).SetEase(Ease.OutBounce);
		}
	}

	//Animate text on over
	public void onPointerEnter (GameObject caller) {	
		// caller.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		caller.transform.DOScale(new Vector3(1.13f, 1.13f, 1.13f), .17f).SetEase(Ease.InOutSine);
	}

	//Animate text on end hover
	public void onPointerLeave (GameObject caller) {
			// caller.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
			caller.transform.DOScale(new Vector3(1, 1, 1), .20f).SetEase(Ease.InOutSine);
	}
	//
	public void updateGameSize(GameObject caller) {
		if (caller == sixOptions){
			Debug.Log("Six options clicked");
			manager.sixOptions();
			sixOptions.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
			sixOptions.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			tenOptions.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
			tenOptions.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);
		} else if (caller == tenOptions){
			Debug.Log("Ten options clicked");
			tenOptions.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
			tenOptions.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			sixOptions.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
			sixOptions.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);
			manager.tenOptions();
		}
	}
	public void restartSession(GameObject caller){

		//Hides the "Are you sure" question.
		hideRestart();
		
		//Clears all the current data and cards
		manager.restart();

		//Closes the menu
		updateMenuState(menuButton);

		//Resets the Exerscent logo
		title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);


		//Returns the user to the waiting for scent screen
		updateUIState(UIState.waitingForScent);
	}

		public void exitSessionScript(){

		//Hides the "Are you sure" question.
		hideExit();
		
		//Clears all the current data and cards
		manager.restart();

		//Closes the menu
		updateMenuState(menuButton);

		//Resets the Exerscent logo
		title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);


		//Returns the user to the waiting for scent screen
		updateUIState(UIState.welcome);
	}

	public void playAgain(GameObject caller){
		
		//Clears all the current data
		manager.playAgain();
		
		//Resets the Exerscent logo
		title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);

		//Returns the user to the waiting for scent screen
		updateUIState(UIState.waitingForScent);

		//Moves the "Play Again?" option out of the screen
		caller.transform.DOLocalMove(new Vector3(-1427, -420, 0), transitionSpeed);

		//Removes the EndScreen text
		endScreen.transform.DOLocalMove(new Vector3(-1066, 0, 0), transitionSpeed);

	}
}