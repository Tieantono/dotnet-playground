using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Couchbase;

namespace CbServer.Services;

public class CouchbaseService
{
    public ICluster Cluster { get; private set; }
    public IBucket NaomiBucket { get; private set; }
    public ICouchbaseCollection RuleCollection { get; private set; }

    public CouchbaseService(IClusterProvider clusterProvider)
    {
        Console.WriteLine($"Connecting to couchbase");

        try
        {
            var task = Task.Run(async () =>
            {
                var cluster = await clusterProvider.GetClusterAsync();

                Cluster = cluster;
                NaomiBucket = await Cluster.BucketAsync("naomi");
                var defaultScope = await NaomiBucket.ScopeAsync("_default");
                RuleCollection = await defaultScope.CollectionAsync("rule");
            });
            task.Wait();
        }
        catch (AggregateException ae)
        {
            ae.Handle((x) => throw x);
        }
    }

    public async Task<ICouchbaseCollection> TenantCollection(string tenant, string collection)
    {
        var tenantScope = await NaomiBucket.ScopeAsync(tenant);
        var tenantCollection = await tenantScope.CollectionAsync(collection);
        return tenantCollection;
    }
}