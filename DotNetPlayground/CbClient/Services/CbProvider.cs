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

        var rulesCollection = Db.GetCollection("rule");
        rulesCollection ??= Db.CreateCollection("rule");

        var credential = new NetworkCredential("naomi-user", "helloworld");

        var replicatorConfig = new ReplicatorConfiguration(new URLEndpoint(new Uri("ws://localhost:4984/naomidb")))
        {
            ReplicatorType = ReplicatorType.PushAndPull,
            Continuous = true,
            Authenticator = new BasicAuthenticator(credential.UserName, credential.SecurePassword)
        };

        replicatorConfig.AddCollection(rulesCollection);

        Replicator = new Replicator(replicatorConfig);

        Replicator.AddChangeListener((sender, e) =>
        {
            var status = e.Status;

            switch (status.Activity)
            {
                case ReplicatorActivityLevel.Busy:
                    Console.WriteLine("Busy transferring data.");
                    break;
                case ReplicatorActivityLevel.Connecting:
                    Console.WriteLine("Connecting to Sync Gateway.");
                    break;
                case ReplicatorActivityLevel.Idle:
                    Console.WriteLine("Replicator in idle state.");
                    break;
                case ReplicatorActivityLevel.Offline:
                    Console.WriteLine("Replicator in offline state.");
                    break;
                case ReplicatorActivityLevel.Stopped:
                    Console.WriteLine("Completed syncing documents.");
                    break;
            }

            if (status.Error != null)
            {
                Console.WriteLine(status.Error.Message);
            }
            else if (status.Progress.Completed == status.Progress.Total)
            {
                Console.WriteLine("All documents synced.");
            }
            else
            {
                Console.WriteLine($"Documents {status.Progress.Total - status.Progress.Completed} still pending sync");
            }
        });

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
