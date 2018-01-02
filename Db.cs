using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace RandomNumberApp
{
    class Db
    {
        private SQLiteConnection Connection = null; 

        public Db()
        {
            Connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
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

        public Dictionary<int, Dictionary<string, string>> randomNumbersToPeople(HashSet<int> numbers)
        {
            Dictionary<int,Dictionary<string,string>> ret = new Dictionary<int, Dictionary<string, string>>();
            string In = string.Join(",", numbers);
            string qry = "SELECT Name,Surname from Students where Id IN (@{In})";
            return ret;
        }

        public Dictionary<int, int> GetProbability()
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            List<int> single = new List<int>();

            SQLiteCommand cmd = new SQLiteCommand("SELECT Student FROM Registry where Date = @day ", Connection);
            cmd.Parameters.Add(new SQLiteParameter("@day", getNearestDateOfLesson()));

            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    single.Add(rdr.GetInt32(0));
                }
            }


            for (int i = 1; i <= 33; i++)
            {
                if(single.Contains(i))
                    ret.Add(i,1);
                else
                {
                    ret.Add(i,2);
                }
            }


            return ret;
        }

        public string getNearestDateOfLesson()
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT Date FROM Registry where Date < @day ORDER BY Date DESC LIMIT 1", Connection);
                        cmd.Parameters.Add(new SQLiteParameter("@day", Time.SQLiteDateFormat()));
            
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    return rdr.GetString(0);
                
            }
            return String.Empty;
        }

    }
}
