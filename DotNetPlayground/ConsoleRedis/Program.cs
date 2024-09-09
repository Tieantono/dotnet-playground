using StackExchange.Redis;

// Run the Docker Compose file using run.bat file first.
// Reference: https://medium.com/@mohsenmahoski/setting-up-sentinel-with-docker-compose-5cad962c7643.

// Using ConfigurationOptions object.
// Reference: https://github.com/StackExchange/StackExchange.Redis/issues/1534#issuecomment-722201778.
var configuration = new ConfigurationOptions
{
    ServiceName = "mymaster",
    EndPoints =
    {
        { "localhost", 26379 },
        { "localhost", 26380 },
        { "localhost", 26381 },
    },
    AbortOnConnectFail = false
};

// Using configuration string.
// Reference: https://stackexchange.github.io/StackExchange.Redis/Configuration#basic-configuration-strings
// var configString = "localhost:26379,localhost:26380,localhost:26381,serviceName=mymaster";

var redis = ConnectionMultiplexer.Connect(configuration);

var db = redis.GetDatabase(0);

Console.WriteLine("Press anything to start set a random Redis value. Press 'q' to quit.");
var input = Console.ReadLine();
while (input != "q")
{
    try
    {
        db.StringSet("key", Guid.NewGuid().ToString());
    }
    catch (Exception)
    {
        continue;
    }
    
    input = Console.ReadLine();
}

Console.WriteLine("Press anything to exit.");
Console.ReadLine();