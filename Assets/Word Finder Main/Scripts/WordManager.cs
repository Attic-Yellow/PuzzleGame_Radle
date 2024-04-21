using System.Collections;
using System.Collections.Generic;
using System.Text; 
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header(" Elements ")]
    [SerializeField] private string secretWord;
    [SerializeField] private TextAsset wordsText;
    private string[] words;

    [Header(" Settings ")]
    private bool shouldReset;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // UTF8로 인코딩된 한글 단어 목록을 제대로 처리하도록 수정
        words = Encoding.UTF8.GetString(wordsText.bytes).Split('\n');
    }

    void Start()
    {
        SetNewSecretWord();
        GameManager.onGameStateChanged += GameStateChanegedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChanegedCallback;
    }

    private void GameStateChanegedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                break;

            case GameState.Game:
                if (shouldReset)
                    SetNewSecretWord();
                break;

            case GameState.LevelComplete:
            case GameState.Gameover:
                shouldReset = true;
                break;
        }
    }

    public string GetSecretWord()
    {
        return secretWord;
    }

    private void SetNewSecretWord()
    {
        secretWord = words[Random.Range(0, words.Length)].Trim();
        shouldReset = false;
    }
}
