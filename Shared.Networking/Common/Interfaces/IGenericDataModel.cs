using System.Collections.Generic;

namespace Shared.Networking.Common.Interfaces
{
    public interface IGenericDataModel<T, TConnector>
    {
        TConnector Model { get; }
        List<ISendReceiveModel<T>> DataExchangers { get; set; }
    }
}