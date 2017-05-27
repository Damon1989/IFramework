﻿using System.Collections.Generic;
using System.Linq;
using IFramework.Command;
using IFramework.Infrastructure;

namespace IFramework.Event.Impl
{
    public class EventBus : IEventBus
    {
        protected List<ICommand> CommandQueue;
        protected List<IEvent> EventQueue;
        protected List<object> SagaResultQueue;
        protected List<IEvent> ToPublishAnywayEventQueue;

        //protected IEventSubscriberProvider EventSubscriberProvider { get; set; }
        public EventBus( /*IEventSubscriberProvider provider*/)
        {
            //EventSubscriberProvider = provider;
            EventQueue = new List<IEvent>();
            CommandQueue = new List<ICommand>();
            SagaResultQueue = new List<object>();
            ToPublishAnywayEventQueue = new List<IEvent>();
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            EventQueue.Add(@event);
            //if (EventSubscriberProvider != null)
            //{
            //    var eventSubscriberTypes = EventSubscriberProvider.GetHandlerTypes(@event.GetType());
            //    eventSubscriberTypes.ForEach(eventSubscriberType =>
            //    {
            //        var eventSubscriber = IoCFactory.Resolve(eventSubscriberType);
            //        ((dynamic)eventSubscriber).Handle((dynamic)@event);
            //    });
            //}
        }

        public void Publish<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent
        {
            events.ForEach(@event => Publish(@event));
        }

        public virtual void Dispose()
        {
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return EventQueue;
        }

        public IEnumerable<object> GetSagaResults()
        {
            return SagaResultQueue;
        }

        public void ClearMessages()
        {
            SagaResultQueue.Clear();
            EventQueue.Clear();
            CommandQueue.Clear();
            ToPublishAnywayEventQueue.Clear();
        }

        public void PublishAnyway(params IEvent[] events)
        {
            Publish(events.AsEnumerable());
            ToPublishAnywayEventQueue.AddRange(events);
        }

        public IEnumerable<IEvent> GetToPublishAnywayMessages()
        {
            return ToPublishAnywayEventQueue;
        }

        public void SendCommand(ICommand command)
        {
            CommandQueue.Add(command);
        }

        public IEnumerable<ICommand> GetCommands()
        {
            return CommandQueue;
        }

        public void FinishSaga(object sagaResult)
        {
            SagaResultQueue.Add(sagaResult);
        }
    }
}