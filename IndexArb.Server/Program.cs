using System;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reflection;
using IndexArb.Server.DataServices;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndexArb.Server.DataSource;
using System.Collections.Concurrent;
using IndexArb.CommonModel;
using RabbitMQ.Client;
using System.Text;

namespace IndexArb.Server
{
    class Program
    {
        static object lockObj = new object();
        public static LiborUSD currentLiborRates;
        private static ConnectionFactory connectionFactory;
        private static IConnection rabbitConnection;
        private static IModel channel;
        private static IModel futureChannel;

        static void Main(string[] args)
        {
            SetupMessageQueue();

            DataService dataService = new DataService();
            DataService dataService1 = new DataService();
            TaskFactory taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

            var spotPriceObservable = dataService.GetDataStream(100, 325).Skip(0).Subscribe((data) => OnNextSpotData("spot  = " + data, DataType.Spot));
            var futuresPriceObservable = dataService1.GetDataStream(100, 325).Skip(0).Subscribe((data) => OnNextFuturesData("future = " + data, DataType.Future));

            Console.ReadLine();

            DisposeResources();
        }

        private static void DisposeResources()
        {
            channel?.Dispose();
            rabbitConnection?.Dispose();
        }

        private static void SetupMessageQueue()
        {
            connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                RequestedConnectionTimeout = new TimeSpan(0, 0, 0, 3000), 
            };
            rabbitConnection = connectionFactory.CreateConnection();
            channel = rabbitConnection.CreateModel();
        }

        private static object OnNextLiborData(string data)
        {
            Console.WriteLine(data);
            return Unit.Default;
        }

        private static object OnNextSpotData(string data, DataType dataType)
        {
            Publish(data, dataType);
            Console.WriteLine(data);
            return Unit.Default;
        }

        private static object OnNextFuturesData(string data, DataType dataType)
        {
            Publish(data, dataType);
            Console.WriteLine(data);
            return Unit.Default;
        }

        private static void Publish(string data, DataType dataType)
        {
            lock (lockObj)
            {
                channel.BasicPublish(
                                    exchange: "SP500",
                                    routingKey: dataType.ToString(),
                                    basicProperties: null,
                                    body: Encoding.UTF8.GetBytes(data));
            }
        }
    }
}