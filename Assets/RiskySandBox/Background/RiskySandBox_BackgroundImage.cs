using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

#if UNITY_WEBGL == true
using UnityEngine.Networking;
#endif

public partial class RiskySandBox_BackgroundImage : MonoBehaviour
{
    public static RiskySandBox_BackgroundImage instance;
    public static event Action<Texture2D> OnupdateTexture2D_STATIC;

    [SerializeField] bool debugging;

    public byte[] current_byte_array { get { return ((Texture2D)background_Image.mainTexture).EncodeToPNG(); } }

    [SerializeField] UnityEngine.UI.RawImage background_Image;

    [SerializeField] ObservableFloat PRIVATE_background_image_x_scale;
    [SerializeField] ObservableFloat PRIVATE_background_image_y_scale;

    [SerializeField] ObservableInt backround_width;
    [SerializeField] ObservableInt background_height;

    [SerializeField] ObservableVector3 background_bottom_left;
    [SerializeField] ObservableVector3 background_top_right;


    private void Awake()
    {
        instance = this;

        RiskySandBox_Map.OnloadMapCompleted += EventReceiver_OnloadMapCompleted;
        RiskySandBox_Map.OnclearMap += EventReceiver_OnclearMap;

        RiskySandBox_MainMenu.Onenable += MainMenuEventReceiver_Onenable;
    }

    private void OnDestroy()
    {

        RiskySandBox_Map.OnloadMapCompleted -= EventReceiver_OnloadMapCompleted;
        RiskySandBox_Map.OnclearMap -= EventReceiver_OnclearMap;

        RiskySandBox_MainMenu.Onenable -= MainMenuEventReceiver_Onenable;

    }

    void MainMenuEventReceiver_Onenable()
    {
        this.background_Image.gameObject.SetActive(false);
    }


    void EventReceiver_OnclearMap()
    {
        this.background_Image.gameObject.SetActive(false);
    }





    void EventReceiver_OnloadMapCompleted()
    {

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        StartCoroutine("loadTexture_Webgl");
#else
        loadTexture();
#endif
        this.background_Image.gameObject.SetActive(true);
        
    }


    public void updateTextureFromServer(byte[] _bytes)
    {
        updateTexture(_bytes);
        this.background_Image.gameObject.SetActive(true);
    }

    void updateTexture(byte[] _bytes)
    {
        Texture2D _Texture2D = new Texture2D(2, 2);
        _Texture2D.LoadImage(_bytes); // Load the PNG data into the texture
        background_Image.texture = _Texture2D;


        float _target_width = _Texture2D.width;
        float _target_height = _Texture2D.height;

        while (_target_width > 1 || _target_height > 1)
        {
            _target_width = _target_width / 10f;
            _target_height = _target_height / 10f;
        }

        background_bottom_left.value = new Vector3(-(_target_width * 100 / 2), 0, -(_target_height * 100 / 2));
        background_top_right.value = new Vector3(_target_width * 100 / 2, 0, _target_height * 100 / 2);

        PRIVATE_background_image_x_scale.value = _target_width;
        PRIVATE_background_image_y_scale.value = _target_height;


        OnupdateTexture2D_STATIC?.Invoke(_Texture2D);

    }


    void loadTexture()
    {
        

        Texture2D _Texture2D = new Texture2D(2, 2);

        string _png_path = "";
        if (RiskySandBox_LevelEditor.is_enabled) //if the level editor is enabled?
        {
            _png_path = System.IO.Path.Combine(RiskySandBox.maps_folder_path, RiskySandBox_MainGame.instance.map_ID, "LevelEditor.png");//get the level editor image...
        }
        else
        {
            _png_path = System.IO.Path.Combine(RiskySandBox.maps_folder_path, RiskySandBox_MainGame.instance.map_ID, "MainGame.png");//get the "MainGame" Image
        }

        if (System.IO.File.Exists(_png_path) == false)
        {
            GlobalFunctions.printWarning("unable to find the background image Texture... " + _png_path,this);
            this.background_Image.gameObject.SetActive(false);
            return;
        }
            
        
        byte[] fileData = System.IO.File.ReadAllBytes(_png_path);
        updateTexture(fileData);

    }

#if UNITY_WEBGL == true
    IEnumerator loadTexture_Webgl()
    {

        string _map_ID = RiskySandBox_MainGame.instance.map_ID;
        string _png_url = "";

        if(RiskySandBox_LevelEditor.is_enabled)
        {
            _png_url = Application.streamingAssetsPath + string.Format("/RiskySandBox/Maps/{0}/LevelEditor.png", _map_ID);
        }
        else
        {
            _png_url = Application.streamingAssetsPath + string.Format("/RiskySandBox/Maps/{0}/MainGame.png", _map_ID);
        }
     

        // For WebGL, we need to use UnityWebRequest
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_png_url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load texture: " + request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            byte[] textureBytes = texture.EncodeToPNG();

            updateTexture(textureBytes);
        }
    }
#endif

}
