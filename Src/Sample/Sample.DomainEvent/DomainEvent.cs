﻿using IFramework.Event;
using IFramework.Infrastructure;
using IFramework.Message;

namespace Sample.DomainEvents
{
    [Topic("DomainEvent")]
    public class DomainEvent : IDomainEvent
    {
        public DomainEvent()
        {
            ID = ObjectId.GenerateNewId().ToString();
        }

        public DomainEvent(object aggregateRootID)
            : this()
        {
            AggregateRootID = aggregateRootID;
            Key = aggregateRootID.ToString();
        }

        public int Version { get; set; }

        public object AggregateRootID { get; }

        public string AggregateRootName { get; set; }

        public string ID { get; set; }

        public virtual string Key { get; set; }
    }
}