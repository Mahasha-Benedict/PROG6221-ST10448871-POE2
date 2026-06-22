using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /// Summary
    /// Manages THE cybersecurity tasks with database integration
    /// Task 1: Task Assistant with Reminders
    public class TaskManager
    {
        private DatabaseHelper dbHelper;
        private ActivityLogger logger;

        public TaskManager(ActivityLogger logger)
        {
            dbHelper = new DatabaseHelper();
            this.logger = logger;
        }

        /// Summary
        /// Adds a new task with optional reminder
        public string AddTask(string userName, string title, string description, DateTime? reminderDate)
        {
            try
            {
                bool success = dbHelper.AddTask(userName, title, description, reminderDate);

                if (success)
                {
                    logger.LogAction($"Task added: '{title}' - {description}");

                    if (reminderDate.HasValue)
                    {
                        logger.LogAction($"Reminder set for '{title}' on {reminderDate.Value.ToShortDateString()}");
                        return $"Task added with the description \"{description}\". Reminder set for {reminderDate.Value.ToShortDateString()}.";
                    }

                    return $"Task added with the description \"{description}\".";
                }

                return "Sorry, I couldn't add the task. Please try again.";
            }
            catch (Exception ex)
            {
                return $"Error adding task: {ex.Message}";
            }
        }

        /// Summary
        /// Gets all tasks for a user
        public string GetTasks(string userName)
        {
            List<TaskItem> tasks = dbHelper.GetTasks(userName);

            if (tasks.Count == 0)
            {
                return "You have no tasks yet. Would you like to add one?";
            }

            string result = "Here are your cybersecurity tasks:\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                result += $"{i + 1}. {tasks[i].ToString()}\n";
            }

            return result;
        }

        /// Summary
        /// Marks task as completed
        public string MarkTaskComplete(string userName, int taskIndex)
        {
            List<TaskItem> tasks = dbHelper.GetTasks(userName);

            if (taskIndex < 1 || taskIndex > tasks.Count)
            {
                return "Invalid task number. Please check your tasks and try again.";
            }

            TaskItem task = tasks[taskIndex - 1];
            bool success = dbHelper.MarkTaskComplete(task.Id);

            if (success)
            {
                logger.LogAction($"Task marked complete: '{task.Title}'");
                return $"Great job! Task '{task.Title}' marked as complete!";
            }

            return "Sorry, I couldn't mark the task as complete. Please try again.";
        }

        /// Deletes a task
        public string DeleteTask(string userName, int taskIndex)
        {
            List<TaskItem> tasks = dbHelper.GetTasks(userName);

            if (taskIndex < 1 || taskIndex > tasks.Count)
            {
                return "Invalid task number. Please check your tasks and try again.";
            }

            TaskItem task = tasks[taskIndex - 1];
            bool success = dbHelper.DeleteTask(task.Id);

            if (success)
            {
                logger.LogAction($"Task deleted: '{task.Title}'");
                return $"Task '{task.Title}' has been deleted.";
            }

            return "Sorry, I couldn't delete the task. Please try again.";
        }
    }
}