using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;

public partial class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    [Header("�α���")]
    public TMP_InputField loginIdInput;
    public TMP_InputField loginPasswordInput;
    public TextMeshProUGUI loginInfoTxt;
    public Button loginButton;
    public Button quitGameButton;
    public Button registerButton;
    [Tooltip("�α��� UI")]
    public GameObject loginUI;

    [Header("ȸ������")]
    public TMP_InputField signNickNameInput;
    public TMP_InputField signIdInput;
    public TMP_InputField signPassWordInput;
    public TextMeshProUGUI signInInfoTxt;
    public Button signButton;
    public Button backButton;
    [Tooltip("ȸ������ �˾�UI")]
    public GameObject signUI;

    public PlayerLeaderboardEntry myPlayerInfo;
    public List<PlayerLeaderboardEntry> playfabUserList = new List<PlayerLeaderboardEntry>();

    private void Start()
    {
        PlayFabSettings.TitleId = "74971";
        PhotonNetwork.GameVersion = "1";
        loginButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    // �α���
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest 
        { 
            Email = loginIdInput.text, 
            Password = loginPasswordInput.text 
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => 
        {
            GetLeaderboard(result.PlayFabId);
            loginInfoTxt.text = "LoginSuccess";
            loginUI.gameObject.SetActive(false);
            lobbyUI.gameObject.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        },
        (error) => loginInfoTxt.text = "Login Failed");
    }

    // ȸ������
    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = signIdInput.text,
            Password = signPassWordInput.text,
            DisplayName = signNickNameInput.text,
            Username = signNickNameInput.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
        {
            signInInfoTxt.text = "Sign In";
            SetStat();
            SetData("Default");
            signUI.SetActive(false);
        },
        (error) => Debug.Log(error));
    }

    // Test
    void GetLeaderboard(string myID)
    {
        playfabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
            };
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) { return; }
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    playfabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) 
                    { 
                        myPlayerInfo = result.Leaderboard[j];
                    }
                }
            },
            (error) => { });
        }
    }
    // Test
    private void SetData(string curData)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { "Home", curData } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log(error));
    }
    // Test
    private void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log(error));
    }


    // Master ����
    public override void OnConnectedToMaster()
    {
        loginButton.interactable = true;
    }


    // Photon ����
    public void Connect()
    {
        loginButton.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else { PhotonNetwork.ConnectUsingSettings(); }
    }

    // �� ���н�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Room", new RoomOptions { MaxPlayers = 4 });
    }

    // ��Ʈ��ũ ���� ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        loginButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    // �� ���� ���н�
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���� ����
    public void OnGameQuit()
    {
        Application.Quit();
    }

    // ���� ���
    public void QuitRegister()
    {
        signUI.SetActive(false);
    }

    // ���� ��ư
    public void OnRegister()
    { 
        signUI.SetActive(true);
    }
}
