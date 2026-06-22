using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /// Summary
    /// QUESTION 6: Enhanced sentiment analyzer for detecting emotions
    /// Detects worried, curious, frustrated, positive, negative, neutral
    public class SentimentAnalyzer
    {
        // QUESTION 8: Using HashSets for efficient lookups
        private HashSet<string> positiveWords;
        private HashSet<string> negativeWords;
        private HashSet<string> worriedWords;
        private HashSet<string> curiousWords;
        private HashSet<string> frustratedWords;

        public SentimentAnalyzer()
        {
            // Initialize all sentiment word sets
            positiveWords = new HashSet<string>
            {
                "happy", "good", "great", "excellent", "wonderful", "awesome", "thanks",
                "thank", "love", "helpful", "amazing", "perfect", "cool", "nice"
            };

            negativeWords = new HashSet<string>
            {
                "bad", "terrible", "awful", "upset", "angry", "hate", "stupid", "useless"
            };

            // QUESTION 6: Specific emotion detection
            worriedWords = new HashSet<string>
            {
                "worried", "scared", "nervous", "anxious", "concerned", "fear", "afraid",
                "unsafe", "vulnerable", "threatened", "panic", "anxiety"
            };

            curiousWords = new HashSet<string>
            {
                "curious", "interested", "want to learn", "tell me", "explain", "how does",
                "why", "what is", "teach me", "learn about"
            };

            frustratedWords = new HashSet<string>
            {
                "frustrated", "annoying", "difficult", "hard", "complicated", "confusing",
                "tired of", "sick of", "fed up"
            };
        }

        /// Summary
        /// QUESTION 6: Analyzes message and returns specific sentiment
        /// Returns: worried, curious, frustrated, positive, negative, or neutral
        public string Analyze(string message)
        {
            string lowerMsg = message.ToLower();

            // Check for specific emotions first (Question 6)
            foreach (string word in worriedWords)
                if (lowerMsg.Contains(word))
                    return "worried";

            foreach (string word in curiousWords)
                if (lowerMsg.Contains(word))
                    return "curious";

            foreach (string word in frustratedWords)
                if (lowerMsg.Contains(word))
                    return "frustrated";

            // Then check positive/negative sentiment
            int positiveCount = 0;
            int negativeCount = 0;

            foreach (string word in positiveWords)
                if (lowerMsg.Contains(word))
                    positiveCount++;

            foreach (string word in negativeWords)
                if (lowerMsg.Contains(word))
                    negativeCount++;

            if (positiveCount > negativeCount)
                return "positive";
            else if (negativeCount > positiveCount)
                return "negative";

            return "neutral";
        }
    }
}
