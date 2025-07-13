using Journal.Data;
using System.Windows;

namespace Journal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseManager.InitializeDatabase();
        }

        private void Show_Home()
        {
            
            HomeGrid.Visibility = Visibility.Visible;
            MainFrame.Visibility = Visibility.Collapsed;
        }

        public void Log_Click(object sender, RoutedEventArgs e)
        {
            HomeGrid.Visibility = Visibility.Collapsed; 
            MainFrame.Visibility = Visibility.Visible;  
            //MainFrame.Navigate(new LoginPage());        
        }

        public void Reg_Click(object sender, RoutedEventArgs e)
        {
            HomeGrid.Visibility = Visibility.Collapsed; 
            MainFrame.Visibility = Visibility.Visible;  
            //MainFrame.Navigate(new RegistrationPage()); 
        }

    }
}