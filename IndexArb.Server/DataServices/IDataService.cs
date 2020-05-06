using System;
using System.Collections.Generic;
using System.Text;

namespace IndexArb.Server.DataServices
{
    interface IDataService<T>
    {
        IObservable<T> GetDataStream(string dataSource);
    }
}
