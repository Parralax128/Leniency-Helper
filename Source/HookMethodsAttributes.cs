using System;

namespace Celeste.Mod.LeniencyHelper;
[AttributeUsage(AttributeTargets.Method)]
class OnLoad : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
class OnUnload : Attribute { }