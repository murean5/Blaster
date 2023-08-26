using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField createField;
    [SerializeField] InputField joinField;
    [SerializeField] InputField nicknameField;
    [SerializeField] Text nicknamePlaceHolderText;
    [SerializeField] GameObject gearObject;
    [SerializeField] Text errorMsgText;

    private void Start()
    {
        errorMsgText.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(PhotonNetwork.NickName)) {
            nicknamePlaceHolderText.text = $"игрок#{PhotonNetwork.CountOfPlayers}";
        }
        else
        {
            nicknameField.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    public void CreateRoom()
    {
        gearObject.SetActive(true);
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        if (string.IsNullOrEmpty(createField.text) || string.IsNullOrWhiteSpace(createField.text))
        {
            ActivateErrorMsg(0);
            return;
        }
        PhotonNetwork.CreateRoom(createField.text, roomOptions);
    }

    public void JoinRoom()
    {
        gearObject.SetActive(true);
        if (string.IsNullOrEmpty(joinField.text) || string.IsNullOrWhiteSpace(joinField.text))
        {
            ActivateErrorMsg(0);
            return;
        }
        PhotonNetwork.JoinRoom(joinField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = nicknameField.text == string.Empty ? nicknamePlaceHolderText.text : nicknameField.text;
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"{returnCode} {message}");
        ActivateErrorMsg(returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"{returnCode} {message}");
        ActivateErrorMsg(returnCode);
    }

    private void ActivateErrorMsg(short errorCode)
    {
        gearObject.SetActive(false);
        errorMsgText.gameObject.SetActive(true);
        switch (errorCode)
        {
            case 0:
                errorMsgText.text = "Пустое название!";
                break;
            case 32758:
                errorMsgText.text = "Такой комнаты не существует!";
                break;
            case 32764:
                errorMsgText.text = "В комнате уже идет игра!";
                break;
            case 32765:
                errorMsgText.text = "Комната переполнена!";
                break;
            case 32766:
                errorMsgText.text = "Такая комната уже существует!";
                break;
            default:
                errorMsgText.text = "Непредвиденная ошибка!";
                break;
        }
        InvokeRepeating("FadeOut", 1.5f, 0.05f);
    }

    void FadeOut()
    {
        if(errorMsgText.color.a <= 0f)
        {
            CancelInvoke("FadeOut");
            errorMsgText.gameObject.SetActive(false);
            errorMsgText.color = new Color(errorMsgText.color.r, errorMsgText.color.g, errorMsgText.color.b, 1f);
            return;
        }
        errorMsgText.color = new Color(errorMsgText.color.r, errorMsgText.color.g, errorMsgText.color.b, errorMsgText.color.a - 0.05f);
    }
}
