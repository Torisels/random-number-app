using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberApp
{
    class Db
    {
        SQLiteConnection Connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");

        public Db()
        {
            Connection.Open();
        }

        public void universalQuery(string query)
        {
            SQLiteCommand command = new SQLiteCommand(query, Connection);

            using (SQLiteDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Console.Write("{0} ", rdr["time_start"]);
                    Console.Write("{0} ", rdr["time_end"]);
                    Console.Write("{0} \n", rdr["Number"]);
                }
            }
        }

        public Dictionary<int, BellTime> getBells()
        {
            Dictionary<int, BellTime> dict = new Dictionary<int, BellTime>();
            SQLiteCommand command = new SQLiteCommand("SELECT * from Bells", Connection);

            using (SQLiteDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    dict.Add((int)rdr["Number"],new BellTime((string)rdr["time_start"],(string)rdr["time_end"]));
                }
            }
            return dict;
        }
    }
}
