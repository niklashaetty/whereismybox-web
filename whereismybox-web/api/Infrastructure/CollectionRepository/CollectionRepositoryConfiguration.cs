namespace Infrastructure.CollectionRepository;

public class CollectionRepositoryConfiguration
{
    public string ConnectionString { get; }
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public CollectionRepositoryConfiguration(string connectionString, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        ConnectionString = connectionString;
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}