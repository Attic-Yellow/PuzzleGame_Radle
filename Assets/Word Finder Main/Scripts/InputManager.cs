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

    // Start is called before the first frame update
    void Start()
    {
        //연결되어 있음
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

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize()
    {
        currentWordContaainerIndex = 0;
        canAddLetter = true;

        DisableEnterButton();

        for (int i = 0; i < wordContainers.Length; i++)
            // 연결 되어있음
            wordContainers[i].Initialize();

        shouldReset = false;
    }

    private void KeyPressedCallback(char letter)
    {
        if (!canAddLetter)
            return;

        wordContainers[currentWordContaainerIndex].Add(letter);

        HapticsManager.Vibrate();

        if (wordContainers[currentWordContaainerIndex].IsComplete())
        {
            canAddLetter= false;
            EnableEnterButton();
            //CheckWord();
            //currentWordContaainerIndex++;
        }

        onLetterAdded?.Invoke();
    }

    public void CheckWord()
    {
        string wordToCheck = wordContainers[currentWordContaainerIndex].GetWord();
        string secretWord = WordManager.instance.GetSecretWord();

        wordContainers[currentWordContaainerIndex].Colorize(secretWord);
        keyboardColorizer.Colorize(secretWord, wordToCheck);

        if (secretWord == wordToCheck)
        {
            SetLevelComplete();
        }
        else
        {
            //Debug.Log("Wrong Word");
            currentWordContaainerIndex++;
            DisableEnterButton();

            if (currentWordContaainerIndex >= wordContainers.Length)
            {
                //Debug.Log("Gameover");
                DataManager.instance.ResetScore();
                GameManager.instance.SetGameState(GameState.Gameover);
            }
            else
            {
                canAddLetter = true;
                
            }

            
        }
    }

    private void SetLevelComplete()
    {
        UpdateDate();
        GameManager.instance.SetGameState(GameState.LevelComplete);
    }

    private void UpdateDate()
    {
        int scoreToAdd = 6 - currentWordContaainerIndex;

        DataManager.instance.IncreaseScore(scoreToAdd);
        DataManager.instance.AddCoins(scoreToAdd * 3);
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
