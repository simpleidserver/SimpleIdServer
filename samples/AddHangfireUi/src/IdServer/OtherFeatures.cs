using System.Diagnostics;

namespace IdServer
{
    public static class OtherFeatures
    {
        public static void ListenActivity()
        {
            var activityListener = new ActivityListener();
            activityListener.ShouldListenTo = (activitySource) => activitySource.Name == SimpleIdServer.IdServer.Tracing.ActivitySourceName;
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
