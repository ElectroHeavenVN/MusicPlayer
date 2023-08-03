using LitJSON;
using Mod.Graphics;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TagLib;
using UnityEngine;

namespace MusicPlayer
{
    public class Song
    {
        public string filePath;

        [JsonSkip]
        List<Image> albumArtworks = new List<Image>();

        [JsonSkip]
        static List<Image> defaultAlbumArtworks = new List<Image>();

        [JsonSkip]
        static byte[] defaultAlbumArtworkImage;

        [JsonSkip]
        internal TagLib.File file;

        [JsonSkip]
        internal TimeSpan Duration => file.Properties.Duration;

        internal Song()
        {
            if (Path.GetExtension(filePath).ToLower() != ".wav" && Path.GetExtension(filePath).ToLower() != ".mp3" && Path.GetExtension(filePath).ToLower() != ".aiff" && Path.GetExtension(filePath).ToLower() != ".aif")
                throw new Exception("Invalid file type: " + Path.GetExtension(filePath));
            file = TagLib.File.Create(filePath);
        }

        internal Song(string path)
        {
            if (Path.GetExtension(path).ToLower() != ".wav" && Path.GetExtension(path).ToLower() != ".mp3" && Path.GetExtension(path).ToLower() != ".aiff" && Path.GetExtension(path).ToLower() != ".aif")
                throw new Exception("Invalid file type: " + Path.GetExtension(path));
            filePath = path;
            file = TagLib.File.Create(filePath);
        }

        static Song()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicPlayer.Resources.Default_album_artwork.png");
            defaultAlbumArtworkImage = new byte[stream.Length];
            stream.Read(defaultAlbumArtworkImage, 0, (int)stream.Length);
        }

        internal Image getAlbumArtwork(int size = 150/*, bool isCropToCircle = true*/, bool roundCorner = true)
        {
            size *= mGraphics.zoomLevel;
            if (file.Tag.Pictures.Length == 0)
                foreach (Image albumArtwork in defaultAlbumArtworks)
                {
                    if (albumArtwork.w == size && albumArtwork.h == size)
                        return albumArtwork;
                }
            else
                foreach (Image albumArtwork in albumArtworks)
                {
                    if (albumArtwork.w == size && albumArtwork.h == size)
                        return albumArtwork;
                }
            Image image = new Image();
            if (file.Tag.Pictures.Length == 0)
                image.texture.LoadImage(defaultAlbumArtworkImage);
            else
                image.texture.LoadImage(file.Tag.Pictures[0].Data.Data);
            image.texture = CustomGraphics.CropToSquare(image.texture, size);
            //if (isCropToCircle)
            //    image.texture = Utils.CropFromSquareToCircle(image.texture, mGraphics.zoomLevel);
            if (roundCorner)
                image.texture = CustomGraphics.RoundCorner(image.texture, 10 * mGraphics.zoomLevel);
            image.texture.anisoLevel = 0;
            image.texture.filterMode = FilterMode.Point;
            image.texture.mipMapBias = 0f;
            image.texture.wrapMode = TextureWrapMode.Clamp;
            image.texture.Apply();
            image.w = size;
            image.h = size;
            if (file.Tag.Pictures.Length == 0)
                defaultAlbumArtworks.Add(image);
            else
                albumArtworks.Add(image);
            return image;
        }
    }
}
