using System.Collections.Generic;
using System.Threading.Tasks;
using doteasy.rpc.interfaces;

namespace doteasy.rpc.implement
{
    public class UserService : IUserService
    {
        public Task<string> Async(int id)
        {
            return Task.FromResult($"我执行了异步方法{id}.");
        }

        public Task<IDictionary<string, string>> GetDictionary()
        {
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string> {{"key", "value"}});
        }

        public string Sync(int id)
        {
            return $"我执行了同步方法{id}.";
        }
    }
}