using System;
using System.Collections.Generic;
using System.Text;

namespace SpecEntityNetCore3.Domain
{
    public interface IUserSpecificationVisitor : ISpecificationVisitor<IUserSpecificationVisitor, User>
    {
        void Visit(UserIsAgeOfMajority spec);
        void Visit(UserHasGender spec);
    }

    public interface IUserSpecification : ISpecification<User, IUserSpecificationVisitor>
    {
    }

    public class UserIsAgeOfMajority : IUserSpecification
    {
        public UserIsAgeOfMajority(int majorityAge)
        {
            MajorityAge = majorityAge;
        }

        public int MajorityAge { get; }
        public void Accept(IUserSpecificationVisitor visitor) => visitor.Visit(this);
        public bool IsSatisfiedBy(User item) => DateTime.Today.Year - item.Birthday.Year > MajorityAge; // lamest implementation
    }



    public class UserHasGender : IUserSpecification
    {
        public UserHasGender(Gender gender)
        {
            Gender = gender;
        }

        public Gender Gender { get; }
        public void Accept(IUserSpecificationVisitor visitor) => visitor.Visit(this);
        public bool IsSatisfiedBy(User item) => item.Gender == Gender;
    }

    public static class UserSpecs
    {
        public static UserHasGender IsMale() => new UserHasGender(Gender.Male);
        public static UserHasGender IsFemale() => new UserHasGender(Gender.Female);
        public static UserIsAgeOfMajority IsOfMajority() => new UserIsAgeOfMajority(18);
    }
}