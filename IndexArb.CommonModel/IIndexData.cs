using System;
using System.Collections.Generic;
using System.Text;

namespace IndexArb.CommonModel
{
    interface IIndexData
    {
        DateTime Date { get; set; }
        decimal Price { get; set; }
        decimal Open { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        long Volume { get; set; }

        decimal PercentChange { get; set; }
    }
}
