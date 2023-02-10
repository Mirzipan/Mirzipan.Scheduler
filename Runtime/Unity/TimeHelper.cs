namespace Mirzipan.Scheduler.Unity
{
    public static class TimeHelper
    {
        public static IProvideTime GetTime(TickerTime tickerTime)
        {
            return tickerTime switch
            {
                TickerTime.Realtime => new RealTime(),
                TickerTime.FixedUnscaled => new FixedTime(),
                _ => new RealTime()
            };
        }
    }
}