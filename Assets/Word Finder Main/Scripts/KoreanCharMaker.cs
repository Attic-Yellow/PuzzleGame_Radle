using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

class KoreanCharMaker
{
    private static string[] chostrs = { "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
    private static string[] jungstrs = { "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ" };
    private static string[] jongstrs = { " ", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
    private static string[] compoundVowels = { "ㅘ", "ㅙ", "ㅚ", "ㅝ", "ㅞ", "ㅟ", "ㅢ" };

    public static string ConvertToKorean(string c1, string c2, string c3)
    {
        int chostrs_i = Array.IndexOf(chostrs, c1);
        int jungstrs_i = Array.IndexOf(jungstrs, c2);
        int jongstrs_i = Array.IndexOf(jongstrs, c3);

        if (chostrs_i < 0 || jungstrs_i < 0 || jongstrs_i < 0)
        {
            return null;
        }

        int uniValue = (chostrs_i * 21 * 28) + (jungstrs_i * 28) + (jongstrs_i) + 0xAC00;

        return ((char)uniValue).ToString();
    }

    public static int GetInitSoundCode(char letter)
    {
        return Array.IndexOf(chostrs, letter.ToString());
    }

    public static int GetVowelCode(char letter)
    {
        return Array.IndexOf(jungstrs, letter.ToString());
    }

    public static int GetFinalConsonantCode(char letter)
    {
        return Array.IndexOf(jongstrs, letter.ToString());
    }

    public static int GetCompoundVowelCode(char letter)
    {
        return Array.IndexOf(compoundVowels, letter.ToString());
    }

    public static char GetInitSoundFromFinalConsonant(char finalConsonant)
    {
        // 종성 코드를 가져옵니다.
        int finalConsonantCode = GetFinalConsonantCode(finalConsonant);
        char initSound = '\0';

        // 종성 코드를 기반으로 초성 코드를 찾습니다.
        switch (finalConsonantCode)
        {
            case 1: // ㄱ
                initSound = 'ㄱ';
                break;
            case 4: // ㄴ
                initSound = 'ㄴ';
                break;
            case 7: // ㄷ
                initSound = 'ㄷ';
                break;
            case 8: // ㄹ
                initSound = 'ㄹ';
                break;
            case 16: // ㅁ
                initSound = 'ㅁ';
                break;
            case 17: // ㅂ
                initSound = 'ㅂ';
                break;
            case 19: // ㅅ
                initSound = 'ㅅ';
                break;
            case 21: // ㅇ
                initSound = 'ㅇ';
                break;
            case 22: // ㅈ
                initSound = 'ㅈ';
                break;
            case 23: // ㅊ
                initSound = 'ㅊ';
                break;
            case 24: // ㅋ
                initSound = 'ㅋ';
                break;
            case 25: // ㅌ
                initSound = 'ㅌ';
                break;
            case 26: // ㅍ
                initSound = 'ㅍ';
                break;
            case 27: // ㅎ
                initSound = 'ㅎ';
                break;
            // "ㅇ" 이후의 자음 처리
            case 2: // ㄲ
            case 3: // ㄳ
            case 5: // ㄵ
            case 6: // ㄶ
            case 9: // ㄺ
            case 10: // ㄻ
            case 11: // ㄼ
            case 12: // ㄽ
            case 13: // ㄾ
            case 14: // ㄿ
            case 15: // ㅀ
            case 18: // ㅄ
                initSound = finalConsonant;
                break;
            default:
                Debug.Log("종성으로부터 초성 변환 실패: " + finalConsonant);
                break;
        }

        return initSound;
    }
    public static char CombineFinalConsonants(char finalConsonant1, char finalConsonant2)
    {
        string combined = finalConsonant1.ToString() + finalConsonant2.ToString();

        switch (combined)
        {
            case "ㄱㅅ":
                return 'ㄳ';
            case "ㄴㅈ":
                return 'ㄵ';
            case "ㄴㅎ":
                return 'ㄶ';
            case "ㄹㄱ":
                return 'ㄺ';
            case "ㄹㅁ":
                return 'ㄻ';
            case "ㄹㅂ":
                return 'ㄼ';
            case "ㄹㅅ":
                return 'ㄽ';
            case "ㄹㅌ":
                return 'ㄾ';
            case "ㄹㅍ":
                return 'ㄿ';
            case "ㄹㅎ":
                return 'ㅀ';
            case "ㅂㅅ":
                return 'ㅄ';
            default:
                // If there is no valid combination, return a default value
                return '\0';
        }
    }

    public static char CombineVowels(char vowel1, char vowel2)
    {
        string combined = vowel1.ToString() + vowel2.ToString();

        switch (combined)
        {
            case "ㅑㅣ":
                return 'ㅒ';
            case "ㅕㅣ":
                return 'ㅖ';
            case "ㅗㅏ":
                return 'ㅘ';
            case "ㅗㅐ":
                return 'ㅙ';
            case "ㅗㅣ":
                return 'ㅚ';
            case "ㅜㅓ":
                return 'ㅝ';
            case "ㅜㅔ":
                return 'ㅞ';
            case "ㅜㅣ":
                return 'ㅟ';
            case "ㅡㅣ":
                return 'ㅢ';
            default:

                return '\0';
        }
    }


    public static bool IsInitialConsonant(char letter)
    {
        return Array.IndexOf(chostrs, letter.ToString()) != -1;
    }

    public static bool IsFinalConsonant(char letter)
    {
        return Array.IndexOf(jongstrs, letter.ToString()) != -1;
    }

    public static bool IsCompoundVowel(char c)
    {
        List<char> compoundVowels = new List<char> { 'ㅒ', 'ㅖ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅢ' };
        return compoundVowels.Contains(c);
    }

    public static string[] SplitToMorphemes(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 1)
        {
            return new string[0];
        }

        char ch = text[0];
        int uniValue = Convert.ToInt32(ch) - 0xAC00;
        int jong = uniValue % 28;
        int jung = ((uniValue - jong) / 28) % 21;
        int cho = ((uniValue - jong) / 28) / 21;

        if (cho < 0 || cho >= chostrs.Length || jung < 0 || jung >= jungstrs.Length || jong < 0 || jong >= jongstrs.Length)
        {
            return new string[0];
        }

        string[] morphemes = new string[3];
        morphemes[0] = chostrs[cho];
        morphemes[1] = jungstrs[jung];
        morphemes[2] = jongstrs[jong];

        if (morphemes[2] == " ")
        {
            morphemes[2] = "";
        }

        return morphemes;
    }

    public static char[] SplitToLetters(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 1)
        {
            return new char[0];
        }

        List<char> letters = new List<char>();

        foreach (char ch in text)
        {
            int uniValue = Convert.ToInt32(ch) - 0xAC00;
            int jong = uniValue % 28;
            int jung = ((uniValue - jong) / 28) % 21;
            int cho = ((uniValue - jong) / 28) / 21;

            if (cho < 0 || cho >= chostrs.Length || jung < 0 || jung >= jungstrs.Length || jong < 0 || jong >= jongstrs.Length)
            {
                continue; 
            }


            letters.Add(char.Parse(chostrs[cho]));


            letters.Add(char.Parse(jungstrs[jung]));


            if (jongstrs[jong] != " ")
            {
                letters.Add(char.Parse(jongstrs[jong]));
            }
        }

        return letters.ToArray();
    }

}
