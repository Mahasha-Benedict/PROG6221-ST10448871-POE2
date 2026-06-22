using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace CybersecurityChatbotGUI
{
    /// Summary
    /// Core chatbot logic completed Phase 3 version
    /// Integrating the Task Assistant, Quiz, NLP, and Activity Log

    public class ChatbotGUI
    {
        private MainWindow window;
        private Random random;

        // Memory
        private string userName;
        private string favoriteTopic;
        private Dictionary<string, string> userMemory;

        // Conversation
        private string currentTopic;
        private string lastResponse;

        // Phase 3 Components
        private SentimentAnalyzer sentimentAnalyzer;
        private NLPSimulator nlpSimulator;
        private TaskManager taskManager;
        private QuizManager quizManager;
        private ActivityLogger logger;

        // Response dictionaries
        private Dictionary<string, Func<string>> keywordResponses;
        private Dictionary<string, Func<string>> followUpResponses;
        private Dictionary<string, string[]> responseLists;

        public ChatbotGUI(MainWindow mainWindow)
        {
            window = mainWindow;
            random = new Random();
            userMemory = new Dictionary<string, string>();

            // Initializing the Phase 3 components
            logger = new ActivityLogger();
            sentimentAnalyzer = new SentimentAnalyzer();
            nlpSimulator = new NLPSimulator(logger);
            taskManager = new TaskManager(logger);
            quizManager = new QuizManager(logger);

            userName = "";
            favoriteTopic = "";
            currentTopic = "";
            lastResponse = "";

            InitializeKeywordResponses();
            InitializeFollowUpResponses();
            InitializeResponseLists();

            // Log startup
            logger.LogAction("Chatbot started");
        }

        private void InitializeKeywordResponses()
        {
            keywordResponses = new Dictionary<string, Func<string>>
            {
                { "password", () => GetPasswordResponse() },
                { "scam", () => GetScamResponse() },
                { "phish", () => GetScamResponse() },
                { "privacy", () => GetPrivacyResponse() },
                { "brows", () => GetSafeBrowsingResponse() },
                { "social", () => GetSocialEngineeringResponse() },
                { "malware", () => GetMalwareResponse() },
                { "2fa", () => GetTwoFactorResponse() },
                { "hello", () => GetGreetingResponse() },
                { "hi", () => GetGreetingResponse() },
                { "thank", () => GetThankYouResponse() },
                { "help", () => GetHelpResponse() }
            };
        }

        private void InitializeFollowUpResponses()
        {
            followUpResponses = new Dictionary<string, Func<string>>
            {
                { "password", () => GetAnotherPasswordTip() },
                { "scam", () => GetAnotherScamTip() },
                { "privacy", () => GetAnotherPrivacyTip() },
                { "browsing", () => GetAnotherBrowsingTip() },
                { "social", () => GetAnotherSocialTip() },
                { "malware", () => GetAnotherMalwareTip() },
                { "2fa", () => GetAnotherTwoFactorTip() }
            };
        }

        private void InitializeResponseLists()
        {
            responseLists = new Dictionary<string, string[]>
            {
                { "password_main", new string[] {
                    "PASSWORD SAFETY: Use strong passwords at least 12 characters long with uppercase, lowercase, numbers, and symbols.",
                    "PASSWORD SAFETY: Use a different password for every account. Password managers help you store them securely.",
                    "PASSWORD SAFETY: Enable Two-Factor Authentication (2FA). Even if someone steals your password, they can't access your account."
                }},
                // keeping all existing response lists from previous phase
            };
        }

        public void Start()
        {
            window.DisplayAsciiArt();
            logger.LogAction("Displayed ASCII art");
            window.AddToChatHistory("Bot: Hello! Welcome to the Cybersecurity Awareness Bot!", System.Windows.Media.Brushes.LightGreen);
            window.AddToChatHistory("Bot: What is your name?", System.Windows.Media.Brushes.LightGreen);
        }

        public void PlayGreeting()
        {
            PlayVoiceGreeting();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(audioPath))
                {
                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        player.PlaySync();
                    }
                    logger.LogAction("Voice greeting played");
                }
            }
            catch { }
        }

        public void ProcessMessage(string message)
        {
            // Error handling for empty input
            if (string.IsNullOrWhiteSpace(message))
            {
                window.AddToChatHistory("Bot: I didn't hear anything. Please type a message.", System.Windows.Media.Brushes.LightYellow);
                return;
            }

            string lowerMsg = message.ToLower().Trim();

            // First-time user then name is collect 
            if (string.IsNullOrEmpty(userName))
            {
                userName = message.Trim();
                userMemory["name"] = userName;
                window.SetUserInfo($"User: {userName}");
                logger.LogAction($"User logged in: {userName}");
                window.AddToChatHistory($"Bot: Nice to meet you, {userName}! I'm here to help you stay safe online.", System.Windows.Media.Brushes.LightGreen);
                window.AddToChatHistory("Bot: You can ask me about passwords, scams, privacy, or try the quiz or task manager!", System.Windows.Media.Brushes.LightGreen);
                return;
            }

            // Detect and store favorite topic
            DetectAndStoreFavoriteTopic(lowerMsg);

            // Analyze sentiment
            string sentiment = sentimentAnalyzer.Analyze(message);
            window.SetSentiment(sentiment);

            try
            {
                // Uses NLP to detect intent
                string intent = nlpSimulator.DetectIntent(lowerMsg);

                // Route based on intent
                string response = "";

                switch (intent)
                {
                    // Task Assistant intents
                    case "add_task":
                        response = HandleAddTask(message);
                        break;
                    case "view_tasks":
                        response = taskManager.GetTasks(userName);
                        break;
                    case "complete_task":
                        response = HandleCompleteTask(message);
                        break;
                    case "delete_task":
                        response = HandleDeleteTask(message);
                        break;

                    // Quiz intents
                    case "start_quiz":
                        response = quizManager.StartQuiz();
                        break;

                    // Activity log intent
                    case "show_log":
                        response = logger.GetLog();
                        break;

                    // Cybersecurity topics
                    case "password":
                    case "scam":
                    case "privacy":
                    case "browsing":
                    case "social":
                    case "malware":
                    case "2fa":
                        response = GetTopicResponse(intent);
                        break;

                    // Unknown - use standard processing
                    default:
                        response = GenerateStandardResponse(lowerMsg);
                        break;
                }

                // Apply empathy prefix based on sentiment
                string empathyPrefix = GetEmpathyPrefix(sentiment, lowerMsg);
                string finalResponse = empathyPrefix + response;

                // Personalize with favorite topic if applicable
                if (!string.IsNullOrEmpty(favoriteTopic) && ShouldUseFavoriteTopic())
                {
                    finalResponse = $"As someone interested in {favoriteTopic}, {finalResponse.ToLower()}";
                }

                window.AddToChatHistory($"Bot: {finalResponse}", System.Windows.Media.Brushes.LightGreen);
                UpdateConversationMemory(lowerMsg);
            }
            catch (Exception ex)
            {
                window.AddToChatHistory($"Bot: I encountered an issue. Please try again.", System.Windows.Media.Brushes.LightYellow);
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        /// Ssummary
        /// Handles adding a task with NLP support
        private string HandleAddTask(string message)
        {
            string taskTitle = nlpSimulator.ExtractTaskTitle(message);

            // Log the extracted task for debugging
            logger.LogAction($"Extracted task title: '{taskTitle}' from: '{message}'");

            if (string.IsNullOrWhiteSpace(taskTitle) || taskTitle.Length < 3)
            {
                return "What task would you like to add? Please specify the task description. For example: 'Add task - Review privacy settings'";
            }

            // Check for reminder
            DateTime? reminderDate = nlpSimulator.ExtractReminderDate(message);

            string description = $"Cybersecurity task: {taskTitle}";
            string result = taskManager.AddTask(userName, taskTitle, description, reminderDate);

            // Log the result
            logger.LogAction($"Add task result: {result}");

            return result;
        }

        /// Summary
        /// Handles marking a task as complete
        private string HandleCompleteTask(string message)
        {
            // Try to extract task number from "complete task 1" or "mark complete 1"
            var words = message.Split(' ');
            foreach (string word in words)
            {
                if (int.TryParse(word, out int taskNumber))
                {
                    return taskManager.MarkTaskComplete(userName, taskNumber);
                }
            }

            return "Please specify which task number you want to mark complete. For example: 'Complete task 2'";
        }

        /// Summary
        /// Handles deleting a task
        private string HandleDeleteTask(string message)
        {
            var words = message.Split(' ');
            foreach (string word in words)
            {
                if (int.TryParse(word, out int taskNumber))
                {
                    return taskManager.DeleteTask(userName, taskNumber);
                }
            }

            return "Please specify which task number you want to delete. For example: 'Delete task 2'";
        }

        /// Summary
        /// Gets response for cybersecurity topics
        private string GetTopicResponse(string topic)
        {
            currentTopic = topic;
            switch (topic)
            {
                case "password": return GetPasswordResponse();
                case "scam": return GetScamResponse();
                case "privacy": return GetPrivacyResponse();
                case "browsing": return GetSafeBrowsingResponse();
                case "social": return GetSocialEngineeringResponse();
                case "malware": return GetMalwareResponse();
                case "2fa": return GetTwoFactorResponse();
                default: return GetDefaultResponse();
            }
        }

        /// Summary
        /// Generates standard response (non-NLP)
        private string GenerateStandardResponse(string input)
        {
            // Check for follow-up
            if (IsFollowUpRequest(input))
            {
                return GetFollowUpResponse();
            }

            // Check if quiz is active
            if (quizManager.IsActive)
            {
                return quizManager.SubmitAnswer(input);
            }

            // Check if user is answering a quiz question
            if (input == "a" || input == "b" || input == "c" || input == "d" ||
                input == "1" || input == "2" || input == "3" || input == "4")
            {
                return quizManager.SubmitAnswer(input);
            }

            // Check for exit
            if (input.Contains("exit") || input.Contains("quit") || input.Contains("bye"))
            {
                logger.LogAction($"User {userName} exited");
                return $"Goodbye {userName}! Remember to stay safe online!";
            }

            // Check for confusion
            if (input.Contains("confused") || input.Contains("dont understand"))
            {
                return GetConfusionResponse();
            }

            // Check for task-related keywords
            if (input.Contains("show tasks") || input.Contains("list tasks") || input.Contains("my tasks"))
            {
                return taskManager.GetTasks(userName);
            }

            // Default
            return GetDefaultResponse();
        }

        // SUPPORTING METHODS 
        private void DetectAndStoreFavoriteTopic(string input)
        {
            string[] topics = { "password", "scam", "privacy", "browsing", "social", "malware", "2fa" };
            foreach (string topic in topics)
            {
                if ((input.Contains("interested in") || input.Contains("like") || input.Contains("love")) && input.Contains(topic))
                {
                    favoriteTopic = topic;
                    userMemory["favorite_topic"] = topic;
                    window.SetUserInfo($"User: {userName} | Favorite: {favoriteTopic}");
                    logger.LogAction($"User favorite topic set: {favoriteTopic}");
                    window.AddToChatHistory($"Bot: Great! I remember you're interested in {favoriteTopic}. That's very important for cybersecurity!", System.Windows.Media.Brushes.LightGreen);
                    break;
                }
            }
        }

        private string GetEmpathyPrefix(string sentiment, string input)
        {
            if (input.Contains("worried") || input.Contains("scared"))
                return "It's completely understandable to feel that way. Let me help you. ";
            if (input.Contains("frustrated"))
                return "I know cybersecurity can be frustrating. Let me simplify this. ";
            if (input.Contains("curious") || input.Contains("interested"))
                return "That's great that you want to learn! ";
            switch (sentiment)
            {
                case "negative": return "I understand your concern. ";
                case "positive": return "I'm glad you're taking an interest! ";
                default: return "";
            }
        }

        private bool IsFollowUpRequest(string input)
        {
            return input.Contains("tell me more") || input.Contains("another tip") ||
                   input.Contains("explain more") || input.Contains("more info") ||
                   input == "more";
        }

        private string GetFollowUpResponse()
        {
            if (followUpResponses.ContainsKey(currentTopic))
            {
                logger.LogAction($"Follow-up requested on topic: {currentTopic}");
                return followUpResponses[currentTopic]();
            }
            return "What would you like to learn more about? Ask me about passwords, scams, privacy, or other cybersecurity topics.";
        }

        private string GetRandomFromList(string listKey)
        {
            if (responseLists.ContainsKey(listKey))
            {
                var list = responseLists[listKey];
                return list[random.Next(list.Length)];
            }
            return "Always think before you click!";
        }

        // RESPONSE METHODS 

        private string GetPasswordResponse() => GetRandomFromList("password_main");
        private string GetAnotherPasswordTip() => GetRandomFromList("password_follow");
        private string GetScamResponse() => GetRandomFromList("scam_main");
        private string GetAnotherScamTip() => GetRandomFromList("scam_follow");
        private string GetPrivacyResponse() => GetRandomFromList("privacy_main");
        private string GetAnotherPrivacyTip() => GetRandomFromList("privacy_follow");
        private string GetSafeBrowsingResponse() => GetRandomFromList("browsing_main");
        private string GetAnotherBrowsingTip() => GetRandomFromList("browsing_follow");
        private string GetSocialEngineeringResponse() => GetRandomFromList("social_main");
        private string GetAnotherSocialTip() => GetRandomFromList("social_follow");
        private string GetMalwareResponse() => GetRandomFromList("malware_main");
        private string GetAnotherMalwareTip() => GetRandomFromList("malware_follow");
        private string GetTwoFactorResponse() => GetRandomFromList("2fa_main");
        private string GetAnotherTwoFactorTip() => GetRandomFromList("2fa_follow");

        private string GetGreetingResponse()
        {
            string[] responses = {
                $"Hello {userName}! How can I help you with cybersecurity today?",
                $"Hi {userName}! Ready to learn about staying safe online?",
                $"Greetings {userName}! Ask me about passwords, scams, privacy, or try the quiz!"
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetThankYouResponse()
        {
            string[] responses = {
                $"You're welcome {userName}! Stay safe online!",
                $"Happy to help {userName}! Cybersecurity is everyone's responsibility.",
                $"Anytime {userName}! Let me know if you have more questions."
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetHelpResponse()
        {
            return "I can help you with: passwords, scams, privacy, safe browsing, social engineering, malware, and 2FA. I also have a task manager and quiz! Try: 'Add task' or 'Start quiz'";
        }

        private string GetConfusionResponse()
        {
            string[] responses = {
                "I understand cybersecurity can be confusing. Which topic would you like help with?",
                "No worries! Try asking me for a tip about passwords, scams, or privacy.",
                "Let me help. What specific cybersecurity concern do you have?"
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetDefaultResponse()
        {
            string[] responses = {
                "I didn't quite understand. Try asking about passwords, scams, or privacy.",
                "Could you rephrase? I specialize in cybersecurity topics.",
                "I'm here to help! Try: 'Add task', 'Start quiz', or ask about any cybersecurity topic."
            };
            return responses[random.Next(responses.Length)];
        }

        private void UpdateConversationMemory(string input)
        {
            if (!string.IsNullOrEmpty(currentTopic))
            {
                userMemory["last_topic"] = currentTopic;
                string displayInfo = $"User: {userName}";
                if (!string.IsNullOrEmpty(favoriteTopic))
                    displayInfo += $" | Favorite: {favoriteTopic}";
                displayInfo += $" | Last topic: {currentTopic}";
                window.SetUserInfo(displayInfo);
            }
        }

        private bool ShouldUseFavoriteTopic()
        {
            return !string.IsNullOrEmpty(favoriteTopic) && random.Next(100) < 30;
        }
    }
}