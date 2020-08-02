using MyMapper.Entities;
using MyMapper.Extensions;
using MyMapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyMapper
{
    class Program
    {
        static StringBuilder Map(string namespaceDTO= "MyMapper.Models", string namespaceEntity= "MyMapper.Entities")
        {
            var classDTONames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceDTO)
                      .Select(x=>x.Name).ToList();


            var classEntityNames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceEntity)
                      .Select(x => x.Name).ToList();

            var x = new List<string>();
            int i = 0;
            Dictionary<string, object> mainPropertiesEntity = new Dictionary<string, object>();
            Dictionary<string, object> mainPropertiesDTO=new Dictionary<string, object>();
            StringBuilder mainLogic=new StringBuilder();
            //mainLogic.AppendLine($"using System;\nusing {namespaceDTO};\nusing {namespaceEntity};");
            //string namespaceForCreation = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
            //mainLogic.AppendLine($"namespace {namespaceForCreation}.Extensions");
            //mainLogic.AppendLine("{");
            while (i!=classEntityNames.Count)
            {
                for (int k = 0; k < classDTONames.Count; k++)
                {
                    if (($"{classEntityNames[i]}dto").Equals(classDTONames[k], StringComparison.InvariantCultureIgnoreCase))
                    {
                        mainLogic.AppendLine($"using System;\nusing {namespaceDTO};\nusing {namespaceEntity};");
                        string namespaceForCreation = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
                        mainLogic.AppendLine($"namespace {namespaceForCreation}.Extensions");
                        mainLogic.AppendLine("{");
                        mainLogic.AppendLine($"   public static class {classDTONames[k]}Extensions");
                        mainLogic.AppendLine("   {");
                        mainLogic.AppendLine($"      public static {classEntityNames[i]} To{classEntityNames[i]}(this {classDTONames[k]} {classDTONames[k].Replace(classDTONames[k][0], char.ToLower(classDTONames[k][0]))})");
                        mainLogic.AppendLine("      {");
                        mainLogic.AppendLine($"\t\t{classEntityNames[i]} {classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))} = new {classEntityNames[i]}(); ");

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
                                    mainLogic.AppendLine($"\t\t{classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))}.{mainPropertiesEntity.ElementAt(j).Key} = {classDTONames[k].Replace(classDTONames[k][0], char.ToLower(classDTONames[k][0]))}.{mainPropertiesDTO.ElementAt(h).Key};");
                                }
                            }
                            j++;
                        }
                        mainLogic.AppendLine($"\t\treturn {classEntityNames[i].Replace(classEntityNames[i][0], char.ToLower(classEntityNames[i][0]))};");
                        mainLogic.AppendLine("      }");
                        mainLogic.AppendLine("   }");
                        mainLogic.AppendLine("}");

                        string folderLocation = @"D:\C#\Console\MyMapper\MyMapper\Extensions";

                        bool exists = System.IO.Directory.Exists(folderLocation);

                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(folderLocation);
                            Console.WriteLine("Creating a folder...");
                        }
                        else
                        {
                            Console.WriteLine("Folder is currently exist..");
                        }
                        string path = $@"{folderLocation}\{classDTONames[k]}Extensions.cs";
                        if (!File.Exists(path))
                        {
                            Console.WriteLine("Creating a file...");

                            using (StreamWriter sw = File.CreateText(path))
                            {
                                sw.Write(mainLogic);
                            }
                        }
                        else
                        {
                            Console.WriteLine("File is currently exist..");
                        }
                    }
                }
                i++;
                Console.WriteLine(mainLogic);
                Console.WriteLine("***********************");
                mainLogic = new StringBuilder();
                mainPropertiesEntity = new Dictionary<string, object>();
                mainPropertiesDTO = new Dictionary<string, object>();
            }

           

            
            

            return mainLogic;
        }
        static void Main(string[] args)
        {
            Map();
            
        }
    }
}
