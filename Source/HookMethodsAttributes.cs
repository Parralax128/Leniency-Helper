using System;

namespace Celeste.Mod.LeniencyHelper;
[AttributeUsage(AttributeTargets.Method)]
public class OnLoad : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class OnUnload : Attribute { }