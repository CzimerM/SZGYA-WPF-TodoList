using System;
using System.Windows;

namespace SZGYA_WPF_TodoList.Dialogs
{
    public partial class CopyDialogBox : Window
    {
        public CopyDialogBox(List<TodoListWindow> lists)
        {
            InitializeComponent();
            lstvAnswer.ItemsSource = lists.Select(l => l.Title).ToList();;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        public int Answer
        {
            get { return lstvAnswer.SelectedIndex; }
        }
    }
}