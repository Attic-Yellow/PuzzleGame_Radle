using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

class KoreanCharMaker
{
    private static string[] chostrs = { "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��" };
    private static string[] jungstrs = { "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��" };
    private static string[] jongstrs = { " ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��" };
    private static string[] compoundVowels = { "��", "��", "��", "��", "��", "��", "��" };

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
        // ���� �ڵ带 �����ɴϴ�.
        int finalConsonantCode = GetFinalConsonantCode(finalConsonant);
        char initSound = '\0';

        // ���� �ڵ带 ������� �ʼ� �ڵ带 ã���ϴ�.
        switch (finalConsonantCode)
        {
            case 1: // ��
                initSound = '��';
                break;
            case 4: // ��
                initSound = '��';
                break;
            case 7: // ��
                initSound = '��';
                break;
            case 8: // ��
                initSound = '��';
                break;
            case 16: // ��
                initSound = '��';
                break;
            case 17: // ��
                initSound = '��';
                break;
            case 19: // ��
                initSound = '��';
                break;
            case 21: // ��
                initSound = '��';
                break;
            case 22: // ��
                initSound = '��';
                break;
            case 23: // ��
                initSound = '��';
                break;
            case 24: // ��
                initSound = '��';
                break;
            case 25: // ��
                initSound = '��';
                break;
            case 26: // ��
                initSound = '��';
                break;
            case 27: // ��
                initSound = '��';
                break;
            // "��" ������ ���� ó��
            case 2: // ��
            case 3: // ��
            case 5: // ��
            case 6: // ��
            case 9: // ��
            case 10: // ��
            case 11: // ��
            case 12: // ��
            case 13: // ��
            case 14: // ��
            case 15: // ��
            case 18: // ��
                initSound = finalConsonant;
                break;
            default:
                Debug.Log("�������κ��� �ʼ� ��ȯ ����: " + finalConsonant);
                break;
        }

        return initSound;
    }
    public static char CombineFinalConsonants(char finalConsonant1, char finalConsonant2)
    {
        string combined = finalConsonant1.ToString() + finalConsonant2.ToString();

        switch (combined)
        {
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
            case "����":
                return '��';
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
            case "����":
                return '��';
            case "�Ť�":
                return '��';
            case "�Ǥ�":
                return '��';
            case "�Ǥ�":
                return '��';
            case "�Ǥ�":
                return '��';
            case "�̤�":
                return '��';
            case "�̤�":
                return '��';
            case "�̤�":
                return '��';
            case "�Ѥ�":
                return '��';
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
        List<char> compoundVowels = new List<char> { '��', '��', '��', '��', '��', '��', '��', '��', '��' };
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
