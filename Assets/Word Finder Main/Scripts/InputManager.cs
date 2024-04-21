using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [Header(" Elements ")]
    [SerializeField] private WordContainer[] wordContainers;
    [SerializeField] private Button enterButton;
    [SerializeField] private KeyboardColorizer keyboardColorizer;

    [Header(" Settings ")]
    private int currentWordContaainerIndex;
    private bool canAddLetter = true;
    private bool shouldReset;

    [Header(" Events ")]
    public static Action onLetterAdded;
    public static Action onLetterRemoved;


    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {

        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
        GameManager.onGameStateChanged += GameStateChanegedCallback;
    }

    private void OnDestroy()
    {
        KeyboardKey.onKeyPressed -= KeyPressedCallback;
        GameManager.onGameStateChanged -= GameStateChanegedCallback;
    }

    private void GameStateChanegedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Game:
                if(shouldReset)
                    Initialize();
                break;

            case GameState.LevelComplete:
                shouldReset = true;
                break;

            case GameState.Gameover:
                shouldReset = true;
                    break;
        }
    }


    void Update()
    {

    }

    private void Initialize()
    {
        currentWordContaainerIndex = 0;
        canAddLetter = true;

        DisableEnterButton();

        for (int i = 0; i < wordContainers.Length; i++)

            wordContainers[i].Initialize();

        shouldReset = false;
    }

    //입력한 값을 wordContianer에 보냄 그리고 정답 칸이 모두 찼는지 확인
    private void KeyPressedCallback(char letter)
    {
        int VowelCode = KoreanCharMaker.GetVowelCode(letter);


        if (enterButton.interactable && (KoreanCharMaker.GetFinalConsonantCode(letter) != -1 || KoreanCharMaker.GetVowelCode(letter) != -1))
        {
            wordContainers[currentWordContaainerIndex].Add(letter);
            HapticsManager.Vibrate();
            onLetterAdded?.Invoke();
            return;
        }

        if (!canAddLetter)
            return;

        int initSoundCode = KoreanCharMaker.GetInitSoundCode(letter);
        int vowelCode = KoreanCharMaker.GetVowelCode(letter);
        int finalConsonantCode = KoreanCharMaker.GetFinalConsonantCode(letter);


        if (initSoundCode != -1 || vowelCode != -1 || finalConsonantCode != -1)
        {
            wordContainers[currentWordContaainerIndex].Add(letter);
            HapticsManager.Vibrate();

            if (wordContainers[currentWordContaainerIndex].IsComplete())
            {
                canAddLetter = false;
                EnableEnterButton();
            }
        }

        onLetterAdded?.Invoke();
    }

    public void CheckWord()
    {
        string wordToCheck = wordContainers[currentWordContaainerIndex].GetWord();
        string secretWord = WordManager.instance.GetSecretWord();


        Debug.Log("Secret Word: " + secretWord);
        Debug.Log("Word To Check: " + wordToCheck);

        wordContainers[currentWordContaainerIndex].Colorize(secretWord);
        keyboardColorizer.Colorize(secretWord, wordToCheck);

        if (secretWord == wordToCheck)
        {
            SetLevelComplete();
        }
        else
        {

            currentWordContaainerIndex++;
            DisableEnterButton();

            if (currentWordContaainerIndex >= wordContainers.Length)
            {

                GameManager.instance.ResetLocalScore();
                GameManager.instance.SetGameState(GameState.Gameover);
            }
            else
            {
                wordContainers[currentWordContaainerIndex].Initialize();
                canAddLetter = true;
            }
        }

    }

    private void SetLevelComplete()
    {
        UpdateDate();


        foreach (var wordContainer in wordContainers)
        {
            wordContainer.Reset();
        }

        GameManager.instance.SetGameState(GameState.LevelComplete);
    }


    private void UpdateDate()
    {
        int scoreToAdd = 6 - currentWordContaainerIndex;

        GameManager.instance.IncreaseLocalScore(scoreToAdd);
        GameManager.instance.AddLocalCoins(scoreToAdd * 3);
    }

    public void BackspacePressedCallback()
    {
        if (!GameManager.instance.IsGameState())
            return;

        bool removedLetter = wordContainers[currentWordContaainerIndex].RemoveLetter();

        if (removedLetter)
            DisableEnterButton();

        canAddLetter = true;

        onLetterAdded?.Invoke();
    }

    private void EnableEnterButton()
    {
        enterButton.interactable = true;
    }

    private void DisableEnterButton()
    {
        enterButton.interactable = false;
    }

    public WordContainer GetCurrentWordContainer()
    {
        return wordContainers[currentWordContaainerIndex];
    }
}
