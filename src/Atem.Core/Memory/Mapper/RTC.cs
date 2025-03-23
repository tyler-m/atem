using System;

namespace Atem.Core.Memory.Mapper
{
    internal class RTC
    {
        private int _seconds, _minutes, _hours, _day;
        private bool _halt, _dayCarry;
        double _secondsElapsed = 0.0;
        long _lastUnixTimestamp = 0;
        public bool Latched = false;

        public int Seconds
        {
            get
            {
                Update();
                return _seconds;
            }
            set
            {
                _seconds = value;
            }
        }

        public int Minutes
        {
            get
            {
                Update();
                return _minutes;
            }
            set
            {
                _minutes = value;
            }
        }

        public int Hours
        {
            get
            {
                Update();
                return _hours;
            }
            set
            {
                _hours = value;
            }
        }

        public int Day
        {
            get
            {
                Update();
                return _day;
            }
            set
            {
                _day = value;
            }
        }

        public bool Halt
        {
            get
            {
                Update();
                return _halt;
            }
            set
            {
                _halt = value;
            }
        }

        public bool DayCarry
        {
            get
            {
                Update();
                return _dayCarry;
            }
            set
            {
                _dayCarry = value;
            }
        }

        public RTC()
        {
            _lastUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public RTC(RTC rtc)
        {
            _seconds = rtc.Seconds;
            _minutes = rtc.Minutes;
            _hours = rtc.Hours;
            _day = rtc.Day;
            _halt = rtc.Halt;
            _dayCarry = rtc.DayCarry;

            _lastUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public RTC(int seconds, int minutes, int hours, int day, bool dayCarry, bool halt, int savedUnixTimestamp)
        {
            _seconds = seconds;
            _minutes = minutes;
            _hours = hours;
            _day = day;
            _dayCarry = dayCarry;
            _halt = halt;

            _secondsElapsed += DateTimeOffset.UtcNow.ToUnixTimeSeconds() - savedUnixTimestamp;

            _lastUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private void Update()
        {
            if (!_halt && !Latched)
            {
                long _currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _secondsElapsed += (_currentUnixTimestamp - _lastUnixTimestamp) / 1000.0;
                _lastUnixTimestamp = _currentUnixTimestamp;

                if (_secondsElapsed >= 1.0)
                {
                    int secondsToAdd = (int)Math.Floor(_secondsElapsed);
                    _secondsElapsed -= secondsToAdd;
                    
                    _seconds += secondsToAdd;
                    int minutesElapsed = (int)Math.Floor(_seconds / 60.0);
                    _minutes += minutesElapsed;
                    int hoursElapsed = (int)Math.Floor(_minutes / 60.0);
                    _hours += hoursElapsed;
                    int daysElapsed = (int)Math.Floor(_hours / 24.0);
                    _day += daysElapsed;

                    if (_day >= 512)
                    {
                        DayCarry = true;
                    }

                    _seconds %= 60;
                    _minutes %= 60;
                    _hours %= 24;
                    _day %= 512;
                }
            }
        }
    }
}
