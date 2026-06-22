using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool isActive;
        private ActivityLogger logger;

        public QuizManager(ActivityLogger logger)
        {
            this.logger = logger;
            currentQuestionIndex = 0;
            score = 0;
            isActive = false;
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            questions = new List<QuizQuestion>
            {
                // Question 1: Multiple Choice (Correct Answer  3 = C)
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new string[] {
                        "A) Reply with your password",
                        "B) Delete the email",
                        "C) Report the email as phishing",
                        "D) Ignore it"
                    },
                    CorrectAnswer = 2,  // C is correct (1=A, 2=B, 3=C, 4=D)
                    Explanation = "Reporting phishing emails helps prevent scams and protects others from falling victim."
                },
                
                // Question 2: True/False (Correct Answer : 2 = False)
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for multiple accounts is safe.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 1,  // False is correct (1=True, 2=False)
                    Explanation = "Using the same password across multiple accounts puts all your accounts at risk if one gets compromised."
                },
                
                // Question 3: Multiple Choice (Correct Answer: 2 = B)
                new QuizQuestion
                {
                    Question = "What is a strong password practice?",
                    Options = new string[] {
                        "A) Using your birthday",
                        "B) Using a mix of uppercase, lowercase, numbers, and symbols",
                        "C) Using your pet's name",
                        "D) Using only numbers"
                    },
                    CorrectAnswer = 1,  // B is correct (1=A, 2=B, 3=C, 4=D)
                    Explanation = "A strong password combines uppercase, lowercase, numbers, and symbols to be difficult to guess."
                },
                
                // Question 4: True/False (Correct Answer: 2 = False)
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is always safe to use without protection.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 1,  // False is correct
                    Explanation = "Public Wi-Fi is not secure. Use a VPN to protect your data when using public networks."
                },
                
                // Question 5: Multiple Choice (Correct Answer 2 = B)
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new string[] {
                        "A) A type of fishing",
                        "B) A cyber attack where scammers trick you into revealing personal info",
                        "C) A type of computer virus",
                        "D) A firewall setting"
                    },
                    CorrectAnswer = 1,  // B is correct
                    Explanation = "Phishing is a cyber attack where scammers impersonate legitimate organizations to steal personal information."
                },
                
                // Question 6: True/False (Correct Answer 1 = True)
                new QuizQuestion
                {
                    Question = "True or False: Two-Factor Authentication (2FA) adds an extra layer of security.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 0,  // True is correct
                    Explanation = "2FA adds a second verification step, significantly reducing the risk of unauthorized access."
                },
                
                // Question 7: Multiple Choice (Correct Answer: 1 = A)
                new QuizQuestion
                {
                    Question = "What should you look for when visiting a website to ensure it's secure?",
                    Options = new string[] {
                        "A) The padlock icon in the address bar",
                        "B) The website has many ads",
                        "C) The website has a lot of text",
                        "D) The website has videos"
                    },
                    CorrectAnswer = 0,  // A is correct
                    Explanation = "The padlock icon indicates the connection is encrypted using HTTPS, keeping your data secure."
                },
                
                // Question 8: True/False (Correct Answer: 2 = False)
                new QuizQuestion
                {
                    Question = "True or False: You should share your OTP (One-Time Password) with IT support if they ask.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 1,  // False is correct
                    Explanation = "Never share OTPs with anyone, even if they claim to be from IT support. This is a common social engineering tactic."
                },
                
                // Question 9: Multiple Choice (Correct Answer: 2 = B)
                new QuizQuestion
                {
                    Question = "What is the best way to protect against malware?",
                    Options = new string[] {
                        "A) Download files from any website",
                        "B) Use antivirus software and keep it updated",
                        "C) Click on pop-up ads",
                        "D) Ignore software updates"
                    },
                    CorrectAnswer = 1,  // B is correct
                    Explanation = "Antivirus software with regular updates helps detect and prevent malware infections."
                },
                
                // Question 10: True/False (Correct Answer: 2 = False)
                new QuizQuestion
                {
                    Question = "True or False: Social engineering is a type of hardware attack.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 1,  // False is correct
                    Explanation = "Social engineering is a psychological manipulation technique, not a hardware attack. It tricks people into revealing information."
                },
                
                // Question 11: Multiple Choice (Correct Answer: 2 = B)
                new QuizQuestion
                {
                    Question = "What should you do if you suspect a scam?",
                    Options = new string[] {
                        "A) Engage with the scammer",
                        "B) Report it to the authorities",
                        "C) Share the link with friends",
                        "D) Enter your details"
                    },
                    CorrectAnswer = 1,  // B is correct
                    Explanation = "Reporting scams to authorities helps prevent others from becoming victims and supports law enforcement efforts."
                },
                
                // Question 12: True/False (Correct Answer: 2 = False)
                new QuizQuestion
                {
                    Question = "True or False: A strong password should contain personal information like your name.",
                    Options = new string[] { "A) True", "B) False" },
                    CorrectAnswer = 1,  // False is correct
                    Explanation = "Personal information like names or birthdays is easy to guess. Avoid using it in passwords."
                }
            };
        }

        public string StartQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            isActive = true;
            logger.LogAction("Quiz started");
            return "Cybersecurity Quiz started! Let's test your knowledge.\n" + GetCurrentQuestion();
        }

        public string GetCurrentQuestion()
        {
            if (!isActive || currentQuestionIndex >= questions.Count)
            {
                return EndQuiz();
            }

            QuizQuestion q = questions[currentQuestionIndex];
            string result = $"\nQuestion {currentQuestionIndex + 1}/{questions.Count}: {q.Question}\n";
            foreach (string option in q.Options)
            {
                result += $"{option}\n";
            }
            result += "\nType A, B, C, D or the number:";
            return result;
        }

        public string SubmitAnswer(string input)
        {
            if (!isActive)
            {
                return "No active quiz. Type 'start quiz' to begin!";
            }

            QuizQuestion q = questions[currentQuestionIndex];

            // Handle input - support both letter and number formats
            int userChoice = -1;

            // Check for letter input (A, B, C, D)
            string upperInput = input.ToUpper().Trim();
            if (upperInput == "A") userChoice = 0;
            else if (upperInput == "B") userChoice = 1;
            else if (upperInput == "C") userChoice = 2;
            else if (upperInput == "D") userChoice = 3;
            else if (upperInput == "TRUE") userChoice = 0;
            else if (upperInput == "FALSE") userChoice = 1;
            else if (int.TryParse(input, out int num))
            {
                userChoice = num - 1; // Convert 1-based to 0-based
            }

            // If input is "True" or "False" without number prefix
            if (userChoice == -1 && q.Options.Length == 2)
            {
                if (input.ToLower().Contains("true")) userChoice = 0;
                else if (input.ToLower().Contains("false")) userChoice = 1;
            }

            if (userChoice < 0 || userChoice >= q.Options.Length)
            {
                return $"Invalid input. Please type A, B, C, D or 1, 2, 3, 4.";
            }

            bool isCorrect = userChoice == q.CorrectAnswer;
            if (isCorrect) score++;

            logger.LogAction($"Quiz: Q{currentQuestionIndex + 1} - Answer: {(isCorrect ? "Correct" : "Wrong")}");

            string result = isCorrect ? "✓ Correct! " : "✗ Incorrect. ";
            result += q.Explanation + "\n";

            currentQuestionIndex++;

            if (currentQuestionIndex >= questions.Count)
            {
                result += "\n" + EndQuiz();
            }
            else
            {
                result += "\n" + GetCurrentQuestion();
            }

            return result;
        }

        public string EndQuiz()
        {
            isActive = false;
            logger.LogAction($"Quiz completed - Score: {score}/{questions.Count}");

            string feedback;
            if (score == questions.Count)
                feedback = " Perfect score! You're a cybersecurity expert! ";
            else if (score >= questions.Count * 0.8)
                feedback = " Great job! You're a cybersecurity pro!";
            else if (score >= questions.Count * 0.6)
                feedback = " Good effort! Keep learning to stay safe online!";
            else
                feedback = " Keep learning! Cybersecurity is important for everyone. Try again!";

            return $"Quiz complete! Your score: {score}/{questions.Count}\n{feedback}";
        }

        public bool IsActive => isActive;
    }

    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = new string[0];
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; } = "";
    }
}