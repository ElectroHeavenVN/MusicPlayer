using Mod;
using Mod.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MusicPlayer
{
    internal static class Utils
    {
        //static float avgR = 0;
        //static float avgG = 0;
        //static float avgB = 0;
        //static float avgA = 0;
        //static float blurPixelCount = 0;

        //internal static Texture2D CropToSquare(Texture2D textureToCrop, int size)
        //{
        //    int minSide = Math.min(textureToCrop.width, textureToCrop.height);
        //    Texture2D texture2D = new Texture2D(minSide, minSide);
        //    for (int i = 0; i < minSide * minSide; i++)
        //    {
        //        int offset = Math.abs(textureToCrop.width - textureToCrop.height) / 2;
        //        int x = i % minSide;
        //        int y = i / minSide;
        //        if (textureToCrop.width > textureToCrop.height)
        //            texture2D.SetPixel(x, y, textureToCrop.GetPixel(x + offset, y));
        //        else if (textureToCrop.width < textureToCrop.height)
        //            texture2D.SetPixel(x, y, textureToCrop.GetPixel(x, y + offset));
        //        else
        //            texture2D.SetPixel(x, y, textureToCrop.GetPixel(x, y));
        //    }
        //    return TextureScaler.ScaleTexture(texture2D, size, size);
        //}

        //internal static Texture2D CropFromSquareToCircle(Texture2D textureToCrop, int borderThickness = 0, bool isApply = true)
        //{
        //    if (textureToCrop.width != textureToCrop.height)
        //        throw new ArgumentException("textureToCrop isn't a square texture!");
        //    int centerX = textureToCrop.width / 2;
        //    int centerY = textureToCrop.height / 2;
        //    for (int i = 0; i < textureToCrop.width * textureToCrop.height; i++)
        //    {
        //        int x = i % textureToCrop.width;
        //        int y = i / textureToCrop.height;   
        //        double distance = getDistance(centerX, centerY, x, y);
        //        if (distance >= textureToCrop.width / 2d - 1)
        //            textureToCrop.SetPixel(x, y, Color.clear);
        //        else if (distance >= textureToCrop.width / 2d - 1 - borderThickness)
        //            textureToCrop.SetPixel(x, y, new Color(.5f, .5f, .5f));
        //    }
        //    if (isApply)
        //        textureToCrop.Apply();
        //    return textureToCrop;
        //}

        //public static Texture2D FastBlur(Texture2D tex, int radius, int iterations)
        //{
        //    for (var i = 0; i < iterations; i++)
        //    {
        //        tex = BlurImage(tex, radius, true);
        //        tex = BlurImage(tex, radius, false);
        //    }
        //    return tex;
        //}

        //public static Texture2D BlurImage(Texture2D image, int blurSize, bool horizontal)
        //{
        //    avgR = 0;
        //    avgG = 0;
        //    avgB = 0;
        //    //avgA = 0;
        //    blurPixelCount = 0;
        //    Texture2D blurred = new Texture2D(image.width, image.height);
        //    int _W = image.width;
        //    int _H = image.height;
        //    int xx, yy, x, y;

        //    if (horizontal)
        //    {
        //        for (yy = 0; yy < _H; yy++)
        //        {
        //            for (xx = 0; xx < _W; xx++)
        //            {
        //                ResetPixel();
        //                //Right side of pixel
        //                for (x = xx; (x < xx + blurSize && x < _W); x++)
        //                    AddPixel(image.GetPixel(x, yy));
        //                //Left side of pixel
        //                for (x = xx; (x > xx - blurSize && x > 0); x--)
        //                    AddPixel(image.GetPixel(x, yy));
        //                CalcPixel();
        //                for (x = xx; x < xx + blurSize && x < _W; x++)
        //                    blurred.SetPixel(x, yy, new Color(avgR, avgG, avgB, image.GetPixel(x, yy).a));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (xx = 0; xx < _W; xx++)
        //        {
        //            for (yy = 0; yy < _H; yy++)
        //            {
        //                ResetPixel();
        //                //Over pixel
        //                for (y = yy; (y < yy + blurSize && y < _H); y++)
        //                    AddPixel(image.GetPixel(xx, y));
        //                //Under pixel
        //                for (y = yy; (y > yy - blurSize && y > 0); y--)
        //                    AddPixel(image.GetPixel(xx, y));
        //                CalcPixel();
        //                for (y = yy; y < yy + blurSize && y < _H; y++)
        //                    blurred.SetPixel(xx, y, new Color(avgR, avgG, avgB, image.GetPixel(xx, y).a));
        //            }
        //        }
        //    }
        //    //blurred.Apply();
        //    return blurred;
        //}

        //static void AddPixel(Color pixel)
        //{
        //    avgR += pixel.r;
        //    avgG += pixel.g;
        //    avgB += pixel.b;
        //    blurPixelCount++;
        //}

        //static void ResetPixel()
        //{
        //    avgR = 0.0f;
        //    avgG = 0.0f;
        //    avgB = 0.0f;
        //    blurPixelCount = 0;
        //}

        //static void CalcPixel()
        //{
        //    avgR = avgR / blurPixelCount;
        //    avgG = avgG / blurPixelCount;
        //    avgB = avgB / blurPixelCount;
        //}

        //static double getDistance(int x1, int y1, int x2, int y2)
        //{
        //    return System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        //}

        //internal static Texture2D RoundCorner(Texture2D texture, int radius)
        //{
        //    int xLeft = radius;
        //    int xRight = texture.width - radius;
        //    int yUpper = radius;
        //    int yLower = texture.height - radius;
        //    Color transparent = new Color(0, 0, 0, 0);
        //    for (int x = 0; x < xLeft; x++)
        //    {
        //        for (int y = 0; y < yUpper; y++)
        //        {
        //            double distance = getDistance(xLeft, yUpper, x, y);
        //            if (distance > radius)
        //                texture.SetPixel(x, y, transparent);
        //        }
        //        for (int y = yLower; y < texture.height; y++)
        //        {
        //            double distance = getDistance(xLeft, yLower, x, y);
        //            if (distance > radius)
        //                texture.SetPixel(x, y, transparent);
        //        }
        //    } 
        //    for (int x = xRight; x < texture.width; x++)
        //    {
        //        for (int y = 0; y < yUpper; y++)
        //        {
        //            double distance = getDistance(xRight, yUpper, x, y);
        //            if (distance > radius)
        //                texture.SetPixel(x, y, transparent);
        //        }
        //        for (int y = yLower; y < texture.height; y++)
        //        {
        //            double distance = getDistance(xRight, yLower, x, y);
        //            if (distance > radius)
        //                texture.SetPixel(x, y, transparent);
        //        }
        //    }

        //    return texture;
        //}

        //internal static string TrimUntilFit(string str, GUIStyle style, int width)
        //{
        //    int originalWidth = (int)(Utilities.getWidth(style, str) / 1.025f);
        //    if (originalWidth > width)
        //    {
        //        while (Utilities.getWidth(style, str + "...") > width)
        //            str = str.Remove(str.Length - 1, 1);
        //        str = str.Trim() + "...";
        //    }
        //    return str;
        //}

        internal static string toString(this TimeSpan timeSpan)
        {
            string result = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            if ((int)timeSpan.TotalHours > 0)
                result = $"{(int)timeSpan.TotalHours:00}:" + result;
            return result;
        }

        internal static void InitializeImage(Image image, string resourceName, int width, int height)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicPlayer.Resources." + resourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            image.texture.LoadImage(data);
            image.texture = TextureScaler.ScaleTexture(image.texture, width, height);
            image.texture.anisoLevel = 0;
            image.texture.filterMode = FilterMode.Point;
            image.texture.mipMapBias = 0f;
            image.texture.wrapMode = TextureWrapMode.Clamp;
            image.texture.Apply();
            image.w = image.texture.width;
            image.h = image.texture.height;
        }
    }
}
