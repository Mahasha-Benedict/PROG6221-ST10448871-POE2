using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityChatbotGUI
{
    public class NLPSimulator
    {
        private ActivityLogger logger;
        private Dictionary<string, List<string>> intentPatterns;

        public NLPSimulator(ActivityLogger logger)
        {
            this.logger = logger;
            InitializePatterns();
        }

        private void InitializePatterns()
        {
            intentPatterns = new Dictionary<string, List<string>>
            {
                ["add_task"] = new List<string>
                {
                    "add task", "create task", "new task", "add a task",
                    "create a task", "remind me to", "set reminder",
                    "add reminder", "remember to", "task -", "task:"
                },
                ["view_tasks"] = new List<string>
                {
                    "show tasks", "view tasks", "list tasks", "my tasks",
                    "what tasks", "display tasks", "show my tasks",
                    "show all tasks"
                },
                ["complete_task"] = new List<string>
                {
                    "complete task", "mark done", "finish task", "task complete",
                    "complete", "done", "finished", "mark complete"
                },
                ["delete_task"] = new List<string>
                {
                    "delete task", "remove task", "cancel task", "delete", "remove"
                },
                ["start_quiz"] = new List<string>
                {
                    "start quiz", "take quiz", "begin quiz", "do quiz", "play quiz",
                    "quiz me", "test me", "cybersecurity quiz", "quiz"
                },
                ["show_log"] = new List<string>
                {
                    "show log", "activity log", "view log", "what have you done",
                    "show activity", "recent actions", "summary",
                    "what have you done for me"
                },
                ["password"] = new List<string>
                {
                    "password", "passphrase", "password safety", "strong password"
                },
                ["scam"] = new List<string>
                {
                    "scam", "phishing", "phish", "fraud", "scammer"
                },
                ["privacy"] = new List<string>
                {
                    "privacy", "private", "personal info", "data protection"
                }
            };
        }

        public string DetectIntent(string input)
        {
            string lowerInput = input.ToLower().Trim();

            // Check for "add task" pattern with dash or colon
            if (lowerInput.Contains("add task") || lowerInput.Contains("task -") || lowerInput.Contains("task:"))
            {
                logger.LogAction($"NLP Detected: Intent 'add_task' from input: '{input}'");
                return "add_task";
            }

            // Check for "remind me" pattern
            if (lowerInput.Contains("remind me"))
            {
                logger.LogAction($"NLP Detected: Intent 'add_task' from input: '{input}'");
                return "add_task";
            }

            foreach (var intent in intentPatterns)
            {
                foreach (string pattern in intent.Value)
                {
                    if (lowerInput.Contains(pattern))
                    {
                        logger.LogAction($"NLP Detected: Intent '{intent.Key}' from input: '{input}'");
                        return intent.Key;
                    }
                }
            }

            return "unknown";
        }

        public string ExtractTaskTitle(string input)
        {
            string lowerInput = input.ToLower();
            string result = input;

            // Remove common phrases
            string[] removePhrases = {
                "add task ", "create task ", "new task ", "add a task ",
                "create a task ", "remind me to ", "set reminder ",
                "add reminder ", "remember to ", "task - ", "task: ",
                "add task - ", "add task: "
            };

            foreach (string phrase in removePhrases)
            {
                if (lowerInput.Contains(phrase))
                {
                    int index = lowerInput.IndexOf(phrase);
                    result = input.Substring(index + phrase.Length).Trim();
                    break;
                }
            }

            // If still contains "add task" at start
            if (result.StartsWith("add task", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(8).Trim();
            }

            return string.IsNullOrWhiteSpace(result) ? input : result;
        }

        public DateTime? ExtractReminderDate(string input)
        {
            string lowerInput = input.ToLower();

            // Check for "tomorrow"
            if (lowerInput.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }

            // Check for "in X days"
            Regex daysRegex = new Regex(@"in\s+(\d+)\s+days?");
            Match daysMatch = daysRegex.Match(lowerInput);
            if (daysMatch.Success)
            {
                int days = int.Parse(daysMatch.Groups[1].Value);
                return DateTime.Now.AddDays(days);
            }

            // Check for "next week"
            if (lowerInput.Contains("next week"))
            {
                return DateTime.Now.AddDays(7);
            }

            // Check for "in X weeks"
            Regex weeksRegex = new Regex(@"in\s+(\d+)\s+weeks?");
            Match weeksMatch = weeksRegex.Match(lowerInput);
            if (weeksMatch.Success)
            {
                int weeks = int.Parse(weeksMatch.Groups[1].Value);
                return DateTime.Now.AddDays(weeks * 7);
            }

            return null;
        }
    }
}