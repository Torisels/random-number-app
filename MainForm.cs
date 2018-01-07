using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using WPFCustomMessageBox;
namespace RandomNumberApp
{
    public partial class MainForm : Form
    {

        private static MainForm UiChanger;
        private readonly Time _time;
        private readonly Sql _sql;
        private readonly Db _db;
        private RandomM _rand;
        private readonly Ssh _ssh;

        public MainForm()
        {
            UiChanger = this;
            InitializeComponent();
            CustomInitialization();
            _db = new Db();
            _ssh = new Ssh();
            _sql = _ssh.EstablishMySQLConnection();
            handleLessonTime();
            _time = new Time(_db, _sql.GetBellOffset());
        }


        private void btnLosuj_Click(object sender, EventArgs e)
        {
            var s = _time.GetCurrentTime().ToString("dd-MM-yyyy");
            HashSet<int> people;
            if (_db.CheckIfDrawnToday(_time.verifiedDateString, out people))
            {
                var d = CustomMessageBox.ShowYesNoCancel(
                    $"W dniu {s} było przeprowadzone losowanie. Nowe losowanie usunie poprzedni zapis. Czy chcesz kontynować?",
                    "Uwaga!", "Tak", "Wczytaj poprzedni zapis", "Anuluj");
                if (d == MessageBoxResult.Cancel)
                    return;
                if (d == MessageBoxResult.Yes)
                    FillList();
                if (d == MessageBoxResult.No)
                {
                    FillList(people);
                }
            }
            else
            {
                FillList();
            }
        
        }

        private void FillList(HashSet<int> prop = null)
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            HashSet<int> randomPeopleHashSet = null;
            if (prop == null)
            {
                var rand = new RandomM(_db.GetProbability());
                 randomPeopleHashSet = rand.Randomize();
            }
            else
            {
                 randomPeopleHashSet = prop;
            }
           
            var fullRandomPeople = _db.GetDrawnPeople(randomPeopleHashSet);
            _db.InsertDrawnPeople(randomPeopleHashSet, _time.verifiedDateString);

            var propp = _db.GetProbabilityWithDate(_time.verifiedDateString);

            bool g = false;
            foreach (var ee in randomPeopleHashSet)
            {

                if (prop != null) 
                g = propp[ee] == 1;

                AddElementToDataGridView(ee, fullRandomPeople[ee], g);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                bool current = !(bool) dataGridView1.Rows[e.RowIndex].Cells[2].Value;
                _db.AddRemoveUserPresence((int)dataGridView1.Rows[e.RowIndex].Cells[0].Value, _time.verifiedDateString , current);

                dataGridView1.Rows[e.RowIndex].Cells[2].Value = current;
            }
        }
        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
                dataGridView1.ClearSelection();
        }

        private void AddElementToDataGridView(int number, string name,bool checkbox)
        {
            dataGridView1.Rows.Add(number, name, checkbox);
        }

        //make columns not sortable
        private void CustomInitialization()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        
        //this method display how much time is left till the end of the lesson
        public static void SetTextOnTimeLabel(string s)
        {
            if (UiChanger.labelTillTheEnd.InvokeRequired)
            {
                Action<string> method = SetTextOnTimeLabel;
                UiChanger.BeginInvoke(method, s);
            }
            else
            {
                UiChanger.labelTillTheEnd.Text = s;
            }
        }

        public void test()
        {
           //Time t = new Time();
            Console.WriteLine(
            Time.GetTimeDifference(DateTime.Parse("16:14"), DateTime.Parse("16:50")));
       //     t.checkIfLessonIsToday();
            RandomM m = new RandomM(new Dictionary<int, int>{});
            Console.WriteLine("xd");

        }

        public async void handleLessonTime()
        {
            await Task.Delay(50);
            await Task.Run(() =>
            {
              
                while (true)
                {
                    if (!_time.LessonToday)
                    {
                        SetTextOnTimeLabel("Nie ma dzisiaj lekcji.");
                        return;
                    }
                    if (_time.GetCurrentTime()>_time.getTodayEndLessonTime())
                    {
                        SetTextOnTimeLabel("Lekcje skończyły się.");
                        return;
                    }

                    int lesson = _time.DetermineNumberOfLesson();

                    if (lesson == -1)
                    {
                        SetTextOnTimeLabel("Najbliższa lekcja rozpocznie się za: "+ Time.GetTimeDifference(_time.GetCurrentTime(), _time.getLessonTimeStart(_time.GetNearestLesson())));
 
                        return;
                    }
                        SetTextOnTimeLabel("Do końca lekcji pozostało: "+ Time.GetTimeDifference(_time.GetCurrentTime(), _time.getLessonTimeEnd(lesson)));
                    
                }
            });
        }

        private void labelTillTheEnd_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(
            _sql.GetBellOffset());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
