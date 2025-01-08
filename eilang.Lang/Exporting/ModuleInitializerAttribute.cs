using System;

namespace eilang.Exporting;

/// <summary>
/// Apply on module initializers, required when defining new classes in a module.
/// Can only be used on a static method with the signature <see cref="Void"/>(<see cref="Interfaces.IEnvironment"/>).
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ModuleInitializerAttribute : Attribute;
