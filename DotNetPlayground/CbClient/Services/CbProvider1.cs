using Couchbase.Lite;
using Couchbase.Lite.Sync;
using System.Net;

namespace CbClient.Services;

public class CbProvider1 : IDisposable
{
    public Database Db { get; private set; }

    public Replicator Replicator { get; private set; }

    public CbProvider1()
    {
        Db = new Database("fbi", new DatabaseConfiguration{
            Directory = "./Data/F738"
        });

        var rulesCollection = Db.GetCollection("promo_rule");
        rulesCollection ??= Db.CreateCollection("promo_rule");

        var credential = new NetworkCredential("F738", "helloworld");

        var replicatorConfig = new ReplicatorConfiguration(new URLEndpoint(new Uri("ws://localhost:4984/fbi")))
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
