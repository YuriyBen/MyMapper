using System;
using System.Collections.Generic;
using System.Text;

namespace MyMapper.Models
{
    public class UserDTO
    {
        public int MyProperty { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}\nPassword: {Password}\nUserName: {UserName}\nMyProperty: {MyProperty}";
        }
    }
}
