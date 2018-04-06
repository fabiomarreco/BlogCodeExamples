using System;
using System.Linq;
using System.Linq.Expressions;
using SpecificationExample.Products;
using SpecificationExample.Specifications;

namespace SpecificationExample.Infrastructure
{
    public class ProductEFExpressionVisitor
        : EFExpressionVisitor<EFProduct, IProductSpecificationVisitor, Product>,
        IProductSpecificationVisitor
        {
            public override Expression<Func<EFProduct, bool>> ExpressionForSpecification (ISpecification<Product, IProductSpecificationVisitor> spec)
            {
                var visitor = new ProductEFExpressionVisitor ();
                spec.Accept (visitor);
                return visitor.Expr;
            }

            public void Visit (ProductMatchesCategory spec)
            {
                var category = spec.Category;
                Expr = ef => ef.Category == category;
            }

        }

}