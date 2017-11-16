using System;
using System.Windows.Forms;

namespace RandomNumberApp
{
    public partial class MainForm : Form
    {
        private Connector _connection;

       // SQLiteConnection m_dbConnection;
        public MainForm()
        {
            InitializeComponent();
            decimal d = (decimal) 1 / 3;
            d = d * 3;
            //d = Decimal.Floor(d);    
           // c.parseDataToDecimals();
            Console.WriteLine(d);


            _connection = new Connector();
            /*SQLiteConnection.CreateFile("MyDatabase.sqlite");
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();*/
        }

        private async  void btnLosuj_Click(object sender, EventArgs e)
        {
            btnLosuj.Enabled = false;
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

            await _connection.sendRequest();

            btnLosuj.Enabled = true;
            progressBar1.Visible = false;
        }
    }
}
