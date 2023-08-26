using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;

    public PlayerController player;

    float timePass = 0f;
    float spawnInterval = 0.3f;

    bool canShoot = true;
    public void SpawnBullet()
    {
        if((int)Global.Instance.GetRoomProperty("alivePlayers") < 2)
        {
            return;
        }
        if (canShoot)
        {
            Quaternion playerRotation = player.texture.transform.rotation;
            var bullet = PhotonNetwork.Instantiate(bulletPrefab.name, player.transform.position + playerRotation.eulerAngles * 1.2f, playerRotation);
            bullet.GetComponentInChildren<SpriteRenderer>().color = player.texture.color;
            canShoot = false;
        }
    }

    private void Update()
    {
        if(!canShoot && timePass < spawnInterval)
        {
            timePass += Time.deltaTime;
        }
        else
        {
            canShoot = true;
            timePass = 0f;
        }
    }
}
