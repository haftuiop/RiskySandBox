using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public partial class FriendsOfRiskBridge : MonoBehaviour
{

    public static FriendsOfRiskBridge instance;

    public static List<FriendsOfRisk_PlayerData> ranking_data = new List<FriendsOfRisk_PlayerData>();
    public static event Action OnUpdate_RankingData;

    /// <summary>
    /// invoked once the battle log has been sent to the website...
    /// </summary>
    public static event Action OnsentBattleLog;


    [SerializeField] bool debugging;
    [SerializeField] bool testing;

    public KeyCode get_ffa_data_KeyCode = KeyCode.T;

    [SerializeField] string testing_map_ID = "";

    //TODO for uploading battle logs to this website????
    public static string secret_api_key { get { return "secretapikey"; } }




    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        if (Input.GetKeyDown(get_ffa_data_KeyCode))
            updateRankingData("FFA", testing_map_ID);
    }

    public void updateRankingData(string _channel, string _map_ID)
    {
        StartCoroutine(GET_rankingData(_channel, _map_ID));
    }


    public void sendBattleLog()
    {

    }



    /// <summary>
    /// send the battle log to the FriendsOfRisk website so they can update ranks...
    /// </summary>
    static IEnumerator POST_battleLog()
    {
        //TODO
        //if we are a official dedicated server == false... return


        Debug.LogWarning("unimplemented... (sending game data to the friends of risk webstite...)");
        yield return null;

        //send whatever data they want for the ranking system... using post request???





        OnsentBattleLog?.Invoke();

    }



    /// <summary>
    /// channel should be things like ffa, 1v1, capitals_ffa etc...  also per map?
    /// </summary>
    static IEnumerator GET_rankingData(string _channel,string _map_id)
    {
        List<FriendsOfRisk_PlayerData> _return_list = new List<FriendsOfRisk_PlayerData>();

        // TODO - implement the channel...
        string _url = string.Format("https://friendsofrisk.com/rankdata.php?{0}&format=csv",_channel);




        using (UnityWebRequest webRequest = UnityWebRequest.Get(_url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                
                
            }
            else
            {



                // Print the response
                Debug.Log("Response: " + webRequest.downloadHandler.text);




                // Parse the CSV data
                string[] lines = webRequest.downloadHandler.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();//skip(1) - the first line is the "headers" of the data
                foreach (var line in lines)
                {
                    _return_list.Add(new FriendsOfRisk_PlayerData(line));
                }




                ranking_data = _return_list;
                if (instance.debugging)
                    GlobalFunctions.print("invoking OnUpdate_RankingData", instance);
                OnUpdate_RankingData?.Invoke();//'ranking ui' can listen to this event and update as needed....
            }
        }


    }



    public struct FriendsOfRisk_PlayerData
    {
        public FriendsOfRisk_PlayerData(string _csv_string)
        {
            try
            {
                string[] _data = _csv_string.Split(',');
                this.playerid = int.Parse(_data[0]);
                this.rank = int.Parse(_data[1]);
                this.player = _data[2];
                this.points = int.Parse(_data[3]);
                this.appearances = int.Parse(_data[4]);
            }
            catch
            {
                //something is going wrong... e.g. someone has a name that contains a ','??? sa, yesmin
                this.playerid = -1;
                this.rank = -1;
                this.player = "error....";
                this.points = -1;
                this.appearances = -1;
            }
        }
        
        public int playerid;        // The player's ID
        public int rank;             // The player's rank
        public string player;  // e.g. 'arco', 'IamMomba', etc.
        public int points;
        public int appearances;
    }
}
