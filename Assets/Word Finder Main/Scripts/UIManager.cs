using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header(" Elements ")]
    [SerializeField] private CanvasGroup loginCG;
    [SerializeField] private CanvasGroup signupCG;
    [SerializeField] private CanvasGroup menuCG;
    [SerializeField] private CanvasGroup gameCG;
    [SerializeField] private CanvasGroup levelCompleteCG;
    [SerializeField] private CanvasGroup gameoverCG;
    [SerializeField] private CanvasGroup settingsCG;
    [SerializeField] private CanvasGroup usersettingsCG;
    [SerializeField] private CanvasGroup userRankingCG;

    [Header(" Menu Elements ")]
    [SerializeField] private TextMeshProUGUI menuCoins;
    [SerializeField] private TextMeshProUGUI menuBestScore;

    [Header(" Level Complete Elements ")]
    [SerializeField] private TextMeshProUGUI levelCompleteCoins;
    [SerializeField] private TextMeshProUGUI levelCompleteSecretWord;
    [SerializeField] private TextMeshProUGUI levelCompleteScore;
    [SerializeField] private TextMeshProUGUI levelCompleteBestScore;

    [Header(" Gameover Elements ")]
    [SerializeField] private TextMeshProUGUI gameoverCoins;
    [SerializeField] private TextMeshProUGUI gameoverSecretWord;
    [SerializeField] private TextMeshProUGUI gameoverBestScore;

    [Header(" Game Elements")]
    [SerializeField] private TextMeshProUGUI gameScore;
    [SerializeField] private TextMeshProUGUI gameCoins;

    [Header(" Login and Signup Buttons ")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button logoutButton;

    private bool isLoggedIn = false; // 로그인 상태를 나타내는 변수

    private void Awke()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {

        ShowMenu();
        HideLogin();
        HideSignup();
        HideShowRanking();
        HideGame();
        HideLevelComplete();
        HideGameover();


        GameManager.onGameStateChanged += GameStateChanegedCallback;
        GameManager.onCoinsUpdated += UpdateCoinsTexts;
    }

    private void onDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChanegedCallback;
        GameManager.onCoinsUpdated -= UpdateCoinsTexts;
    }

    private void GameStateChanegedCallback(GameState gameState)
    {
        switch (gameState)
        {

            case GameState.Menu:
                ShowMenu();
                HideSettings();
                HideShowUsersettings();
                HideLogin();
                HideSignup();
                HideGame();
                HideShowRanking();
                UpdateLoginUI(); 
                break;

            case GameState.Login:

                ShowLogin();
                HideSettings();
                HideShowUsersettings();
                HideMenu();
                HideGame();

                break;

            case GameState.Signup:

                ShowSignup();
                HideSettings();
                HideShowUsersettings();
                HideMenu();
                HideGame();

                break;


            case GameState.Game:

                ShowGame();
                HideMenu();
                HideLevelComplete();
                HideGameover();
                break;

            case GameState.LevelComplete:
                ShowLevelComplete();
                HideGame();
                break;

            case GameState.Gameover:
                ShowGameover();
                HideGame();
                break;
        }
    }

    void Update()
    {

    }

    public void UpdateCoinsTexts()
    {
        menuCoins.text = GameManager.instance.GetLocalCoins().ToString();
        gameCoins.text = menuCoins.text;
        levelCompleteCoins.text = menuCoins.text;
        gameoverCoins.text = menuCoins.text;
    }

    private void ShowLogin()
    {
        ShowCG(loginCG);
    }

    private void HideLogin()
    {
        HideCG(loginCG);
    }

    private void ShowSignup()
    {

        ShowCG(signupCG);
    }

    private void HideSignup()
    {
        HideCG(signupCG);
    }

    private void ShowMenu()
    {
        // 메뉴에 표시되는 코인과 최고 점수 텍스트 업데이트
        menuCoins.text = GameManager.instance.GetLocalCoins().ToString();
        menuBestScore.text = GameManager.instance.GetLocalBestScore().ToString();

        // 메뉴 캔버스 그룹을 활성화
        ShowCG(menuCG);
    }

    private void UpdateLoginUI()
    {
        isLoggedIn = GameManager.instance.IsLoggedIn();

        if (isLoggedIn) // 로그인 상태일 경우
        {
            // 로그인 상태에 따른 UI 업데이트를 여기에 추가하세요
            loginButton.gameObject.SetActive(false);
            signupButton.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(true);

            // 코인과 최고 점수 텍스트를 불러온 데이터로 업데이트
            menuCoins.text = GameManager.instance.GetLocalCoins().ToString();
            menuBestScore.text = GameManager.instance.GetLocalBestScore().ToString();
        }
        else // 비로그인 상태일 경우
        {
            // 비로그인 상태에 따른 UI 업데이트를 여기에 추가하세요
            loginButton.gameObject.SetActive(true);
            signupButton.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(false);
        }
    }

    private void HideMenu()
    {
        HideCG(menuCG);
    }

    private void ShowGame()
    {
        gameCoins.text = GameManager.instance.GetLocalCoins().ToString();
        gameScore.text = GameManager.instance.GetLocalScore().ToString();

        ShowCG(gameCG);
    }

    private void HideGame()
    {
        HideCG(gameCG);
    }

    private void ShowLevelComplete()
    {
        levelCompleteCoins.text = GameManager.instance.GetLocalCoins().ToString();
        levelCompleteSecretWord.text = WordManager.instance.GetSecretWord();
        levelCompleteScore.text = GameManager.instance.GetLocalScore().ToString();
        levelCompleteBestScore.text = GameManager.instance.GetLocalBestScore().ToString();

        ShowCG(levelCompleteCG);
    }

    private void HideLevelComplete()
    {
        HideCG(levelCompleteCG);
    }

    private void ShowGameover()
    {
        gameoverCoins.text = GameManager.instance.GetLocalCoins().ToString();
        gameoverSecretWord.text = WordManager.instance.GetSecretWord();
        gameoverBestScore.text = GameManager.instance.GetLocalBestScore().ToString();

        ShowCG(gameoverCG);
    }

    private void HideGameover()
    {
        HideCG(gameoverCG);
    }

    public void ShowSettings()
    {
        isLoggedIn = GameManager.instance.IsLoggedIn();

        if (isLoggedIn)
        {
            ShowCG(usersettingsCG);
            HideCG(settingsCG);
        }
        else
        {
            ShowCG(settingsCG);
            HideCG(usersettingsCG);
        }
        
    }

    public void HideSettings()
    {
        HideCG(settingsCG);
    }

    public void ShowUsersettings()
    {
        ShowCG(usersettingsCG);
    }

    public void HideShowUsersettings()
    {
        HideCG(usersettingsCG);
    }

    public void ShowRanking()
    {
        ShowCG(userRankingCG);
    }

    public void HideShowRanking()
    {
        HideCG(userRankingCG);
    }


    private void ShowCG(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;

    }

    private void HideCG(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

    }
}

