using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            collision.GetComponent<PlayerController>().GainCoin();
            if (GetComponent<PhotonView>().IsMine)
            {
                Global.Instance.SetRoomProperty("coinsInTheRoom", FindObjectsOfType<Coin>().Length);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
