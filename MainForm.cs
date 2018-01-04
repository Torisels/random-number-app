using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            _time = new Time(_db);
            _ssh = new Ssh();
            _sql = _ssh.EstablishMySQLConnection();
            handleLessonTime();




   ;
        }


        private void btnLosuj_Click(object sender, EventArgs e)
        {
            //            btnLosuj.Enabled = false;
            //            progressBar1.Visible = true;
            //            progressBar1.Style = ProgressBarStyle.Marquee;
            //
            //            await _connection.SendRequest();
            //
            //            btnLosuj.Enabled = true;
            //            progressBar1.Visible = false;



                var rand = new RandomM(_db.GetProbability());
                var randomPeopleHashSet = rand.Randomize();

                var fullRandomPeople = _db.GetDrawnPeople(randomPeopleHashSet);
                


            foreach (var ee in randomPeopleHashSet)
            {
                AddElementToDataGridView(ee, fullRandomPeople[ee],false);
            }

        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
//            {
//                var x = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
//                Console.WriteLine("sss"+x);
//            }
//        }

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
            await Task.Delay(1000);
            await Task.Run(() =>
            {
              
                while (true)
                {
                    if (!_time.checkIfLessonIsToday())
                    {
                        SetTextOnTimeLabel("Nie ma dzisiaj lekcji.");
                        return;
                    }
                    if (DateTime.Now>_time.getTodayEndLessonTime())
                    {
                        SetTextOnTimeLabel("Lekcje skończyły się.");
                        return;
                    }

                    int lesson = _time.DetermineNumberOfLesson();

                    if (lesson == -1)
                    {
                        SetTextOnTimeLabel("Najbliższa lekcja rozpocznie się za: "+ Time.GetTimeDifference(_time.MasterDate, _time.getLessonTimeStart(_time.GetNearestLesson())));
 
                        return;
                    }
                        SetTextOnTimeLabel("Do końca lekcji pozostało: "+ Time.GetTimeDifference(_time.MasterDate, _time.getLessonTimeEnd(lesson)));
                    
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
    }
}
