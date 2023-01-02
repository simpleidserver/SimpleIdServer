namespace SimpleIdServer.OAuth.Options
{
    public static class OAuthHostOptionsExtensions
    {
        /// <summary>
        /// Expiration time auth_req_id in seconds.
        /// </summary>
        public static int GetAuthRequestExpirationTimeInSeconds(this OAuthHostOptions options) => options.GetIntParameter("AuthRequestExpirationTimeInSeconds");
        /// <summary>
        /// Default interval in seconds.
        /// </summary>
        public static int GetDefaultBCAuthorizeWaitIntervalInSeconds(this OAuthHostOptions options) => options.GetIntParameter("DefaultBCAuthorizeWaitIntervalInSeconds");
        /// <summary>
        /// FCM title.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetFcmTitle(this OAuthHostOptions options) => options.GetStringParameter("FcmTitle");
        /// <summary>
        /// FCM Body
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetFcmBody(this OAuthHostOptions options) => options.GetStringParameter("FcmBody");
    }
}
