namespace SpecEntityNetCore3
{
    //************** GENERIC SPECIFICATION (SHOUD BE ON A SHARED LIBRARY) *********************//
    public interface ISpecificationVisitor<TVisitor, T> where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        void Visit(AndSpecification<T, TVisitor> spec);
        void Visit(OrSpecification<T, TVisitor> spec);
        void Visit(NotSpecification<T, TVisitor> spec);
    }

    public interface ISpecification<in T, in TVisitor> where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        bool IsSatisfiedBy(T item);
        void Accept(TVisitor visitor);
    }


    public class AndSpecification<T, TVisitor> : ISpecification<T, TVisitor> where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        public ISpecification<T, TVisitor> Left { get; }
        public ISpecification<T, TVisitor> Right { get; }

        public AndSpecification(ISpecification<T, TVisitor> left, ISpecification<T, TVisitor> right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void Accept(TVisitor visitor) => visitor.Visit(this);
        public bool IsSatisfiedBy(T obj) => Left.IsSatisfiedBy(obj) && Right.IsSatisfiedBy(obj);
    }



    public class OrSpecification<T, TVisitor> : ISpecification<T, TVisitor> where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        public ISpecification<T, TVisitor> Left { get; }
        public ISpecification<T, TVisitor> Right { get; }

        public OrSpecification(ISpecification<T, TVisitor> left, ISpecification<T, TVisitor> right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void Accept(TVisitor visitor) => visitor.Visit(this);
        public bool IsSatisfiedBy(T obj) => Left.IsSatisfiedBy(obj) || Right.IsSatisfiedBy(obj);
    }



    public class NotSpecification<T, TVisitor> : ISpecification<T, TVisitor> where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        public ISpecification<T, TVisitor> Specification { get; }

        public NotSpecification(ISpecification<T, TVisitor> specification)
        {
            Specification = specification;
        }

        public void Accept(TVisitor visitor) => visitor.Visit(this);
        public bool IsSatisfiedBy(T obj) => !Specification.IsSatisfiedBy(obj);
    }


    public static class SpecificationExtensions
    {
        public static ISpecification<T, TVisitor> And<T, TVisitor>(this ISpecification<T, TVisitor> left, ISpecification<T, TVisitor> right) where TVisitor : ISpecificationVisitor<TVisitor, T>
            => new AndSpecification<T, TVisitor>(left, right);
        public static ISpecification<T, TVisitor> Or<T, TVisitor>(this ISpecification<T, TVisitor> left, ISpecification<T, TVisitor> right) where TVisitor : ISpecificationVisitor<TVisitor, T>
            => new OrSpecification<T, TVisitor>(left, right);
        public static ISpecification<T, TVisitor> Not<T, TVisitor>(this ISpecification<T, TVisitor> spec) where TVisitor : ISpecificationVisitor<TVisitor, T>
            => new NotSpecification<T, TVisitor>(spec);
    }

}