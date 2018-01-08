using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace RandomNumberApp
{
    class Time
    {

        private DateTime _masterDate;

        public bool LessonToday;

        /*bells fetched from Db*/
        private readonly Dictionary<int, BellTime> _bellsDictionary;
        private int _numberOfLesson = -1;

        private readonly int _dayOfWeek;


        private List<int> _todayLessons;
        private readonly Db _db;

        public string VerifiedDateString;
        public int BellOffset;

        public Time(Db db,int belloffset)
        {
          RefreshTime();
          _dayOfWeek = (int)_masterDate.DayOfWeek;
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
                if (_masterDate >= o.Value.TimeStart && _masterDate <= o.Value.TimeEnd)
                    return o.Key;
            }
            return -1;
        }

        public bool VerifyDate()
        {
            VerifiedDateString = SqLiteDateFormatCustom(_masterDate);//TODO change to string.empty
            var list = _db.GetLessonsForToday(_dayOfWeek);
            if (list != null)
            {
                _todayLessons = list;
                VerifiedDateString = SqLiteDateFormatCustom(_masterDate);
                return true;
            }
            return false;
        }

        public bool CheckIfLessonIsToday()
        {
            List<int> lessons = _db.GetLessonsForToday(_dayOfWeek);
            if (lessons.Count == 0)
                return false;

            _todayLessons = lessons;
            return true;
        }

        public DateTime GetTodayEndLessonTime()
        {
            int max = _todayLessons.Max();
            return _bellsDictionary[max].TimeEnd;
        }

        public DateTime GetLessonTimeStart(int lesson)
        {
            return _bellsDictionary[lesson].TimeStart;
        }

        public DateTime GetLessonTimeEnd(int lesson)
        {
            return _bellsDictionary[lesson].TimeEnd;
        }

        public int GetNearestLesson()
        {
            foreach (var lesson in _todayLessons)
            {
                if (_masterDate <= _bellsDictionary[lesson].TimeStart)
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
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void RefreshTime()
        {
            _masterDate = DateTime.Now.AddSeconds(BellOffset);
        }

        public DateTime GetCurrentTime()
        {
            RefreshTime();
            return _masterDate;
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
