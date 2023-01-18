using Domain.Models;
using Domain.Repositories;
using Infrastructure.UserRepository;

namespace Infrastructure.BoxRepository;

public class InMemoryBoxRepository : IBoxRepository
{
    private Dictionary<Guid, List<Box>> Boxes { get; } = new Dictionary<Guid, List<Box>>()
    {
        {
            Guid.Parse("deadbea7-deaf-d00d-c0de-1337da7aba5e"), new List<Box>()
            {
                new Box(Guid.Parse("deadbea7-deaf-d00d-c0de-000000000000"), 1, "My first testable box", new List<Item>()
                {
                    new Item(Guid.NewGuid(), "Ski gloves", "Red ones"),
                    new Item(Guid.NewGuid(), "Glasögon", "Boss, svarta bågar"),
                    new Item(Guid.NewGuid(), "Keys", "Random collection of keys"),
                })
            }
        }
    };


    public Task<Box> Add(Guid userId, Box box)
    {
        if (Boxes.ContainsKey(userId) is false)
        {
            Boxes.Add(userId, new List<Box>{box});
            
        } else
        {
            Boxes[userId].Add(box);
        }
        
        return Task.FromResult(box);
    }

    public Task<Box> Get(Guid userId, Guid boxId)
    {
        if (Boxes.ContainsKey(userId) is false)
        {
            throw new BoxNotFoundException(userId, boxId);
        }

        var existingBox = Boxes[userId].FirstOrDefault(b => b.BoxId == boxId);
        if (existingBox is null)
        {
            throw new BoxNotFoundException(userId, boxId);
        }

        return Task.FromResult(existingBox);
    }

    public Task<Box> PersistUpdate(Guid userId, Box updatedBox)
    {
        if (Boxes.ContainsKey(userId) is false)
        {
            throw new BoxNotFoundException(userId, updatedBox.BoxId);
        }

        var boxesForUser = Boxes[userId];
        var existingBox = boxesForUser.FirstOrDefault(b => b.BoxId == updatedBox.BoxId);
        if (existingBox is null)
        {
            throw new BoxNotFoundException(userId, updatedBox.BoxId);
        }

        boxesForUser.Remove(existingBox);
        boxesForUser.Add(updatedBox);
        return Task.FromResult(updatedBox);
    }

    public Task<List<Box>> ListBoxesByUser(Guid userId)
    {
        if (Boxes.ContainsKey(userId) is false)
        {
            return Task.FromResult(new List<Box>());
        }

        return Task.FromResult(Boxes[userId]);
    }
}