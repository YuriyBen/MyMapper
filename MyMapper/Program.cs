using MyMapper.Entities;
using MyMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyMapper
{
    
    
    struct MainProperties
    {
        public string PropertyName { get; set; }
        public object PropertyType { get; set; }
    }
    class Program
    {
        static MainProperties Map(string namespaceDTO, string namespaceEntity)
        {
            var classDTONames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceDTO)
                      .Select(x=>x.Name).ToList();


            var classEntityNames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceEntity)
                      .Select(x => x.Name).ToList();

            var x = new List<string>();
            int i = 0;
            Dictionary<string,object> mainPropertiesEntity=new Dictionary<string, object>(); 
            Dictionary<string, object> mainPropertiesDTO=new Dictionary<string, object>();
            StringBuilder mainLogic=new StringBuilder();
            mainLogic.AppendLine($"using System;\nusing {namespaceDTO};\nusing {namespaceEntity};");
            string namespaceForCreation = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
            mainLogic.AppendLine($"namespace {namespaceForCreation}.Extensions");
            mainLogic.AppendLine("{");
            while (i!=classEntityNames.Count)
            {
                for (int k = 0; k < classDTONames.Count; k++)
                {
                    if (($"{classEntityNames[i]}dto").Equals(classDTONames[k], StringComparison.InvariantCultureIgnoreCase))
                    {
                        mainLogic.AppendLine($"  public static {classEntityNames[i]} To{classEntityNames[i]}(this {classDTONames[k]} {classDTONames[k].Replace(classDTONames[k][0], char.ToLower(classDTONames[k][0]))})");
                        mainLogic.AppendLine("   {");
                        mainLogic.AppendLine($"\t{classEntityNames[i]} {classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))} = new {classEntityNames[i]}(); ");

                        var propertiesEntity = Type.GetType($"{namespaceEntity}.{classEntityNames[i]}").GetProperties();
                        foreach (var item in propertiesEntity)
                        {
                            mainPropertiesEntity.Add(item.Name, item.PropertyType);
                        }

                        var propertiesDTO = Type.GetType($"{namespaceDTO}.{classDTONames[k]}").GetProperties();
                        foreach (var item in propertiesDTO)
                        {
                            mainPropertiesDTO.Add(item.Name, item.PropertyType);
                        }
                        int j = 0;
                        while (j != mainPropertiesEntity.Count)
                        {
                            for (int h = 0; h < mainPropertiesDTO.Count; h++)
                            {
                                if (mainPropertiesEntity.ElementAt(j).Equals(mainPropertiesDTO.ElementAt(h)))
                                {
                                    mainLogic.AppendLine($"\t{classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))}.{mainPropertiesEntity.ElementAt(j).Key} = {classDTONames[k].Replace(classDTONames[k][0], char.ToLower(classDTONames[k][0]))}.{mainPropertiesDTO.ElementAt(h).Key};");
                                }
                            }
                            j++;
                        }
                        mainLogic.AppendLine($"\treturn {classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))};");
                    }
                }
                i++;
            }

            mainLogic.AppendLine("   }");
            mainLogic.AppendLine("}");

            Console.WriteLine(mainLogic);
            

            return default;
        }
        static void Main(string[] args)
        {

            Map("MyMapper.Models", "MyMapper.Entities");
            //TODO
           
        }
    }
}
