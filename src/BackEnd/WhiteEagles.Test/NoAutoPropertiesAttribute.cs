﻿namespace WhiteEagles.Test
{
    using System;
    using System.Reflection;
    using AutoFixture;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NoAutoPropertiesAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var targetType = parameter.ParameterType;
            return new NoAutoPropertiesCustomization(targetType);
        }
    }
}
