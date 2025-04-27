using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace QuestionPicker
{
    public class V4MCQ : MonoBehaviour

    {
        public TMP_Text questionText;
        public Button optionAButton, optionBButton, optionCButton, optionDButton, optionEButton, optionFButton;
        public TMP_Text optionAText, optionBText, optionCText, optionDText, optionEText, optionFText;
        public TextAsset csvFile;
        public Animator brainAnimator;
        private List<Question> questions;
        public float totalTime = 10f;
        private float timeRemaining;
        private bool timerRunning = false;
        public TextMeshProUGUI timerText;
        public Slider timerSlider;


        private Dictionary<Button, string> buttonToOptionDictionary;

        public class Question
        {
            public string question;
            public List<string> shuffledOptions;
            public string correctAnswer;
            public string animationName;
            public string structureName;
            public string info;
            public string coord; // ✅ Added coord field to store coordinate data

            public string anatomy;
            public string function;
            public string pathology;

            public Question(string question, List<string> options, string correctAnswer, string animationName, string anatomy, string function, string pathology, string coord)
            {
                this.question = question;
                this.shuffledOptions = options;
                this.correctAnswer = correctAnswer;
                this.animationName = animationName;
                this.structureName = ExtractStructureName(question);
                this.anatomy = anatomy;
                this.function = function;
                this.pathology = pathology;
                this.coord = coord;
            }

            private string ExtractStructureName(string sentence)
            {
                string marker = "of the ";
                int markerIndex = sentence.ToLower().IndexOf(marker);

                if (markerIndex != -1)
                {
                    string extractedName = sentence.Substring(markerIndex + marker.Length).Trim();
                    return extractedName.TrimEnd('?'); // Remove trailing question marks
                }

                return "Unknown"; // Fallback if "of the" isn't found
            }

        }

        void Start()
        {
            SceneSwitcher.timerModeEnabled = true;
            //if (timerText != null)
            //{
             //   timerText.gameObject.SetActive(true);
              //  timerText.text = "Time: —"; // Optional: placeholder until countdown starts
            //}
            if (SceneSwitcher.timerModeEnabled)
            {
                timeRemaining = totalTime;
                timerRunning = true;
            }
            if (SceneSwitcher.timerModeEnabled)
            {
                timeRemaining = totalTime;
                timerRunning = true;

                if (timerSlider != null)
                {
                    timerSlider.value = 0f;
                }
            }
            if (csvFile != null)
            {
                questions = ReadCSVFile(csvFile);
                if (questions.Count > 0)
                {
                    DisplayQuestion(SceneSwitcher.currentQuestionIndex);
                }

                optionAButton.onClick.AddListener(() => OnButtonTouch(optionAButton));
                optionBButton.onClick.AddListener(() => OnButtonTouch(optionBButton));
                optionCButton.onClick.AddListener(() => OnButtonTouch(optionCButton));
                optionDButton.onClick.AddListener(() => OnButtonTouch(optionDButton));
                optionEButton.onClick.AddListener(() => OnButtonTouch(optionEButton));
                optionFButton.onClick.AddListener(() => OnButtonTouch(optionFButton));
            }
            else
            {
                Debug.LogError("CSV file not assigned! Please drag and drop a CSV file into the V4MCQ component.");
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

                // ✅ Correctly split fields, preserving commas inside quotes
                string[] values = ParseCSVLine(line);

                if (values.Length >= 10) // Ensure "Coord" column exists
                {
                    string question = values[0].Trim();
                    string correctAnswer = values[1].Trim(); // still using A as correct?
                    string animationName = values[7].Trim(); // "Image" column
                    string anatomy = values[8].Trim();
                    string function = values[9].Trim();
                    string pathology = values[10].Trim();
                    string coord = values[11].Trim();

                    List<string> options = new List<string>
            {
                values[1].Trim(),
                values[2].Trim(),
                values[3].Trim(),
                values[4].Trim(),
                values[5].Trim(),
                values[6].Trim()
            };

                    ShuffleList(options);
                    questionList.Add(new Question(question, options, correctAnswer, animationName, anatomy, function, pathology, coord));

                }
            }
            return questionList;
        }

        // ✅ New Function: Correctly Splits CSV While Keeping Quotes
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            foreach (char c in line)
            {
                if (c == '\"') // Toggle inQuotes state if a double quote is encountered
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes) // If comma outside quotes, split field
                {
                    result.Add(currentField.Trim());
                    currentField = "";
                }
                else // Otherwise, add character to current field
                {
                    currentField += c;
                }
            }
            result.Add(currentField.Trim()); // Add the last field

            return result.ToArray();
        }


        void DisplayQuestion(int questionIndex)
        {
            if (questionIndex >= questions.Count)
            {
                // Switch to EndingSlide scene
                SceneManager.LoadScene("EndingSlide");
                return;
            }

            // ✅ Always show the timer (even if not counting down)
            if (SceneSwitcher.timerModeEnabled)
            {
                timeRemaining = totalTime;
                timerRunning = true;
            }

            // Always turn on the timerText UI
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = "Time: —"; // Optional: placeholder when not in use
            }


            Question q = questions[questionIndex];
            questionText.text = q.question;

            buttonToOptionDictionary = new Dictionary<Button, string>();
            List<TMP_Text> answerTexts = new List<TMP_Text> { optionAText, optionBText, optionCText, optionDText, optionEText, optionFText };
            List<Button> answerButtons = new List<Button> { optionAButton, optionBButton, optionCButton, optionDButton, optionEButton, optionFButton };

            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].text = q.shuffledOptions[i];
                buttonToOptionDictionary[answerButtons[i]] = q.shuffledOptions[i];
            }

            if (brainAnimator != null && !string.IsNullOrEmpty(q.animationName))
            {
                V2AnimationController animationController = brainAnimator.GetComponent<V2AnimationController>();
                if (animationController != null)
                {
                    animationController.PlayAnimation(q.animationName);
                }
                else
                {
                    Debug.LogError("V2AnimationController script not found on brainAnimator.");
                }
            }

            ResetButtonColors();
        }


        void OnButtonTouch(Button clickedButton)
        {
            if (SceneSwitcher.currentQuestionIndex >= questions.Count) return;

            Question q = questions[SceneSwitcher.currentQuestionIndex];
            string selectedAnswer = buttonToOptionDictionary[clickedButton];

            if (selectedAnswer == q.correctAnswer)
            {
                clickedButton.GetComponent<Image>().color = Color.green;
                int baseScore = 100;
                float multiplier = 1.0f;

                if (SceneSwitcher.timerModeEnabled && totalTime > 0)
                {
                    multiplier = Mathf.Clamp01(timeRemaining / totalTime);
                }

                int finalScore = Mathf.RoundToInt(baseScore * multiplier);
                ScoreCounter.AddScore(finalScore);
                StartCoroutine(SwitchToSingleSlicer(q.animationName, int.Parse(q.correctAnswer)));
            }
            else
            {
                clickedButton.GetComponent<Image>().color = Color.red;
                ScoreCounter.AddScore(-10); // ✅ -10 for wrong
            }
        }


        IEnumerator SwitchToSingleSlicer(string animationName, int sliceNumber)
        {
            yield return new WaitForSeconds(2);

            // ✅ Get the structure name, info, and coordinates from the current question
            Question currentQuestion = questions[SceneSwitcher.currentQuestionIndex];

            SceneSwitcher.SwitchToFeedbackScene(
    animationName,
    sliceNumber,
    currentQuestion.structureName,
    currentQuestion.anatomy,
    currentQuestion.function,
    currentQuestion.pathology,
    currentQuestion.coord
);
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
        void Update()
        {
            if (SceneSwitcher.timerModeEnabled && timerRunning)
            {
                timeRemaining -= Time.deltaTime;

                if (timerText != null)
                {
                    timerText.text = $"{Mathf.Ceil(timeRemaining)}";
                }

                Debug.Log($"[TIMER] timeRemaining = {timeRemaining}");

                if (timerSlider != null)
                {
                    float progress = Mathf.Clamp01(1f - (timeRemaining / totalTime)); // fill over time
                    timerSlider.value = progress;
                }


                if (timeRemaining <= 0)
                {
                    timerRunning = false;
                    Debug.Log("[TIMER] Time is up!");

                    ScoreCounter.AddScore(-25); // penalty for running out of time

                    // Go to feedback screen just like a wrong answer would
                    Question currentQuestion = questions[SceneSwitcher.currentQuestionIndex];

                    SceneSwitcher.SwitchToFeedbackScene(
                        currentQuestion.animationName,
                        int.Parse(currentQuestion.correctAnswer),
                        currentQuestion.structureName,
                        currentQuestion.anatomy,
                        currentQuestion.function,
                        currentQuestion.pathology,
                        currentQuestion.coord
                    );
                }
            }
        }

    }

}
