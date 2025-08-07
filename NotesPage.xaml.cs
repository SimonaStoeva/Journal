using Journal.Models;
using Journal.Services;
using System.Windows;
using System.Windows.Controls;

namespace Journal
{
    public partial class NotesPage : Page
    {
        private int currentUserId;
        private Note currentNote;

        public NotesPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadNotes();
            LoadCategories();
        }

        private void LoadNotes()
        {
            List<Note> notes = NoteService.GetNotesByUser(currentUserId);
            NotesListView.ItemsSource = notes;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(searchText)
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadNotes(); 
            }
            else
            {
                var filteredNotes = NoteService.FilterByKeyword(currentUserId, searchText);
                NotesListView.ItemsSource = filteredNotes;
            }
        }

        private void NotesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListView.SelectedItem is Note selectedNote)
            {
                currentNote = selectedNote;
                TitleTextBox.Text = currentNote.Title;
                ContentTextBox.Text = currentNote.Content;
                NoteDateTextBlock.Text = currentNote.CreatedAt.ToString("g");

                if (currentNote.CategoryId.HasValue)
                {
                    var matchingCategory = CategoryComboBox.Items.Cast<Category>()
                        .FirstOrDefault(c => c.Id == currentNote.CategoryId.Value);

                    if (matchingCategory != null)
                    {
                        CategoryComboBox.SelectedItem = matchingCategory;
                    }
                }
                else
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
            }
        }

        private void NewNoteClick(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Clear();
            ContentTextBox.Clear();
            currentNote = null;
        }

        private void DeleteNoteClick(object sender, RoutedEventArgs e)
        {
            if (currentNote != null)
            {
                bool isDeleted = NoteService.DeleteNote(currentNote.Id);
                if (isDeleted)
                {
                    MessageBox.Show("Note deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNotes();
                    TitleTextBox.Clear();
                    ContentTextBox.Clear();
                    currentNote = null;
                }
                else
                {
                    MessageBox.Show("Failed to delete note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No note selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveNoteClick(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string content = ContentTextBox.Text;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please fill in both title and content.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            int? selectedCategoryId = null;
            if (CategoryComboBox.SelectedItem is Category selectedCategory && selectedCategory.Id != 0)
            {
                selectedCategoryId = selectedCategory.Id;
            }

            if (currentNote == null)
            {
                NoteService noteService = new NoteService();
                bool isAdded = noteService.AddNote(currentUserId, title, content, selectedCategoryId); 
                if (isAdded)
                {
                    MessageBox.Show("Note saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNotes();
                }
                else
                {
                    MessageBox.Show("Failed to save note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                bool isUpdated = NoteService.EditNote(currentNote.Id, title, content, selectedCategoryId);
                if (isUpdated)
                {
                    MessageBox.Show("Note updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNotes();
                }
                else
                {
                    MessageBox.Show("Failed to update note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadCategories()
        {
            
            var categoryService = new CategoryService();
            var categories = categoryService.GetCategoriesByUser(currentUserId);

            categories.Insert(0, new Category { Id = 0, Name = "All notes" });
            
            CategoryComboBox.ItemsSource = null;
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }
        
        private void NewCategoryClick(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryWindow
            {
                Owner = Window.GetWindow(this) 
            };

            if (addCategoryWindow.ShowDialog() == true)
            {
                var categoryName = addCategoryWindow.CategoryName;

                var categoryService = new CategoryService();
                var category = new Category
                {
                    Name = categoryName,
                    UserId = currentUserId
                };

                bool isAdded = categoryService.AddCategory(category);
                if (isAdded)
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

        private void LoadNotesByCategory(int categoryId)
        {
            var notes = NoteService.GetNotesByCategory(currentUserId, categoryId);
            NotesListView.ItemsSource = notes;
        }
        
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is Category selectedCategory)
            {
                if (currentNote == null)
                {
                    TitleTextBox.Clear();
                    ContentTextBox.Clear();
                    NoteDateTextBlock.Text = string.Empty;
                }
                
                if (selectedCategory.Id == 0) 
                {
                    LoadNotes();
                }
                else
                {
                    LoadNotesByCategory(selectedCategory.Id);
                }
            }
        }
    }
}
