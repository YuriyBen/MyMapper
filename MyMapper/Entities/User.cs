using System;
using System.Collections.Generic;
using System.Text;

namespace MyMapper.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        
        public override string ToString()
        {
            return $"Id: {Id}\nPassword: {Password}\nUserName: {UserName}";
        }
    }
}
