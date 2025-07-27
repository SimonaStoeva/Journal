using Journal.Services;
using System.Windows;
using System.Windows.Controls;

namespace Journal
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            string username = textBoxUserName.Text;
            string password = passwordBox1.Password;
            string confirmPassword = passwordBoxConfirm.Password;

            ClearError();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please fill in all fields!");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match!");
                return;
            }

            AuthenticationService authService = new AuthenticationService();
            bool isRegistered = authService.RegisterUser(username, password);
            if (isRegistered)
            {
                int userId = AuthenticationService.Get_User_Id(username); 

                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NavigationService.Navigate(new NotesPage(userId));

                ClearForm();
            }
            else
            {
                ShowError("Error! The username might already be taken.");
            }
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Log_Click(sender, e);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            ClearError();
        }

        private void ClearForm()
        {
            textBoxUserName.Text = "";
            passwordBox1.Password = "";
            passwordBoxConfirm.Password = "";
        }

        private void ShowError(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }

        private void ClearError()
        {
            ErrorMessageTextBlock.Text = "";
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
