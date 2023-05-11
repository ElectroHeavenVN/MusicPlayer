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
            Utilities.saveRMSInt("indexPlay", Music.indexPlaying);
            Utilities.saveRMSString("musicPaths", string.Join(Environment.NewLine, Music.musicPaths.ToArray()));
            Utilities.saveRMSString("time", Music.listSongs[Music.indexPlaying].Position.Ticks.ToString());
        }

        internal static void Load()
        {
            try
            {
                Music.musicPaths = Utilities.loadRMSString("musicPaths").Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => Directory.Exists(p)).ToList();
            }
            catch (Exception) { }
            if (Music.musicPaths.Count == 0)
                Music.musicPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            new Thread(() =>
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (string path in Music.musicPaths)
                {
                   files.AddRange(new DirectoryInfo(path).GetFiles());
                }
                files.Sort((f1, f2) => -f1.LastWriteTime.Ticks.CompareTo(f2.LastWriteTime.Ticks));  //sort by date
                foreach (FileInfo item in files)
                {
                    try
                    {
                        Music.listSongs.Add(new Music(item.FullName));
                    }
                    catch (Exception) { }
                }
                try
                {
                    Music.indexPlaying = Utilities.loadRMSInt("indexPlay");
                    if (Music.indexPlaying > Music.listSongs.Count - 1)
                        Music.indexPlaying = -1;
                    else
                    {
                        try
                        {
                            Music.listSongs[Music.indexPlaying].Init();
                            Music.listSongs[Music.indexPlaying].Position = TimeSpan.FromTicks(long.Parse(Utilities.loadRMSString("time")));
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception)
                {
                    Music.indexPlaying = -1;
                }
            })
            { IsBackground = true }.Start();
        }
    }
}
