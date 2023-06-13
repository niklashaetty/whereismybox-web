using Domain.Primitives;

namespace Domain.CommandHandlers;


public interface ICommandHandler<in T> where T: ICommand
{
    Task Execute(T command);
}

public interface ICommand
{
};
