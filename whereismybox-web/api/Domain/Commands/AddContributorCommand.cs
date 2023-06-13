using Domain.CommandHandlers;
using Domain.Models;
using Domain.Primitives;

namespace Domain.Commands;

public record AddContributorCommand(Permissions Permissions, UserId OwnerOfCollection, string UsernameToBeAddedAsContributor, CollectionId CollectionId) : ICommand
{
    public readonly Permissions Permissions = Permissions ?? throw new ArgumentNullException(nameof(Permissions));
    public readonly UserId OwnerOfCollection = OwnerOfCollection ?? throw new ArgumentNullException(nameof(OwnerOfCollection));
    public readonly string UsernameToBeAddedAsContributor = UsernameToBeAddedAsContributor ?? throw new ArgumentNullException(nameof(UsernameToBeAddedAsContributor));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}