using Atem.Core.Memory.Mapper;

namespace Atem.Core.Test.Memory.Mapper
{
    public class RTCTests
    {
        [Fact]
        public void Update_IncrementsSeconds_WhenTimePasses()
        {
            var rtc = new RTC();
            var initialSeconds = rtc.Seconds;
            var lastTimestamp = 0;
            var currentTimestamp = 1500; // 1.5 seconds since last update

            rtc.Update(currentTimestamp, lastTimestamp);

            Assert.True(rtc.Seconds > initialSeconds);
        }

        [Fact]
        public void Update_TriggersDayCarry_WhenDayOverflows()
        {
            var rtc = new RTC
            {
                Day = 511, // 1 second before overflow
                Hours = 23,
                Minutes = 59,
                Seconds = 59,
                DayCarry = false
            };
            var lastTimestamp = 0;
            var currentTimestamp = 1500; // 1.5 seconds since last update

            rtc.Update(currentTimestamp, lastTimestamp);

            Assert.Equal(0, rtc.Day);
            Assert.True(rtc.DayCarry);
        }

        [Fact]
        public void Update_DoesNotIncrementTime_WhenHalted()
        {
            RTC rtc = new() { Halt = true };
            int initialSeconds = rtc.Seconds;
            long lastTimestamp = 0;
            long currentTimestamp = 1500; // 1.5 seconds since last update

            rtc.Update(currentTimestamp, lastTimestamp);

            Assert.Equal(initialSeconds, rtc.Seconds);
        }
    }
}
