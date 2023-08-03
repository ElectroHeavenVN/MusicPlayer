using LitJson;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MusicPlayer
{
    internal class Data
    {
        internal static void Save()
        {
            //Utilities.saveRMSInt("indexPlay", Music.indexPlaying);
            //Utilities.saveRMSString("musicPaths", string.Join(Environment.NewLine, Playlist.musicPaths.ToArray()));
            //Utilities.saveRMSString("time", Music.currentlyPlaying.Position.Ticks.ToString());

            try
            {
                Utilities.saveRMSString("defaultPlaylist", JsonMapper.ToJson(PlayerCore.defaultPlaylist));
                Utilities.saveRMSString("favorite", JsonMapper.ToJson(PlayerCore.favorite));
                Utilities.saveRMSString("userPlaylists", JsonMapper.ToJson(PlayerCore.userPlaylists));
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        internal static void Load()
        {
            try{ PlayerCore.defaultPlaylist = JsonMapper.ToObject<Playlist>(Utilities.loadRMSString("defaultPlaylist")); }
            catch (Exception) { }
            try { PlayerCore.favorite = JsonMapper.ToObject<Playlist>(Utilities.loadRMSString("favorite")); }
            catch (Exception) { }
            try { PlayerCore.userPlaylists = JsonMapper.ToObject<List<Playlist>>(Utilities.loadRMSString("userPlaylists")); }
            catch (Exception) { }
            if (PlayerCore.musicPaths.Count == 0)
            {
                PlayerCore.musicPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                List<FileInfo> files = new List<FileInfo>();
                foreach (string path in PlayerCore.musicPaths)
                {
                    files.AddRange(new DirectoryInfo(path).GetFiles());
                }
                files.Sort((f1, f2) => -f1.LastWriteTime.Ticks.CompareTo(f2.LastWriteTime.Ticks));  //sort by date
                foreach (FileInfo item in files)
                {
                    try
                    {
                        PlayerCore.defaultPlaylist.songs.Add(new Song(item.FullName));
                    }
                    catch (Exception) { }
                }
            }
            new Thread(() =>
            {
                LoadPlaylist(PlayerCore.defaultPlaylist);
                LoadPlaylist(PlayerCore.favorite);
                foreach (Playlist userPlaylist in PlayerCore.userPlaylists)
                    LoadPlaylist(userPlaylist);
            })
            { IsBackground = true }.Start();
        }

        static void LoadPlaylist(Playlist playlist)
        {
            try
            {
                if (playlist.indexPlaying > playlist.songs.Count - 1)
                    playlist.indexPlaying = -1;
                else
                {
                    try
                    {
                        TimeSpan position = playlist.currentPosition;
                        if (position >= playlist.currentlyPlaying.Duration)
                            playlist.currentPosition = TimeSpan.Zero;
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception)
            {
                playlist.indexPlaying = -1;
            }
        }
    }
}
