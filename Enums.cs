using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicPlayer
{
    internal enum SortMode
    {
        Undefined = 0,
        ByName = 1,
        ByArtists = 2,
        ByAlbum = 4,
        ByTime = 8,
        ByDate = 0x10,

        Up = 0x00000000,
        Down = 0x40000000,
    }

    internal enum PlayMode
    {
        LoopASong,
        LoopPlaylist,
        Shuffle
    }
}
