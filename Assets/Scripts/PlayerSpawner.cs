using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Text roomName;

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject waitingBox;

    Camera mainCamera;

    public (Vector2, Vector2) boundaries;


    private void Start()
    {
        roomName.text = $"комната: {PhotonNetwork.CurrentRoom.Name}";

        mainCamera = FindObjectOfType<Camera>();
        SetScreenBoundaries();
        
        var spawnedPlayer = PhotonNetwork.Instantiate(
            playerPrefab.name,
            Global.Instance.CalculateSpawnPosition(boundaries), 
            Quaternion.identity)
            .GetComponent<PlayerController>();
        spawnedPlayer.texture.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360 + 1), Vector3.forward);
        Global.Instance.SetRoomProperty("isStarted", false);
        Global.Instance.SetRoomProperty("alivePlayers", PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Global.Instance.SetRoomProperty("alivePlayers", PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private void Update()
    {
        if (!((bool)Global.Instance.GetRoomProperty("isStarted")))
        {
            waitingBox.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                waitingBox.GetComponentInChildren<Text>().text = "ќжидание игроков...";
                if ((int)Global.Instance.GetRoomProperty("alivePlayers") > 1 && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    waitingBox.SetActive(false);
                    startButton.SetActive(true);
                }
                else
                {
                    startButton.SetActive(false);
                }
            }
            else
            {
                waitingBox.GetComponentInChildren<Text>().text = "ќжидание начала...";
            }
        }
        else
        {
            waitingBox.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        waitingBox.SetActive(false);
        startButton.SetActive(false);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        Global.Instance.SetRoomProperty("isStarted", true);
    }

    void SetScreenBoundaries()
    {
        var width = mainCamera.pixelWidth;
        var height = mainCamera.pixelHeight;

        Vector2 bottomLeft = mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 topRight = mainCamera.ScreenToWorldPoint(new Vector2(width, height));

        boundaries = (bottomLeft, topRight);
    }
}
