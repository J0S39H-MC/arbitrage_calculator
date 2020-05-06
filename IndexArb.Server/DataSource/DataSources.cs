using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace IndexArb.Server.DataSource
{
    public static class DataSources
    {
        public static readonly string FuturesSrc = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DataSource\S&P 500 Futures Historical Data.csv");
        public static readonly string SpotSrc = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DataSource\S&P 500 Historical Data (1).csv");
        public static readonly string LiborSrc = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DataSource\LIBOR USD.csv");
    }
}
