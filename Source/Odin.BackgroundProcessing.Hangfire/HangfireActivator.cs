using System;
using Hangfire;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// To get Hangfire working with .NET Core dependency injection 
    /// </summary>
    public sealed class HangfireActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets a concrete instance of the Type from dependency injection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object? ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }  
}