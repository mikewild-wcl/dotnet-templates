using Aspire.EmptyStarter.Shared;

#if (hasApplicationName)
var builder = DistributedApplication.CreateBuilder(
    new DistributedApplicationOptions
    {
        Args = args,
        DashboardApplicationName = ApplicationConstants.Name,
    });
#else
var builder = DistributedApplication.CreateBuilder(args);
#endif

await builder.Build().RunAsync().ConfigureAwait(true);
