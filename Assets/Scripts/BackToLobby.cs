using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BackToLobby : MonoBehaviour
{
    public void Exit()
    {

        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.Disconnect();
            }
        }
        SceneManager.LoadScene("Lobby");
    }
}
