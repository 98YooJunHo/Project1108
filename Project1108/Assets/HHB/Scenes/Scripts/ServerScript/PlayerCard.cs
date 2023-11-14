using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerCard : MonoBehaviourPunCallbacks
{
    Transform lobbyUI;
    Transform playerCard;

    // �ν��Ͻ��� ��ġ����
    public void Start()
    {
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            if (root.name == "LobbyUI")
            {
                lobbyUI = root.transform;
            }
        }

        playerCard = lobbyUI.transform.GetChild(0);
        this.transform.SetParent(playerCard);
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0f);
        photonView.RPC("PrintUserNames", RpcTarget.AllBuffered);
    }

    // �̸� ����ȭ
    [PunRPC]
    public void PrintUserNames()
    {
        TextMeshProUGUI text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        text.text = customProperties[this.transform.gameObject.GetComponent<PhotonView>().Owner.NickName.ToString()].ToString();
    }


    // ������ �� ������ playerCard �ı� & Hashtable ����
    public override void OnLeftRoom()
    {
        Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        foreach (var prop in customProperties)
        {
            if (prop.Key.ToString() == PhotonNetwork.NickName)
            { 
                customProperties.Remove(prop.Key);
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
