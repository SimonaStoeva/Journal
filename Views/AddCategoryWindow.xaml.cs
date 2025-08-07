using System.Windows;

namespace Journal.Views
{
    public partial class AddCategoryWindow : Window
    {
        public string CategoryName { get; private set; }

        public AddCategoryWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Please enter a category name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryNameTextBox.Focus();
                return;
            }

            CategoryName = CategoryNameTextBox.Text.Trim();
            DialogResult = true;  
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}