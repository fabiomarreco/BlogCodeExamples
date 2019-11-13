using System.Collections.Generic;
using SpecEntityNetCore3.Domain;

namespace SpecEntityNetCore3
{
    public interface IRepository
    {
        void Add(User user);
        IReadOnlyCollection<User> GetUsers(IUserSpecification spec);
    }
}