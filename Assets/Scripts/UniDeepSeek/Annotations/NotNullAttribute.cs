using System;

namespace Xiyu.UniDeepSeek.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}