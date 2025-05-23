namespace Infrastructure.UserRepository;

public class UserRepositoryConfiguration
{
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public UserRepositoryConfiguration(string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}