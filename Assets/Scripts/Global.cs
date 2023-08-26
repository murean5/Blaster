using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Global : MonoBehaviour
{
    private static Global instance;
    public static Global Instance
    {
        get => instance ?? (instance = new GameObject("Global").AddComponent<PhotonView>().gameObject.AddComponent<Global>());
    }
    void UpdateRoomProperty(string key, object newValue)
    {
        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
        properties[key] = newValue;
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public void SetRoomProperty(string key, object value)
    {
        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (properties.ContainsKey(key))
        {
            UpdateRoomProperty(key, value);
            return;
        }
        properties.Add(key, value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public object GetRoomProperty(string key)
    {
        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
        return properties[key];
    }

    public Vector2 CalculateSpawnPosition((Vector2, Vector2) boundaries)
    {
        var pos = new Vector2(
            Random.Range(boundaries.Item1.x * 0.65f, boundaries.Item2.x * 0.65f),
            Random.Range(boundaries.Item1.y * 0.65f, boundaries.Item2.y * 0.65f));
        if (Physics2D.OverlapCircleAll(pos, 0.7f).Length > 0)
        {
            return CalculateSpawnPosition(boundaries);
        }
        return pos;
    }
}
