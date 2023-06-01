using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.QueryHandlers;
using Infrastructure.BoxRepository;
using NarrowIntegrationTests.Fakes;
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

    // Commands
    public ICommandHandler<CreateUserCommand> CreateUserCommandHandler { get; }
    public ICommandHandler<CreateBoxCommand> CreateBoxCommandHandler { get; }
    public ICommandHandler<AddItemCommand> AddItemCommandHandler { get; }
    public ICommandHandler<DeleteBoxCommand> DeleteBoxCommandHandler { get; }
    public ICommandHandler<DeleteItemCommand> DeleteItemCommandHandler { get; }

    // Queries
    public IQueryHandler<GetBoxCollectionQuery, List<Box>> GetBoxCollectionQueryHandler { get; }
    public IQueryHandler<GetUserQuery, User> GetUserQueryHandler { get; }
    public IQueryHandler<GetBoxQuery, Box> GetBoxQueryHandler { get; }

    public Fixture(ITestOutputHelper testOutputHelper)
    {
        var xunitLoggerFactory = new XunitLoggerFactory(testOutputHelper);
        XunitLoggerFactory = xunitLoggerFactory;

        // Commands
        CreateUserCommandHandler = new CreateUserCommandHandler(FakeUserRepository);
        CreateBoxCommandHandler = new CreateBoxCommandHandler(FakeBoxRepository);
        AddItemCommandHandler = new AddItemCommandHandler(FakeBoxRepository);
        DeleteBoxCommandHandler = new DeleteBoxCommandHandler(FakeBoxRepository);
        DeleteItemCommandHandler =
            new DeleteItemCommandHandler(XunitLoggerFactory, FakeBoxRepository, FakeUnattachedItemRepository);

        // Queries
        GetUserQueryHandler = new GetUserQueryHandler(FakeUserRepository);
        GetBoxCollectionQueryHandler = new GetBoxCollectionQueryHandler(FakeBoxRepository);
        GetBoxQueryHandler = new GetBoxQueryHandler(FakeBoxRepository);
    }
}