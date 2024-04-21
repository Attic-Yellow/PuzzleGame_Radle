using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KeyboardColorizer : MonoBehaviour
{
    [Header(" Elements ")]
    private KeyboardKey[] keys;

    [Header(" Settings ")]
    private bool shouldReset;

    private Dictionary<char, KeyboardKey> keyDictionary;

    private void Awake()
    {
        keys = GetComponentsInChildren<KeyboardKey>();

        // 키보드의 키와 형태소를 매핑하는 딕셔너리 생성
        keyDictionary = new Dictionary<char, KeyboardKey>();
        foreach (KeyboardKey key in keys)
        {
            char letter = key.GetLetter();
            if (!keyDictionary.ContainsKey(letter))
            {
                keyDictionary.Add(letter, key);
            }
        }
    }

    void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Game:
                if (shouldReset)
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

    private void Initialize()
    {
        foreach (KeyboardKey key in keys)
        {
            key.Initialize();
        }

        shouldReset = false;
    }

    void Update()
    {

    }

    public void Colorize(string secretWord, string wordToCheck)
    {

        char[] secretLetters = KoreanCharMaker.SplitToLetters(secretWord);
        char[] wordToCheckLetters = KoreanCharMaker.SplitToLetters(wordToCheck);


        Dictionary<char, int> secretCounts = new Dictionary<char, int>();
        foreach (char letter in secretLetters)
        {
            if (secretCounts.ContainsKey(letter))
                secretCounts[letter]++;
            else
                secretCounts.Add(letter, 1);
        }


        for (int i = 0; i < wordToCheckLetters.Length; i++)
        {
            char letter = wordToCheckLetters[i];

            if (keyDictionary.ContainsKey(letter))
            {
                KeyboardKey key = keyDictionary[letter];

                if (i < secretLetters.Length && secretLetters[i] == letter)
                {

                    key.SetValid();
                }
                else if (secretCounts.ContainsKey(letter))
                {

                    key.SetPotential();
                }
                else
                {

                    key.SetInvalid();
                }
            }
        }
    }

}
