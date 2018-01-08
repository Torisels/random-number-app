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
    internal class Db
    {
        private readonly SQLiteConnection _connection; 

        public Db()
        {
            _connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            _connection.Open();
        }

        public void universalQuery(string query)
        {
            SQLiteCommand command = new SQLiteCommand(query, _connection);

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
            SQLiteCommand command = new SQLiteCommand("SELECT * from Bells", _connection);

            using (SQLiteDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    dict.Add((int)rdr["Number"],new BellTime((string)rdr["time_start"],(string)rdr["time_end"]));
                }
            }
            return dict;
        }

        public List<int> GetLessonsForToday(int day)
        {
            var lessons = new List<int>();
            SQLiteCommand cmd = new SQLiteCommand("SELECT Number FROM Lessons where Day = @day",_connection);
            cmd.Parameters.Add(new SQLiteParameter("@day", day));
         
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    if (!rdr.HasRows)
                        return null;
                    if (rdr[0].GetType() != typeof(DBNull))
                    {
                        var v = rdr.GetString(0);
                        Console.WriteLine(v.Length);
                        lessons = v.Split(',').Select(n => Convert.ToInt32(n)).ToList();
                        return lessons;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        public Dictionary<int, int> GetProbability()
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            List<int> single = new List<int>();

            SQLiteCommand cmd = new SQLiteCommand("SELECT Student FROM Registry where Date = @day ", _connection);
            cmd.Parameters.Add(new SQLiteParameter("@day", GetNearestDateOfLesson()));

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

        public Dictionary<int, int> GetProbabilityWithDate(string date)
        {
            date = "\"" + date + "\"";
            Dictionary<int, int> ret = new Dictionary<int, int>();
            List<int> single = new List<int>();

            SQLiteCommand cmd = new SQLiteCommand($"SELECT Student FROM Registry where Date = {date} ", _connection);
       

            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    single.Add(rdr.GetInt32(0));
                }
            }


            for (int i = 1; i <= 33; i++)
            {
                if (single.Contains(i))
                    ret.Add(i, 1);
                else
                {
                    ret.Add(i, 2);
                }
            }
            return ret;
        }

        public string GetNearestDateOfLesson()
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT Date FROM Registry where Date < @day ORDER BY Date DESC LIMIT 1", _connection);
                        cmd.Parameters.Add(new SQLiteParameter("@day", Time.SqLiteDateFormat()));
            
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    return rdr.GetString(0);            
            }
            return String.Empty;
        }

        public Dictionary<int, string> GetDrawnPeople(HashSet<int> numbers)
        {
            var ret = new Dictionary<int, string>();
            var par = string.Join(",", numbers);
            var cmd = new SQLiteCommand($"SELECT Id,Name,Surname FROM Students where Id IN({par})",_connection);
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                Console.WriteLine(rdr.HasRows);
                while (rdr.Read())
                    ret.Add(rdr.GetInt32(0),rdr.GetString(1)+" "+rdr.GetString(2));
            }
            return ret;
        }

        public HashSet<int> GetDrawnPeople()
        {
            HashSet<int> ret = new HashSet<int>();




            return null;
        }

        public void InsertDrawnPeople(HashSet<int> numbers,string date)
        {
            date = "\"" + date + "\"";
            var cmd1 = new SQLiteCommand($"DELETE FROM Draw WHERE date = {date}",_connection);
            cmd1.ExecuteNonQuery();           
            var nums = '\"'+string.Join(",", numbers)+'\"';
            var qry = $"INSERT INTO Draw(date,people)VALUES ({date},{nums})";
            var cmd = new SQLiteCommand(qry, _connection);
            cmd.ExecuteNonQuery();
        }

        public bool CheckIfDrawnToday(string date, out HashSet<int> people)
        {
            people = new HashSet<int>();
            date = "\"" + date + "\"";
            var cmd = new SQLiteCommand($"SELECT people FROM Draw WHERE date = {date}", _connection);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
                return false;

            while (reader.Read())
            {
                var v = reader.GetString(0);
                Console.WriteLine(v.Length);
                var l = v.Split(',').Select(n => Convert.ToInt32(n)).ToList();
                people = new HashSet<int>(l);
            }
            return true;
        }

        public void AddRemoveUserPresence(int id,string date, bool insert = true)
        {
            date = "\"" + date + "\"";
            var qry = insert ? $"INSERT INTO Registry VALUES({date},{id})" : $"DELETE FROM Registry WHERE Date = {date} AND Student = {id}";
            SQLiteCommand cmd = new SQLiteCommand(qry);
            //cmd.Parameters.Add(new SQLiteParameter("@param1",id));
            //cmd.Parameters.Add(new SQLiteParameter("@param2", date));
            cmd.Connection = _connection;
            cmd.ExecuteReader();
//            Console.WriteLine(
//            cmd.ExecuteNonQuery());
        }
    }
}
