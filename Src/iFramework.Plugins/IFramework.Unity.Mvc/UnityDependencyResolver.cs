﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using IFramework.IoC;
using Microsoft.Practices.Unity;

namespace IFramework.Unity.Mvc
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;

        public UnityDependencyResolver(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.ResolveAll(serviceType);
        }
    }
}