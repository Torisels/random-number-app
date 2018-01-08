using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WPFCustomMessageBox;
namespace RandomNumberApp
{
    public partial class MainForm : Form
    {
        private static MainForm _uiChanger;
        private readonly Time _time;
        private readonly Db _db;

        public MainForm()
        {
            _uiChanger = this;
            InitializeComponent();
            CustomInitialization();
            _db = new Db();
            var ssh = new Ssh();
            var sql = ssh.EstablishMySqlConnection();

            _time = new Time(_db, sql.GetBellOffset());
            HandleLessonTime();
        }


        private void btnLosuj_Click(object sender, EventArgs e)
        {
            var s = _time.GetCurrentTime().ToString("dd-MM-yyyy");
            if (_db.CheckIfDrawnToday(_time.VerifiedDateString, out var people))
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

            HashSet<int> randomPeopleHashSet;
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
            _db.InsertDrawnPeople(randomPeopleHashSet, _time.VerifiedDateString);

            var propp = _db.GetProbabilityWithDate(_time.VerifiedDateString);

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
                _db.AddRemoveUserPresence((int)dataGridView1.Rows[e.RowIndex].Cells[0].Value, _time.VerifiedDateString , current);

                dataGridView1.Rows[e.RowIndex].Cells[2].Value = current;
            }
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
            if (_uiChanger.labelTillTheEnd.InvokeRequired)
            {
                Action<string> method = SetTextOnTimeLabel;
                _uiChanger.BeginInvoke(method, s);
            }
            else
            {
                _uiChanger.labelTillTheEnd.Text = s;
            }
        }
        public void HandleLessonTime()
        {
          Thread.Sleep(50);
            var t = new Task(() =>
            {
                while (true)
                {
                    if (!_time.LessonToday)
                    {
                        SetTextOnTimeLabel("Nie ma dzisiaj lekcji.");
                        return;
                    }
                    if (_time.GetCurrentTime() > _time.GetTodayEndLessonTime())
                    {
                        SetTextOnTimeLabel("Lekcje skończyły się.");
                        return;
                    }

                    int lesson = _time.DetermineNumberOfLesson();

                    if (lesson == -1)
                    {
                        SetTextOnTimeLabel("Najbliższa lekcja rozpocznie się za: " + Time.GetTimeDifference(_time.GetCurrentTime(), _time.GetLessonTimeStart(_time.GetNearestLesson())));

                        return;
                    }
                    SetTextOnTimeLabel("Do końca lekcji pozostało: " + Time.GetTimeDifference(_time.GetCurrentTime(), _time.GetLessonTimeEnd(lesson)));

                }
            });
          t.Start();
        }
    }
}
