namespace Infrastructure.BoxRepository;

public class BoxRepositoryConfiguration
{
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public BoxRepositoryConfiguration(string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}