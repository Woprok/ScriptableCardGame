using Shared.Networking.Protocol.Entities;

namespace Shared.Networking.Protocol.Extensions
{
    public static class AccountExtensions
    {
        public static AccountEntity ShareableAccount(this AccountEntity accountEntity)
        {
            return new AccountEntity(accountEntity.Username, "") { Id = accountEntity.Id };
        }
    }
}