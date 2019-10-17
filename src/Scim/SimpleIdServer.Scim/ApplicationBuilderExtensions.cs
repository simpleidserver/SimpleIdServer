namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add SID to the IApplicationBuilder request execution pipeline.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSID(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseMvc();
            return applicationBuilder;
        }
    }
}
