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

        public List<int> getLessonsForToday(int day)
        {
            var lessons = new List<int>();
            SQLiteCommand cmd = new SQLiteCommand("SELECT Number FROM Lessons where Day = @day",Connection);
            cmd.Parameters.Add(new SQLiteParameter("@day", day));
         
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    if (rdr[0].GetType() != typeof(DBNull))
                    {
                        var v = (string)rdr.GetValue(0);
                        Console.WriteLine(v.Length);
                        lessons = v.Split(',').Select(n => Convert.ToInt32(n)).ToList();
                    }
                }
            }
            return lessons;
        }
    }
}
