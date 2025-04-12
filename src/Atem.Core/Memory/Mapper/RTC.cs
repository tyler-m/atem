using System;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Memory.Mapper
{
    internal class RTC : IStateful
    {
        private int _seconds, _minutes, _hours, _day;
        private bool _halt, _dayCarry;
        private double _secondsElapsed;
        private long _lastUnixTimestamp;
        private bool _latched = false;

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

        public bool Latched { get => _latched; set => _latched = value; }

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
            if (!_halt && !_latched)
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

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_seconds);
            writer.Write(_minutes);
            writer.Write(_hours);
            writer.Write(_day);
            writer.Write(_halt);
            writer.Write(_dayCarry);
            writer.Write(_secondsElapsed);
            writer.Write(_lastUnixTimestamp);
            writer.Write(_latched);
        }

        public void SetState(BinaryReader reader)
        {
            _seconds = reader.ReadInt32();
            _minutes = reader.ReadInt32();
            _hours = reader.ReadInt32();
            _day = reader.ReadInt32();
            _halt = reader.ReadBoolean();
            _dayCarry = reader.ReadBoolean();
            _secondsElapsed = reader.ReadDouble();
            _lastUnixTimestamp = reader.ReadInt64();
            _latched = reader.ReadBoolean();
        }
    }
}
