using LitJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicPlayer
{
    public class Playlist
    {
        public int indexPlaying = -1;

        public string name;

        public List<Song> songs = new List<Song>();

        internal TimeSpan currentPosition = TimeSpan.Zero;

        public long currentPosTick
        {
            get => currentPosition.Ticks;
            set => currentPosition = TimeSpan.FromTicks(value);
        }

        [JsonSkip]
        internal Song currentlyPlaying => songs[indexPlaying];

        internal Playlist() { }

        internal Playlist(int index) 
        {
            indexPlaying = index;
        }

        internal Playlist(int index, string name) : this(index)
        {
            this.name = name;
        }

        internal void Next()
        {
            indexPlaying++;
            if (indexPlaying > songs.Count - 1)
                indexPlaying = 0;   //loop
            Play(indexPlaying);
        }

        internal void Previous()
        {
            indexPlaying--;
            if (indexPlaying < 0)
                indexPlaying = songs.Count - 1;   //loop
            Play(indexPlaying);
        }

        internal void Play(int index)
        {
            indexPlaying = index;
            PlayerCore.Play();
        }
    }
}
