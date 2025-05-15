namespace Infrastructure.UnattachedItemRepository;

public class UnattachedItemRepositoryConfiguration
{
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public UnattachedItemRepositoryConfiguration(string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}