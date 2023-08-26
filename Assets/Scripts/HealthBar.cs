using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] RectTransform healthBarRectTransform;
    Image handlerImage;
    [SerializeField] Gradient colorGradient;

    public PlayerController player;

    Vector2 originalSize;

    public float maxHealth = 100;

    private void InitHealthBar()
    {
        originalSize = healthBarRectTransform.sizeDelta;
        handlerImage = healthBarRectTransform.GetComponent<Image>();
    }

    public void UpdateHealthBar()
    {   
        if(handlerImage is null)
        {
            InitHealthBar();
        } 
        healthBarRectTransform.sizeDelta = new Vector2((player.health * originalSize.x) / maxHealth, originalSize.y);
        handlerImage.color = colorGradient.Evaluate(player.health / maxHealth);
    }

    [PunRPC]
    public void Hit(float damage)
    {
        player.health -= damage;
        player.health = Mathf.Max(0, player.health);
        UpdateHealthBar();
    }
}
