using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicPlayer
{
    internal class PlayerCore
    {
        internal static Playlist defaultPlaylist = new Playlist(-1, "default"); //2DO: DefaultPlaylist không cần lưu đường dẫn file
        internal static Playlist favorite = new Playlist(-1, "favorite");
        internal static List<Playlist> userPlaylists = new List<Playlist>();
        internal static List<string> musicPaths = new List<string>();
        internal static Playlist CurrentPlaylist
        {
            get
            {
                if (m_currentPlaylist == "default")
                    return defaultPlaylist;
                else if (m_currentPlaylist == "favorite")
                    return favorite;
                else if (m_currentPlaylist.StartsWith("user_playlist_"))
                    return userPlaylists[int.Parse(m_currentPlaylist.TrimStart("user_playlist_".ToCharArray()))];
                else
                    return null;
            }
            set
            {
                m_currentPlaylist = value.name;
            }
        }
        internal static WaveStream musicStream;

        static WaveOutEvent output = new WaveOutEvent();
        static bool m_lockEvents;
        static string m_currentPlaylist = "default";

        internal static TimeSpan Position
        {
            get => musicStream.CurrentTime;
            set => musicStream.CurrentTime = value;
        }

        internal static PlaybackState state => output.PlaybackState;

        internal static void SetVolume(float volume) => output.Volume = volume;

        static PlayerCore()
        {
            output.PlaybackStopped += Output_PlaybackStopped;
        }

        private static void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (!m_lockEvents)
                CurrentPlaylist.Next();
        }

        internal static void Play()
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

        internal static void Init()
        {
            musicStream = new AudioFileReader(CurrentPlaylist.currentlyPlaying.filePath);
            output.Init(musicStream);
        }

        internal static void StopBeforeExit()
        {
            m_lockEvents = true;
            output.Stop();
        }

        internal static void PauseOrPlay()
        {
            if (output.PlaybackState == PlaybackState.Playing)
                output.Pause();
            else
                output.Play();
        }
    }
}
