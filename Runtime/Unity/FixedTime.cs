namespace Mirzipan.Scheduler.Unity
{
    public struct FixedTime: IProvideTime
    {
        public double Time => UnityEngine.Time.fixedUnscaledTimeAsDouble;
    }
}