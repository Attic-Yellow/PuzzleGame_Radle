using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

// 글자를 보관하고 처리하는 클래스
public class WordContainer : MonoBehaviour
{
    private KoreanCharMaker koreanCharMaker; // 한글을 만드는 데 사용되는 KoreanCharMaker 인스턴스
    private StringBuilder currentKoreanChar; // 현재 만들고 있는 한글 글자
    private bool waitingForVowel; // 모음이 오기를 기다리는지의 여부
    private List<char> morphemes = new List<char>(); // 형태소 저장 공간
    private List<string> combinedKoreanChars = new List<string>(); // 완성된 글자 저장 공간
    private bool isVowel = false;  // 현재 입력된 문자가 모음인지 여부

    // UI 요소 그룹
    [Header(" Elements ")]
    private LetterContainer[] letterContainers;

    // 설정 변수들
    [Header(" Settings ")]
    private int currentLetterIndex;

    // 초기 설정 함수들
    private void Awake()
    {
        letterContainers = GetComponentsInChildren<LetterContainer>();
        Initialize();
        currentKoreanChar = new StringBuilder();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // 초기화 함수
    public void Initialize()
    {
        currentLetterIndex = 0;
        koreanCharMaker = new KoreanCharMaker();
        currentKoreanChar = new StringBuilder();

        for (int i = 0; i < letterContainers.Length; i++)
            letterContainers[i].Initialize();
    }

    // 문자 추가 함수
    public void Add(char letter)
    {

        int initSoundCode = KoreanCharMaker.GetInitSoundCode(letter);
        int vowelCode = KoreanCharMaker.GetVowelCode(letter);
        int finalConsonantCode = KoreanCharMaker.GetFinalConsonantCode(letter);


        if (initSoundCode != -1)
        {
            if (waitingForVowel) 
            {
                waitingForVowel = false;
            }

            if (currentKoreanChar.Length == 2) 
            {                                  
                currentKoreanChar.Append(letter);
                string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), currentKoreanChar[1].ToString(), letter.ToString());

                if (combinedKoreanChar.Length > 0)
                {
                    char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                    letterContainers[currentLetterIndex - 1].SetLetter(lastChar);

                    if (currentLetterIndex == 5 && currentKoreanChar.Length == 3)
                    {
                        combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1); 
                        combinedKoreanChars.Add(lastChar.ToString());
                    }
                    else
                    {
                        combinedKoreanChars.Add(lastChar.ToString());
                    }

                    morphemes.Clear();
                }
            }
            else if (currentKoreanChar.Length == 3) 
            {
                char convertedInitSound = KoreanCharMaker.GetInitSoundFromFinalConsonant(currentKoreanChar[2]);
                string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), currentKoreanChar[1].ToString(), currentKoreanChar[2].ToString());

                if (combinedKoreanChar.Length > 0)
                {
                    if (combinedKoreanChars.Count > 0 && combinedKoreanChars[combinedKoreanChars.Count - 1].Length == 1)
                    {
                        combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
                    }

                    if (combinedKoreanChars.Count > 0)
                    {
                        combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
                    }

                    char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                    letterContainers[currentLetterIndex - 1].SetLetter(lastChar);
                    combinedKoreanChars.Add(lastChar.ToString());

                    morphemes.Clear();
                }

                currentKoreanChar.Clear();
                currentKoreanChar.Append(letter);
                letterContainers[currentLetterIndex].SetLetter(letter);
                morphemes.Add(letter);
            }
            else if (currentKoreanChar.Length == 1 && KoreanCharMaker.GetInitSoundCode(currentKoreanChar[0]) != -1)
            {

                currentKoreanChar.Remove(0, 1);
                currentKoreanChar.Append(letter);
                letterContainers[currentLetterIndex].SetLetter(letter);
                morphemes[morphemes.Count - 1] = letter;
            }
            else
            {
                currentKoreanChar.Append(letter);
                letterContainers[currentLetterIndex].SetLetter(letter);
                morphemes.Add(letter);
            }
        }

        else if (vowelCode != -1)
        {
            waitingForVowel = true; 
            if (currentKoreanChar.Length == 1)
            {

                currentKoreanChar.Append(letter);
                string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), letter.ToString(), " ");

                if (combinedKoreanChar.Length > 0)
                {
                    char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                    letterContainers[currentLetterIndex].SetLetter(lastChar);

                    combinedKoreanChars.Add(lastChar.ToString());

                    morphemes.Clear();
                }

                currentLetterIndex++;
            }

            else if (currentKoreanChar.Length == 2 && KoreanCharMaker.GetVowelCode(currentKoreanChar[1]) != -1)
            {
                char combinedVowel = KoreanCharMaker.CombineVowels(currentKoreanChar[1], letter);

                if (combinedVowel == '\0')
                {

                    currentKoreanChar.Remove(1, 1);
                    currentKoreanChar.Append(letter);

                    if (combinedKoreanChars.Count > 0)
                    {
                        combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
                    }

                    string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), letter.ToString(), " ");
                    if (combinedKoreanChar.Length > 0)
                    {
                        char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                        letterContainers[currentLetterIndex - 1].SetLetter(lastChar);
                        combinedKoreanChars.Add(lastChar.ToString());

                        morphemes.Clear();
                    }
                    waitingForVowel = true; 

                }
                else
                {

                    char currentVowel = currentKoreanChar[1];
                    currentKoreanChar.Remove(1, 1);

                    char newcombinedVowel = KoreanCharMaker.CombineVowels(currentVowel, letter);

                    currentKoreanChar.Append(newcombinedVowel);

                    if (morphemes.Count > 0)
                    {
                        morphemes[morphemes.Count - 1] = newcombinedVowel;
                    }

                    if (combinedKoreanChars.Count > 0)
                    {
                        combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
                    }

                    string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), newcombinedVowel.ToString(), " ");

                    if (combinedKoreanChar != null && combinedKoreanChar.Length > 0)
                    {
                        char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                        letterContainers[currentLetterIndex - 1].SetLetter(lastChar);

                        combinedKoreanChars.Add(lastChar.ToString());

                        morphemes.Clear();
                    }
                    waitingForVowel = true;
                }

            }

            else if (currentKoreanChar.Length == 3)
            {
                if (combinedKoreanChars.Count > 0 && combinedKoreanChars[combinedKoreanChars.Count - 1].Length == 1)
                {
                    combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
                }

                string preCombinedKoreanChar = KoreanCharMaker.ConvertToKorean(currentKoreanChar[0].ToString(), currentKoreanChar[1].ToString(), " ");
                char backWord = preCombinedKoreanChar[preCombinedKoreanChar.Length - 1];
                letterContainers[currentLetterIndex - 1].SetLetter(backWord);

                char convertedInitSound = KoreanCharMaker.GetInitSoundFromFinalConsonant(currentKoreanChar[2]);
                string combinedKoreanChar = KoreanCharMaker.ConvertToKorean(convertedInitSound.ToString(), letter.ToString(), " ");

                if (combinedKoreanChar.Length > 0)
                {
                    char lastChar = combinedKoreanChar[combinedKoreanChar.Length - 1];
                    letterContainers[currentLetterIndex].SetLetter(lastChar);
                    combinedKoreanChars.Add(lastChar.ToString());
                    morphemes.Clear();
                }

                currentKoreanChar.Clear();
                currentKoreanChar.Append(convertedInitSound);
                currentKoreanChar.Append(letter);
                currentLetterIndex++;
            }
        }
 
    }

    // 문자 제거 함수
    public bool RemoveLetter()
    {
        if (currentLetterIndex <= 0)
            return false;

        currentLetterIndex--;
        letterContainers[currentLetterIndex].Initialize();

        if (morphemes.Count > 0)
        {
            morphemes.RemoveAt(morphemes.Count - 1);
        }

        if (currentKoreanChar.Length > 0)
        {
            currentKoreanChar.Remove(currentKoreanChar.Length - 1, 1);
        }

        if (combinedKoreanChars.Count > 0)
        {
            combinedKoreanChars.RemoveAt(combinedKoreanChars.Count - 1);
        }

        return true;
    }

    // 전체 리셋 함수
    public void Reset()
    {
        for (int i = 0; i < currentLetterIndex; i++)
        {
            letterContainers[i].Initialize();
        }

        currentLetterIndex = 0;
        currentKoreanChar.Clear();
        waitingForVowel = false;
        isVowel = false;

        morphemes.Clear();
        combinedKoreanChars.Clear();
    }

    // 현재까지 만들어진 단어와 형태소를 디버그 로그에 출력
    public void FinishWord()
    {
        Debug.Log("형태소: " + string.Join("", morphemes.ToArray()));
        Debug.Log("완성된 글자: " + string.Join("", combinedKoreanChars.ToArray()));
    }

    // 힌트 추가 함수
    public void AddAsHint(int letterIndex, char letter)
    {
        letterContainers[letterIndex].SetLetter(letter, true);
    }

    // 현재까지 완성된 단어를 반환
    public string GetWord()
    {
        return string.Join("", combinedKoreanChars.ToArray());
    }

    // 색상 변경 함수
    public void Colorize(string secretWord)
    {
        List<char> chars = new List<char>(secretWord.ToCharArray());

        for (int i = 0; i < letterContainers.Length; i++)
        {
            char letterToCheck = letterContainers[i].GetLetter();

            if (letterToCheck == secretWord[i])
            {
                letterContainers[i].SetValid();
                chars.Remove(letterToCheck);
            }
            else if (chars.Contains(letterToCheck))
            {
                letterContainers[i].SetPotential();
                chars.Remove(letterToCheck);
            }
            else
            {
                letterContainers[i].SetInvalid();
            }
        }
    }

    // 단어가 완성되었는지 여부 반환
    public bool IsComplete()
    {
        if (currentLetterIndex == letterContainers.Length - 1 && KoreanCharMaker.GetFinalConsonantCode(currentKoreanChar[currentKoreanChar.Length - 1]) == -1)
        {
            return false;
        }
        return currentLetterIndex >= letterContainers.Length;
    }

}
