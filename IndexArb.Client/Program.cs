using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IndexArb.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Spot", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "Future", durable: true, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);

                // create a stream of trades from rabbitmq
                var messages = Observable.FromEventPattern<BasicDeliverEventArgs>(consumer, nameof(consumer.Received));
                var formattedMessages = messages.Select((deliveryArgs) =>
                                     {
                                         var convertedStrng = Encoding.UTF8.GetString(deliveryArgs.EventArgs.Body.IsEmpty ? new byte[] { } : deliveryArgs.EventArgs.Body.ToArray());
                                         var splitStringArray = convertedStrng.Split("=", StringSplitOptions.RemoveEmptyEntries);
                                         return (priceType: splitStringArray[0].Trim(), price: int.Parse(splitStringArray[1].Trim()));
                                     });
                var spotPrices = formattedMessages.Where(p => p.priceType.Equals("spot", StringComparison.OrdinalIgnoreCase));
                var futurePrices = formattedMessages.Where(p => p.priceType.Equals("future", StringComparison.OrdinalIgnoreCase));
                var combined = Observable.CombineLatest(spotPrices, futurePrices, (spotPrice, futurePrice) =>
               {
                   // rate and time factor are variables that need to be changed for this example.
                   var theoreticalFuturePrice = spotPrice.price * Math.Exp(0.0113 * 0.25); 

                    string actionType = "HOLD";
                   if (theoreticalFuturePrice < futurePrice.price)
                       actionType = "BUY";
                   else if (theoreticalFuturePrice > futurePrice.price)
                       actionType = "SHORT";

                   return (action: actionType, price: theoreticalFuturePrice);
               });
                //var combinedDisposable = combined.Subscribe(x => Console.WriteLine($"Theoretical Future Price : {x}"));
                var combinedDisposable = combined.Subscribe(x =>
                {
                    Console.WriteLine($"{x.action,5}    |  Theoretical Price: {Decimal.Round(Convert.ToDecimal(x.price), 4)}");
                });

                channel.BasicConsume(queue: "Spot", autoAck: true, consumer: consumer);
                channel.BasicConsume(queue: "Future", autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                combinedDisposable.Dispose();
            }
        }
    }
}