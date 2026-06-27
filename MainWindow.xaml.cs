using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CyberSecurityBotGUI
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private List<string> activityLog = new List<string>();
        private int quizScore = 0;
        private int quizIndex = 0;
        private List<QuizQuestion> quizQuestions;
        private bool quizActive = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuizQuestions();
            txtUserInput.Focus();
            ShowWelcomeMessage();
            UpdateQuizStatus();
        }

        private void ShowWelcomeMessage()
        {
            string asciiArt = @"
       ******       ******
      **********   **********
    ************* *************
   ***************************
   ***************************
    *************************
     ***********************
      *********************
       *******************
        *****************
         ***************
          *************
           ***********
            *********
             *******
              *****
               ***
                *
    ✨🌸  By Xolile Chilli  🌸✨
                                                                             
";
            AppendToChat(asciiArt);
            AppendToChat("🔐 Welcome to the CyberSecurity Bot! I'm here to help you with tasks, reminders, and quizzes.");
            AppendToChat("📌 Commands: 'add task to ...', 'remind me to ...', 'quiz', 'activity log'");
            AppendToChat("💡 Use the menu above or type commands directly.\n");
        }

        private void InitializeQuizQuestions()
        {
            //questions
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion("What should you do if you receive an email asking for your password?",
                    new[] {"a. Reply with your password", "b. Delete the email", "c. Report the email as phishing", "d. Ignore it"},
                    "c", "Reporting phishing emails helps prevent scams."),

                new QuizQuestion("True or False: Using '123456' is a secure password.",
                    new[] {"a. True", "b. False"}, "b", "'123456' is one of the most commonly used and insecure passwords."),

                new QuizQuestion("Which is an example of a strong password?",
                    new[] {"a. Password123", "b. MyBirthday!", "c. p@55w0rd!9$", "d. 12345678"},
                    "c", "A mix of uppercase, lowercase, numbers, and symbols makes a password strong."),

                new QuizQuestion("What is phishing?",
                    new[] {"a. A type of fishing hobby", "b. A scam to trick you into revealing personal info", "c. A secure way to send emails", "d. An antivirus program"},
                    "b", "Phishing is a social engineering attack to steal sensitive data."),

                new QuizQuestion("True or False: You should share your passwords with coworkers.",
                    new[] {"a. True", "b. False"}, "b", "Never share passwords; use approved sharing methods if needed."),

                new QuizQuestion("What does 2FA (Two-Factor Authentication) add to security?",
                    new[] {"a. A second password", "b. A second layer of verification", "c. A fingerprint", "d. A physical key"},
                    "b", "2FA requires something you know and something you have."),

                new QuizQuestion("Which is NOT safe on public Wi-Fi?",
                    new[] {"a. Using a VPN", "b. Accessing your bank account", "c. Turning off file sharing", "d. Using HTTPS sites"},
                    "b", "Avoid sensitive transactions on public Wi-Fi unless using a VPN."),

                new QuizQuestion("What should you do if your account is compromised?",
                    new[] {"a. Ignore it", "b. Change your password immediately", "c. Wait and see", "d. Post about it on social media"},
                    "b", "Immediately change password and notify the service provider."),

                new QuizQuestion("True or False: Antivirus is the only protection you need.",
                    new[] {"a. True", "b. False"}, "b", "You also need updates, firewalls, and safe habits."),

                new QuizQuestion("How often should you update your software?",
                    new[] {"a. Once a year", "b. Only when prompted", "c. Regularly when updates are available", "d. Never"},
                    "c", "Regular updates patch security vulnerabilities.")
            };
        }

        private void UpdateQuizStatus()
        {
            if (quizActive)
            {
                lblQuizStatus.Content = $"Quiz: Question {quizIndex + 1} of {quizQuestions.Count}";
                btnCancelQuiz.Visibility = Visibility.Visible;
                menuCancelQuiz.IsEnabled = true;
            }
            else
            {
                lblQuizStatus.Content = "Quiz: Not started";
                btnCancelQuiz.Visibility = Visibility.Collapsed;
                menuCancelQuiz.IsEnabled = false;
            }
        }

        private void StartQuiz()
        {
            if (quizActive)
            {
                MessageBox.Show("A quiz is already in progress. Finish or cancel it first.", "Quiz Active", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            quizScore = 0;
            quizIndex = 0;
            quizActive = true;
            activityLog.Add("Quiz started");
            UpdateQuizStatus();
            AskQuizQuestion();
        }

        private void CancelQuiz()
        {
            if (!quizActive) return;
            var result = MessageBox.Show("Are you sure you want to cancel the quiz? Your progress will be lost.", "Cancel Quiz", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Show score out of answered questions
                int answered = quizIndex; // number of questions answered so far
                if (answered > 0)
                {
                    AppendToChat($"📊 Quiz cancelled. You answered {quizScore} out of {answered} questions correctly.");
                }
                else
                {
                    AppendToChat("📊 Quiz cancelled before any questions were answered.");
                }
                quizActive = false;
                AppendToChat("Bot: Quiz cancelled.");
                activityLog.Add("Quiz cancelled");
                UpdateQuizStatus();
            }
        }

        private void AskQuizQuestion()
        {
            if (quizIndex >= quizQuestions.Count)
            {
                // Quiz completed - show total score out of all questions
                AppendToChat($"🏁 Quiz complete! Score: {quizScore}/{quizQuestions.Count}");
                if (quizScore >= 8)
                    AppendToChat("🌟 Great Job! You’re a cybersecurity pro!");
                else
                    AppendToChat("📚 Keep learning to stay safe online!");
                activityLog.Add($"Quiz completed – Score: {quizScore}/{quizQuestions.Count}");
                quizActive = false;
                UpdateQuizStatus();
                return;
            }

            var q = quizQuestions[quizIndex];
            AppendToChat($"📝 Question {quizIndex + 1} of {quizQuestions.Count}: {q.Question}\n{string.Join("\n", q.Choices)}");
            UpdateQuizStatus();
        }

        //Event Handlers

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
                e.Handled = true;
            }
        }

        private void btnAnswer_Click(object sender, RoutedEventArgs e)
        {
            string answer = txtUserInput.Text.Trim();
            txtUserInput.Clear();

            if (!quizActive)
            {
                AppendToChat("Bot: No quiz is active. Start one by typing 'quiz' or using the Quiz menu.");
                return;
            }

            if (quizIndex >= quizQuestions.Count) return;

            var q = quizQuestions[quizIndex];
            if (q.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
            {
                AppendToChat("✅ Correct! " + q.Feedback);
                quizScore++;
            }
            else
            {
                AppendToChat("❌ Incorrect. " + q.Feedback);
            }
            quizIndex++;
            AskQuizQuestion();
        }

        private void btnCancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            CancelQuiz();
        }

        private void ProcessUserInput()
        {
            string input = txtUserInput.Text.Trim();
            txtUserInput.Clear();

            if (string.IsNullOrWhiteSpace(input)) return;

            AppendToChat("You: " + input);
            NLPHandleInput(input);
        }

        private void NLPHandleInput(string input)
        {
            string response = "";

            if (input.ToLower().Contains("add task") || Regex.IsMatch(input, "(?i)add.+task"))
            {
                string title = Regex.Match(input, "(?i)add.+task.+to (.+)").Groups[1].Value;
                if (string.IsNullOrEmpty(title)) title = "New Cyber Task";
                TaskItem task = new TaskItem { Title = title, Description = "Cybersecurity task: " + title };
                tasks.Add(task);
                activityLog.Add($"Task added: '{title}'");
                response = $"✅ Task added: '{title}'. Would you like to set a reminder?";
            }
            else if (input.ToLower().Contains("remind me") || input.ToLower().Contains("set a reminder"))
            {
                var match = Regex.Match(input, "remind me to (.+) (in|on) (.+)");
                if (match.Success)
                {
                    string task = match.Groups[1].Value;
                    string time = match.Groups[3].Value;
                    activityLog.Add($"Reminder set: '{task}' on {time}");
                    response = $"⏰ Reminder set for '{task}' on {time}.";
                }
                else
                {
                    response = "I didn't catch the reminder time. Please rephrase (e.g., 'remind me to scan in 10 minutes').";
                }
            }
            else if (input.ToLower().Contains("quiz"))
            {
                StartQuiz();
                return;
            }
            else if (input.ToLower().Contains("activity log") || input.ToLower().Contains("what have you done"))
            {
                response = "📋 Here’s a summary of recent actions:";
                var recent = activityLog.Count > 10 ? activityLog.GetRange(activityLog.Count - 10, 10) : activityLog;
                foreach (var log in recent)
                    response += "\n- " + log;
            }
            else
            {
                response = "🤖 I'm not sure how to help with that. Try asking about tasks, reminders, the quiz, or activity log. Use the menu for guidance.";
            }

            AppendToChat("Bot: " + response);
        }

        private void AppendToChat(string text)
        {
            txtChat.AppendText(text + "\n\n");
            txtChat.ScrollToEnd();
        }

        //Menu Handlers

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            CancelQuiz();
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            txtChat.Clear();
            ShowWelcomeMessage();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ShowGoodbyeAndExit();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ShowGoodbyeAndExit();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CyberSecurity Bot v2.0\n\nA WPF application for task management, reminders, and cybersecurity quizzes.\n\nDeveloped with ❤️ using WPF and C#.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- Exit Logic ---
        private void ShowGoodbyeAndExit()
        {
            string cowAscii = @"
 /\_/\
( o.o )
 > ^ <
";
            // Combine with "Cyber Security" beside it (simulated by a message)
            string goodbyeMsg = $"{cowAscii}\n\n🔒 Cyber Security Bot\n\nThank you for using the CyberSecurity Bot!\nStay safe and secure online. 👋";
            MessageBox.Show(goodbyeMsg, "Goodbye!", MessageBoxButton.OK, MessageBoxImage.Information);
            Application.Current.Shutdown();
        }

       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }
    }

    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool Completed { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Choices { get; set; }
        public string CorrectAnswer { get; set; }
        public string Feedback { get; set; }

        public QuizQuestion(string q, string[] c, string a, string f)
        {
            Question = q;
            Choices = c;
            CorrectAnswer = a;
            Feedback = f;
        }
    }
}