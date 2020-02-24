using Spotty.Exceptions;
using System.Collections.Generic;

namespace Spotty
{
    public interface ISpottyState
    {
        (string theme, string track, int offset, int duration)[] GetTrackList();

        (string theme, string track, int offset, int duration) GetTrack(int trackNumber);

        int GetCurrentTrackNumber();

        void SetCurrentTrackNumber(int trackNumber);
    }

    public class SpottyState : ISpottyState
    {
        private (string theme, string track, int offset, int duration)[] TrackList { get; }
        private int CurrentTrackNumber = 0;

        public SpottyState()
        {
            var trackList = new List<(string theme, string track, int offset, int duration)>
            {
                ("Classics","spotify:track:3SGP8It5WDnCONyApJKRTJ", 25000, 5000), //queen - who wants to live forever
                ("Classics","spotify:track:40bvnVS14b2gF3ZfiTKPAf", 0, 5000), //the who - my generation
                ("Classics","spotify:track:6TVfRcgeZJFZ6563OhFVE2", 80000, 5000), //bruce springsteen - streets of philadelphia,
                ("Nog een keer!","spotify:track:3SGP8It5WDnCONyApJKRTJ", 25000, 5000), //queen - who wants to live forever
                ("Nog een keer!","spotify:track:40bvnVS14b2gF3ZfiTKPAf", 0, 5000), //the who - my generation
                ("Nog een keer!","spotify:track:6TVfRcgeZJFZ6563OhFVE2", 80000, 5000) //bruce springsteen - streets of philadelphia
            };

            TrackList = trackList.ToArray();
        }

        public int GetCurrentTrackNumber()
        {
            return CurrentTrackNumber;
        }

        public (string theme, string track, int offset, int duration)[] GetTrackList()
        {
            return TrackList;
        }

        public void SetCurrentTrackNumber(int trackNumber)
        {
            if(trackNumber < 0 || trackNumber > TrackList.Length)
            {
                throw new SpottyException("Invalid tracknumber");
            }

            CurrentTrackNumber = trackNumber;
        }

        public (string theme, string track, int offset, int duration) GetTrack(int trackNumber)
        {
            if (trackNumber <= 0 || trackNumber > TrackList.Length)
            {
                throw new SpottyException("Invalid tracknumber");
            }

            return TrackList[trackNumber - 1];
        }
    }
}
