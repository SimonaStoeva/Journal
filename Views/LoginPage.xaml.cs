using Journal.Services;
using System.Windows;
using System.Windows.Controls;

namespace Journal.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Reg_Click(sender, e);
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            string username = textBoxUserName.Text;
            string password = passwordBox1.Password;

            ClearError();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            bool isAuthenticated = AuthenticationService.LoginUser(username, password); 
            if (isAuthenticated)
            {
                int userId = AuthenticationService.Get_User_Id(username); 

                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NavigationService.Navigate(new NotesPage(userId));
            }
            else
            {
                ShowError("Invalid username or password.");
            }
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