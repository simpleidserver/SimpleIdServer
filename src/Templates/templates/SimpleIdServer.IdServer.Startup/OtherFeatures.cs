using System.Diagnostics;

namespace SimpleIdServer.IdServer.Startup
{
    public static class OtherFeatures
    {
        public static void ListenActivity()
        {
            var activityListener = new ActivityListener();
            activityListener.ShouldListenTo = (activitySource) => activitySource.Name == Tracing.ActivitySourceName;
            activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
            activityListener.ActivityStarted += (e) =>
            {

            };
            activityListener.ActivityStopped += (e) =>
            {

            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}
