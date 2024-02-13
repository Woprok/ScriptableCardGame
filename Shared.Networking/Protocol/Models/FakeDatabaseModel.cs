using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Extensions;

namespace Shared.Networking.Protocol.Models
{
    public interface IDatabaseModel
    {
        AccountEntity ValidateAccount(AccountEntity account);
        LobbyEntity ValidateLobby(LobbyEntity lobby);
    }

    //ToDo replace with real DB magic
    public class FakeDatabaseModel : IDatabaseModel
    {
        //ToDo real DB stuff like taking real ID from some DB table
        public AccountEntity ValidateAccount(AccountEntity account)
        {
            //Todo fix me
            //null is not existing
            //otherwise its ok
            return account.ShareableAccount();
        }

        public LobbyEntity ValidateLobby(LobbyEntity lobby)
        {
            return lobby;
        }
    }
}