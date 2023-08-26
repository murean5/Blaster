using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text errorMsgText;

    [SerializeField] Animation fadeIn;
    private void Start()
    {
        errorMsgText.gameObject.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        Invoke("NetworkFailure", 5f);
    }
    public override void OnConnectedToMaster()
    {
        CancelInvoke("NetworkFailure");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        fadeIn.Play("FadeIn");
        StartCoroutine(FadingIn());
    }

    IEnumerator FadingIn()
    {
        yield return new WaitUntil(() => !fadeIn.isPlaying);
        SceneManager.LoadScene("Lobby");
    }
    void NetworkFailure()
    {
        if(PhotonNetwork.GetPing() >= 170)
        {
            errorMsgText.gameObject.SetActive(true);
        }
    }
}
