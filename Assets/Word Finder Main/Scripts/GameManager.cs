using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public enum GameState { Login, Signup, Menu, Game, LevelComplete, Gameover, Idle }

public class GameManager : MonoBehaviour
{

    public void LoginClick()
    {
        StartCoroutine(LoginWithDelay("login", loginId.text, loginPassword.text, "", 0.5f));
    }

    public void SignupClick()
    {
        StartCoroutine(SignupWithDelay("register", userId.text, userPassword.text, userName.text, 0.5f));
    }

    public void LogoutClick() => StartCoroutine(AccountCo("logout", loginId.text, loginPassword.text, ""));

    IEnumerator LoginWithDelay(string command, string id, string password, string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(AccountCo(command, id, password, name));
    }

    IEnumerator SignupWithDelay(string command, string id, string password, string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(AccountCo(command, id, password, name));
    }


    public static GameManager instance;

    [SerializeField] string url; // ���� URL


    [Header(" Login Elements")]
    public TMP_InputField loginId; // �α��� ID �ʵ�
    public TMP_InputField loginPassword; // �α��� ��й�ȣ �ʵ�

    [Header(" Signup Elements")]
    public TMP_InputField userId; // ȸ������ ID �ʵ�
    public TMP_InputField userPassword; // ȸ������ ��й�ȣ �ʵ�
    public TMP_InputField userName; // ȸ������ ����� �̸� �ʵ�

    [Header(" Settings ")]
    private GameState gameState; // ������ ���� ����

    [Header(" Data ")]
    private int localcoins; // ������� ���� ����
    private int localscore; // ������� ���� ����
    private int localbestScore; // ������� �ְ� ����

    [Header(" Events ")]
    public static Action<GameState> onGameStateChanged; // ���� ���� ���� �� ȣ��Ǵ� �̺�Ʈ
    public static Action onCoinsUpdated; // ���� ������Ʈ �� ȣ��Ǵ� �̺�Ʈ


    public bool isLoggedIn = false; // ������� �α��� ����
    
    // �ʱ� ����, �̱��� ���� ����
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadLocalData(); // ���� ������ �ε�
    }

    // �α��� ���� �޽���
    private string loginSuccessMessage = "Login Success";
    private bool loginSuccess = false;

    // ȸ������ ���� �޽���
    private string signupSuccessMessage = "Signup Success";
    private bool signupSuccess = false;

    // ���� ���¸� �����ϴ� �Լ�
    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
        onGameStateChanged?.Invoke(gameState);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // �α׾ƿ� ��ư �ݹ� �Լ�
    public void LogoutBuutinCallback()
    {
        loginSuccess = false;
        isLoggedIn = false;
        SetGameState(GameState.Menu);
    }

    // �α��� ��ư �ݹ� �Լ�
    public void LogingoBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Login, 0.5f));
    }

    // ȸ������ ��ư �ݹ� �Լ�
    public void SignupBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Signup, 0.5f));
    }

    // �α��� ȭ�鿡�� �ڷΰ��� ��ư �ݹ� �Լ�
    public void BackLoginBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Menu, 0.5f));
    }

    // ���� ��ư �ݹ� �Լ�
    public void NextBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Game, 0.5f));
    }

    // ���� ���� ��ư �ݹ� �Լ�
    public void PlayBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Game, 0.5f));
    }

    // �ڷ� ���� ��ư �ݹ� �Լ�
    public void BackBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Menu, 0.5f));
    }


    // ������ ���·� ��ȯ�ϴ� �ڷ�ƾ
    IEnumerator SwitchToStateWithDelay(GameState targetState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetGameState(targetState);
    }

    public bool IsGameState()
    {
        return gameState == GameState.Game;
    }

    // ������ ���� ������ ������ �ڷ�ƾ
    IEnumerator AccountCo(string command, string id, string password, string name)
    {
        WWWForm form = new WWWForm();
        form.AddField("command", command);
        form.AddField("id", id);
        form.AddField("password", password);
        if (command == "register")
        {
            form.AddField("info", name);
        }



        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();
        print(www.downloadHandler.text);

        if (command == "login" && www.downloadHandler.text.Contains(loginSuccessMessage))
        {
            isLoggedIn = true;
            SetGameState(GameState.Menu);
            
            StartCoroutine(CheckServerData(id, password));


        }


        else if (command == "register" && www.downloadHandler.text.Contains(signupSuccessMessage))
        {
            signupSuccess = true;
            SetGameState(GameState.Menu);


            SaveLocalData();
            isLoggedIn = true;
            StartCoroutine(AccountCo("login", id, password, ""));
        }


        else if (command == "logout" && www.downloadHandler.text.Contains("Logout Success"))
        {
            isLoggedIn = false;
            SetGameState(GameState.Menu);
        }
    }

    IEnumerator CheckServerData(string id, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "check_data");
        form.AddField("id", id);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            bool hasData = Convert.ToBoolean(www.downloadHandler.text);

            if (hasData)
            {

                StartCoroutine(LoadDataFromServer(id, password));
            }
            else
            {

                int localBestScore = GetLocalBestScore();
                int localScore = GetLocalScore();
                int localCoins = GetLocalCoins();
                StartCoroutine(SaveDataToServer(id, password, localScore, localCoins, localBestScore));
            }
        }
        else
        {
            Debug.LogError("Failed to check server data: " + www.error);
        }
    }

    IEnumerator SaveDataToServer(string id, string password, int score, int coins, int best_score)
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "save_data");
        form.AddField("id", id);
        form.AddField("password", password);
        form.AddField("score", score.ToString());
        form.AddField("coins", coins.ToString());
        form.AddField("best_score", best_score.ToString());


        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data saved to server successfully");
        }
        else
        {
            Debug.LogError("Failed to save data to server: " + www.error);
        }
    }

    public IEnumerator LoadDataFromServer(string id, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "load_data");
        form.AddField("id", id);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string[] data = www.downloadHandler.text.Split(',');

            int.TryParse(data[0], out localscore);
            int.TryParse(data[1], out localcoins);
            int.TryParse(data[2], out localbestScore);

            // ���� �����͸� ������Ʈ
            PlayerPrefs.SetInt("localscore", localscore);
            PlayerPrefs.SetInt("localcoins", localcoins);
            PlayerPrefs.SetInt("localbestScore", localbestScore);

            // ���� �����͸� �ҷ���
            LoadLocalData();

            // �ְ� ���� UI�� ������Ʈ�ǵ��� �̺�Ʈ�� ȣ��
            onCoinsUpdated?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to load data from server: " + www.error);
        }
    }


    public bool IsLoggedIn()
    {
        return isLoggedIn;
    }

    // ������ �߰��ϴ� �Լ�
    public void AddLocalCoins(int amount)
    {
        localcoins += amount;

        // ���� ���� �ø��� ������ ������ ����
        StartCoroutine(SaveDataToServer(loginId.text, loginPassword.text, localscore, localcoins, localbestScore));

        SaveLocalData();
        onCoinsUpdated?.Invoke();
    }

    public void RemoveLocalCoins(int amount)
    {
        localcoins -= amount;
        localcoins = Mathf.Max(localcoins, 0);

        // ���� ���� �ø��� ������ ������ ����
        StartCoroutine(SaveDataToServer(loginId.text, loginPassword.text, localscore, localcoins, localbestScore));

        SaveLocalData();
        onCoinsUpdated?.Invoke();
    }

    public void IncreaseLocalScore(int amount)
    {
        localscore += amount;

        if (localscore > localbestScore)
            localbestScore = localscore;

        SaveLocalData();
    }

    public void ResetLocalScore()
    {
        localscore = 0;
        SaveLocalData();
    }

    // �ְ� ������ �����ϴ� �Լ�
    public int GetLocalBestScore()
    {
        return localbestScore;
    }

    // ���� ������ ��ȯ�ϴ� �Լ�
    public int GetLocalScore()
    {
        return localscore;

    }

    // ���� ���� ������ ��ȯ�ϴ� �Լ�
    public int GetLocalCoins()
    {
        return localcoins;
    }

    // ���� �����͸� �ε��ϴ� �Լ�
    private void LoadLocalData()
    {
        localcoins = PlayerPrefs.GetInt("localcoins", 400);
        localscore = PlayerPrefs.GetInt("localscore");
        localbestScore = PlayerPrefs.GetInt("localbestScore");
    }

    private void SaveLocalData()
    {
        PlayerPrefs.SetInt("localcoins", localcoins);
        PlayerPrefs.SetInt("localscore", localscore);
        PlayerPrefs.SetInt("localbestScore", localbestScore);
    }

    
}

