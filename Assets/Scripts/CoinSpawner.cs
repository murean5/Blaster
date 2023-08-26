using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;

    [SerializeField] float spawnInterval = 1.5f;
    float timePass = 0f;

    [SerializeField] int maxCoinsInTheRoom = 7;

    [SerializeField] PlayerSpawner playerSpawner;

    PhotonView view;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        Global.Instance.SetRoomProperty("coinsInTheRoom", 0);
    }

    private void Update()
    {
        if ((bool)Global.Instance.GetRoomProperty("isStarted") &&
            (int)Global.Instance.GetRoomProperty("alivePlayers") > 1 && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {

            timePass += Time.deltaTime;
            if (timePass >= spawnInterval)
            {
                if (FindObjectsOfType<Coin>().Length < maxCoinsInTheRoom)
                {
                    var coin = PhotonNetwork.InstantiateRoomObject(
                        coinPrefab.name, 
                        Global.Instance.CalculateSpawnPosition(playerSpawner.boundaries), 
                        Quaternion.identity);
                }
                timePass = 0f;
            }
        }
    }
}
