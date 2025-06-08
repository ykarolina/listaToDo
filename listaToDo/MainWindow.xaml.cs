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
using System.Windows.Threading;



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
            //przypisanie funkcji która wykona się zaraz po załadowaniu aplikacji
            this.Loaded += MainWindow_Loaded;

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //alert
            ShowOneDayLeftAlerts();
        }

        private readonly string dataFile = "tasks.txt";

        //zapisywanie zadań do pliku
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

        //zastepowanie znaków
        private string Escape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            
            return s.Replace(";", "\\;").Replace("\n", "\\n").Replace("\r", "");
        }
        //odwracanie znaków
        private string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            return s.Replace("\\;", ";").Replace("\\n", "\n");
        }
        //wczytywanie zadań z pliku
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
        //pokazywanie alertów dla zadań które mają zostać ukończone za 1 dzień
        private void ShowOneDayLeftAlerts()
        {
            var tasksDueTomorrow = Tasks.Where(t =>
                (t.DueDate.Date - DateTime.Now.Date).TotalDays == 1 && !t.IsCompleted).ToList();

            foreach (var task in tasksDueTomorrow)
            {
                MessageBox.Show($"Został 1 dzień do ukończenia zadania:\n{task.Title}", "Przypomnienie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        // Metoda do podziału linii CSV na części (dzielenie na osobne zdania)
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
        //zapis zadań do pliku przy zamykaniu aplikacji
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveTasksToFile();
            base.OnClosing(e);
        }
        // Sortowanie zadań według wybranej opcji w ComboBoxie
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
        //Zmiana daty w labelu po wybraniu daty z DatePicker
        private void change_data(object sender, SelectionChangedEventArgs e)
        {
            if (datapic.SelectedDate.HasValue)
                labdata.Content = datapic.SelectedDate.Value.ToString("dd.MM");
            else
                labdata.Content = "data";
        }
        //Usuwanie wszystkich zadań po kliknięciu przycisku
        private void btndeleteall_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć wszystkie zadania?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Tasks.Clear();
                SaveTasksToFile();
            }
        }
        // Dodawanie nowego zadania po kliknięciu przycisku
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string tytul = texttitle.Text;
            string opis = textdescrip.Text;
            DateTime data = datapic.SelectedDate ?? DateTime.Now;
            string priorytet = (comboprio.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(tytul) || tytul == "Wpisz tytuł...")
            {
                MessageBox.Show("Proszę wprowadzić tytuł zadania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(opis) || opis == "Dodaj krótki opis")
            {
                MessageBox.Show("Proszę wprowadzić opis zadania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (datapic.SelectedDate == null)
            {
                MessageBox.Show("Proszę wybrać datę zadania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (comboprio.SelectedIndex == 0)
            {
                MessageBox.Show("Proszę wybrać priorytet zadania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Tasks.Add(new TaskItem
            {
                Title = tytul,
                Description = opis,
                DueDate = data,
                Priority = priorytet,
                IsCompleted = false
            });
            SaveTasksToFile();
            // Czyszczenie pól po dodaniu zadania
            texttitle.Text = "Wpisz tytuł...";
            textdescrip.Text = "Dodaj krótki opis";
            datapic.SelectedDate = null;
            comboprio.SelectedIndex = 0;
        }
        //ustawianie textu w textboxach po kliku na nie
        private void texttitle_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox texttitle = sender as TextBox;
            if (texttitle.Text == "Wpisz tytuł...")
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
        //zmiana statu ukończenia zadania po kliknięciu przycisku
        private void buttonzak_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;
                //odświeżenie widoku po zmianie statusu
                CollectionViewSource.GetDefaultView(Tasks).Refresh();
            }

        }
        // Usuwanie zadania po kliknięciu przycisku
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is TaskItem taskToRemove)
            {
                Tasks.Remove(taskToRemove);
                SaveTasksToFile();
            }
        }
        // Filtracja zadań według wybranej opcji (wszystkie, ukończone, nieukończone)
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
        //funkcja odswiezająca widok po zmianie stanu radio buttonów
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Tasks).Refresh();
        }

    }
    //classa TAskItem reprezentująca pojedyncze zadanie 
    public class TaskItem : INotifyPropertyChanged
    {
        //pola klasy TaskItem
        private string title;
        private string description;
        private DateTime dueDate;
        private bool isCompleted;
        private string priority;

        //właściwości Title, Description, DueDate, IsCompleted, Priority i IsOneDayLeft
        public string Title
        {
            
            get => title;//zwraca tytuł zadania
            set { title = value; OnPropertyChanged(nameof(Title)); }//ustawia nową wartość title i wywołuje metodę OnPropertyChanged
        }
        
        public string Description
        {
            get => description;//zwraca opis zadania
            set { description = value; OnPropertyChanged(nameof(Description)); }//ustawia nową wartość description i wywołuje metodę OnPropertyChanged
        }
        
        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                if (dueDate != value)//sprawdzanie, czy nowa data różni się od aktualnej wartości pola dueDate.
                {
                    dueDate = value;
                    OnPropertyChanged(nameof(DueDate));
                    OnPropertyChanged(nameof(IsOneDayLeft));  
                }
            }
        }
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (isCompleted != value)// sprawdzanie, czy nowa wartość różni się od aktualnej wartości pola isCompleted.
                {
                    isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                    OnPropertyChanged(nameof(IsOneDayLeft));  
                }
            }
        }
        public string Priority
        {
            get => priority;// zwraca priorytet zadania
            set { priority = value; OnPropertyChanged(nameof(Priority)); //ustawia nową wartość priority i wywołuje metodę OnPropertyChanged
            }
        }
        //sprawdzanie czy zadanie ma zostać ukończone za 1 dzień
        public bool IsOneDayLeft
        {
            get
            {
                var daysLeft = (DueDate.Date - DateTime.Now.Date).TotalDays;// obliczanie różnicy dni między datą zadania a aktualną datą
                return !IsCompleted && daysLeft == 1;// zwraca true jeśli zadanie nie jest ukończone i zostało 1 dzień do jego ukończenia
            }
        }
        public void Refresh()//fukcja odświeżająca widok zadania
        {
            OnPropertyChanged(nameof(IsOneDayLeft));
        }

        public event PropertyChangedEventHandler PropertyChanged;//zdarzenie które jest wywoływane gdy zmienia się wartość dowolnej właściwości klasy.
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));//powiadomienie o zmianie wartości właściwości jeśli jest zarejestrowany jakiś odbiorca tego zdarzenia.
    }
    public class PriorityComparer : IComparer//porównywanuje zadania według priorytetu
    {
        private readonly Dictionary<string, int> priorityOrder = new Dictionary<string, int>
    {
        { "Wysoki", 1 },
        { "Średni", 2 },
        { "Niski", 3 }
    };

        public int Compare(object x, object y)
        {
            if (x is TaskItem a && y is TaskItem b)//porównuje dwa zadania na podstawie ich priorytetu
            {
                int aValue = priorityOrder.TryGetValue(a.Priority ?? "", out var ap) ? ap : int.MaxValue;
                int bValue = priorityOrder.TryGetValue(b.Priority ?? "", out var bp) ? bp : int.MaxValue;
                return aValue.CompareTo(bValue);
            }
            return 0;
        }

    }


}