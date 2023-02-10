namespace Mirzipan.Scheduler.Unity
{
    public struct RealTime : IProvideTime
    {
        public double Time => UnityEngine.Time.realtimeSinceStartupAsDouble;
    }
}