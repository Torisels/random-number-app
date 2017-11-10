using System;
using System.Windows.Forms;

namespace RandomNumberApp
{
    public partial class MainForm : Form
    {
       // SQLiteConnection m_dbConnection;
        public MainForm()
        {
            InitializeComponent();
            decimal d = (decimal) 1 / 3;
            d = d * 3;
            //d = Decimal.Floor(d);

            Console.WriteLine(d);
            /*SQLiteConnection.CreateFile("MyDatabase.sqlite");
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();*/
        }
    }
}
