using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Odin.DesignContracts;

namespace Odin.System;

/// <summary>
/// 
/// </summary>
public class ClassFactory
{
    /// <summary>
    /// Attempts to create the specified type from the currently loaded application assemblies
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <param name="assemblyToLoadFrom"></param>
    /// <returns></returns>
    public Outcome<T> TryCreate<T>(string fullTypeName, string assemblyToLoadFrom) where T : class
    {
        PreCondition.RequiresNotNullOrWhitespace(fullTypeName);
        PreCondition.RequiresNotNullOrWhitespace(assemblyToLoadFrom);

        AssemblyName assemblyToLoad = new AssemblyName(assemblyToLoadFrom);
        Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyToLoad);
        if (assembly == null) return Outcome.Fail<T>($"Unable to load assembly {assemblyToLoadFrom}");
        
        Type? type = assembly.GetType(fullTypeName);
        if (type == null) return Outcome.Fail<T>($"Unable to create type {fullTypeName} from assembly {assemblyToLoadFrom}");

        object? instance = Activator.CreateInstance(type);
        if (instance == null) return Outcome.Fail<T>($"Could not create instance of type {type.Name}");
        if (instance is T objT)
        {
            return Outcome.Succeed(objT);
        }

        return Outcome.Fail<T>($"Type {type.FullName} is not of type {nameof(T)}");
    }


    /// <summary>
    /// Attempts to create the specified type from the currently loaded application assemblies
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <returns></returns>
    public Outcome<T> TryCreate<T>(string fullTypeName) where T : class
    {
        PreCondition.RequiresNotNullOrWhitespace(fullTypeName);
        Type? typeToCreate = Type.GetType(fullTypeName);
        if (typeToCreate != null)
        {
            return TryCreate<T>(typeToCreate);
        }

        List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        foreach (Assembly asm in assemblies)
        {
            typeToCreate = asm.GetType(fullTypeName);
            if (typeToCreate != null)
                return TryCreate<T>(typeToCreate);
        }

        return Outcome.Fail<T>($"No assembly contains {fullTypeName}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeToCreate"></param>
    /// <returns></returns>
    public Outcome<T> TryCreate<T>(Type typeToCreate) where T : class
    {
        PreCondition.RequiresNotNull(typeToCreate);
        try
        {
            object? obj = Activator.CreateInstance(typeToCreate);
            if (obj == null) return Outcome.Fail<T>($"Could not create instance of type {typeToCreate.Name}");
            if (obj is T objT)
            {
                return Outcome.Succeed(objT);
            }

            return Outcome.Fail<T>($"Type {typeToCreate.FullName} is not of type {nameof(T)}");
        }
        catch (Exception e)
        {
            return Outcome.Fail<T>($"Type {typeToCreate.FullName} could not be created. {e.Message}");
        }
    }
}