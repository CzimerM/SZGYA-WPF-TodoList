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

namespace SZGYA_WPF_TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TodoItem.wInstance = this;
            lstTodoBox.Items.Add(new TodoItem() { Title = "test", tesztadat = true });
            lstTodoBox.Items.Add(new TodoItem() { Title = "test2", tesztadat = true });
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
            lstTodoBox.Items.Add(new TodoItem() { Title = txbTask.Text });
        }

        private void btnDeleteTestData(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in lstTodoBox.Items)
            {
                //if (((TodoItem)(item.DataContext)).tesztadat) lstTodoBox.Items.Remove(item);
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

        public static MainWindow wInstance;

        public string Title { get; set; }
        public void btnHandler(Button btn)
        {
            int itemIndex = wInstance.lstTodoBox.Items.IndexOf(this);
            switch (btn.Content.ToString())
            {
                case "Kész":
                    this.Title = ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1]).Text;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Collapsed;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Collapsed;
                    wInstance.lstTodoBox.Items.Refresh();
                    break;
                case "Módosítás":
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Visible;
                    ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Visible;
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

       