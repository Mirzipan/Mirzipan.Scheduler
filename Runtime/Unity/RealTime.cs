namespace Mirzipan.Scheduler.Unity
{
    public struct RealTime : IProvideTime
    {
        public double Now => UnityEngine.Time.realtimeSinceStartupAsDouble;
    }
}