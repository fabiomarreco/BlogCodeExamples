using System;

namespace SpecEntityNetCore3
{
    public enum Gender
    {
        Male = 0,
        Female = 1
    }


    public class User
    {
        public User(string name, Gender gender, DateTime birthDay)
        {
            Name = name;
            Gender = gender;
            Birthday = birthDay;
        }

        public string Name { get; private set; }
        public Gender Gender { get; private set; }
        public DateTime Birthday { get; private set; }
    }
}
