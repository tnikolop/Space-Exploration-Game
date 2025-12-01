using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
public class Level4_Manager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform answersContainer;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Screens")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color32(35, 62, 139, 255);
    [SerializeField] private Color correctColor = new Color32(78, 204, 163, 255);
    [SerializeField] private Color wrongColor = new Color32(233, 69, 96, 255);
    
    [Header("Game Settings")]
    [SerializeField] private int numberOfQuestions = 5;  


    private List<QuizData.Question> _questions;     // the list we store the questions
    private int _current_Q_index = 0;
    private int _score = 0;
    private bool _can_answear = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Show_Start_Screen();
        Load_JSON(numberOfQuestions);
        Start_Game();
    }

    // Loads x random questions from the JSON file
    private void Load_JSON(int x)
    {
        // Load from ../Resources
        TextAsset json = Resources.Load<TextAsset>("Questions_data");

        if (json == null)
        {
            Debug.LogError("JSON file for questions not found!");
            return;
        }
        // make text into C# object
        QuizData.QuestionWrapper data = JsonUtility.FromJson<QuizData.QuestionWrapper>(json.text);
        // store the list
        List<QuizData.Question> allQuestions = new List<QuizData.Question>(data.questions);

        // shuffle the list (Fisher-Yates Shuffle Algorithm)
        for (int i = 0; i < allQuestions.Count; i++)
        {
            QuizData.Question temp = allQuestions[i];
            int k = Random.Range(i, allQuestions.Count);
            allQuestions[i] = allQuestions[k];
            allQuestions[k] = temp;
        }

        // Select x first questions
        // check which is smaller, x or the total number of questions
        // so the game wont crash
        int min = Mathf.Min(x, allQuestions.Count);
        _questions = allQuestions.GetRange(0, min);
    }

    public void Show_Start_Screen()
    {
        startPanel.SetActive(true);
        endPanel.SetActive(false);
    }

    public void Start_Game()
    {
        if (_questions == null || _questions.Count == 0)
        {
            Debug.LogError("Questions list is null or empty!");
            return;
        }

        // startPanel.SetActive(true);
        endPanel.SetActive(false);
        _score = 0;
        _current_Q_index = 0;
        Update_Score_UI();
        Load_Question();
    }

    // Loads the next question
    // also checks if the game is finished
    private void Load_Question()
    {
        if (_current_Q_index >= _questions.Count)
        {
            Game_Over();
            return;
        }
        _can_answear = true;
        QuizData.Question q = _questions[_current_Q_index];
        questionText.text = q.text;

        // Remove previous question buttons
        foreach (Transform child in answersContainer)
        {
            Destroy(child.gameObject);
        }
        // Create the new ones
        for (int i = 0; i < q.options.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, answersContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
            button.GetComponent<Image>().color = normalColor;
            // add Listener
            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => On_Answer_Selected(index, button));
        }

    }
    // Coroutine so the program will wait 2 seconds before moving on
    // and loads the next question
    IEnumerator Next_Question_Delay()
    {
        yield return new WaitForSeconds(2f);
        _current_Q_index++;
        Load_Question();
    }

    // Answer button listener
    // Checks if the button that was clicked(itself) was the correct answer or not
    // and acts accordingly
    private void On_Answer_Selected(int index, GameObject button)
    {
        if (_can_answear == false)
            return;
        else
            _can_answear = false;

        QuizData.Question q = _questions[_current_Q_index];
        Image img = button.GetComponent<Image>();

        if (index == q.correctIndex)
        {
            // correct answer
            Global_Audio_Manager.Instance.Play_SFX_correct();
            img.color = correctColor;
            _score += 10;
        }
        else
        {
            // wrong answer
            Global_Audio_Manager.Instance.Play_Error_SFX();
            img.color = wrongColor;
            // show correct answer
            Transform correct_btn = answersContainer.GetChild(q.correctIndex);
            correct_btn.GetComponent<Image>().color = correctColor;
        }
        Update_Score_UI();
        StartCoroutine(Next_Question_Delay());

    }
 
    // Updates the UI text for the score
    private void Update_Score_UI()
    {
        scoreText.text = "SCORE: " + _score;
    }

    private void Game_Over()
    {
        Global_Audio_Manager.Instance.Play_Win_SFX();
        endPanel.SetActive(true);
        // finalScoreText.text = _score.ToString();
    }
}
