using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] RenderTexture screenshotRenderTexture;

    internal string TakeAndSaveScreenshot()
    {
        Texture2D renderResult = new Texture2D(screenshotRenderTexture.width, screenshotRenderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = screenshotRenderTexture;
        renderResult.ReadPixels(new Rect(0, 0, screenshotRenderTexture.width, screenshotRenderTexture.height), 0, 0);
        renderResult.Apply();
        byte[] screenshotBytes = renderResult.EncodeToPNG();

        string iconFilename = Application.dataPath + "/screens/";
        if (!Directory.Exists(iconFilename)) //pokud neexistuje slozka screens
            Directory.CreateDirectory(iconFilename);


        iconFilename += System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";

        File.WriteAllBytes(iconFilename, screenshotBytes);

        return iconFilename;
    }
}
