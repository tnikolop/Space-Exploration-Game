public class QuizData
{
    [System.Serializable]
    public class Question
    {
        public string text;         // the Question
        public string[] options;    // possible answers
        public int correctIndex;    // correct answer index
    }
    [System.Serializable]
    public class QuestionWrapper
    {
        // this is needed for jsonUtility of Unity
        public Question[] questions;
    }
}
