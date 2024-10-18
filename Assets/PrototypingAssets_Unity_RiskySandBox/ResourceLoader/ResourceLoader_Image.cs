using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ResourceLoader_Image : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] string path;


    [SerializeField] List<UnityEngine.UI.Image> my_Images = new List<UnityEngine.UI.Image>();
    [SerializeField] List<UnityEngine.UI.RawImage> my_RawImages = new List<UnityEngine.UI.RawImage>();
    public Texture2D loaded_image { get { return this.PRIVATE_loaded_image; } }
    [SerializeField] Texture2D PRIVATE_loaded_image;

    private void Start()
    {
        loadImage();
    }

    void loadImage()
    {
        this.PRIVATE_loaded_image = Resources.Load<Texture2D>(path);
        applyTexture(PRIVATE_loaded_image);
    }

    void applyTexture(Texture2D _Texture2D)
    {
        if(this.my_Images.Count() == 0 && this.my_RawImages.Count() == 0)
        {
            GlobalFunctions.printWarning("no Images???",this);
            return;
        }

        foreach(UnityEngine.UI.Image _Image in this.my_Images)
        {
            if(_Image == null)
            {
                GlobalFunctions.printWarning("null Image???",this);
                continue;
            }    

            Sprite newSprite = Sprite.Create(_Texture2D, new Rect(0, 0, _Texture2D.width, _Texture2D.height), new Vector2(0.5f, 0.5f));
            _Image.sprite = newSprite;
        }

        foreach(UnityEngine.UI.RawImage _RawImage in this.my_RawImages)
        {
            if(_RawImage == null)
            {
                GlobalFunctions.printWarning("null RawImage???", this);
                continue;
            }
            _RawImage.texture = _Texture2D;
        }
    }
}
