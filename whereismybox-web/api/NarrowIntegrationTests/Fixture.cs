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
    public ICommandHandler<AddContributorCommand> AddContributorCommandHandler { get; }
    public ICommandHandler<AddItemCommand> AddItemCommandHandler { get; }
    public ICommandHandler<CreateBoxCommand> CreateBoxCommandHandler { get; }
    public ICommandHandler<CreateCollectionCommand> CreateCollectionCommandHandler { get; }
    public ICommandHandler<CreateUserCommand> CreateUserCommandHandler { get; }
    public ICommandHandler<DeleteBoxCommand> DeleteBoxCommandHandler { get; }
    public ICommandHandler<DeleteCollectionCommand> DeleteCollectionCommandHandler { get; }
    public ICommandHandler<DeleteContributorCommand> DeleteContributorCommandHandler { get; }
    public ICommandHandler<DeleteItemCommand> DeleteItemCommandHandler { get; }
    public ICommandHandler<DeleteUnattachedItemCommand> DeleteUnattachedItemCommandHandler { get; }
    public ICommandHandler<MoveItemCommand> MoveItemCommandHandler { get; }
    public ICommandHandler<MoveUnattachedItemToBoxCommand> MoveUnattachedItemToBoxCommandHandler { get; }
    public ICommandHandler<RegisterUserCommand> RegisterUserCommandHandler { get; }
    public ICommandHandler<UpdateBoxCommand> UpdateBoxCommandCommandHandler { get; }

    // Queries
    public IQueryHandler<GetBoxCollectionQuery, List<Box>> GetBoxCollectionQueryHandler { get; }
    public IQueryHandler<GetBoxQuery, Box> GetBoxQueryHandler { get; }
    public IQueryHandler<GetCollectionContributorsQuery, List<User>> GetCollectionContributorsQueryHandler { get; }
    public IQueryHandler<GetCollectionOwnerQuery, User> GetCollectionOwnerQueryHandler { get; }
    public IQueryHandler<GetCollectionQuery, Collection> GetCollectionQueryHandler { get; }
    public IQueryHandler<GetContributorCollectionsQuery, List<Collection>> GetContributorCollectionsQueryHandler { get;}

    public IQueryHandler<GetOwnedCollectionQuery, List<Collection>> GetOwnedCollectionQueryHandler { get; }
    public IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> GetUnattachedItemsQueryHandler { get; }
    public IQueryHandler<GetUserByExternalUserIdQuery, User> GetUserByExternalUserIdQueryHandler { get; }
    public IQueryHandler<GetUserPermissionsQuery, Permissions> GetUserPermissionsQueryHandler { get; }

    public Fixture(ITestOutputHelper testOutputHelper)
    {
        var xunitLoggerFactory = new XunitLoggerFactory(testOutputHelper);
        XunitLoggerFactory = xunitLoggerFactory;
        AuthorizationService = new AuthorizationService(FakeUserRepository, FakeCollectionRepository);

        // Commands

        AddContributorCommandHandler = new AddContributorCommandHandler(FakeUserRepository, FakeCollectionRepository);
        AddItemCommandHandler = new AddItemCommandHandler(AuthorizationService, FakeBoxRepository);
        CreateBoxCommandHandler = new CreateBoxCommandHandler(AuthorizationService, FakeBoxRepository);
        CreateCollectionCommandHandler = new CreateCollectionCommandHandler(FakeCollectionRepository);
        CreateUserCommandHandler = new CreateUserCommandHandler(FakeUserRepository);
        DeleteBoxCommandHandler = new DeleteBoxCommandHandler(AuthorizationService, FakeBoxRepository);
        DeleteCollectionCommandHandler =
            new DeleteCollectionCommandHandler(FakeCollectionRepository, FakeBoxRepository);
        DeleteContributorCommandHandler = new DeleteContributorCommandHandler(FakeCollectionRepository);
        DeleteItemCommandHandler = new DeleteItemCommandHandler(AuthorizationService, XunitLoggerFactory,
            FakeBoxRepository, FakeUnattachedItemRepository);
        DeleteUnattachedItemCommandHandler = new DeleteUnattachedItemCommandHandler(AuthorizationService,
            XunitLoggerFactory,FakeUnattachedItemRepository);
        MoveItemCommandHandler = new MoveItemCommandHandler(AuthorizationService, XunitLoggerFactory, FakeBoxRepository);
            MoveUnattachedItemToBoxCommandHandler = new MoveUnattachedItemToCommandHandler(AuthorizationService,
                XunitLoggerFactory, FakeBoxRepository, FakeUnattachedItemRepository);
        RegisterUserCommandHandler = new RegisterUserCommandHandler(FakeUserRepository);
        UpdateBoxCommandCommandHandler = new UpdateBoxCommandHandler(AuthorizationService, FakeBoxRepository);

            // Queries
        GetUserByExternalUserIdQueryHandler = new GetUserByExternalUserIdQueryHandler(FakeUserRepository);
        GetCollectionQueryHandler = new GetCollectionQueryHandler(FakeCollectionRepository);
        GetOwnedCollectionQueryHandler = new GetOwnedCollectionsQueryHandler(FakeCollectionRepository);
        GetBoxCollectionQueryHandler = new GetBoxCollectionQueryHandler(AuthorizationService, FakeBoxRepository);
        GetBoxQueryHandler = new GetBoxQueryHandler(AuthorizationService, FakeBoxRepository);
        GetUnattachedItemsQueryHandler =
            new GetUnattachedItemsQueryHandler(AuthorizationService, FakeUnattachedItemRepository, FakeBoxRepository);
        GetCollectionContributorsQueryHandler =
            new GetCollectionContributorsQueryHandler(FakeCollectionRepository, FakeUserRepository);
        GetCollectionOwnerQueryHandler = new GetCollectionOwnerQueryHandler(FakeCollectionRepository, FakeUserRepository);
        GetContributorCollectionsQueryHandler = new GetContributorCollectionsQueryHandler(FakeCollectionRepository);
        GetUserPermissionsQueryHandler = new GetUserPermissionsQueryHandler(FakeCollectionRepository);
    }
}