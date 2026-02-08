using System;
using System.Collections.Generic;
using UnityEngine;

public static class UIQualityColor
{
    private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>();

    static UIQualityColor()
    {
        Color white = Color.white; // #FFFFFF
        Color blue;
        Color purple;
        Color green;
        Color orange; 

        ColorUtility.TryParseHtmlString("#FFFFFF", out white);
        ColorUtility.TryParseHtmlString("#00BFFF", out blue);
        ColorUtility.TryParseHtmlString("#9932CC", out purple);
        ColorUtility.TryParseHtmlString("#008000", out green);
        ColorUtility.TryParseHtmlString("#FFA500", out orange); 


        Colors["White"] = white;
        Colors["Blue"] = blue;
        Colors["Purple"] = purple;
        Colors["Green"] = green;
        Colors["Orange"] = orange;
        
        // Add Chinese mappings just in case data uses Chinese
        Colors["白色"] = white;
        Colors["蓝色"] = blue;
        Colors["紫色"] = purple;
        Colors["绿色"] = green;
        Colors["橙色"] = white; 
    }

    public static Color GetColor(string quality)
    {
        if (string.IsNullOrEmpty(quality))
            return Colors["White"];

        if (Colors.TryGetValue(quality, out Color color))
        {
            return color;
        }

        return Colors["White"];
    }
}
