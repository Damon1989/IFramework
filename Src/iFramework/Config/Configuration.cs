﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using IFramework.Command;
using IFramework.Event;
using IFramework.Message;
using System.Web.Configuration;
using IFramework.IoC;
using IFramework.Infrastructure.Logging;
using IFramework.Event.Impl;
using IFramework.Message.Impl;
using IFramework.Infrastructure.Caching;
using IFramework.Infrastructure.Caching.Impl;

namespace IFramework.Config
{
    public class Configuration
    {
        public static readonly Configuration Instance = new Configuration();

        Configuration()
        {
           
        }

        public Configuration RegisterCommonComponents()
        {
            UseNoneLogger();
            UseMemoryCahce();
            UseMessageStore<MockMessageStore>();
            this.UseMockMessageQueueClient();
            this.UseMockMessagePublisher();
            RegisterDefaultEventBus();
            return this;
        }

        public bool NeedMessageStore
        {
            get;protected set;
        }


        public Configuration UseMemoryCahce(Lifetime lifetime = Lifetime.Hierarchical)
        {
            IoCFactory.Instance.CurrentContainer.RegisterType<ICacheManager, MemoryCacheManager>(lifetime);
            return this;
        }

        public Configuration UseMessageStore<TMessageStore>(Lifetime lifetime = Lifetime.Hierarchical)
        where TMessageStore : IMessageStore
        {
            NeedMessageStore = typeof(TMessageStore) != typeof(MockMessageStore);
            IoCFactory.Instance.CurrentContainer.RegisterType<IMessageStore, TMessageStore>(lifetime);
            return this;
        }

        public Configuration UseNoneLogger()
        {
            IoCFactory.Instance.CurrentContainer
                            .RegisterInstance(typeof(ILoggerFactory)
                                       , new MockLoggerFactory());
            return this;
        }

        public Configuration RegisterDefaultEventBus(Lifetime lifetime = Lifetime.Hierarchical)
        {
            return RegisterDefaultEventBus(null, lifetime);
        }

        public Configuration RegisterDefaultEventBus(IContainer contaienr, Lifetime lifetime = Lifetime.Hierarchical)
        {
            var container = contaienr ?? IoCFactory.Instance.CurrentContainer;
            container.RegisterType<IEventBus, EventBus>(lifetime);
            return this;
        }



        bool CommitPerMessage { get; set; }
        public bool GetCommitPerMessage()
        {
            return CommitPerMessage;
        }
        public Configuration SetCommitPerMessage(bool commitPerMessage = false)
        {
            CommitPerMessage = commitPerMessage;
            return this;
        }

        public static CompilationSection GetCompliationSection()
        {
            return ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
        }


        public static T GetAppConfig<T>(string key)
        {
            T val = default(T);
            try
            {
                var value = GetAppConfig(key);
                if (typeof(T).IsEquivalentTo(typeof(Guid)))
                {
                    val = (T)Convert.ChangeType(new Guid(value), typeof(T));
                }
                else
                {
                    val = (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (Exception)
            {
               
            }
            return val;
        }

        public static string GetAppConfig(string keyname, string configPath = "Config")
        {
            var config = System.Configuration.ConfigurationManager.AppSettings[keyname];
            try
            {
                if (string.IsNullOrWhiteSpace(config))
                {
                    string filePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
                    if (File.Exists(filePath))
                    {
                        using (TextReader reader = new StreamReader(filePath))
                        {
                            XElement xml = XElement.Load(filePath);
                            if (xml != null)
                            {
                                var element = xml.Elements().SingleOrDefault(e => e.Attribute("key") != null && e.Attribute("key").Value.Equals(keyname));
                                if (element != null)
                                {
                                    config = element.Attribute("value").Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                config = string.Empty;
            }
            return config;
        }
    }
}
