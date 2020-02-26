using System.Collections.Generic;

namespace Spotty
{
    public interface ISpottyState
    {
        SpottyQuiz[] GetQuizzes();
    }

    public class SpottyState : ISpottyState
    {
        private SpottyQuiz[] Quizzes { get; }

        public SpottyState(SpottyQuiz[] quizzes)
        {
            Quizzes = quizzes;
        }

        public SpottyQuiz[] GetQuizzes()
        {
            return Quizzes;
        }
    }

    public class SpottyQuiz
    {
        public string Title { get; set; }

        public List<SpottyQuestion> Questions { get; set; }
    }

    public class SpottyQuestion
    {
        public string Question { get; set; }

        public string Answer { get; set; }

        public List<SpottyTrack> Tracks { get; set; }
    }

    public class SpottyTrack
    {
        public string SpotifyUrl { get; set; }

        public int Offset { get; set; }

        public int Duration { get; set; }
    }
}
