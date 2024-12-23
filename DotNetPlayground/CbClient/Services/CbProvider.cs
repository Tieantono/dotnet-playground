using Couchbase.Lite;
using Couchbase.Lite.Sync;
using System.Net;

namespace CbClient.Services;

public class CbProvider : IDisposable
{
    public Database Db { get; private set; }

    public Replicator Replicator { get; private set; }

    public CbProvider()
    {
        Db = new Database("naomi");

        var rulesCollection = Db.GetCollection("rules", "promotions");
        if (rulesCollection is null)
            Db.CreateCollection("rules", "promotions");

        var credential = new NetworkCredential("naomi-user", "helloworld");

        var replicatorConfig = new ReplicatorConfiguration(new URLEndpoint(new Uri("ws://localhost:4984/naomidb")))
        {
            ReplicatorType = ReplicatorType.PushAndPull,
            Continuous = false,
            Authenticator = new BasicAuthenticator(credential.UserName, credential.SecurePassword)
        };

        replicatorConfig.AddCollection(rulesCollection);

        Replicator = new Replicator(replicatorConfig);

        Replicator.Start();
    }

    public void Dispose()
    {
        Replicator.Stop();
        Replicator.Dispose();

        Db.Close();
        Db.Dispose();
    }
}
