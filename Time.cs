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

        private DateTime MasterDate;

        public bool LessonToday = false;

        /*bells fetched from Db*/
        private readonly Dictionary<int, BellTime> _bellsDictionary;
        private int _numberOfLesson = -1;

        private TimeSpan _bellOffset;
        private readonly int _dayOfWeek;


        private List<int> _todayLessons;
        private readonly Db _db;

        public string verifiedDateString;
        public int BellOffset;

        public Time(Db db,int belloffset)
        {
          RefreshTime();
          _dayOfWeek = (int)MasterDate.DayOfWeek;
          _db = db;
          _bellsDictionary = _db.getBells();
          LessonToday = VerifyDate();
            BellOffset = belloffset;
        }


        public int DetermineNumberOfLesson()
        {
            if (_bellsDictionary == null) return -1;
            foreach (var o in _bellsDictionary)
            {
                if (MasterDate >= o.Value.TimeStart && MasterDate <= o.Value.TimeEnd)
                    return o.Key;
            }
            return -1;
        }

        public bool VerifyDate()
        {
            verifiedDateString = SqLiteDateFormatCustom(MasterDate);//TODO change to string.empty
            if (_db.GetLessonsForToday(_dayOfWeek) != null)
            {
                verifiedDateString = SqLiteDateFormatCustom(MasterDate);
                return true;
            }
            return false;
        }


        private void getRingOffsetFromConfig()
        {
            _bellOffset = new TimeSpan(0, Properties.Settings.Default.TimeOffsetMin,
                Properties.Settings.Default.TimeOffsetSec);
        }

        public bool checkIfLessonIsToday()
        {
            List<int> lessons = _db.GetLessonsForToday(_dayOfWeek);
            if (lessons.Count == 0)
                return false;

            _todayLessons = lessons;
            return true;
        }

        public DateTime getTodayEndLessonTime()
        {
            int max = _todayLessons.Max();
            return _bellsDictionary[max].TimeEnd;
        }

        public DateTime getLessonTimeStart(int lesson)
        {
            return _bellsDictionary[lesson].TimeStart;
        }

        public DateTime getLessonTimeEnd(int lesson)
        {
            return _bellsDictionary[lesson].TimeEnd;
        }

        public int GetNearestLesson()
        {
            foreach (var lesson in _todayLessons)
            {
                if (MasterDate <= _bellsDictionary[lesson].TimeStart)
                    return lesson;
            }

            return -1;
        }

        public static string GetTimeDifference(DateTime one, DateTime two)
        {
            TimeSpan diff = one.Subtract(two);
            if (diff.Hours == 0)
                return diff.ToString(@"mm\:ss");
            return diff.ToString(@"hh\:mm\:ss");
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void RefreshTime()
        {
            MasterDate = DateTime.Now.AddSeconds(BellOffset);
        }

        public DateTime GetCurrentTime()
        {
            RefreshTime();
            return MasterDate;
        }

        public static string SqLiteDateFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string SqLiteDateFormatCustom(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

    }
    class BellTime
    {
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }

        public BellTime(string start, string end)
        {
            TimeStart = Parse(start);
            TimeEnd = Parse(end);
        }

        public static DateTime Parse(string time1)
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
