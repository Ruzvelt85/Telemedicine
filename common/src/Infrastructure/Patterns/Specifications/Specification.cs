using System;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines base specification class
    /// </summary>
    public abstract record Specification<T> : ISpecification<T>
    {
        /// <summary>
        /// Expression of specification. It allows to check if item is satisfied by specification
        /// </summary>
        protected abstract Expression<Func<T, bool>> Predicate { get; }

        /// <inheritdoc />
        public bool IsSatisfiedBy(T item) => Predicate.Compile()(item);

        public Specification<TNew> As<TNew>() where TNew : T => new AsSpecification<T, TNew>(this);

        public static Specification<T> operator !(Specification<T> specification) => new NotSpecification<T>(specification);

        public static Specification<T> operator &(Specification<T> left, Specification<T> right) => new AndSpecification<T>(left, right);

        public static Specification<T> operator |(Specification<T> left, Specification<T> right) => new OrSpecification<T>(left, right);

        public static implicit operator Predicate<T>(Specification<T> specification) => specification.IsSatisfiedBy;

        public static implicit operator Func<T, bool>(Specification<T> specification) => specification.IsSatisfiedBy;

        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.Predicate;
    }
}
