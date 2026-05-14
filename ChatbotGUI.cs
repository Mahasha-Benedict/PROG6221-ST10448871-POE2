using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace CybersecurityChatbotGUI
{
    /// <summary>
    /// Core chatbot logic handler - Complete version with Memory, Sentiment, Error Handling
    /// Demonstrates OOP principles, generic collections, and professional error handling
    /// </summary>
    public class ChatbotGUI
    {
        private MainWindow window;
        private Random random;

        // QUESTION 5: Memory storage for user details
        private string userName;
        private string favoriteTopic;
        private Dictionary<string, string> userMemory;

        // Conversation flow tracking
        private string currentTopic;
        private string lastResponse;

        // QUESTION 6: Sentiment analyzer
        private SentimentAnalyzer sentimentAnalyzer;

        // QUESTION 8: Dictionary for keyword-to-response mapping (Optimized)
        private Dictionary<string, Func<string>> keywordResponses;

        // QUESTION 8: Dictionary for follow-up responses
        private Dictionary<string, Func<string>> followUpResponses;

        // QUESTION 3 & 8: Arrays/Lists for random responses
        private Dictionary<string, string[]> responseLists;

        public ChatbotGUI(MainWindow mainWindow)
        {
            window = mainWindow;
            random = new Random();
            userMemory = new Dictionary<string, string>();
            sentimentAnalyzer = new SentimentAnalyzer();
            currentTopic = "";
            lastResponse = "";
            userName = "";
            favoriteTopic = "";

            // QUESTION 8: Initialize optimized response dictionaries
            InitializeKeywordResponses();
            InitializeFollowUpResponses();
            InitializeResponseLists();
        }

        /// <summary>
        /// QUESTION 8: Dictionary mapping keywords to response methods
        /// Optimizes code readability and performance
        /// </summary>
        private void InitializeKeywordResponses()
        {
            keywordResponses = new Dictionary<string, Func<string>>
            {
                { "password", () => GetPasswordResponse() },
                { "passphrase", () => GetPasswordResponse() },
                { "scam", () => GetScamResponse() },
                { "phish", () => GetScamResponse() },
                { "phishing", () => GetScamResponse() },
                { "privacy", () => GetPrivacyResponse() },
                { "brows", () => GetSafeBrowsingResponse() },
                { "website", () => GetSafeBrowsingResponse() },
                { "social", () => GetSocialEngineeringResponse() },
                { "engineering", () => GetSocialEngineeringResponse() },
                { "malware", () => GetMalwareResponse() },
                { "virus", () => GetMalwareResponse() },
                { "2fa", () => GetTwoFactorResponse() },
                { "two factor", () => GetTwoFactorResponse() },
                { "mfa", () => GetTwoFactorResponse() },
                { "hello", () => GetGreetingResponse() },
                { "hi", () => GetGreetingResponse() },
                { "hey", () => GetGreetingResponse() },
                { "thank", () => GetThankYouResponse() },
                { "help", () => GetHelpResponse() },
                { "topics", () => GetHelpResponse() }
            };
        }

        /// <summary>
        /// QUESTION 8: Dictionary for follow-up responses by topic
        /// </summary>
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

        /// <summary>
        /// QUESTION 8: Arrays/Lists for random response selection
        /// Each topic has multiple responses for variety
        /// </summary>
        private void InitializeResponseLists()
        {
            responseLists = new Dictionary<string, string[]>
            {
                { "password_main", new string[] {
                    "PASSWORD SAFETY: Use strong passwords at least 12 characters long with uppercase, lowercase, numbers, and symbols. Never use personal information.",
                    "PASSWORD SAFETY: Use a different password for every account. Password managers help you store them securely.",
                    "PASSWORD SAFETY: Enable Two-Factor Authentication (2FA). Even if someone steals your password, they can't access your account."
                }},
                { "password_follow", new string[] {
                    "Another tip: Avoid common passwords like 'password123' or 'qwerty'. Hackers try these first.",
                    "Another tip: Change your passwords every 3-6 months, especially for email and banking.",
                    "Another tip: Use passphrases - random words like 'PurpleTigerJumpingCloud' - they're long but easy to remember."
                }},
                { "scam_main", new string[] {
                    "SCAM AWARENESS: Never click links in unsolicited emails. Hover first to see where they really go.",
                    "SCAM AWARENESS: Be suspicious of urgent messages saying your account will close or you've won a prize.",
                    "SCAM AWARENESS: Verify requests by calling the organization using their official website number."
                }},
                { "scam_follow", new string[] {
                    "Another scam tip: Check sender email addresses carefully - scammers use similar-looking addresses.",
                    "Another scam tip: Look for spelling and grammar mistakes. Real companies proofread carefully.",
                    "Another scam tip: If it seems too good to be true, it probably is a scam."
                }},
                { "privacy_main", new string[] {
                    "PRIVACY PROTECTION: Review social media privacy settings. Limit who can see your posts and personal info.",
                    "PRIVACY PROTECTION: Avoid posting location, travel plans, birth dates, or ID photos online.",
                    "PRIVACY PROTECTION: Use a VPN on public Wi-Fi. Public networks are not secure."
                }},
                { "privacy_follow", new string[] {
                    "Another privacy tip: Check what apps have access to your accounts. Remove unused ones.",
                    "Another privacy tip: Use different emails for shopping and newsletters to protect your primary email.",
                    "Another privacy tip: Clear browser history and cookies regularly or use private browsing."
                }},
                { "browsing_main", new string[] {
                    "SAFE BROWSING: Look for 'https://' and the padlock icon - this means your connection is encrypted.",
                    "SAFE BROWSING: Never download software from pop-up ads. Always use official sources.",
                    "SAFE BROWSING: Keep your browser and extensions updated for security fixes."
                }},
                { "browsing_follow", new string[] {
                    "Another browsing tip: Use an ad-blocker to avoid malicious advertisements.",
                    "Another browsing tip: Be careful with shortened URLs - you can't see where they lead.",
                    "Another browsing tip: Clear browsing data regularly to remove tracking cookies."
                }},
                { "social_main", new string[] {
                    "SOCIAL ENGINEERING: Never share passwords, OTP codes, or personal info over the phone.",
                    "SOCIAL ENGINEERING: Hang up and call back using official numbers. Don't trust caller ID.",
                    "SOCIAL ENGINEERING: Be suspicious of anyone asking for remote computer access."
                }},
                { "social_follow", new string[] {
                    "Another tip: Watch for 'pretexting' - fake scenarios to get information. Always verify identities.",
                    "Another tip: Don't post too much personal info online. Hackers use it to seem trustworthy.",
                    "Another tip: If something feels wrong, trust your instinct. Better safe than sorry."
                }},
                { "malware_main", new string[] {
                    "MALWARE PROTECTION: Install reputable antivirus software. Keep it updated and scan regularly.",
                    "MALWARE PROTECTION: Don't open attachments from unknown senders. Verify unexpected attachments.",
                    "MALWARE PROTECTION: Back up files regularly. Ransomware can encrypt files - backups protect you."
                }},
                { "malware_follow", new string[] {
                    "Another malware tip: Keep your operating system updated for security patches.",
                    "Another malware tip: Be careful with USB drives from unknown sources.",
                    "Another malware tip: If computer slows or shows pop-ups, run a malware scan immediately."
                }},
                { "2fa_main", new string[] {
                    "TWO-FACTOR AUTHENTICATION: Enable 2FA on all accounts - it blocks 99.9% of account takeovers.",
                    "TWO-FACTOR AUTHENTICATION: Use authenticator apps like Google Authenticator instead of SMS.",
                    "TWO-FACTOR AUTHENTICATION: Save backup codes safely. You'll need them if you lose your phone."
                }},
                { "2fa_follow", new string[] {
                    "Another 2FA tip: Security keys like YubiKey are the most secure form of 2FA.",
                    "Another 2FA tip: Don't store backup codes on your computer. Write them down physically.",
                    "Another 2FA tip: If you get an unexpected 2FA code, change your password immediately."
                }}
            };
        }

        public void Start()
        {
            window.DisplayAsciiArt();
            window.AddToChatHistory("Bot: Hello! Welcome to the Cybersecurity Awareness Bot!", System.Windows.Media.Brushes.LightGreen);
            window.AddToChatHistory("Bot: What is your name?", System.Windows.Media.Brushes.LightGreen);
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
                }
            }
            catch { } // QUESTION 7: Silent fail - doesn't crash application
        }

        /// <summary>
        /// QUESTION 5 & 6 & 7: Main message processor with memory, sentiment, error handling
        /// </summary>
        public void ProcessMessage(string message)
        {
            // QUESTION 7: Handle null or empty input gracefully
            if (string.IsNullOrWhiteSpace(message))
            {
                window.AddToChatHistory("Bot: I didn't hear anything. Please type a message.", System.Windows.Media.Brushes.LightYellow);
                return;
            }

            string lowerMsg = message.ToLower().Trim();

            // QUESTION 5: First-time user - collect name
            if (string.IsNullOrEmpty(userName))
            {
                userName = message.Trim();
                userMemory["name"] = userName;
                window.SetUserInfo($"User: {userName}");
                window.AddToChatHistory($"Bot: Nice to meet you, {userName}! I'm here to help you stay safe online.", System.Windows.Media.Brushes.LightGreen);
                window.AddToChatHistory("Bot: You can ask me about passwords, scams, privacy, safe browsing, social engineering, malware, or 2FA.", System.Windows.Media.Brushes.LightGreen);
                return;
            }

            // QUESTION 5: Check if user mentions a favorite topic
            DetectAndStoreFavoriteTopic(lowerMsg);

            // QUESTION 6: Analyze sentiment and get empathetic prefix
            string sentiment = sentimentAnalyzer.Analyze(message);
            window.SetSentiment(sentiment);
            string empathyPrefix = GetEmpathyPrefix(sentiment, lowerMsg);

            // QUESTION 7: Try-catch for unexpected errors
            try
            {
                string response = GenerateResponse(lowerMsg);
                string finalResponse = empathyPrefix + response;

                // QUESTION 5: Personalize response with user's favorite topic if available
                if (!string.IsNullOrEmpty(favoriteTopic) && ShouldUseFavoriteTopic())
                {
                    finalResponse = $"As someone interested in {favoriteTopic}, {finalResponse.ToLower()}";
                }

                window.AddToChatHistory($"Bot: {finalResponse}", System.Windows.Media.Brushes.LightGreen);

                // Update memory with conversation context
                UpdateConversationMemory(lowerMsg);
            }
            catch (Exception ex)
            {
                // QUESTION 7: Professional error handling - no crashes
                window.AddToChatHistory($"Bot: I encountered an issue processing that. Please try again.", System.Windows.Media.Brushes.LightYellow);
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// QUESTION 5: Detects and stores user's favorite cybersecurity topic
        /// </summary>
        private void DetectAndStoreFavoriteTopic(string input)
        {
            string[] topics = { "password", "scam", "privacy", "browsing", "social", "malware", "2fa" };

            foreach (string topic in topics)
            {
                if ((input.Contains("interested in") || input.Contains("like") || input.Contains("love")) && input.Contains(topic))
                {
                    favoriteTopic = topic;
                    userMemory["favorite_topic"] = topic;
                    window.SetUserInfo($"User: {userName} | Favorite: {favoriteTopic} | Last topic: {currentTopic}");
                    window.AddToChatHistory($"Bot: Great! I remember you're interested in {favoriteTopic}. That's a very important area of cybersecurity!", System.Windows.Media.Brushes.LightGreen);
                    break;
                }
            }
        }

        /// <summary>
        /// QUESTION 6: Returns empathetic prefix based on detected sentiment
        /// Adjusts response to be supportive or encouraging
        /// </summary>
        private string GetEmpathyPrefix(string sentiment, string input)
        {
            // QUESTION 6: Detect worried sentiment
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("nervous"))
            {
                return "It's completely understandable to feel that way. Let me help you stay safe. ";
            }

            // QUESTION 6: Detect frustrated sentiment
            if (input.Contains("frustrated") || input.Contains("annoying") || input.Contains("difficult"))
            {
                return "I know cybersecurity can be frustrating. Let me simplify this for you. ";
            }

            // QUESTION 6: Detect curious sentiment
            if (input.Contains("curious") || input.Contains("interested") || input.Contains("want to learn"))
            {
                return "That's great that you want to learn! ";
            }

            // Based on sentiment analysis
            switch (sentiment)
            {
                case "negative":
                    return "I understand your concern. ";
                case "positive":
                    return "I'm glad you're taking an interest! ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// QUESTION 7 & 8: Optimized response generation using dictionaries
        /// Handles follow-ups, keywords, and provides default for unrecognized input
        /// </summary>
        private string GenerateResponse(string input)
        {
            // QUESTION 4 & 7: Check for follow-up requests first
            if (IsFollowUpRequest(input))
            {
                return GetFollowUpResponse();
            }

            // QUESTION 7: Check for exit/quit commands
            if (input.Contains("exit") || input.Contains("quit") || input.Contains("bye"))
            {
                return $"Goodbye {userName}! Remember to stay safe online!";
            }

            // QUESTION 7: Handle confusion
            if (input.Contains("confused") || input.Contains("dont understand") || input.Contains("don't understand"))
            {
                return GetConfusionResponse();
            }

            // QUESTION 8: Use dictionary for keyword matching (optimized)
            foreach (var keyword in keywordResponses)
            {
                if (input.Contains(keyword.Key))
                {
                    currentTopic = GetTopicFromKeyword(keyword.Key);
                    string response = keyword.Value();
                    return response;
                }
            }

            // QUESTION 7: Default response for unrecognized input
            return GetDefaultResponse();
        }

        /// <summary>
        /// Helper: Maps keyword to topic name for memory tracking
        /// </summary>
        private string GetTopicFromKeyword(string keyword)
        {
            if (keyword.Contains("password")) return "password";
            if (keyword.Contains("scam") || keyword.Contains("phish")) return "scam";
            if (keyword.Contains("privacy")) return "privacy";
            if (keyword.Contains("brows") || keyword.Contains("website")) return "browsing";
            if (keyword.Contains("social") || keyword.Contains("engineering")) return "social";
            if (keyword.Contains("malware") || keyword.Contains("virus")) return "malware";
            if (keyword.Contains("2fa") || keyword.Contains("two factor") || keyword.Contains("mfa")) return "2fa";
            return currentTopic;
        }

        /// <summary>
        /// QUESTION 4: Detects follow-up requests
        /// </summary>
        private bool IsFollowUpRequest(string input)
        {
            return input.Contains("tell me more") ||
                   input.Contains("another tip") ||
                   input.Contains("another one") ||
                   input.Contains("explain more") ||
                   input.Contains("more info") ||
                   input.Contains("more information") ||
                   input.Contains("continue") ||
                   (input.Contains("more") && input.Contains("about")) ||
                   input == "more";
        }

        /// <summary>
        /// QUESTION 4 & 8: Returns follow-up response using dictionary
        /// </summary>
        private string GetFollowUpResponse()
        {
            if (followUpResponses.ContainsKey(currentTopic))
            {
                return followUpResponses[currentTopic]();
            }
            return "What would you like to learn more about? Ask me about passwords, scams, privacy, or other cybersecurity topics.";
        }

        // ========== RESPONSE METHODS USING OPTIMIZED ARRAYS (QUESTION 8) ==========

        private string GetRandomFromList(string listKey)
        {
            if (responseLists.ContainsKey(listKey))
            {
                var list = responseLists[listKey];
                return list[random.Next(list.Length)];
            }
            return "Here's a cybersecurity tip: always think before you click!";
        }

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
                $"Greetings {userName}! Ask me about passwords, scams, privacy, or other cybersecurity topics."
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetThankYouResponse()
        {
            string[] responses = {
                $"You're welcome {userName}! Stay safe online!",
                $"Happy to help {userName}! Remember, cybersecurity is everyone's responsibility.",
                $"Anytime {userName}! Let me know if you have more questions."
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetHelpResponse()
        {
            return "I can help you with: passwords, scams, privacy, safe browsing, social engineering, malware, and two-factor authentication. Try asking: 'Tell me about password safety' or 'Give me a scam tip'.";
        }

        private string GetConfusionResponse()
        {
            string[] responses = {
                "I understand cybersecurity can be confusing. Which topic would you like help with? Passwords, scams, or privacy?",
                "No worries! Try asking me for a tip about passwords, scams, or privacy protection.",
                "Let me help clarify. What specific cybersecurity concern do you have?"
            };
            return responses[random.Next(responses.Length)];
        }

        private string GetDefaultResponse()
        {
            string[] responses = {
                "I didn't quite understand. Try asking about passwords, scams, or privacy protection.",
                "Could you rephrase? I specialize in cybersecurity topics like password safety, scam detection, and online privacy.",
                "I'm here to help with cybersecurity! Try asking: 'Tell me about password safety' or 'Give me a scam tip'."
            };
            return responses[random.Next(responses.Length)];
        }

        /// <summary>
        /// QUESTION 5: Updates conversation memory with current topic
        /// </summary>
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

        /// <summary>
        /// QUESTION 5: Determines if bot should reference favorite topic
        /// </summary>
        private bool ShouldUseFavoriteTopic()
        {
            // 30% chance to reference favorite topic - makes it feel natural, not forced
            return !string.IsNullOrEmpty(favoriteTopic) && random.Next(100) < 30;
        }

        public void PlayGreeting()
        {
            PlayVoiceGreeting();
        }
    }
}
