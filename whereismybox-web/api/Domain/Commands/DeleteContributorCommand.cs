using Domain.CommandHandlers;
using Domain.Models;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteContributorCommand(Permissions Permissions, UserId UserToBeRemoved, CollectionId CollectionId) : ICommand
{
    public readonly Permissions Permissions = Permissions ?? throw new ArgumentNullException(nameof(Permissions));
    public readonly UserId UserToBeRemoved = UserToBeRemoved ?? throw new ArgumentNullException(nameof(UserToBeRemoved));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}