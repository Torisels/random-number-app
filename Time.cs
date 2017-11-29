using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace RandomNumberApp
{
    class Time
    {
        private readonly DateTime _currentDay;
        private TimeSpan _ringOffset;
        private int _dayOfWeek;

        public Time()
        {
         _currentDay = DateTime.Today;
         _dayOfWeek = (int)_currentDay.DayOfWeek;

        }




        private void getRingOffsetFromConfig()
        {
            _ringOffset = new TimeSpan(0, Properties.Settings.Default.TimeOffsetMin,
                Properties.Settings.Default.TimeOffsetSec);
        }
    }
}
