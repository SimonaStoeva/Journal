using Journal.Models;
using Journal.Services;
using System.Windows;
using System.Windows.Controls;

namespace Journal.Views
{
    public partial class NotesPage : Page
    {
        private readonly int currentUserId;
        private Note currentNote;
        private readonly NoteService noteService = new NoteService();
        private readonly CategoryService categoryService = new CategoryService();

        public NotesPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadCategories();
            LoadNotes();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.Logout();
        }

        private void LoadCategories()
        {
            var categories = categoryService.GetCategoriesByUser(currentUserId);
            categories.Insert(0, new Category { Id = 0, Name = "All notes" });

            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }
        
        private void LoadNotes()
        {
            NotesListView.ItemsSource = noteService.GetNotesByUser(currentUserId);
        }
        
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(searchText) 
                ? Visibility.Visible 
                : Visibility.Collapsed;

            NotesListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? noteService.GetNotesByUser(currentUserId)
                : noteService.FilterByKeyword(currentUserId, searchText);
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is Category selectedCategory)
            {
                NotesListView.ItemsSource = selectedCategory.Id == 0
                    ? noteService.GetNotesByUser(currentUserId)
                    : noteService.GetNotesByCategory(currentUserId, selectedCategory.Id);
            }
        }
        private void FilterByDate_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value.AddDays(1).AddTicks(-1); 


            var notes = noteService.FilterByDate(currentUserId, startDate, endDate);


            if (CategoryComboBox.SelectedItem is Category selectedCategory && selectedCategory.Id != 0)
            {
                notes = notes
                    .Where(n => n.CategoryId.HasValue && n.CategoryId.Value == selectedCategory.Id)
                    .ToList();
            }

            NotesListView.ItemsSource = notes;
        }


        private void NotesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListView.SelectedItem is Note selectedNote)
            {
                currentNote = selectedNote;
                TitleTextBox.Text = currentNote.Title;
                ContentTextBox.Text = currentNote.Content;
                NoteDateTextBlock.Text = currentNote.CreatedAt.ToString("g");

                CategoryComboBox.SelectionChanged -= CategoryComboBox_SelectionChanged;

                if (currentNote.CategoryId.HasValue)
                {
                    CategoryComboBox.SelectedItem = CategoryComboBox.Items
                        .Cast<Category>()
                        .FirstOrDefault(c => c.Id == currentNote.CategoryId.Value);
                }
                else
                {
                    CategoryComboBox.SelectedIndex = 0;
                }

                CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
            }
        }

        private void NewNoteClick(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Clear();
            ContentTextBox.Clear();
            NoteDateTextBlock.Text = string.Empty;
            currentNote = null;
        }

        private void SaveNoteClick(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string content = ContentTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please fill in both title and content.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? selectedCategoryId = (CategoryComboBox.SelectedItem as Category)?.Id;
            if (selectedCategoryId == 0) selectedCategoryId = null;

            bool success = currentNote == null
                ? noteService.AddNote(currentUserId, title, content, selectedCategoryId)
                : NoteService.EditNote(currentNote.Id, title, content, selectedCategoryId);

            if (success)
            {
                MessageBox.Show(currentNote == null ? "Note saved successfully." : "Note updated successfully.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadNotes();
            }
            
            else
            {
                MessageBox.Show("Failed to save note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteNoteClick(object sender, RoutedEventArgs e)
        {
            if (currentNote == null)
            {
                MessageBox.Show("No note selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NoteService.DeleteNote(currentNote.Id))
            {
                MessageBox.Show("Note deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadNotes();
                TitleTextBox.Clear();
                ContentTextBox.Clear();
                NoteDateTextBlock.Text = string.Empty;
                currentNote = null;
            }
            
            else
            {
                MessageBox.Show("Failed to delete note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewCategoryClick(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryWindow { Owner = Window.GetWindow(this) };
            if (addCategoryWindow.ShowDialog() == true)
            {
                var category = new Category
                {
                    Name = addCategoryWindow.CategoryName,
                    UserId = currentUserId
                };

                if (categoryService.AddCategory(category))
                {
                    MessageBox.Show("Category added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCategories();
                }
                else
                {
                    MessageBox.Show("Failed to add category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
