using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class MultiplayerBridge_PhotonPun : MonoBehaviourPunCallbacks
{
	public static MultiplayerBridge_PhotonPun instance;
	public static event Action<Photon.Realtime.Player> OnplayerConnected;

	public static ObservableBool in_room { get { return instance.PRIVATE_in_room; } }



	[SerializeField] bool debugging;

	[SerializeField] ObservableBool PRIVATE_in_room;


	string create_room_code = "";
	string joining_room_code = "";
	bool creating_single_player = false;


	void Awake()
    {
		if (this.debugging)
			GlobalFunctions.print("called Awake", this);
		instance = this;

    }

	public void createSinglePlayerRoom()
    {
		creating_single_player = true;
		PhotonNetwork.OfflineMode = true;
    }


	public void joinRoom(string _code)
    {
		if(PhotonNetwork.IsConnected)
        {
			PhotonNetwork.JoinRoom(_code);
        }
		else
        {
			this.joining_room_code = _code;
			PhotonNetwork.ConnectUsingSettings();
        }
    }

	public void createMultiplayerRoom(string _code)
    {
		if(PhotonNetwork.IsConnected)
        {
			PhotonNetwork.CreateRoom(_code);
        }
		else
        {
			create_room_code = _code;
			PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

		if (this.joining_room_code != "")
		{
			PhotonNetwork.JoinRoom(this.joining_room_code);
			return;
		}

		if(this.create_room_code != "")
        {
			PhotonNetwork.CreateRoom(this.create_room_code);
			return;
        }

		if(this.creating_single_player == true)
        {
			PhotonNetwork.CreateRoom("SINGLEPLAYERROOM");
			return;
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

		OnplayerConnected?.Invoke(newPlayer);
    }

	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();

		PrototypingAssets.run_server_code.value = true;
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);

		PrototypingAssets.run_server_code.value = PhotonNetwork.IsMasterClient;
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();

		PrototypingAssets.run_client_code.value = false;
		this.PRIVATE_in_room.value = false;
		PrototypingAssets.run_server_code.value = false;
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		this.joining_room_code = "";
		this.creating_single_player = false;
		this.create_room_code = "";

		PrototypingAssets.run_client_code.value = true;
		this.PRIVATE_in_room.value = true;
	}

}
