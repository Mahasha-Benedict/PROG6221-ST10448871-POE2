using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CybersecurityChatbotGUI
{
    /// Summary
    /// Handles all database operations for task management
    /// Implements CRUD operations: Create, Read, Update, Delete
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            // Updated with MySQL credentials
            connectionString = "Server=localhost;Database=cybersecurity_chatbot;Uid=root;Pwd=436372;";
        }

        /// Summary
        /// Adds a new task to the database
        public bool AddTask(string userName, string title, string description, DateTime? reminderDate)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO tasks (user_name, title, description, reminder_date) 
                                     VALUES (@userName, @title, @description, @reminderDate)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", userName);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@reminderDate", (object)reminderDate ?? DBNull.Value);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        /// Summary
        /// Gets all tasks for a user
        public List<TaskItem> GetTasks(string userName)
        {
            List<TaskItem> tasks = new List<TaskItem>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM tasks WHERE user_name = @userName ORDER BY created_at DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", userName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new TaskItem
                                {
                                    Id = reader.GetInt32("id"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    ReminderDate = reader.IsDBNull("reminder_date") ? null : reader.GetDateTime("reminder_date"),
                                    IsCompleted = reader.GetBoolean("is_completed"),
                                    CreatedAt = reader.GetDateTime("created_at")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return tasks;
        }

        /// Summary
        /// Marks a task as completed
        public bool MarkTaskComplete(int taskId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @taskId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        /// Deletes task from the database

        public bool DeleteTask(int taskId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tasks WHERE id = @taskId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
    }


    /// Task item class for storing task data

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "[COMPLETED]" : "[PENDING]";
            string reminder = ReminderDate.HasValue ? $" | Reminder: {ReminderDate.Value.ToShortDateString()}" : "";
            return $"{status} {Title} - {Description}{reminder}";
        }
    }
}