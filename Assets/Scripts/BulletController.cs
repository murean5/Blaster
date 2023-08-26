using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] float speed = 5.5f;

    private void Start()
    {
        Destroy(gameObject, 2.5f);
    }
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemyTag = GetComponent<PhotonView>().IsMine ? "EnemyPlayer" : "Player";
       

        if (collision.CompareTag(enemyTag))
        {
            var hittedPlayer = collision.GetComponent<PlayerController>();
            var healthBar = hittedPlayer.healthBar;
            healthBar.GetComponent<PhotonView>().RPC("Hit", RpcTarget.Others, 5f);
            Destroy(gameObject, 0.1f);
        } 
    }
}
