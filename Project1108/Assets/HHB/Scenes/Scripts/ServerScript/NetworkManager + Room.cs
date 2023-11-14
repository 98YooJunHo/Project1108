using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public partial class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("�κ�")]
    public GameObject lobbyUI;

    public GameObject playerCard;
    public Transform playerCardHolder;
    public Button startButton;
    public TextMeshProUGUI readyOrStartTxt;

    [Header("ReadyState")]
    private int readyCount = 0;
    private bool isReady = false;

    [Header("HashTable Key/Value")]
    public string playerActorNum;
    public string playfabDisplayName;



    public override void OnJoinedRoom()
    {

        Invoke("InitUserInfo", 2f);
        Invoke("InstantiatePlayerCard", 3f);

        if (!PhotonNetwork.IsMasterClient)
        {
            readyOrStartTxt.text = "Ready";
        }
        else { readyOrStartTxt.text = "Start"; }
    }

    // ���� ���� ����
    public void InitUserInfo()
    {
        Hashtable customProperties = new Hashtable();
        PhotonNetwork.NickName = myPlayerInfo.DisplayName.ToString();
        foreach (Player player in PhotonNetwork.PlayerList)
        {         
            customProperties[player.NickName.ToString()] = player.NickName.ToString();
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    // PlayerCard Instance
    public void InstantiatePlayerCard()
    {
        GameObject holder = PhotonNetwork.Instantiate("PlayerCard", Vector3.zero, Quaternion.identity);
        holder.name = "PlayerCard";
        TextMeshProUGUI text = holder.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = myPlayerInfo.DisplayName;
    }

    public void ReadyOrStart()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AddReadyCount", RpcTarget.MasterClient);
        }
        else
        {
            if (readyCount / PhotonNetwork.CountOfPlayersInRooms == 0)
            {
                PhotonNetwork.LoadLevel("Main");
            }
        }
    }

    [PunRPC]
    public void AddReadyCount()
    {
        // ĳ���� ������ �Ǿ����� �߰� �ʿ�
        if (!isReady)
        {
            readyCount++;
        }
        else { readyCount--; }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        //foreach (var prop in customProperties)
        //{
        //    Debug.LogError("Key : " + prop.Key + "Value : " + prop.Value);
        //}

        //foreach (var player in PhotonNetwork.PlayerList)
        //{
        //    Debug.LogError("player : " + player.NickName.ToString());
        //}
    }
}
