﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SpecEntityNetCore3.Infrastructure
{

    // SHOULD BE ON SEPARATE LIBRARY
    public abstract class EFExpressionVisitor<TEntity, TVisitor, TItem>
                            where TVisitor : ISpecificationVisitor<TVisitor, TItem>
    {
        public Expression<Func<TEntity, bool>> Expr { get; protected set; }

        public abstract Expression<Func<TEntity, bool>> ConvertSpecToExpression(ISpecification<TItem, TVisitor> spec);

        public void Visit(AndSpecification<TItem, TVisitor> spec)
        {
            var leftExpr = ConvertSpecToExpression(spec.Left);
            var rightExpr = ConvertSpecToExpression(spec.Right);

            var exprBody = Expression.AndAlso(leftExpr.Body, rightExpr.Body);
            Expr = Expression.Lambda<Func<TEntity, bool>>(exprBody, leftExpr.Parameters.Single());
        }

        public void Visit(OrSpecification<TItem, TVisitor> spec)
        {
            var leftExpr = ConvertSpecToExpression(spec.Left);
            var rightExpr = ConvertSpecToExpression(spec.Right);

            var exprBody = Expression.Or(leftExpr.Body, rightExpr.Body);
            Expr = Expression.Lambda<Func<TEntity, bool>>(exprBody, leftExpr.Parameters.Single());
        }

        public void Visit(NotSpecification<TItem, TVisitor> spec)
        {
            var specExpr = ConvertSpecToExpression(spec.Specification);

            var exprBody = Expression.Not(specExpr.Body);
            Expr = Expression.Lambda<Func<TEntity, bool>>(exprBody, specExpr.Parameters.Single());
        }
    }
}
