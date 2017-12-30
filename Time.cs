using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace RandomNumberApp
{
    class Time
    {
        private readonly DateTime _currentDay;
        private DateTime MasterDate;


        private Dictionary<int, BellTime> bellsDictionary;

        private int _numberOfLesson = -1;

        private TimeSpan _ringOffset;
        private int _dayOfWeek;


        private List<int> _todayLessons;
        private Db _db;

        public Time()
        {
         _currentDay = DateTime.Today;
         _dayOfWeek = (int)_currentDay.DayOfWeek;
          MasterDate = DateTime.Now;
          _db = new Db();
            getBells();
        }



        private void getBells()
        {
            bellsDictionary = _db.getBells();
        }

        public int DetermineNumberOfLesson()
        {
            if (bellsDictionary == null) return -1;
            foreach (var o in bellsDictionary)
            {
                if (MasterDate >= o.Value.TimeStart && MasterDate <= o.Value.TimeEnd)
                    return o.Key;
            }
            return -1;
        }


        private void getRingOffsetFromConfig()
        {
            _ringOffset = new TimeSpan(0, Properties.Settings.Default.TimeOffsetMin,
                Properties.Settings.Default.TimeOffsetSec);
        }

        public bool checkIfLessonIsToday()
        {
            List<int> lessons = _db.getLessonsForToday(5); //TODO CHANGE
            if (lessons.Count == 0)
                return false;

            _todayLessons = lessons;
            return true;
        }

        public DateTime getTodayEndLessonTime()
        {
            int max = _todayLessons.Max();
            return bellsDictionary[max].TimeEnd;
        }

        public static string getTimeDifference(DateTime one, DateTime two)
        {
            TimeSpan diff = one.Subtract(two);         
            DateTime di = UnixTimeStampToDateTime(diff.Seconds);
            if (di.Hour == 0)
                return di.ToString("HH:mm");

            return string.Empty;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }


    }
    class BellTime
    {
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }

        public BellTime(string start, string end)
        {
            TimeStart = parse(start);
            TimeEnd = parse(end);
        }

        public static DateTime parse(string time1)
        {
            if (!DateTime.TryParseExact(time1, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt))
            {
                Console.WriteLine("error");
            }
            return dt;
        }




    }
}
