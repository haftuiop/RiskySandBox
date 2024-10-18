using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class MultiplayerBridge_Photon_PrefabInstantiationSettings : MonoBehaviour
{

	public PhotonPrefabInstantiation_settings instantiation_settings;

	public enum PhotonPrefabInstantiation_settings
	{
		UnityEngine,
		Room,
		Photon
	}

}

