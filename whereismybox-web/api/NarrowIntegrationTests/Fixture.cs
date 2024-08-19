using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using Domain.QueryHandlers;
using NarrowIntegrationTests.Fakes;
using NarrowIntegrationTests.Fakes.FakeUserRepository;
using Xunit.Abstractions;

namespace NarrowIntegrationTests;

public class Fixture
{
    // Misc
    public XunitLoggerFactory XunitLoggerFactory { get; }

    // Fakes
    public FakeBoxRepository FakeBoxRepository { get; } = new();
    public FakeUserRepository FakeUserRepository { get; } = new();
    public FakeUnattachedItemRepository FakeUnattachedItemRepository { get; } = new();
    public FakeCollectionRepository FakeCollectionRepository { get; } = new();
    
    public IAuthorizationService AuthorizationService { get; }

    // Commands
    public ICommandHandler<CreateUserCommand> CreateUserCommandHandler { get; }
    public ICommandHandler<CreateBoxCommand> CreateBoxCommandHandler { get; }
    public ICommandHandler<AddItemCommand> AddItemCommandHandler { get; }
    public ICommandHandler<DeleteBoxCommand> DeleteBoxCommandHandler { get; }
    public ICommandHandler<DeleteItemCommand> DeleteItemCommandHandler { get; }
    public ICommandHandler<DeleteUnattachedItemCommand> DeleteUnattachedItemCommandHandler { get; }
    public ICommandHandler<MoveUnattachedItemToBoxCommand> MoveUnattachedItemToBoxCommandHandler { get; }
    public ICommandHandler<RegisterUserCommand> RegisterUserCommandHandler { get; }

    // Queries
    public IQueryHandler<GetUserByExternalUserIdQuery, User> GetUserByExternalUserIdQueryHandler { get; }
    public IQueryHandler<GetBoxCollectionQuery, List<Box>> GetBoxCollectionQueryHandler { get; }
    public IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> GetUnattachedItemsQueryHandler { get; }
    public IQueryHandler<GetBoxQuery, Box> GetBoxQueryHandler { get; }

    public Fixture(ITestOutputHelper testOutputHelper)
    {
        var xunitLoggerFactory = new XunitLoggerFactory(testOutputHelper);
        XunitLoggerFactory = xunitLoggerFactory;
        AuthorizationService = new AuthorizationService(FakeUserRepository, FakeCollectionRepository);

        // Commands
        CreateUserCommandHandler = new CreateUserCommandHandler(FakeUserRepository);
        CreateBoxCommandHandler = new CreateBoxCommandHandler(AuthorizationService, FakeBoxRepository);
        AddItemCommandHandler = new AddItemCommandHandler(AuthorizationService, FakeBoxRepository);
        DeleteBoxCommandHandler = new DeleteBoxCommandHandler(AuthorizationService, FakeBoxRepository);
        DeleteItemCommandHandler =
            new DeleteItemCommandHandler(AuthorizationService, XunitLoggerFactory, FakeBoxRepository, FakeUnattachedItemRepository);
        DeleteUnattachedItemCommandHandler =
            new DeleteUnattachedItemCommandHandler(AuthorizationService, XunitLoggerFactory, FakeUnattachedItemRepository);
        MoveUnattachedItemToBoxCommandHandler = new MoveUnattachedItemToCommandHandler(AuthorizationService, XunitLoggerFactory,
            FakeBoxRepository, FakeUnattachedItemRepository);
        RegisterUserCommandHandler = new RegisterUserCommandHandler(FakeUserRepository);

        // Queries
        GetUserByExternalUserIdQueryHandler = new GetUserByExternalUserIdQueryHandler(FakeUserRepository);
        GetBoxCollectionQueryHandler = new GetBoxCollectionQueryHandler(AuthorizationService, FakeBoxRepository);
        GetBoxQueryHandler = new GetBoxQueryHandler(AuthorizationService, FakeBoxRepository);
        GetUnattachedItemsQueryHandler =
            new GetUnattachedItemsQueryHandler(AuthorizationService, FakeUnattachedItemRepository, FakeBoxRepository);
    }
}