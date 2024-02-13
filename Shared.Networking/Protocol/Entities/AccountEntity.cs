using System;
using Shared.Networking.Common.Entities;

namespace Shared.Networking.Protocol.Entities
{
    [Serializable]
    public sealed class AccountEntity : UniqueEntity
    {
        public AccountEntity(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString() => Username;
    }
}