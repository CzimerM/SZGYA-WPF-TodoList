using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            TodoListWindow.mainWInstance = this;
            InitializeComponent();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }

        private void btnExit(object sender, RoutedEventArgs e)
        {
            int amountDone = 0;
            for (int i = lstbxTodoLists.Items.Count - 1; i >= 0; i--)
            {
                if (lstbxTodoLists.Items[i] is TodoList tItem)
                {
                    amountDone += tItem.wInstance.OpAmount;
                    tItem.wInstance.Close();
                }
            }
            MessageBox.Show($"Elvégzett műveletek száma: {amountDone}", "Kilépés", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        public void handleListDelete(TodoListWindow item)
        {
            for (int i = lstbxTodoLists.Items.Count - 1; i >= 0; i--)
            {
                if (((TodoList)lstbxTodoLists.Items[i]).wInstance == item) 
                {
                    lstbxTodoLists.Items.RemoveAt(i);
                }
            }
        }

        private void btnNewTodo(object sender, RoutedEventArgs e)
        {
            TodoListWindow w = new TodoListWindow();
            w.Show();
            lstbxTodoLists.Items.Add(new TodoList() { wInstance = w});
        }


        public class TodoList
        {
            public TodoListWindow wInstance;
            public string Title { get
                {
                    return wInstance.Title;
                }
                set { wInstance.Title = value; }
            }

            public ItemCollection Items { get { return wInstance.lstTodoBox.Items; } }

            public override string ToString() => Title;
        }
    }
}
