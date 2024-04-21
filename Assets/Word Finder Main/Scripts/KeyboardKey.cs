using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

enum Validity { None, Valid, Potential, Invalid }

public class KeyboardKey : MonoBehaviour
{

    [Header(" Elements ")]
    [SerializeField] private Image renderer;
    [SerializeField] private TextMeshProUGUI letterText;

    [Header(" Settings ")]
    private Validity validity;
    private Validity morphemeValidity;

    [Header(" Events ")]
    public static Action<char> onKeyPressed;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(SendKeyPressedEvent);
        }
        else
        {
            Debug.LogWarning("Button component is missing on the KeyboardKey object: " + gameObject.name);
        }

        Initialize();
    }

    private void SendKeyPressedEvent()
    {
        onKeyPressed?.Invoke(letterText.text[0]);
    }

    public char GetLetter()
    {
        return letterText.text[0];
    }

    public void Initialize()
    {
        renderer.color = Color.white;
        validity = Validity.None;
        morphemeValidity = Validity.None;
    }

    public void SetValid()
    {
        renderer.color = Color.green;
        validity = Validity.Valid;
    }

    public void SetPotential()
    {
        if (validity == Validity.Valid)
        {
            return;
        }

        renderer.color = Color.yellow;
        validity = Validity.Potential;
    }

    public void SetInvalid()
    {
        if (validity == Validity.Valid || validity == Validity.Potential)
        {
            return;
        }

        renderer.color = Color.gray;
        validity = Validity.Invalid;
    }

    public bool IsUntouched()
    {
        return validity == Validity.None;
    }

    public void SetMorphemeValid(string morpheme)
    {
        if (morpheme.Equals(letterText.text))
        {
            renderer.color = Color.green;
            validity = Validity.Valid;
        }
    }

    public void SetMorphemePotential(string morpheme)
    {
        if (validity == Validity.Valid)
        {
            return;
        }

        if (morpheme.Equals(letterText.text))
        {
            renderer.color = Color.yellow;
            validity = Validity.Potential;
        }
    }

    public void SetMorphemeInvalid(string morpheme)
    {
        if (validity == Validity.Valid || validity == Validity.Potential)
        {
            return;
        }

        if (morpheme.Equals(letterText.text))
        {
            renderer.color = Color.gray;
            validity = Validity.Invalid;
        }
    }
}