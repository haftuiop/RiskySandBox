using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ResourceLoader_ImageFolder : MonoBehaviour
{
    [SerializeField] private bool debugging = false;
    [SerializeField] private string path; // Path inside the Resources folder

    public ObservableList<Texture2D> images = new ObservableList<Texture2D>();

    void Start()
    {
        LoadImagesFromResources();
    }

    void LoadImagesFromResources()
    {
        if(path == null || path == "")
        {
            GlobalFunctions.printError("path is empty...",this);
            return;
        }
        // Load all the textures in the specified folder inside Resources
        Texture2D[] loadedImages = Resources.LoadAll<Texture2D>(path);

        if (loadedImages.Length > 0)
        {
            foreach (var image in loadedImages)
            {
                images.Add(image); // Add each image to the observable list
            }

            if (this.debugging)
                GlobalFunctions.print(string.Format("Loaded {0} images from folder: {1}", loadedImages.Length, path), this);
        }
        else
        {
            if (this.debugging)
                GlobalFunctions.printWarning(string.Format("No images found in Resources/{0} folder.", path), this);
        }
    }
}