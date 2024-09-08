// Reference: https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<Coordinates>(new UnboundedChannelOptions
{
    SingleWriter = false,
    SingleReader = false,
    AllowSynchronousContinuations = true
});

_ = Task.Run(() =>
{
    ConsumeWithWhileAsync(channel.Reader).GetAwaiter().GetResult();
});


Console.WriteLine("Press anything to start producing messages. Press 'q' to quit.");
var input = Console.ReadLine();
while (input != "q")
{
    ProduceWithWhileAndTryWrite(channel.Writer, new Coordinates(Guid.NewGuid(), 0, 0));
    input = Console.ReadLine();
}

channel.Writer.Complete();
Console.WriteLine("Press anything to exit.");
Console.ReadLine();

static void ProduceWithWhileAndTryWrite(
    ChannelWriter<Coordinates> writer, Coordinates coordinates)
{
    while (coordinates is { Latitude: < 90, Longitude: < 180 })
    {
        var tempCoordinates = coordinates with
        {
            Latitude = coordinates.Latitude + .5,
            Longitude = coordinates.Longitude + 1
        };

        if (writer.TryWrite(tempCoordinates))
        {
            coordinates = tempCoordinates;
        }
    }
}

static async ValueTask ConsumeWithWhileAsync(
    ChannelReader<Coordinates> reader)
{
    while (await reader.WaitToReadAsync())
    {
        while (reader.TryRead(out var coordinates))
        {
            Console.WriteLine(coordinates);
        }
    }
}

public readonly record struct Coordinates(
    Guid DeviceId,
    double Latitude,
    double Longitude);
