namespace SpecEntityNetCore3
{
    //************** GENERIC SPECIFICATION (SHARED LIBRARY) *********************//
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


    //************** PRODUCT DEFINITION AND SPECIFICATIONS (DOMAIN) *********************//

    public class Product
    {
        public double Price { get; }
        public string Category { get; }
    }


    public class PriceGreaterThen : ISpecification<Product, IProductSpecificationVisitor>
    {
        public PriceGreaterThen(double limit)
        {
            Limit = limit;
        }

        public double Limit { get; }

        public bool IsSatisfiedBy(Product item) => item.Price > Limit;

        public void Accept(IProductSpecificationVisitor visitor) => visitor.Visit(this);
    }


    public class PriceLesserThen : ISpecification<Product, IProductSpecificationVisitor>
    {
        public PriceLesserThen(double limit)
        {
            Limit = limit;
        }

        public double Limit { get; }

        public bool IsSatisfiedBy(Product item) => item.Price < Limit;

        public void Accept(IProductSpecificationVisitor visitor) => visitor.Visit(this);
    }

    public class ProductOfCategory : ISpecification<Product, IProductSpecificationVisitor>
    {
        public ProductOfCategory(string category)
        {
            Category = category;
        }

        public string Category { get; }
        public bool IsSatisfiedBy(Product item) => item.Category == Category;

        public void Accept(IProductSpecificationVisitor visitor) => visitor.Visit(this);
    }

    public interface IProductSpecificationVisitor : ISpecificationVisitor<IProductSpecificationVisitor, Product>
    {
        void Visit(PriceGreaterThen spec);
        void Visit(PriceLesserThen spec);
        void Visit(ProductOfCategory spec);
    }



    //************** PRODUCT SQL QUERY VISITOR (INFRASTRUCTURE)  *********************//

/*

    public class SQLQueryProductSpecVisitor : IProductSpecificationVisitor
    {
        public string QueryFilter { get; private set; }

        public void Visit(AndSpecification<Product, IProductSpecificationVisitor> spec)
            => QueryFilter = $"({SpecToQueryFilter(spec.Left)}) AND ({SpecToQueryFilter(spec.Right)})";

        public void Visit(OrSpecification<Product, IProductSpecificationVisitor> spec)
            => QueryFilter = $"({SpecToQueryFilter(spec.Left)}) OR ({SpecToQueryFilter(spec.Right)})";

        public void Visit(NotSpecification<Product, IProductSpecificationVisitor> spec)
            => QueryFilter = $"NOT ({SpecToQueryFilter(spec)})";

        public void Visit(PriceGreaterThen spec)
            => QueryFilter = $"PRICE >= {spec.Limit}";

        public void Visit(PriceLesserThen spec)
            => QueryFilter = $"PRICE <= {spec.Limit}";

        public void Visit(ProductOfCategory spec)
            => QueryFilter = $"CATEGORY = '{spec.Category}'";


        private static string SpecToQueryFilter(ISpecification<Product, IProductSpecificationVisitor> spec)
        {
            var visitor = new SQLQueryProductSpecVisitor();
            spec.Accept(visitor);
            return visitor.QueryFilter;
        }

        public static string SpecToQuery(ISpecification<Product, IProductSpecificationVisitor> spec)
            => $"SELECT * FROM PRODUCTS WHERE {SpecToQueryFilter(spec)}";
    }

    //************** SAMPLE USAGE (TEST) *********************//

    public class SpecificationsTests
    {

        [Fact]
        public void Sample_sql_query_creation_through_spec_visitor()
        {
            var spec = 
                new PriceGreaterThen(10)
                    .And(new PriceLesserThen(20))
                .Or(new ProductOfCategory("Electronics"));

            var query = SQLQueryProductSpecVisitor.SpecToQuery(spec);
            Assert.Equal("SELECT * FROM PRODUCTS WHERE ((PRICE >= 10) AND (PRICE <= 20)) OR (CATEGORY = 'Electronics')", query);
        }
    }
 */
}