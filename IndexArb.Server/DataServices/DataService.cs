using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;

namespace IndexArb.Server.DataServices
{
    public class DataService : IDataService<string>
    {
        public IObservable<string> GetDataStream(string dataSource)
        {
            // in this implementation there will be no need to loop through the file and read the contents out or encapsulate the parsing and reading logic within a using block.
            // the app will subscribe to this data stream and perfrom any necessary logic as the data is streamed from the file.
            return Observable.Using(() => File.OpenText(dataSource), 
                                    stream => Observable.Generate(stream, state => !state.EndOfStream, state => state, state => state.ReadLine() ?? string.Empty));
        }

        public IObservable<int> GetDataStream(int min, int max)
        {
            Random random = new Random(100);
            return Observable.Interval(TimeSpan.FromSeconds(0.5)).Select(x => random.Next(min, max));
        }

    }
}
