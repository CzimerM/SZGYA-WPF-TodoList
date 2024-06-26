﻿using System;
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
using SZGYA_WPF_TodoList.Dialogs;
using Newtonsoft.Json;

namespace SZGYA_WPF_TodoList
{
    /// <summary>
    /// WPF TODOList (Csizmadia Márk - Airamek)
    /// https://github.com/CzimerM/SZGYA-WPF-TodoList
    /// A blogon leírt összes követelménynek megfelel.
    /// Extra:
    ///     - Végtelen lista támogatás
    ///     - A ui követelmények szabadon értelmezése
    /// Nem működik:
    ///     - Nincs címsor
    ///     - A mentés/betöltés funkcionalitás nem működik
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow instance;

        public MainWindow()
        {
            MainWindow.instance = this;
            TodoListWindow.mainWInstance = this;
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
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

        private void btnCommandHandler(object sender, RoutedEventArgs e)
        {
            // Retrieve the TodoItem associated with the button
            var button = sender as Button;
            if (button != null && button.DataContext is TodoList todoList)
            {
                todoList.btnHandler(button);
            }
        }

        public void handleListDelete(TodoListWindow item)
        {
            for (int i = lstbxTodoLists.Items.Count - 1; i >= 0; i--)
            {
                if (lstbxTodoLists.Items[i] is TodoList tItem && tItem.wInstance == item) 
                {
                    lstbxTodoLists.Items.RemoveAt(i);
                }
            }
        }

        private void btnNewTodo(object sender, RoutedEventArgs e)
        {
            TodoListWindow w = new TodoListWindow(txbTask.Text);
            w.Show();
            lstbxTodoLists.Items.Add(new TodoList() { wInstance = w });
        }


        public struct SimpleTodoItem
        {
            public string Title;
            public bool tesztadat;
        }

        public class TodoList
        {
            public static void Swap(ItemCollection list, int indexA, int indexB)
            {
                object tmp = list[indexA];
                list[indexA] = list[indexB];
                list[indexB] = tmp;
            }

            [JsonIgnore] public TodoListWindow wInstance;
            public string Title {
                get
                {
                    return wInstance.Title;
                }
                set { wInstance.Title = value; }
            }

            public List<SimpleTodoItem> Items { 
                get 
                { 
                    List<SimpleTodoItem> res = new List<SimpleTodoItem>();
                    foreach(var item in wInstance.lstTodoBox.Items)
                    {
                        if (item is TodoItem tItem) res.Add(new SimpleTodoItem() { Title = tItem.Title, tesztadat = tItem.tesztadat});
                    }
                    return res; 
                } 
            }

            public override string ToString() => Title;

            public void btnHandler(Button btn)
            {
                int itemIndex = MainWindow.instance.lstbxTodoLists.Items.IndexOf(this);
                switch (btn.Content.ToString())
                {
                    case "Mentés":
                        break;
                    case "Kész":
                        this.Title = ((TextBox)((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1]).Text;
                        ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[1].Visibility = Visibility.Collapsed;
                        ((StackPanel)((DockPanel)((StackPanel)btn.Parent).Parent).Children[0]).Children[2].Visibility = Visibility.Collapsed;
                        MainWindow.instance.lstbxTodoLists.Items.Refresh();
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
                        Swap(MainWindow.instance.lstbxTodoLists.Items, itemIndex, itemIndex - 1);
                        break;
                    case "↓":
                        if (itemIndex == MainWindow.instance.lstbxTodoLists.Items.Count) break;
                        Swap(MainWindow.instance.lstbxTodoLists.Items, itemIndex, itemIndex + 1);
                        break;
                    case "Törlés":
                        wInstance.Close();
                        MainWindow.instance.lstbxTodoLists.Items.Remove(this);
                        break;
                    default:
                        break;
                }
            }
        }

        private void saveTestClick_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainWindow.instance.lstbxTodoLists.Items)
            {
                if (item is TodoList tItem)
                {
                    MessageBox.Show(JsonConvert.SerializeObject(tItem));
                }
            }
            
        }
    }
}
