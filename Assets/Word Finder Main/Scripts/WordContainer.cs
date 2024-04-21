using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

// ���ڸ� �����ϰ� ó���ϴ� Ŭ����
public class WordContainer : MonoBehaviour
{
    private KoreanCharMaker koreanCharMaker; // �ѱ��� ����� �� ���Ǵ� KoreanCharMaker �ν��Ͻ�
    private StringBuilder currentKoreanChar; // ���� ����� �ִ� �ѱ� ����
    private bool waitingForVowel; // ������ ���⸦ ��ٸ������� ����
    private List<char> morphemes = new List<char>(); // ���¼� ���� ����
    private List<string> combinedKoreanChars = new List<string>(); // �ϼ��� ���� ���� ����
    private bool isVowel = false;  // ���� �Էµ� ���ڰ� �������� ����

    // UI ��� �׷�
    [Header(" Elements ")]
    private LetterContainer[] letterContainers;

    // ���� ������
    [Header(" Settings ")]
    private int currentLetterIndex;

    // �ʱ� ���� �Լ���
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

    // �ʱ�ȭ �Լ�
    public void Initialize()
    {
        currentLetterIndex = 0;
        koreanCharMaker = new KoreanCharMaker();
        currentKoreanChar = new StringBuilder();

        for (int i = 0; i < letterContainers.Length; i++)
            letterContainers[i].Initialize();
    }

    // ���� �߰� �Լ�
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

    // ���� ���� �Լ�
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

    // ��ü ���� �Լ�
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

    // ������� ������� �ܾ�� ���¼Ҹ� ����� �α׿� ���
    public void FinishWord()
    {
        Debug.Log("���¼�: " + string.Join("", morphemes.ToArray()));
        Debug.Log("�ϼ��� ����: " + string.Join("", combinedKoreanChars.ToArray()));
    }

    // ��Ʈ �߰� �Լ�
    public void AddAsHint(int letterIndex, char letter)
    {
        letterContainers[letterIndex].SetLetter(letter, true);
    }

    // ������� �ϼ��� �ܾ ��ȯ
    public string GetWord()
    {
        return string.Join("", combinedKoreanChars.ToArray());
    }

    // ���� ���� �Լ�
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

    // �ܾ �ϼ��Ǿ����� ���� ��ȯ
    public bool IsComplete()
    {
        if (currentLetterIndex == letterContainers.Length - 1 && KoreanCharMaker.GetFinalConsonantCode(currentKoreanChar[currentKoreanChar.Length - 1]) == -1)
        {
            return false;
        }
        return currentLetterIndex >= letterContainers.Length;
    }

}
