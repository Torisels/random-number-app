using System;
using System.Windows.Forms;

namespace RandomNumberApp
{
    public partial class MainForm : Form
    {
        private Connector _connection;

        private static MainForm UiChanger;
       // SQLiteConnection m_dbConnection;
        public MainForm()
        {
            InitializeComponent();
            CustomInitialization();
            UiChanger = this;
             _connection = new Connector();
            /*SQLiteConnection.CreateFile("MyDatabase.sqlite");
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();*/

            Time time = new Time();

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
        public static void SetTextOnTimeLabel(string time)
        {
            if (UiChanger.labelTillTheEnd.InvokeRequired)
            {
                Action<string> method = SetTextOnTimeLabel;
                UiChanger.BeginInvoke(method, time);
            }
            else
            {
                UiChanger.labelTillTheEnd.Text = "Do końca lekcji pozostało: "+ time;
            }
        }


    }
}
