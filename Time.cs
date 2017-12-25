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



        public Time()
        {
         _currentDay = DateTime.Today;
         _dayOfWeek = (int)_currentDay.DayOfWeek;
          MasterDate = DateTime.Now;
        }



        private void getBells(Dictionary<int, BellTime> dict)
        {
            /* DB method*/

            bellsDictionary = dict;
        }

        private int determineNumberOfLesson()
        {
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

        private DateTime parse(string time1)
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
