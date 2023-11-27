namespace WhiteEagles.Test
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.Kernel;

    public class ImmutableArrayCustomization : ICustomization
    {

        public void Customize(IFixture fixture)
            => fixture.Customizations.Add(new ImmutableArrayBuilder(fixture));

        private class ImmutableArrayBuilder : ISpecimenBuilder
        {
            private readonly IFixture _builder;

            public ImmutableArrayBuilder(IFixture builder) => _builder = builder;


            public object Create(object request, ISpecimenContext context)
            {
                return request switch
                {
                    Type type when IsImmutableArrayType(type)
                        => GenerateImmutableArrayInstance(type),
                    _ => new NoSpecimen()
                };
            }

            private static bool IsImmutableArrayType(Type type)
                => type.IsValueType && type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);

            private object GenerateImmutableArrayInstance(Type type)
            {
                var elemType = type.GenericTypeArguments[0];
                var template = typeof(ImmutableArrayBuilder).GetMethod(
                    nameof(GenerateImmutableArray),
                    BindingFlags.NonPublic | BindingFlags.Instance);
                return template.MakeGenericMethod(elemType).Invoke(this, null);
            }

            private ImmutableArray<T> GenerateImmutableArray<T>()
                => ImmutableArray.CreateRange(_builder.CreateMany<T>());

        }
    }
}
