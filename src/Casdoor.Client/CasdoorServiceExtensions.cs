﻿// Copyright 2022 The Casdoor Authors. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Casdoor.Client;

public static class CasdoorServiceExtensions
{
    public static IServiceCollection AddCasdoorClient(
        this IServiceCollection services, Action<CasdoorClientOptions>? optionAction = null) =>
        services.AddCasdoorClient<CasdoorUserClient, CasdoorTokenClient>();

    public static IServiceCollection AddCasdoorClient<TUserClient>(
        this IServiceCollection services,
        Action<CasdoorClientOptions>? optionAction = null)
        where TUserClient : class, ICasdoorUserClient =>
        services.AddCasdoorClient<TUserClient, CasdoorTokenClient>();

    public static IServiceCollection AddCasdoorClient<TUserClient, TTokenClient>(
        this IServiceCollection services,
        Action<CasdoorClientOptions>? optionAction = null)
        where TUserClient : class, ICasdoorUserClient
        where TTokenClient : class, ICasdoorTokenClient
    {
        CasdoorClientOptions casdoorClientOptions = new CasdoorClientOptions();
        optionAction?.Invoke(casdoorClientOptions);
        services.Configure(optionAction);
        services.Configure<TokenClientOptions>(options =>
        {
            options.Address = casdoorClientOptions.Endpoint;
            options.ClientId = casdoorClientOptions.ClientId;
            options.ClientSecret = casdoorClientOptions.ClientSecret;
            options.Parameters.Add("state", casdoorClientOptions.ApplicationName);
        });
        services.AddTransient(p => p.GetRequiredService<IOptions<TokenClientOptions>>().Value);
        services.AddTransient(p => p.GetRequiredService<IOptions<CasdoorClientOptions>>().Value);
        services.AddHttpClient<TokenClient>();
        services.AddHttpClient<TUserClient>();
        services.AddHttpClient<TTokenClient>();
        return services;
    }
}
