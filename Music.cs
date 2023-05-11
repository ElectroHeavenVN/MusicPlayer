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
    internal class Music
    {
        static WaveStream musicStream;

        static WaveOutEvent output = new WaveOutEvent();

        internal string filePath;

        List<Image> albumArtworks = new List<Image>();

        static List<Image> defaultAlbumArtworks = new List<Image>();

        static byte[] defaultAlbumArtworkImage;

        internal static List<Music> listSongs = new List<Music>();

        internal static int indexPlaying = -1;

        internal static List<string> musicPaths = new List<string>();

        static bool m_lockEvents;

        internal TagLib.File file;

        internal Music(string path)
        {
            if (Path.GetExtension(path).ToLower() != ".wav" && Path.GetExtension(path).ToLower() != ".mp3" && Path.GetExtension(path).ToLower() != ".aiff" && Path.GetExtension(path).ToLower() != ".aif")
                throw new Exception("Invalid file type: " + Path.GetExtension(path));
            filePath = path;
            file = TagLib.File.Create(filePath);
        }

        static Music()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicPlayer.Resources.Default_album_artwork.png");
            defaultAlbumArtworkImage = new byte[stream.Length];
            stream.Read(defaultAlbumArtworkImage, 0, (int)stream.Length);
            output.PlaybackStopped += Output_PlaybackStopped;
        }

        private static void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (!m_lockEvents)
            {
                ++indexPlaying;
                if (indexPlaying > listSongs.Count - 1)
                    indexPlaying = 0;   //loop
                Play(indexPlaying);
            }
        }

        internal static void Play(int index)
        {
            indexPlaying = index;
            listSongs[index].Play();
        }

        void Play()
        {
            if (output.PlaybackState != PlaybackState.Stopped)
            {
                m_lockEvents = true;
                output.Stop();
            }
            Init();
            output.Play();
            m_lockEvents = false;
        }

        internal static void StopBeforeExit()
        {
            m_lockEvents = true;
            output.Stop();
        }

        internal void Init()
        {
            musicStream = new AudioFileReader(filePath);
            output.Init(musicStream);
        }

        internal static void PauseOrPlay()
        {
            if (output.PlaybackState == PlaybackState.Playing)
                output.Pause();
            else
                output.Play();
        }

        internal static void Next()
        {
            indexPlaying++;
            if (indexPlaying > listSongs.Count - 1)
                indexPlaying = 0;   //loop
            Play(indexPlaying);
        }

        internal static void Previous()
        {
            indexPlaying--;
            if (indexPlaying < 0)
                indexPlaying = listSongs.Count - 1;   //loop
            Play(indexPlaying);
        }

        internal static Music currentlyPlaying => listSongs[indexPlaying];

        internal static PlaybackState state => output.PlaybackState;

        internal void SetVolume(float volume) => output.Volume = volume;

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

        internal TimeSpan Position
        { 
            get => musicStream.CurrentTime;
            set => musicStream.CurrentTime = value;
        }

        internal TimeSpan Length => file.Properties.Duration;
    }
}
