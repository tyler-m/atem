using System;
using System.Buffers.Binary;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Memory.Mapper
{
    public class RTC : IStateful
    {
        public int _seconds, _minutes, _hours;
        public bool _halt;

        public double SecondsElapsed { get; private set; }
        public long LastUnixTimestamp { get; private set; }
        public int Seconds { get => _seconds; set => _seconds = value & 0b111111; }
        public int Minutes { get => _minutes; set => _minutes = value & 0b111111; }
        public int Hours { get => _hours; set => _hours = value & 0b11111; }
        public int Day { get; set; }
        public bool DayCarry { get; set; }
        
        public bool Halt
        {
            get => _halt;
            set
            {
                // update values before going into a halted state
                if (!_halt && value)
                {
                    Update();
                }
                else if (_halt && !value)
                {
                    // we want no time to have elapsed during a halt
                    LastUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }

                _halt = value;
            }
        }

        public byte DayLower
        {
            get
            {
                return (byte)Day;
            }
            set => Day = (Day & 0b100000000) | value;
        }

        public byte DayUpper
        {
            get
            {
                return (byte)((Day >> 8) & 0b1);
            }
            set => Day = (Day & 0xFF) | (value.GetBit(0).Int() << 8);
        }

        public RTC()
        {
            LastUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public RTC(int seconds, int minutes, int hours, int day, bool dayCarry, bool halt, long savedUnixTimestamp)
        {
            _seconds = seconds;
            _minutes = minutes;
            _hours = hours;
            Day = day;
            DayCarry = dayCarry;
            _halt = halt;
            LastUnixTimestamp = savedUnixTimestamp;
        }

        public void Set(RTC rtc)
        {
            _seconds = rtc.Seconds;
            _minutes = rtc.Minutes;
            _hours = rtc.Hours;
            Day = rtc.Day;
            DayCarry = rtc.DayCarry;
            _halt = rtc.Halt;
            SecondsElapsed = rtc.SecondsElapsed;
            LastUnixTimestamp = rtc.LastUnixTimestamp;
        }

        public void Update(long currentUnixTimestamp, long lastUnixTimestamp)
        {
            if (!_halt)
            {
                SecondsElapsed += (currentUnixTimestamp - lastUnixTimestamp) / 1000.0;
                LastUnixTimestamp = currentUnixTimestamp;

                if (SecondsElapsed >= 1.0)
                {
                    int secondsToAdd = (int)Math.Floor(SecondsElapsed);
                    SecondsElapsed -= secondsToAdd;

                    _seconds += secondsToAdd;
                    int minutesElapsed = (int)Math.Floor(Seconds / 60.0);
                    _minutes += minutesElapsed;
                    int hoursElapsed = (int)Math.Floor(Minutes / 60.0);
                    _hours += hoursElapsed;
                    int daysElapsed = (int)Math.Floor(Hours / 24.0);
                    Day += daysElapsed;

                    if (Day >= 512)
                    {
                        DayCarry = true;
                    }

                    _seconds %= 60;
                    _minutes %= 60;
                    _hours %= 24;
                    Day %= 512;
                }
            }
        }

        public void Update()
        {
            Update(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), LastUnixTimestamp);
        }

        public static RTC FromSaveData(ReadOnlySpan<byte> data, bool latch = false)
        {
            int offset = latch ? 20 : 0;
            return new RTC(
                data[offset],
                data[offset + 4],
                data[offset + 8],
                data[offset + 12].SetBit(8, data[offset + 16].GetBit(0)),
                data[offset + 16].GetBit(7),
                data[offset + 16].GetBit(6),
                BinaryPrimitives.ReadInt64LittleEndian(data.Slice(40, 8))
            );
        }

        public byte[] ToSaveData()
        {
            byte[] data = new byte[20];
            data[0] = (byte)_seconds;
            data[4] = (byte)_minutes;
            data[8] = (byte)_hours;
            data[12] = (byte)Day;
            data[16].SetBit(0, ((ushort)Day).GetBit(8));
            data[16].SetBit(6, _halt);
            data[16].SetBit(7, DayCarry);
            return data;
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_seconds);
            writer.Write(_minutes);
            writer.Write(_hours);
            writer.Write(Day);
            writer.Write(_halt);
            writer.Write(DayCarry);
            writer.Write(SecondsElapsed);
            writer.Write(LastUnixTimestamp);
        }

        public void SetState(BinaryReader reader)
        {
            _seconds = reader.ReadInt32();
            _minutes = reader.ReadInt32();
            _hours = reader.ReadInt32();
            Day = reader.ReadInt32();
            _halt = reader.ReadBoolean();
            DayCarry = reader.ReadBoolean();
            SecondsElapsed = reader.ReadDouble();
            LastUnixTimestamp = reader.ReadInt64();
        }
    }
}
