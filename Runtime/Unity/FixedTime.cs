namespace Mirzipan.Scheduler.Unity
{
    public struct FixedTime: IProvideTime
    {
        public double Now => UnityEngine.Time.fixedUnscaledTimeAsDouble;
    }
}