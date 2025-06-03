using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.IO;

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
            LoadTasksFromFile();

            var view = CollectionViewSource.GetDefaultView(Tasks);
            view.Filter = FilterTasks;
        }

        private readonly string dataFile = "tasks.txt";

        private void SaveTasksToFile()
        {
            var lines = new List<string>();

            foreach (var task in Tasks)
            {
                
                string line = $"{Escape(task.Title)};{Escape(task.Description)};{task.DueDate:yyyy-MM-dd};{task.Priority};{task.IsCompleted}";
                lines.Add(line);
            }

            File.WriteAllLines(dataFile, lines);
        }

        
        private string Escape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            
            return s.Replace(";", "\\;").Replace("\n", "\\n").Replace("\r", "");
        }
        private void LoadTasksFromFile()
        {
            if (!File.Exists(dataFile))
                return;

            var lines = File.ReadAllLines(dataFile);
            Tasks.Clear();

            foreach (var line in lines)
            {
                var parts = SplitCsvLine(line);

                if (parts.Length == 5)
                {
                    var task = new TaskItem
                    {
                        Title = Unescape(parts[0]),
                        Description = Unescape(parts[1]),
                        DueDate = DateTime.TryParse(parts[2], out var dt) ? dt : DateTime.Now,
                        Priority = parts[3],
                        IsCompleted = bool.TryParse(parts[4], out var completed) && completed
                    };

                    Tasks.Add(task);
                }
            }
        }

        // Pomocnicza metoda do "rozpakowania" pola (odwrotność Escape)
        private string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            return s.Replace("\\;", ";").Replace("\\n", "\n");
        }

        // Rozdzielamy linię po średnikach z uwzględnieniem escape
        private string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool escape = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (escape)
                {
                    current.Append(line[i]);
                    escape = false;
                }
                else
                {
                    if (line[i] == '\\')
                    {
                        escape = true;
                    }
                    else if (line[i] == ';')
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                    else
                    {
                        current.Append(line[i]);
                    }
                }
            }
            result.Add(current.ToString());
            return result.ToArray();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveTasksToFile();
            base.OnClosing(e);
        }

        private void combosort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (combosort.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var view = CollectionViewSource.GetDefaultView(Tasks) as ListCollectionView;
            if (view == null) return;

            view.SortDescriptions.Clear();
            view.CustomSort = null;

            if (selected == "daty")
            {
                view.SortDescriptions.Add(new SortDescription(nameof(TaskItem.DueDate), ListSortDirection.Ascending));
            }
            else if (selected == "priorytetu")
            {
                view.CustomSort = new PriorityComparer();
            }
            else if (selected == "status ukończenia")
            {
                view.SortDescriptions.Add(new SortDescription(nameof(TaskItem.IsCompleted), ListSortDirection.Ascending));
            }
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
            string priorytet = (comboprio.SelectedItem as ComboBoxItem)?.Content?.ToString();

            Tasks.Add(new TaskItem
            {
                Title = tytul,
                Description = opis,
                DueDate = data,
                Priority = priorytet,
                IsCompleted = false
            });


            texttitle.Text = "Wpisz zadanie...";
            textdescrip.Text = "Dodaj krótki opis";
            datapic.SelectedDate = null;
            comboprio.SelectedIndex = 0;
        }

        private void texttitle_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox texttitle = sender as TextBox;
            if (texttitle.Text == "Wpisz zadanie...")
            {
                texttitle.Text = "";
            }

        }

        private void textdescrip_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textdescrip = sender as TextBox;
            if (textdescrip.Text == "Dodaj krótki opis")
            {
                textdescrip.Text = "";
            }
        }

        private void buttonzak_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;

                CollectionViewSource.GetDefaultView(Tasks).Refresh();
            }

        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is TaskItem taskToRemove)
            {
                Tasks.Remove(taskToRemove);
            }
        }
        private bool FilterTasks(object obj)
        {
            if (obj is TaskItem task)
            {
                if (radioAll.IsChecked == true)
                    return true;
                else if (radioCompleted.IsChecked == true)
                    return task.IsCompleted;
                else if (radioIncomplete.IsChecked == true)
                    return !task.IsCompleted;
            }
            return false;
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Tasks).Refresh();
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
        private string priority;
        public string Priority
        {
            get => priority;
            set { priority = value; OnPropertyChanged(nameof(Priority)); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
    public class PriorityComparer : IComparer
    {
        private readonly Dictionary<string, int> priorityOrder = new Dictionary<string, int>
    {
        { "Wysoki", 1 },
        { "Średni", 2 },
        { "Niski", 3 }
    };

        public int Compare(object x, object y)
        {
            if (x is TaskItem a && y is TaskItem b)
            {
                int aValue = priorityOrder.TryGetValue(a.Priority ?? "", out var ap) ? ap : int.MaxValue;
                int bValue = priorityOrder.TryGetValue(b.Priority ?? "", out var bp) ? bp : int.MaxValue;
                return aValue.CompareTo(bValue);
            }
            return 0;
        }

    }


}