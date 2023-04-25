using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private WordContainer[] wordContainers;
    [SerializeField] private Button enterButton;
    [SerializeField] private KeyboardColorizer keyboardColorizer;

    [Header(" Settings ")]
    private int currentWordContaainerIndex;
    private bool canAddLetter = true;



    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
    }

    private void OnDestroy()
    {
        KeyboardKey.onKeyPressed -= KeyPressedCallback;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize()
    {
        for (int i = 0; i < wordContainers.Length; i++)
            wordContainers[i].Initialize();
    }

    private void KeyPressedCallback(char letter)
    {
        if (!canAddLetter)
            return;


        wordContainers[currentWordContaainerIndex].Add(letter);

        if (wordContainers[currentWordContaainerIndex].IsComplete())
        {
            canAddLetter= false;
            EnableEnterButton();
            //CheckWord();
            //currentWordContaainerIndex++;
        }
    }

    public void CheckWord()
    {
        string wordToCheck = wordContainers[currentWordContaainerIndex].GetWord();
        string secretWord = WordManager.instance.GetSecretWord();

        wordContainers[currentWordContaainerIndex].Colorize(secretWord);
        keyboardColorizer.Colorize(secretWord, wordToCheck);

        if (secretWord == wordToCheck)
            SetLevelComplete();
        else
        {
            Debug.Log("Wrong Word");

            canAddLetter = true;
            DisableEnterButton();
            currentWordContaainerIndex++;
        }
    }

    private void SetLevelComplete()
    {
        GameManager.instance.SetGameState(GameState.LevelComplete);
    }

    public void BackspacePressedCallback()
    {
        bool removedLetter = wordContainers[currentWordContaainerIndex].RemoveLetter();

        if (removedLetter)
            DisableEnterButton();

        canAddLetter = true;
    }

    private void EnableEnterButton()
    {
        enterButton.interactable = true;
    }

    private void DisableEnterButton()
    {
        enterButton.interactable = false;
    }
}
