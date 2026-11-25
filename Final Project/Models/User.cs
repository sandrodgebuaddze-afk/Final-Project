using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        private string _password = string.Empty;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        public User(int id, string fullname, string email, string password)
        {
            Id = id;
            Fullname = fullname;
            Email = email;
            Password = password;
        }
        public override string ToString()
        {
            return $"{Id}|{Fullname}|{Email}|{Password}";
        }
    }
}