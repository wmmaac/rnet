﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;

using Rnet.Profiles;

namespace Rnet.Drivers
{

    /// <summary>
    /// Provides methods to query an RNET bus object for its supported profiles.
    /// </summary>
    public static class ProfileManager
    {

        /// <summary>
        /// Cache of known metadata.
        /// </summary>
        static readonly ConcurrentDictionary<Type, ProfileMetadata> metadata =
            new ConcurrentDictionary<Type, ProfileMetadata>();

        /// <summary>
        /// Caches the set of loaded profiles for each bus object.
        /// </summary>
        class Cache
        {

            RnetBusObject target;
            Task<Profile[]> profiles;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="target"></param>
            public Cache(RnetBusObject target)
            {
                this.target = target;
            }

            /// <summary>
            /// Obtains the set of profiles supported by the specified device.
            /// </summary>
            /// <param name="device"></param>
            /// <returns></returns>
            async Task<object[]> RequestProfiles(RnetDevice device)
            {
                var driver = await device.GetDriver();
                if (driver != null)
                    return await driver.GetProfilesInternal();

                return null;
            }

            /// <summary>
            /// Obtains the set of profiles supported by the specified zone.
            /// </summary>
            /// <param name="zone"></param>
            /// <returns></returns>
            async Task<object[]> RequestProfiles(RnetZone zone)
            {
                var owner = await zone.Controller.GetProfile<IOwner>();
                if (owner != null)
                    return await owner.GetProfiles(zone);

                return null;
            }

            /// <summary>
            /// Obtains the set of profiles supported by the bus object.
            /// </summary>
            /// <returns></returns>
            async Task<object[]> RequestProfiles()
            {
                if (target is RnetDevice)
                    return await RequestProfiles((RnetDevice)target);

                var context = target.Context.Get<IContainerContext>();
                if (context != null &&
                    context.Owner != null)
                {
                    var profile = await context.Owner.GetProfile<IOwner>();
                    if (profile != null)
                        return await profile.GetProfiles(target);
                }

                return null;
            }

            /// <summary>
            /// Creates a generic <see cref="Profile"/> instance that wraps the given information.
            /// </summary>
            /// <param name="target"></param>
            /// <param name="metadata"></param>
            /// <param name="instance"></param>
            /// <returns></returns>
            Profile CreateProfile(RnetBusObject target, ProfileMetadata metadata, object instance)
            {
                return (Profile)Activator.CreateInstance(
                    typeof(Profile<>).MakeGenericType(metadata.Interface),
                        target, metadata, instance);
            }

            /// <summary>
            /// Extracts the supported profile types out of the given instance and returns a set of <see
            /// cref="Profile"/> objects.
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            IEnumerable<Profile> CreateProfiles(object instance)
            {
                return instance.GetType().GetInterfaces()
                    .Select(i => GetOrCreateMetadata(i))
                    .Where(i => i != null)
                    .Select(i => CreateProfile(target, i, instance));
            }

            /// <summary>
            /// Accepts a list of profile instances and emits a set of completed <see cref="Profile"/> classes which
            /// contain the metadata.
            /// </summary>
            /// <param name="instances"></param>
            /// <returns></returns>
            async Task<Profile[]> CreateProfiles(Task<object[]> instances)
            {
                var o = await instances;
                if (o == null)
                    return new Profile[0];

                var l = o
                    .SelectMany(i => CreateProfiles(i))
                    .GroupBy(i => i.Metadata)
                    .Select(i => i.First())
                    .ToArray();

                // initialize any profiles that require it
                foreach (var p in l.Select(i => i.Instance).OfType<IProfileLifecycle>().Distinct())
                    await p.Initialize();

                return l;
            }

            /// <summary>
            /// Gets the set of profiles supported by the bus object.
            /// </summary>
            /// <returns></returns>
            public Task<Profile[]> GetProfiles()
            {
                // fetch from catch or build
                return profiles ?? (profiles = CreateProfiles(RequestProfiles()));
            }

        }

        /// <summary>
        /// Creates a new metadata instance for the given contract type.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        static ProfileMetadata CreateMetadata(Type contract)
        {
            var attr = contract.GetCustomAttribute<ServiceContractAttribute>();
            if (attr == null)
                return null;

            return new ProfileMetadata(attr.Namespace, attr.Name, contract);
        }

        /// <summary>
        /// Either retrieves or creates metadata for the given info.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        static ProfileMetadata GetOrCreateMetadata(Type contract)
        {
            return ProfileManager.metadata.GetOrAdd(contract, m =>
                CreateMetadata(contract));
        }

        /// <summary>
        /// Obtains the set of supported profiles for the given bus object.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Task<Profile[]> GetProfiles(this RnetBusObject target)
        {
            return target.Context.GetOrCreate<Cache>(() =>
                new Cache(target))
                    .GetProfiles();
        }

        /// <summary>
        /// Obtains the profile implementation of the requested type for the given <see cref="RnetBusObject"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static async Task<object> GetProfile(this RnetBusObject target, Type contract)
        {
            return (await GetProfiles(target))
                .Where(i => i.Metadata.Interface == contract)
                .Select(i => i.Instance);
        }

        /// <summary>
        /// Obtains the profile implementation of the requested type for the given <see cref="RnetBusObject"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static async Task<T> GetProfile<T>(this RnetBusObject target)
            where T : class
        {
            return (await GetProfiles(target))
                .Where(i => i.Metadata.Interface == typeof(T))
                .Select(i => i.Instance)
                .OfType<T>()
                .FirstOrDefault();
        }

    }

}