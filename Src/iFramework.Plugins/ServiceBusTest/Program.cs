﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusTest
{
    public class Payload
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
    }


    internal class Program
    {
        public static BlockingCollection<BrokeredMessage> Messages = new BlockingCollection<BrokeredMessage>();

        private static void Main(string[] args)
        {
            var namespaceManager = NamespaceManager.Create();
            var messageFactory = MessagingFactory.Create();

            var queueName = "ServiceBusTest";
            if (!namespaceManager.QueueExists(queueName))
            {
                var queueDescription = new QueueDescription(queueName)
                {
                    EnableDeadLetteringOnMessageExpiration = true,
                    RequiresSession = false
                };
                namespaceManager.CreateQueue(queueDescription);
                //namespaceManager.DeleteQueue(queueName);
            }

            var queueClient = messageFactory.CreateQueueClient(queueName);
            var toSendMessages = new List<BrokeredMessage>();
            for (var i = 0; i < 10; i++)
            {
                toSendMessages.Add(new BrokeredMessage(new Payload {Id = i, Time = DateTime.Now}));
            }
            queueClient.SendBatch(toSendMessages);
            IEnumerable<BrokeredMessage> brokeredMessages = null;
            long sequenceNumber = 0;
            var needPeek = true;
            Task.Run(() =>
            {
                while (needPeek && (brokeredMessages = queueClient.PeekBatch(sequenceNumber, 5)) != null &&
                       brokeredMessages.Count() > 0)
                {
                    foreach (var message in brokeredMessages)
                    {
                        try
                        {
                            if (message.State != MessageState.Deferred)
                            {
                                needPeek = false;
                                break;
                            }
                            Messages.Add(message);
                            sequenceNumber = message.SequenceNumber + 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetBaseException().Message);
                        }
                    }
                }


                while ((brokeredMessages = queueClient.ReceiveBatch(2, new TimeSpan(0, 0, 5))) != null &&
                       brokeredMessages.Count() > 0)
                {
                    foreach (var message in brokeredMessages)
                    {
                        try
                        {
                            message.Defer();
                            Messages.Add(message);
                            sequenceNumber = message.SequenceNumber + 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetBaseException().Message);
                        }
                    }
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    var message = Messages.Take();
                    try
                    {
                        var payload = message.GetBody<Payload>();
                        var toCompleteMessage = queueClient.Receive(message.SequenceNumber);
                        toCompleteMessage.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetBaseException().Message);
                    }
                }
            });

            Console.ReadLine();
        }
    }
}