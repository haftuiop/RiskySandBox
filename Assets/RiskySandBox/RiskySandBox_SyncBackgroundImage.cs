using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class RiskySandBox_SyncBackgroundImage : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;
    [SerializeField] PhotonView my_PhotonView { get { return GetComponent<PhotonView>(); } }


    //TODO - dedicated server problem...???
    bool is_the_server { get { return PrototypingAssets.run_server_code.value; } }


    private void Awake()
    {
        RiskySandBox_BackgroundImage.OnupdateTexture2D_STATIC += EventReceiver_OnupdateTexture2D_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_BackgroundImage.OnupdateTexture2D_STATIC -= EventReceiver_OnupdateTexture2D_STATIC;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
        base.OnPlayerEnteredRoom(newPlayer);

        if (is_the_server == false)
            return;

        byte[] textureBytes = RiskySandBox_BackgroundImage.instance.current_byte_array;
        my_PhotonView.RPC("ReceiveTexture", newPlayer, textureBytes);
    }

    void EventReceiver_OnupdateTexture2D_STATIC(Texture2D _new_Texture2D)
    {
        if (is_the_server == false)
            return;


        //send everyone the new texture!
        byte[] textureBytes = _new_Texture2D.EncodeToPNG();
        my_PhotonView.RPC("ReceiveTexture", RpcTarget.Others, textureBytes);

    }

    [PunRPC]
    public void ReceiveTexture(byte[] textureBytes,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)
        {
            return;
        }


            RiskySandBox_BackgroundImage.instance.updateTextureFromServer(textureBytes);
    }




}
