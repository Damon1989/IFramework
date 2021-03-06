﻿using System;
using IFramework.Domain;
using IFramework.Event;
using IFramework.Message;
using Sample.DomainEvents.Community;

namespace Sample.Domain.Model
{
    public abstract class People : VersionedAggregateRoot,
                                   IEventSubscriber<PeopleRegisted>,
                                   IEventSubscriber<ItemRegisted>
    {
        protected People() { }

        protected People(string username, string password, DateTime registerTime)
        {
            OnEvent(new PeopleRegisted(Guid.NewGuid(), username, password, registerTime));
        }

        public Guid Id { get; protected set; }
        public string UserName { get; protected set; }
        public string Password { get; protected set; }
        public DateTime RegisterTime { get; protected set; }

        void IMessageHandler<ItemRegisted>.Handle(ItemRegisted @event)
        {
            Console.Write(@event.ToString());
        }

        void IMessageHandler<PeopleRegisted>.Handle(PeopleRegisted @event)
        {
            Id = (Guid) @event.AggregateRootId;
            UserName = @event.UserName;
            Password = @event.Password;
            RegisterTime = @event.RegisterTime;
        }
    }
}