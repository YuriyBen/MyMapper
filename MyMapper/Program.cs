using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyMapper
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
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User() { Id=12,Password="sdf",UserName="sdfgsdf"};
            Dictionary<string, KeyValuePair<object, object>> propertiesBaseClass = new Dictionary<string, KeyValuePair<object, object>>();
            var arrayUser = user.GetType().GetProperties();
            KeyValuePair<object, object> typeValue;
            foreach (var item in arrayUser)
            {
                typeValue = new KeyValuePair<object, object>(item.PropertyType, item.GetValue(user));
                propertiesBaseClass.Add(item.Name, typeValue);
            }

            UserDTO userDTO = new UserDTO();

            Console.WriteLine("User before manipulation");
            Console.WriteLine(user);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("UserDTO before manipulation");
            Console.WriteLine(userDTO);
            Console.WriteLine();
            Console.WriteLine();
            

            Dictionary<string,KeyValuePair<object, object>> propertiesDTOClass = new Dictionary<string, KeyValuePair<object, object>>();
            var arrayUserDTO = userDTO.GetType().GetProperties();
            KeyValuePair<object, object> typeValueDTO;
            foreach (var item in arrayUserDTO)
            {
                typeValueDTO = new KeyValuePair<object, object>(item.PropertyType, item.GetValue(userDTO));
                propertiesDTOClass.Add(item.Name, typeValueDTO);
            }

           

            int baseLenth = propertiesBaseClass.Count;
            int dtoLenth = propertiesDTOClass.Count;
            int i = 0;
            while (i!=baseLenth )
            {
                for (int k = 0; k < dtoLenth; k++)
                {
                    string baseS = propertiesBaseClass.ElementAt(i).Key;
                    string dtoS = propertiesDTOClass.ElementAt(k).Key;

                    if (baseS == dtoS)
                    {
                        var dictInsideDictBase = propertiesBaseClass.ElementAt(i).Value.Value;
                        
                        PropertyInfo dtoSValue = userDTO.GetType().GetProperty(dtoS);

                        dtoSValue.SetValue(userDTO, dictInsideDictBase);

                    }
                }
                i++;

            }
            Console.WriteLine("User");
            Console.WriteLine(user);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("UserDTO");
            Console.WriteLine(userDTO);
            Console.WriteLine();

        }
    }
}
