using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject keyboard;
    private KeyboardKey[] keys;

    private void Awake()
    {
        keys = keyboard.GetComponentsInChildren<KeyboardKey>();

        Debug.Log("We found "+ keys.Length + " keys");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KeyboardHint()
    {
        //����� �׽�Ʈ
        //Debug.Log("Keyboard Hint Activated");
    }

    public void LetterHint()
    {
        //����� �׽�Ʈ
        //Debug.Log("Letter Hint Activated");
    }
}
