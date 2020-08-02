using System;
using MyMapper.Models;
using MyMapper.Entities;
namespace MyMapper.Extensions
{
   public static class UserDTOExtensions
   {
      public static User ToUser(this UserDTO userDTO)
      {
		User user = new User(); 
		user.Id = userDTO.Id;
		user.UserName = userDTO.UserName;
		user.Password = userDTO.Password;
		return user;
      }
   }
}
