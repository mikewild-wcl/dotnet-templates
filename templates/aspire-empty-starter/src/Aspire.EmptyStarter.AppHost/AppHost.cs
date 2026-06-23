using Aspire.EmptyStarter.Shared;

var builder = DistributedApplication.CreateBuilder(args);

await builder.Build().RunAsync().ConfigureAwait(true);
