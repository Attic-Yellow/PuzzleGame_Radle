using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterContainer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SpriteRenderer letterContainer;
    [SerializeField] private TextMeshPro letter;

    void Start()
    {

    }

    void Update()
    {

    }

    // 게임 입력창 초기화
    public void Initialize()
    {
        letter.text = "";
        letterContainer.color = Color.white;
    }

    // 저장된 값을 출력함
    public void SetLetter(char letter, bool isHint = false)
    {
        if (isHint)
            this.letter.color = Color.gray;
        else
            this.letter.color = Color.black;

        // 초성, 중성, 종성을 조합하여 한글을 표시
        this.letter.text = letter.ToString();
    }

    public void SetValid()
    {
        letterContainer.color = Color.green;
    }

    public void SetPotential()
    {
        letterContainer.color = Color.yellow;
    }

    public void SetInvalid()
    {
        letterContainer.color = Color.gray;
    }

    public char GetLetter()
    {
        if (letter.text.Length > 0)
        {
            return letter.text[0];
        }
        else
        {
            // 예외 처리를 위해 공백 문자를 반환하거나 적절한 에러 메시지를 출력할 수 있습니다.
            return ' ';
        }
    }
}
