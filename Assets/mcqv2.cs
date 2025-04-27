using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace QuestionPicker
{
    public class mcqv2 : MonoBehaviour
    {
        public TMP_Text questionText;
        public Transform buttonContainer; // Parent object for buttons
        public Button buttonPrefab; // Button prefab to instantiate
        public TextAsset csvFile; // Drag and drop CSV file here in the Inspector

        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private List<Button> answerButtons = new List<Button>();

        public class Question
        {
            public string question;
            public string correctAnswer;
            public List<string> options;

            public Question(string question, List<string> options)
            {
                this.question = question;
                this.correctAnswer = options[0]; // First choice is the correct answer
                this.options = options.OrderBy(x => UnityEngine.Random.value).ToList(); // Randomize choices
            }
        }

        void Start()
        {
            if (csvFile != null)
            {
                questions = ReadCSVFile(csvFile);
                if (questions.Count > 0)
                    DisplayQuestion();
            }
            else
            {
                Debug.LogError("CSV file not assigned! Please drag and drop a CSV file.");
            }
        }

        public List<Question> ReadCSVFile(TextAsset file)
        {
            List<Question> questionList = new List<Question>();
            string[] lines = file.text.Split('\n');
            bool isFirstLine = true;

            foreach (string line in lines)
            {
                if (isFirstLine) { isFirstLine = false; continue; }

                string[] values = SplitCSVLine(line);

                if (values.Length >= 3) // Ensure at least one question and two answers exist
                {
                    string questionText = values[0].Trim();
                    List<string> options = values.Skip(1).Select(x => x.Trim()).ToList();

                    questionList.Add(new Question(questionText, options));
                }
            }
            return questionList;
        }

        private string[] SplitCSVLine(string line)
        {
            List<string> values = new List<string>();
            bool inQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes; // Toggle quote mode
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            values.Add(current.Trim()); // Add last value
            return values.ToArray();
        }

        void DisplayQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                questionText.text = "Quiz Complete!";
                ClearButtons();
                return;
            }

            Question q = questions[currentQuestionIndex];
            questionText.text = q.question;

            GenerateAnswerButtons(q.options);
        }

        void GenerateAnswerButtons(List<string> options)
        {
            ClearButtons(); // Remove previous buttons

            for (int i = 0; i < options.Count && i < 7; i++) // Support up to 7 buttons (A-G)
            {
                Button newButton = Instantiate(buttonPrefab, buttonContainer);
                TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                buttonText.text = options[i];

                newButton.onClick.AddListener(() => OnButtonTouch(buttonText.text, newButton));
                answerButtons.Add(newButton);
            }
        }

        void ClearButtons()
        {
            foreach (Button btn in answerButtons)
            {
                Destroy(btn.gameObject);
            }
            answerButtons.Clear();
        }

        void OnButtonTouch(string selectedOption, Button clickedButton)
        {
            Question q = questions[currentQuestionIndex];

            if (selectedOption == q.correctAnswer)
            {
                clickedButton.GetComponent<Image>().color = Color.green;
                StartCoroutine(NextQuestionAfterDelay());
            }
            else
            {
                clickedButton.GetComponent<Image>().color = Color.red;
            }
        }

        IEnumerator NextQuestionAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            currentQuestionIndex++;
            DisplayQuestion();
        }
    }
}
