using SpecEntityNetCore3.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SpecEntityNetCore3.Infrastructure
{
    public class UserEFExpressionVisitor : EFExpressionVisitor<DBUser, IUserSpecificationVisitor, User>, IUserSpecificationVisitor
    {
        public override Expression<Func<DBUser, bool>> ConvertSpecToExpression(ISpecification<User, IUserSpecificationVisitor> spec)
        {
            var visitor = new UserEFExpressionVisitor();
            spec.Accept(visitor);
            return visitor.Expr;
        }
    }
}
