namespace Domain.CommandHandlers;


public interface ICommandHandler<in T> where T: ICommand
{
    Task Execute(T command);
}

// marker interface for CommandHandler
public interface ICommand{};
