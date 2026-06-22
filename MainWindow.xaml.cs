using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        private ChatbotGUI chatbot;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();

            //Audio plays when the window runs
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            chatbot.PlayGreeting();
        }

        private void InitializeChatbot()
        {
            chatbot = new ChatbotGUI(this);

            // Set up event handlers
            btnSend.Click += BtnSend_Click;
            txtUserInput.KeyDown += TxtUserInput_KeyDown;

            // Start the chatbot
            chatbot.Start();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendUserMessage();
        }

        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendUserMessage();
                e.Handled = true;
            }
        }

        private void SendUserMessage()
        {
            string message = txtUserInput.Text.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                AddToChatHistory($"You: {message}", System.Windows.Media.Brushes.Cyan);
                chatbot.ProcessMessage(message);
                txtUserInput.Clear();
                txtUserInput.Focus();
            }
        }

        public void AddToChatHistory(string message, System.Windows.Media.Brush color)
        {
            Dispatcher.Invoke(() =>
            {
                lstChatHistory.Items.Add(message);
                lstChatHistory.ScrollIntoView(lstChatHistory.Items[lstChatHistory.Items.Count - 1]);
            });
        }

        public void SetUserInfo(string info)
        {
            Dispatcher.Invoke(() =>
            {
                lblUserInfo.Text = info;
            });
        }

        public void SetSentiment(string sentiment)
        {
            Dispatcher.Invoke(() =>
            {
                lblSentiment.Text = $"Mood detected: {sentiment}";

                switch (sentiment.ToLower())
                {
                    case "positive":
                        lblSentiment.Foreground = new System.Windows.Media.SolidColorBrush(Colors.LightGreen);
                        break;
                    case "negative":
                        lblSentiment.Foreground = new System.Windows.Media.SolidColorBrush(Colors.LightCoral);
                        break;
                    default:
                        lblSentiment.Foreground = new System.Windows.Media.SolidColorBrush(Colors.LightYellow);
                        break;
                }
            });
        }

        public void DisplayAsciiArt()
        {
            string asciiArt = @"
    ╔══════════════════════════════════════════════════════════════════╗
    ║         ██████╗██╗   ██╗██████╗ ███████╗██████╗                  ║
    ║        ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗                 ║
    ║        ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝                 ║
    ║        ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗                 ║
    ║        ╚██████╗   ██║   ██████╔╝███████╗██║  ██║                 ║
    ║         ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝                 ║
    ║                                                                  ║
    ║         South African Cybersecurity Awareness Bot                ║
    ╚══════════════════════════════════════════════════════════════════╝";

            Dispatcher.Invoke(() =>
            {
                lblAsciiArt.Text = asciiArt;
            });
        }
        /// Summary
        /// Displays quiz question with answer options
        public void DisplayQuizQuestion(string question)
        {
            Dispatcher.Invoke(() =>
            {
                // Parse the question to extract multiple choice options if needed
                // The quiz manager handles formatting in the string
                AddToChatHistory($"Quiz: {question}", Brushes.Cyan);
            });
        }

        ///Summary
        /// Displays task list with formatting
        public void DisplayTaskList(string tasks)
        {
            Dispatcher.Invoke(() =>
            {
                AddToChatHistory($"Tasks: {tasks}", Brushes.LightBlue);
            });
        }
    }
}