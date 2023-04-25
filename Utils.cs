using Mod.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MusicPlayer
{
    internal static class Utils
    {
        internal static Texture2D CropToSquare(Texture2D textureToCrop, int size)
        {
            int minSide = Math.min(textureToCrop.width, textureToCrop.height);
            Texture2D texture2D = new Texture2D(minSide, minSide);
            for (int i = 0; i < minSide * minSide; i++)
            {
                int offset = Math.abs(textureToCrop.width - textureToCrop.height) / 2;
                int x = i % minSide;
                int y = i / minSide;
                if (textureToCrop.width > textureToCrop.height)
                    texture2D.SetPixel(x, y, textureToCrop.GetPixel(x + offset, y));
                else if (textureToCrop.width < textureToCrop.height)
                    texture2D.SetPixel(x, y, textureToCrop.GetPixel(x, y + offset));
                else
                    texture2D.SetPixel(x, y, textureToCrop.GetPixel(x, y));
            }
            return TextureScaler.ScaleTexture(texture2D, size, size);
        }

        internal static Texture2D CropFromSquareToCircle(Texture2D textureToCrop, bool isApply = true)
        {
            if (textureToCrop.width != textureToCrop.height)
                throw new ArgumentException("textureToCrop isn't a square texture!");
            int centerX = textureToCrop.width / 2;
            int centerY = textureToCrop.height / 2;
            for (int i = 0; i < textureToCrop.width * textureToCrop.height; i++)
            {
                int x = i % textureToCrop.width;
                int y = i / textureToCrop.height;
                if (Res.distance(centerX, centerY, x, y) > textureToCrop.width / 2 - 1)
                    textureToCrop.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
            if (isApply)
                textureToCrop.Apply();
            return textureToCrop;
        }
    }
}
