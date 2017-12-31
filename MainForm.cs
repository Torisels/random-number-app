﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomNumberApp
{
    public partial class MainForm : Form
    {
        private readonly Connector _connection;

        private static MainForm UiChanger;
        private Time _time;

        public MainForm()
        {
            InitializeComponent();
            CustomInitialization();
            UiChanger = this;
            _connection = new Connector();
            _time = new Time();
            test();
            handleLessonTime();
        }

        private async  void btnLosuj_Click(object sender, EventArgs e)
        {
            btnLosuj.Enabled = false;
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

            await _connection.SendRequest();

            btnLosuj.Enabled = true;
            progressBar1.Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("sss");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var x = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                Console.WriteLine(x);
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
           Time t = new Time();
            Console.WriteLine(
            Time.GetTimeDifference(DateTime.Parse("16:14"), DateTime.Parse("16:50")));
            t.checkIfLessonIsToday();
            RandomM m = new RandomM();

        }

        public void handleLessonTime()
        {
            Task.Run(() =>
            {
              
                while (true)
                {
                    if (!_time.checkIfLessonIsToday())
                    {
                        SetTextOnTimeLabel("Nie masz dzisiaj lekcji");
                        return;
                    }
                    if (DateTime.Now>_time.getTodayEndLessonTime())
                    {
                        SetTextOnTimeLabel("Twoje lekcje skończyły się");
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

    }
}
