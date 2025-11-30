using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;
public class Level4_Manager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform answersContainer; // Εδώ θα μπαίνουν τα κουμπιά
    [SerializeField] private GameObject buttonPrefab;    // Το πρότυπο κουμπιού που φτιάξαμε

    [Header("Screens")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color32(35, 62, 139, 255); // Σκούρο Μπλε
    [SerializeField] private Color correctColor = new Color32(78, 204, 163, 255); // Πράσινο Neon
    [SerializeField] private Color wrongColor = new Color32(233, 69, 96, 255);   // Κόκκινο Neon

    [Header("Data File")]
    [SerializeField] private QuizData _levelData;

    private List<QuizData.Question> _questions;
    private int _current_Q_index = 0;
    private int _score = 0;
    private bool _can_answear = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Show_Start_Screen();
        Start_Game();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show_Start_Screen()
    {
        startPanel.SetActive(true);
        endPanel.SetActive(false);
    }

    public void Start_Game()
    {
        if (_levelData == null)
        {
            Debug.LogError("Level Data not loaded!");
            return;
        }
        // load questions
        _questions = _levelData.questions;

        startPanel.SetActive(true);
        endPanel.SetActive(false);
        _score = 0;
        _current_Q_index = 0;
        Update_Score_UI();
        Load_Question();
    }
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
        for (int i = 0; i < q.options.Count; i++)
        {
            GameObject button = Instantiate(buttonPrefab, answersContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
            button.GetComponent<Image>().color = normalColor;
            // add Listener
            button.GetComponent<Button>().onClick.AddListener(() => On_Answer_Selected(i, button));
        }

    }
    IEnumerator Next_Question_Delay()
    {
        yield return new WaitForSeconds(2f);
        _current_Q_index++;
        Load_Question();
    }
    private void On_Answer_Selected(int index, GameObject button)
    {
        if (_can_answear == false)
            return;
        else
            _can_answear = false;

        QuizData.Question q = _questions[_current_Q_index];
        Image img = button.GetComponent<Image>();

        if (index == q.correct_index)
        {
            // correct answer
            img.color = correctColor;
            _score += 10;
        }
        else
        {
            // wrong answer
            img.color = wrongColor;
            // show correct answer
            Transform correct_btn = answersContainer.GetChild(q.correct_index);
            correct_btn.GetComponent<Image>().color = correctColor;
        }
        Update_Score_UI();
        StartCoroutine(Next_Question_Delay());

    }
    private void Update_Score_UI()
    {
        scoreText.text = "SCORE: " + _score;
    }

    private void Game_Over()
    {
        endPanel.SetActive(true);
        finalScoreText.text = _score.ToString();
    }
}
