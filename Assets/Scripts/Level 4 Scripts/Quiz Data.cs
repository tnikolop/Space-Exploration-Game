using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Level4/QuizData")]
public class QuizData : ScriptableObject
{
    [System.Serializable]
    public class Question
    {
        [TextArea(3, 5)]
        public string text;         // the Question
        public List<string> options;    // possible answers
        public int correct_index;   // correct answer index
    }

    [Header("Question Info")]
    public List<Question> questions;
}
