using System;

namespace freepoll.Helpers
{
    public static class Resources
    {
        public static int SystemUser = 1;
        public static double cacheVal = Convert.ToDouble(Environment.GetEnvironmentVariable("CACHE_TIME_MINUTES"));
        public static TimeSpan CachedTime = TimeSpan.FromMinutes(cacheVal > 0 ? cacheVal : 5) ;
    }
}