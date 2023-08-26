using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviourPunCallbacks
{
    GameObject GUI;
    Joystick joystick;

    [SerializeField] float speed;
    float angle;

    public SpriteRenderer texture;
    CircleCollider2D collider;
    public int coinsCount = 0;

    public BulletSpawner bulletSpawner;

    public float health;
    public HealthBar healthBar;

    public PhotonView view;
    Text nickname;
    Text coinsCountText;

    Vector3 newRotation;

    GameObject gameOverPanel;
    GameObject localExit;

    [SerializeField] Text winnerNameText;
    [SerializeField] Text winnerCoinsCountText;
    public bool alive;

    PlayerSpawner playerSpawner;


    private void Awake()
    {
        newRotation = Vector3.zero;
        joystick = FindObjectOfType<Joystick>();
        coinsCountText = GameObject.Find("localCoin_text").GetComponent<Text>();
        localExit = GameObject.FindGameObjectWithTag("LocalExit");
        view = GetComponent<PhotonView>();

        nickname = GetComponentInChildren<Text>();
        nickname.text = view.Owner.NickName;

        collider = GetComponent<CircleCollider2D>();

        if (view.IsMine)
        {
            alive = true;
            bulletSpawner = FindObjectOfType<BulletSpawner>();
            bulletSpawner.player = this;
            texture.color = new Color32(75, 179, 250, 255);
            gameObject.tag = "Player";
        }
        else
        {
            gameObject.tag = "EnemyPlayer";
        }

        healthBar = GetComponentInChildren<HealthBar>();

        health = healthBar.maxHealth;

        healthBar.UpdateHealthBar();

        playerSpawner = FindObjectOfType<PlayerSpawner>();

        GUI = GameObject.Find("GUI");

        Global.Instance.SetRoomProperty("winnerNickName", string.Empty);
        Global.Instance.SetRoomProperty("winnerCoinsCount", 0);
    }

    private void Start()
    {
        if (view.IsMine)
        {
            gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");
            winnerNameText = gameOverPanel.transform.Find("WinnerName_text").GetComponent<Text>();
            winnerCoinsCountText = gameOverPanel.transform.Find("Bottom/CoinCount_text").GetComponent<Text>();
            gameOverPanel.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if ((bool)Global.Instance.GetRoomProperty("isStarted"))
        {
            if (view.IsMine)
            {
                if (alive && health == 0)
                {
                    Dead();
                }
                if ((int)Global.Instance.GetRoomProperty("alivePlayers") == 1 || PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    if (alive)
                    {
                        Global.Instance.SetRoomProperty("winnerNickName", nickname.text);
                        Global.Instance.SetRoomProperty("winnerCoinsCount", coinsCount);
                    }
                    GameOver();
                }
                if (health > 0)
                {
                    MovementAndRotation();
                }
            }
        }
    }

    void MovementAndRotation()
    {
        transform.Translate(joystick.Direction * speed * Time.fixedDeltaTime);
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, playerSpawner.boundaries.Item1.x + collider.radius, playerSpawner.boundaries.Item2.x - collider.radius),
            Mathf.Clamp(transform.position.y, playerSpawner.boundaries.Item1.y + collider.radius, playerSpawner.boundaries.Item2.y - collider.radius));
        angle = Vector2.SignedAngle(Vector2.up, joystick.Direction);
        if (joystick.Direction != Vector2.zero)
        {
            newRotation = Vector3.forward * angle;
        }
        texture.transform.rotation = Quaternion.Lerp(
            texture.transform.rotation,
            Quaternion.Euler(newRotation.x, newRotation.y, newRotation.z),
            Time.fixedDeltaTime * 10f);
    }
    public void Disconnect()
    {
        Global.Instance.SetRoomProperty("alivePlayers", (int)Global.Instance.GetRoomProperty("alivePlayers") - 1);
        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(Leave());
        }
    }

    IEnumerator Leave()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.CurrentRoom.Players[1]);
        }
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
            Debug.Log("Disconnecting...");
        }
        SceneManager.LoadScene("Lobby");
    }
    public void GainCoin()
    {
        coinsCount++;
        if (view.IsMine)
        {
            coinsCountText.text = coinsCount.ToString();
        }
    }

    void Dead()
    {
        healthBar.UpdateHealthBar();
        alive = false;
        GUI.SetActive(false);
        Global.Instance.SetRoomProperty("alivePlayers", (int)Global.Instance.GetRoomProperty("alivePlayers") - 1);
        localExit.SetActive(true);
        transform.localScale = Vector3.zero;
    }

    void GameOver()
    {
        alive = false;
        localExit.SetActive(false);
        GUI.SetActive(false);
        gameOverPanel.SetActive(true);
       
        if (view.IsMine)
        {
            winnerNameText.text = $"{Global.Instance.GetRoomProperty("winnerNickName")} победил!";
            winnerCoinsCountText.text =
                Global.Instance.GetRoomProperty("winnerCoinsCount").ToString();
            gameOverPanel.GetComponent<Animator>().Play("Base Layer.PopUp");
        }
    }
}
