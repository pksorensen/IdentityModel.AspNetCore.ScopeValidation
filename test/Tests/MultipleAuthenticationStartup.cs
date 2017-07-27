// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tests
{
    public class MultipleAuthenticationStartup
    {
        private readonly bool _automaticAuthenticate;
        private readonly ClaimsPrincipal _principal1;
        private readonly ClaimsPrincipal _principal2;
        private readonly ScopeValidationOptions _scopeOptions;

        public MultipleAuthenticationStartup(ClaimsPrincipal principal1, ClaimsPrincipal principal2, bool automaticAuthenticate, ScopeValidationOptions options)
        {
            _principal1 = principal1;
            _principal2 = principal2;
            _scopeOptions = options;
            _automaticAuthenticate = automaticAuthenticate;
        }

        public void Configure(IApplicationBuilder app)
        {
            //if (_principal1 != null)
            //{
            //    app.UseMiddleware<TestAuthenticationMiddleware>(Options.Create(new TestAuthenticationOptions
            //    {
            //        AuthenticationScheme = "scheme1",
            //        User = _principal1,

            //        AutomaticAuthenticate = _automaticAuthenticate
            //    }));
            //}

            //if (_principal2 != null)
            //{
            //    app.UseMiddleware<TestAuthenticationMiddleware>(Options.Create(new TestAuthenticationOptions
            //    {
            //        AuthenticationScheme = "scheme2",
            //        User = _principal2,

            //        AutomaticAuthenticate = _automaticAuthenticate
            //    }));
            //}

            if (_automaticAuthenticate)
            {
                app.UseAuthentication();
            }

            app.AllowScopes(_scopeOptions);

            app.Run(ctx =>
            {
                ctx.Response.StatusCode = 200;
                return Task.FromResult(0);
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {

         

            services.AddAuthentication(o=>
            {
                if (_automaticAuthenticate)
                {
                    o.DefaultAuthenticateScheme = o.DefaultChallengeScheme = o.DefaultSignInScheme =
                    _principal1 != null ? "scheme1" : _principal2 != null ? "scheme2" : null;
                }
            });
          //  services.Configure<TestAuthenticationOptions>((o) => { });
           // services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TestAuthenticationOptions>, dummy>());

            if (_principal1 != null)
            {
                services.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("scheme1", (o) =>
                {
                    o.User = _principal1;
                });
            };

            if (_principal2 != null)
            {
                services.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("scheme2", (o) =>
                {
                    o.User = _principal2;
                });
            };
        }
    }
}