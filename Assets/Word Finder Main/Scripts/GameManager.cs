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

    [SerializeField] string url; // 서버 URL


    [Header(" Login Elements")]
    public TMP_InputField loginId; // 로그인 ID 필드
    public TMP_InputField loginPassword; // 로그인 비밀번호 필드

    [Header(" Signup Elements")]
    public TMP_InputField userId; // 회원가입 ID 필드
    public TMP_InputField userPassword; // 회원가입 비밀번호 필드
    public TMP_InputField userName; // 회원가입 사용자 이름 필드

    [Header(" Settings ")]
    private GameState gameState; // 게임의 현재 상태

    [Header(" Data ")]
    private int localcoins; // 사용자의 현재 코인
    private int localscore; // 사용자의 현재 점수
    private int localbestScore; // 사용자의 최고 점수

    [Header(" Events ")]
    public static Action<GameState> onGameStateChanged; // 게임 상태 변경 시 호출되는 이벤트
    public static Action onCoinsUpdated; // 코인 업데이트 시 호출되는 이벤트


    public bool isLoggedIn = false; // 사용자의 로그인 상태
    
    // 초기 설정, 싱글톤 패턴 적용
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadLocalData(); // 로컬 데이터 로드
    }

    // 로그인 성공 메시지
    private string loginSuccessMessage = "Login Success";
    private bool loginSuccess = false;

    // 회원가입 성공 메시지
    private string signupSuccessMessage = "Signup Success";
    private bool signupSuccess = false;

    // 게임 상태를 설정하는 함수
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

    // 로그아웃 버튼 콜백 함수
    public void LogoutBuutinCallback()
    {
        loginSuccess = false;
        isLoggedIn = false;
        SetGameState(GameState.Menu);
    }

    // 로그인 버튼 콜백 함수
    public void LogingoBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Login, 0.5f));
    }

    // 회원가입 버튼 콜백 함수
    public void SignupBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Signup, 0.5f));
    }

    // 로그인 화면에서 뒤로가기 버튼 콜백 함수
    public void BackLoginBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Menu, 0.5f));
    }

    // 다음 버튼 콜백 함수
    public void NextBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Game, 0.5f));
    }

    // 게임 시작 버튼 콜백 함수
    public void PlayBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Game, 0.5f));
    }

    // 뒤로 가기 버튼 콜백 함수
    public void BackBuutonCallback()
    {
        StartCoroutine(SwitchToStateWithDelay(GameState.Menu, 0.5f));
    }


    // 지정한 상태로 전환하는 코루틴
    IEnumerator SwitchToStateWithDelay(GameState targetState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetGameState(targetState);
    }

    public bool IsGameState()
    {
        return gameState == GameState.Game;
    }

    // 서버에 계정 정보를 보내는 코루틴
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

            // 로컬 데이터를 업데이트
            PlayerPrefs.SetInt("localscore", localscore);
            PlayerPrefs.SetInt("localcoins", localcoins);
            PlayerPrefs.SetInt("localbestScore", localbestScore);

            // 로컬 데이터를 불러옴
            LoadLocalData();

            // 최고 점수 UI가 업데이트되도록 이벤트를 호출
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

    // 코인을 추가하는 함수
    public void AddLocalCoins(int amount)
    {
        localcoins += amount;

        // 코인 변동 시마다 서버에 데이터 저장
        StartCoroutine(SaveDataToServer(loginId.text, loginPassword.text, localscore, localcoins, localbestScore));

        SaveLocalData();
        onCoinsUpdated?.Invoke();
    }

    public void RemoveLocalCoins(int amount)
    {
        localcoins -= amount;
        localcoins = Mathf.Max(localcoins, 0);

        // 코인 변동 시마다 서버에 데이터 저장
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

    // 최고 점수를 설정하는 함수
    public int GetLocalBestScore()
    {
        return localbestScore;
    }

    // 현재 점수를 반환하는 함수
    public int GetLocalScore()
    {
        return localscore;

    }

    // 현재 코인 개수를 반환하는 함수
    public int GetLocalCoins()
    {
        return localcoins;
    }

    // 로컬 데이터를 로드하는 함수
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

