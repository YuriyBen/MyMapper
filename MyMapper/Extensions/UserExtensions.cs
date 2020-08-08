using MyMapper.Models;
using MyMapper.Entities;
namespace MyMapper.Extensions
{
	public static class UserMapperExtensions
	{
		public static User ToUser(this UserDTO userDTO)
		{
			User user = new User(); 
			user.Id = userDTO.Id;
			user.UserName = userDTO.UserName;
			user.Password = userDTO.Password;
			return user;
		}
		public static UserDTO ToUserDTO(this User user)
		{
			UserDTO userDTO = new UserDTO(); 
			userDTO.Id = user.Id;
			userDTO.UserName = user.UserName;
			userDTO.Password = user.Password;
			return userDTO;
		}
	}
}

