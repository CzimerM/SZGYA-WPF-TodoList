using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.StylusWisp;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SZGYA_WPF_TodoList.Dialogs;

namespace SZGYA_WPF_TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TodoListWindow : Window
    {
        public static List<TodoListWindow> openWindowList = new List<TodoListWindow>();
        public string Title { get; set; }

        public TodoListWindow()
        {
            InitializeComponent();
            lstTodoBox.Items.SortDescriptions.Clear();
            lstTodoBox.Items.Add(new TodoItem() { Title = "test", tesztadat = true, wInstance = this });
            lstTodoBox.Items.Add(new TodoItem() { Title = "test2", tesztadat = true, wInstance = this });
            Title = $"{openWindowList.Count}";
        }

        public void wndClosed(object sender, EventArgs e)
        {
            openWindowList.Remove(this);
        }

        private void btnCommandHandler(object sender, RoutedEventArgs e)
        {
            // Retrieve the TodoItem associated with the button
            var button = sender as Button;
            if (button != null && button.DataContext is TodoItem todoItem)
            {
                todoItem.btnHandler(button);
            }
        }

        private void btnAddTask(object sender, RoutedEventArgs e)
        {
            foreach (object item in lstTodoBox.Items)
            {
                if (item is TodoItem tItem)
                {
                    if (tItem.Title == txbTask.Text)
                    {
                        MessageBox.Show("Van már ilyen feladat!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            lstTodoBox.Items.Add(new TodoItem() { Title = txbTask.Text, wInstance = this});
        }

        private void btnDeleteTestData(object sender, RoutedEventArgs e)
        {
            for (int i = lstTodoBox.Items.Count - 1; i >= 0; i--)
            {
                if (lstTodoBox.Items[i] is TodoItem tItem && tItem.tesztadat == true)
                {
                    lstTodoBox.Items.Remove(tItem);
                }
            }
        }

        private void btnNewList(object sender, RoutedEventArgs e)
        {
            TodoListWindow window2 = new TodoListWindow();
            window2.Show();
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void btnOrder(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Name)
            {
                case "btnOrderAZ":
                    lstTodoBox.Items.SortDescriptions.Clear();
                    lstTodoBox.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("Title",
                            System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "btnOrderZA":
                    lstTodoBox.Items.SortDescriptions.Clear();
                    lstTodoBox.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("Title",
                            System.ComponentModel.ListSortDirection.Descending));
                    break;
                default:
                    break;
            }
        }

    }

    public class TodoItem
    {
        public bool tesztadat = false;
        public static void Swap(ItemCollection list, int indexA, int indexB)
        {
            object tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public TodoItem()
        {
            
        }
        public TodoItem(TodoItem old, TodoListWindow newwInstance)
        {
            Title = old.Title;
            tesztadat = old.tesztadat;
            wInstance = newwInstance;
        }
        
        public TodoListWindow wInstance;

        public string Title { get; set; }
        public void btnHandler(Button btn)
        {
            int itemIndex = wInstance.lstTodoBox.Items.IndexOf(this);
            switch (btn.Content.ToString())
            {
                case "Másolás":
                    CopyDialogBox box = new CopyDialogBox(TodoListWindow.openWindowList);
                    if (box.ShowDialog() == true)
                    {
                        TodoListWindow.openWindowList[box.Answer].lstTodoBox.Items.Add(new TodoItem(this, TodoListWindow.openWindowList[box.Answer]));
                    }
                    break;
                case "Kész":
                    this.Title = ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1]).Text;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Collapsed;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Collapsed;
                    wInstance.lstTodoBox.Items.Refresh();
                    break;
                case "Módosítás":
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Visible;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Visible;
                    ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1]).Text =
                        Title;
                    ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1])
                        .Focus();
                    break;
                case "↑":
                    if (itemIndex == 0) break;
                    Swap(wInstance.lstTodoBox.Items, itemIndex, itemIndex - 1);
                    break;
                case "↓":
                    if (itemIndex == wInstance.lstTodoBox.Items.Count) break;
                    Swap(wInstance.lstTodoBox.Items, itemIndex, itemIndex + 1);
                    break;
                case "✓":
                    wInstance.lstTodoBox.Items.Remove(this);
                    break;
                default:
                    break;
            }
        }
    }
}

       