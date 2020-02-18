using System.Collections.Generic;

namespace Spotty
{
    public interface ISpottyState
    {
        (string track, int offset, int duration) CurrentTrack();

        (string track, int offset, int duration) NextTrack();
    }

    public class SpottyState : ISpottyState
    {
        private (string track, int offset, int duration)[] TrackList { get; }
        private int CurrentTrackNumber = -1;

        public SpottyState()
        {
            var trackList = new List<(string track, int offset, int duration)>
            {
                ("spotify:track:3SGP8It5WDnCONyApJKRTJ", 25000, 5000),
                ("spotify:track:40bvnVS14b2gF3ZfiTKPAf", 0, 5000),
                ("spotify:track:6TVfRcgeZJFZ6563OhFVE2", 80000, 5000)
            };

            TrackList = trackList.ToArray();
        }

        public (string track, int offset, int duration) CurrentTrack()
        {
            return TrackList[CurrentTrackNumber];
        }

        public (string track, int offset, int duration) NextTrack()
        {
            CurrentTrackNumber++;

            return TrackList[CurrentTrackNumber];
        }
    }
}
