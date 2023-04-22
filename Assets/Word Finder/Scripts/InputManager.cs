using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private WordContainer[] wordContainers;

    [Header(" Settings ")]
    private int currentWordContaainerIndex;
    private bool canAddLetter = true;



    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
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

            //CheckWord();
            //currentWordContaainerIndex++;
        }
    }

    public void CheckWord()
    {
        string wordToCheck = wordContainers[currentWordContaainerIndex].GetWord();
        string secretWord = WordManager.instance.GetSecretWord();

        if (secretWord == wordToCheck)
            Debug.Log("Level Complete");
        else
        {
            Debug.Log("Wrong Word");

            canAddLetter = true;
            currentWordContaainerIndex++;
        }
    }

    public void BackspacePressedCallback()
    {
        wordContainers[currentWordContaainerIndex].RemoveLetter();
    }
}
