using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace listaToDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TaskItem> Tasks { get; set; } = new ObservableCollection<TaskItem>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void change_data(object sender, SelectionChangedEventArgs e)
        {
            if (datapic.SelectedDate.HasValue)
                labdata.Content = datapic.SelectedDate.Value.ToString("dd.MM");
            else
                labdata.Content = "data";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string tytul = texttitle.Text;
            string opis = textdescrip.Text;
            DateTime data = datapic.SelectedDate ?? DateTime.Now;

            Tasks.Add(new TaskItem
            {
                Title = tytul,
                Description = opis,
                DueDate = data,
                IsCompleted = false
            });

            
            texttitle.Text = "Wpisz zadanie...";
            textdescrip.Text = "Dodaj krótki opis";
            datapic.SelectedDate = null;
            comboprio.SelectedIndex = 0;
        }
    }

    public class TaskItem : INotifyPropertyChanged
    {
        private string title;
        private string description;
        private DateTime dueDate;
        private bool isCompleted;

        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime DueDate
        {
            get => dueDate;
            set { dueDate = value; OnPropertyChanged(nameof(DueDate)); }
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set { isCompleted = value; OnPropertyChanged(nameof(IsCompleted)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 