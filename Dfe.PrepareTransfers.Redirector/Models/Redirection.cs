namespace Dfe.PrepareTransfers.Redirector.Models;

public class Redirection
{
    public string Destination { get; }
    public string DestinationHost { get; }
    public int Delay { get; }

    public Redirection(string host, string path, string? querystring, int delay = 5)
    {
        UriBuilder destination = new(Uri.UriSchemeHttps, host);
        DestinationHost = destination.ToString();

        destination.Path = path;
        destination.Query = querystring;
        Destination = destination.ToString();

        Delay = delay;
    }
}