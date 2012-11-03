using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SPTDataModel;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ToDoWindow : Window
    {


        ObservableCollection<ToDoData> _toDoCollection = new ObservableCollection<ToDoData>();


        public ObservableCollection<ToDoData> toDoCollection
        { get { return _toDoCollection; } }


        public ToDoWindow()
        {
            InitializeComponent();
        }

        private void todoAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                ToDoData tdData = AddNewToDo();
                _toDoCollection.Add(tdData);
               SptStorage.DbInteraction.ConnectInsertToDoListTable(tdData);
                todoInputTb.Text = null;
            }
        }

        private ToDoData AddNewToDo()
        {
            ToDoData tdData;
            tdData = new ToDoData();
            tdData.date_time = DateTime.Now;
            tdData.to_do = todoInputTb.Text;
            tdData.id = "T-" + DateTime.Now.ToOADate().ToString();

            return tdData;
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        private ToDoData GetSelectedItem()
        {

            ToDoData todoToEdit = null;

            if (todoListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                ToDoData i = (ToDoData)todoListView.SelectedItem;

                todoToEdit = _toDoCollection.Where(item => item.id.Equals(i.id)).First();
            }
            return todoToEdit;
        }

        private void delAddBtn_Click(object sender, RoutedEventArgs e)
        {
            ToDoData todoDelete;
            todoDelete = GetSelectedItem();
            if (todoDelete != null)
            {
                _toDoCollection.Remove(todoDelete);
                SptStorage.DbInteraction.DeleteTodo(todoDelete.id);
            }
        }

        private void printAddBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(todoListView, "To Do Items -" + DateTime.Now.ToString());
        }


       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTodoList();
        }

        private void showToDoItemsBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoList();
        }

        private void RefreshTodoList()
        {
            List<ToDoData> tdList = SptStorage.DbInteraction.fetcheToDoData();

            _toDoCollection.Clear();

            foreach (ToDoData data in tdList)
            {
                _toDoCollection.Add(data);
            }
        }




        #region validation

        private bool CheckDataAvailability()
        {
            bool returnVal = true;

            if (string.IsNullOrEmpty(todoInputTb.Text))
            {
                returnVal = false;
                MessageBox.Show("Enter To Do description");

            }
            return returnVal;
        }

        #endregion

    }





}
