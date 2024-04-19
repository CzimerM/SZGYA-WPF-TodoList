using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
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
using static SZGYA_WPF_TodoList.MainWindow;

namespace SZGYA_WPF_TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TodoListWindow : Window
    {

        public static List<TodoListWindow> openWindowList = new List<TodoListWindow>();
        public string Title { get
            {
                return title;
            }
            set
            {
                title = value;
                lblTitle.Content = title;
            }
        }

        public int OpAmount { get; set; } = 0;

        public static MainWindow mainWInstance;

        private bool testDataDeleted = true;
        private string title;

        public TodoListWindow(string wTitle = "")
        {
            InitializeComponent();
            openWindowList.Add(this);
            lstTodoBox.Items.SortDescriptions.Clear();
            if (openWindowList.Count == 1)
            {
                lstTodoBox.Items.Add(new TodoItem() { Title = "test", tesztadat = true, wInstance = this });
                lstTodoBox.Items.Add(new TodoItem() { Title = "test2", tesztadat = true, wInstance = this });
                testDataDeleted = false;
            }
            if (wTitle == "") Title = $"TODO List - {openWindowList.Count}.";
            else Title = wTitle;
            lblTitle.Content = Title;

            this.KeyDown += new KeyEventHandler(wnd_KeyDown);
        }

        public void wndClosed(object sender, EventArgs e)
        {
            mainWInstance.handleListDelete(this);
            openWindowList.Remove(this);
        }

        void wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
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
            deleteTestData();
            btnDeleteTestData.IsEnabled = false;
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
            txbTask.Text = string.Empty;
            updateOpAmount();
        }

        private void deleteTestData()
        {
            if (!testDataDeleted)
            {
                for (int i = lstTodoBox.Items.Count - 1; i >= 0; i--)
                {
                    if (lstTodoBox.Items[i] is TodoItem tItem && tItem.tesztadat == true)
                    {
                        lstTodoBox.Items.Remove(tItem);
                    }
                }
                testDataDeleted = true;
                updateOpAmount();
            }
        }

        private void btnDeleteTestDataClick(object sender, RoutedEventArgs e)
        {
            deleteTestData();
            ((Button)sender).IsEnabled = false;
        }

        private void btnNewList(object sender, RoutedEventArgs e)
        {
            TodoListWindow window2 = new TodoListWindow();
            window2.Show();
            window2.btnDeleteTestData.IsEnabled = false;
            mainWInstance.lstbxTodoLists.Items.Add(new TodoList() { wInstance = window2 });
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void btnOrder(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            lstTodoBox.Items.SortDescriptions.Clear();
            switch (btn.Name)
            {
                case "btnOrderAZ":
                    lstTodoBox.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("Title",
                            System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "btnOrderZA":
                    lstTodoBox.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("Title",
                            System.ComponentModel.ListSortDirection.Descending));
                    break;
                default:
                    break;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }

        private void btnExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void updateOpAmount()
        {
            OpAmount++;
            lblOpCount.Content = $"Elvégzett műveletek: {OpAmount}";
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
                    wInstance.updateOpAmount();
                    break;
                case "Kész":
                    this.Title = ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1]).Text;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Collapsed;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Collapsed;
                    wInstance.lstTodoBox.Items.Refresh();
                    wInstance.updateOpAmount();
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
                    wInstance.updateOpAmount();
                    break;
                default:
                    break;
            }
        }
    }
}

       