using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /// summary
    /// Records and manages all chatbot actions
    /// Task 4 added Activity Log Feature
    public class ActivityLogger
    {
        private List<ActivityEntry> activities;
        private int maxEntries = 50; // Store last 50 entries

        public ActivityLogger()
        {
            activities = new List<ActivityEntry>();
        }

        /// summary
        /// Logs an action with timestamp
        public void LogAction(string description)
        {
            activities.Add(new ActivityEntry
            {
                Timestamp = DateTime.Now,
                Description = description
            });

            // Keep only last maxEntries
            if (activities.Count > maxEntries)
            {
                activities.RemoveAt(0);
            }
        }

        /// Summary
        /// Gets the activity log
        public string GetLog()
        {
            if (activities.Count == 0)
            {
                return "No activities logged yet.";
            }

            // Show last 5-10 entries
            int startIndex = Math.Max(0, activities.Count - 10);
            string result = "Here's a summary of recent actions:\n";

            for (int i = startIndex; i < activities.Count; i++)
            {
                int number = i - startIndex + 1;
                result += $"{number}. {activities[i].Timestamp.ToShortTimeString()}: {activities[i].Description}\n";
            }

            return result;
        }

        /// Summary
        /// Gets full log with show more option
        public string GetFullLog()
        {
            if (activities.Count == 0)
            {
                return "No activities logged yet.";
            }

            string result = "Complete activity history:\n";
            for (int i = 0; i < activities.Count; i++)
            {
                result += $"{i + 1}. {activities[i].Timestamp.ToShortTimeString()}: {activities[i].Description}\n";
            }

            return result;
        }
    }

    /// Summary
    /// Activity entry class
    public class ActivityEntry
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = "";
    }
}