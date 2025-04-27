using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace QuestionPicker
{
    public class MCQv1 : MonoBehaviour
    {
        public TMP_Text questionText;
        public Button optionAButton, optionBButton, optionCButton, optionDButton, optionEButton, optionFButton;
        public TMP_Text optionAText, optionBText, optionCText, optionDText, optionEText, optionFText;
        public TextAsset csvFile; // Drag and drop CSV file here in the Inspector
        private List<Question> questions;
        private int currentQuestionIndex = 0;

        private Dictionary<string, Button> buttonDictionary;
        private Dictionary<Button, string> buttonToOptionDictionary;

        public class Question
        {
            public string question;
            public List<string> shuffledOptions;
            public string correctAnswer;

            public Question(string question, List<string> options, string correctAnswer)
            {
                this.question = question;
                this.shuffledOptions = options;
                this.correctAnswer = correctAnswer;
            }
        }

        void Start()
        {
            buttonDictionary = new Dictionary<string, Button>
            {
                { "A", optionAButton },
                { "B", optionBButton },
                { "C", optionCButton },
                { "D", optionDButton },
                { "E", optionEButton },
                { "F", optionFButton }
            };

            if (csvFile != null)
            {
                questions = ReadCSVFile(csvFile);
                if (questions.Count > 0)
                    DisplayQuestion();

                optionAButton.onClick.AddListener(() => OnButtonTouch(optionAButton));
                optionBButton.onClick.AddListener(() => OnButtonTouch(optionBButton));
                optionCButton.onClick.AddListener(() => OnButtonTouch(optionCButton));
                optionDButton.onClick.AddListener(() => OnButtonTouch(optionDButton));
                optionEButton.onClick.AddListener(() => OnButtonTouch(optionEButton));
                optionFButton.onClick.AddListener(() => OnButtonTouch(optionFButton));
            }
            else
            {
                Debug.LogError("CSV file not assigned! Please drag and drop a CSV file into the MCQv1 component.");
            }
        }

        public List<Question> ReadCSVFile(TextAsset file)
        {
            List<Question> questionList = new List<Question>();
            string[] lines = file.text.Split('\n');
            bool isFirstLine = true;

            foreach (string line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] values = line.Split(',');

                if (values.Length >= 7) // Ensure at least 7 columns exist (1 question + 6 answers)
                {
                    string question = values[0].Trim();
                    string correctAnswer = values[1].Trim(); // The second column is always the correct answer

                    List<string> options = new List<string>
                    {
                        values[1].Trim(),
                        values[2].Trim(),
                        values[3].Trim(),
                        values[4].Trim(),
                        values[5].Trim(),
                        values[6].Trim()
                    };

                    // Shuffle answer choices
                    ShuffleList(options);

                    questionList.Add(new Question(question, options, correctAnswer));
                }
            }
            return questionList;
        }

        void DisplayQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                questionText.text = "Quiz Completed!";
                HideAllButtons();
                return;
            }

            Question q = questions[currentQuestionIndex];
            questionText.text = q.question;

            // Assign shuffled answers to buttons and track correct answer
            buttonToOptionDictionary = new Dictionary<Button, string>();

            List<TMP_Text> answerTexts = new List<TMP_Text> { optionAText, optionBText, optionCText, optionDText, optionEText, optionFText };
            List<Button> answerButtons = new List<Button> { optionAButton, optionBButton, optionCButton, optionDButton, optionEButton, optionFButton };

            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].text = q.shuffledOptions[i];
                buttonToOptionDictionary[answerButtons[i]] = q.shuffledOptions[i];
            }

            ResetButtonColors();
            EnableAllButtons();
        }

        void OnButtonTouch(Button clickedButton)
        {
            Question q = questions[currentQuestionIndex];
            string selectedAnswer = buttonToOptionDictionary[clickedButton];

            if (selectedAnswer == q.correctAnswer)
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
            yield return new WaitForSeconds(2);
            currentQuestionIndex++;
            DisplayQuestion();
        }

        void ResetButtonColors()
        {
            Color defaultColor = Color.white;
            optionAButton.GetComponent<Image>().color = defaultColor;
            optionBButton.GetComponent<Image>().color = defaultColor;
            optionCButton.GetComponent<Image>().color = defaultColor;
            optionDButton.GetComponent<Image>().color = defaultColor;
            optionEButton.GetComponent<Image>().color = defaultColor;
            optionFButton.GetComponent<Image>().color = defaultColor;
        }

        void EnableAllButtons()
        {
            optionAButton.gameObject.SetActive(true);
            optionBButton.gameObject.SetActive(true);
            optionCButton.gameObject.SetActive(true);
            optionDButton.gameObject.SetActive(true);
            optionEButton.gameObject.SetActive(true);
            optionFButton.gameObject.SetActive(true);
        }

        void HideAllButtons()
        {
            optionAButton.gameObject.SetActive(false);
            optionBButton.gameObject.SetActive(false);
            optionCButton.gameObject.SetActive(false);
            optionDButton.gameObject.SetActive(false);
            optionEButton.gameObject.SetActive(false);
            optionFButton.gameObject.SetActive(false);
        }

        void ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}