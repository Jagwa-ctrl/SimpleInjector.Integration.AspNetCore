﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Integration.AspNetCore
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    internal sealed class RequestScopingStartupFilter : IStartupFilter
    {
        internal static readonly object HttpContextKey = new object();

        private readonly Container container;

        public RequestScopingStartupFilter(Container container)
        {
            this.container = container;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                this.ConfigureRequestScoping(builder);

                next(builder);
            };
        }

        private void ConfigureRequestScoping(IApplicationBuilder builder)
        {
            builder.Use(async (httpContext, next) =>
            {
                Scope scope = AsyncScopedLifestyle.BeginScope(this.container);

                try
                {
                    scope.SetItem(HttpContextKey, httpContext);

                    await next();
                }
                finally
                {
                    await scope.DisposeScopeAsync();
                }
            });
        }
    }
}