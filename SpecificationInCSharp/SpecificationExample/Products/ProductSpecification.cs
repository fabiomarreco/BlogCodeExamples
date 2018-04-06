using System;
using System.Linq.Expressions;
using SpecificationExample.Specifications;

namespace SpecificationExample.Products
{
    public interface IProductSpecificationVisitor : ISpecificationVisitor<IProductSpecificationVisitor, Product> {
        void Visit (ProductMatchesCategory spec);
    }

    public class ProductMatchesCategory : ISpecification<Product, IProductSpecificationVisitor> {
        public readonly string Category;

        public ProductMatchesCategory (string category) {
            this.Category = category;
        }

        public bool IsSatisfiedBy (Product item) => item.Category == Category;

        public void Accept (IProductSpecificationVisitor visitor) {
            visitor.Visit (this); // Now it compiles!
        }
    }

}