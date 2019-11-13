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

        public void Visit(UserIsAgeOfMajority spec)
        {
            var initialDate = DateTime.Today.AddYears(-spec.MajorityAge);
            Expr = ef => ef.Birthday <= initialDate;
        }

        public void Visit(UserHasGender spec)
        {
            //make sure to get the value before out of the spec before passint to the expression!!!
            var gender = spec.Gender;
            Expr = ef => ef.Gender == gender;
        }
    }


    public static class InfrastructureUserSpecificationExtension
    {
        public static Expression<Func<DBUser, bool>> ToEFExpression(this IUserSpecification spec)
        {
            var visitor = new UserEFExpressionVisitor();
            return visitor.ConvertSpecToExpression(spec);
        }
    }
}
