using com.xenturio.basegame;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameUtils
{

   

    public static string GetColorName(Color color)
    {

        if (color.Equals(Color.black))
        {
            return "BLACK";
        }
        if (color.Equals(new Color32(0, 114, 255, 255)))
        {
            return "BLUE";
        }
        if (color.Equals(Color.green))
        {
            return "GREEN";
        }
        if (color.Equals(Color.yellow))
        {
            return "YELLOW";
        }
        if (color.Equals(Color.red))
        {
            return "RED";
        }
        if (color.Equals(new Color(226, 0, 225, 225)))
        {
            return "PURPLE";
        }
        return "";
    }

    public static Color GetColorByName(string name)
    {

        if (name.Equals("BLACK"))
        {
            return Color.black;
        }
        if (name.Equals("BLUE"))
        {
            return new Color32(0, 114, 255, 255);
        }
        if (name.Equals("GREEN"))
        {
            return Color.green;
        }
        if (name.Equals("YELLOW"))
        {
            return Color.yellow;
        }
        if (name.Equals("RED"))
        {
            return Color.red;
        }
        if (name.Equals("PURPLE"))
        {
            new Color(226, 0, 225, 225);
        }
        return Color.clear;


    }
}

static class MyExtensions
{

    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (System.Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    
}