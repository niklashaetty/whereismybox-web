namespace Infrastructure.CollectionRepository;

public class CollectionRepositoryConfiguration
{
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public CollectionRepositoryConfiguration(string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}