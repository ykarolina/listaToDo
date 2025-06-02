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