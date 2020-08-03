using MyMapper.Entities;
using MyMapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MyMapper
{
    class Program
    {
        #warning Relocate the variables
        static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString();
        static string folderLocation = $@"{projectDirectory}\Extensions";
        static void/* ? void ? */ CreateFolder(string folderLocation)
        {
            bool exists = Directory.Exists(folderLocation);

            if (!exists)
            {
                Directory.CreateDirectory(folderLocation);
                Console.WriteLine("Creating a folder...");
            }
            else
            {
                Console.WriteLine("Folder is currently exist..");
            }
        }
        static void/* ? void ? */ CreateFile(string fileName,StringBuilder mainLogicInFile)
        {
            string path = $@"{folderLocation}\{fileName}Extensions.cs";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(mainLogicInFile);

            }

        }
        static string FirstCharacterToLow(string text)
        {
            return text.Replace(text[0], char.ToLower(text[0]));
        }
        static StringBuilder GenerateScriptForFile(string dtoName, string entityName,StringBuilder toDTO, StringBuilder toEntity, string namespaceDTO = "MyMapper.Models", string namespaceEntity = "MyMapper.Entities")
        {
            StringBuilder mainLogic = new StringBuilder();
            mainLogic.AppendLine($"using {namespaceDTO};\nusing {namespaceEntity};");
            string namespaceForCreation = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
            mainLogic.AppendLine($"namespace {namespaceForCreation}.Extensions");
            mainLogic.AppendLine("{");
            mainLogic.AppendLine($"\tpublic static class {entityName}MapperExtensions");
            mainLogic.AppendLine("\t{");
            mainLogic.AppendLine($"\t\tpublic static {entityName} To{entityName}(this {dtoName} {FirstCharacterToLow(dtoName)})");
            mainLogic.AppendLine("\t\t{");
            mainLogic.AppendLine($"\t\t\t{entityName} {FirstCharacterToLow(entityName)} = new {entityName}(); ");

            mainLogic.Append(toDTO);
            
            mainLogic.AppendLine($"\t\t\treturn {FirstCharacterToLow(entityName)};");
            mainLogic.AppendLine("\t\t}");

            mainLogic.AppendLine($"\t\tpublic static {dtoName} To{dtoName}(this {entityName} {FirstCharacterToLow(entityName)})");
            mainLogic.AppendLine("\t\t{");
            mainLogic.AppendLine($"\t\t\t{dtoName} {FirstCharacterToLow(dtoName)} = new {dtoName}(); ");

            mainLogic.Append(toEntity);

            mainLogic.AppendLine($"\t\t\treturn {FirstCharacterToLow(dtoName)};");
            mainLogic.AppendLine("\t\t}");
            mainLogic.AppendLine("\t}");
            mainLogic.AppendLine("}");
            return mainLogic;
        }
        static void Map(string namespaceDTO= "MyMapper.Models", string namespaceEntity= "MyMapper.Entities")
        {
            var classDTONames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceDTO)
                      .Select(x=>x.Name).ToList();


            var classEntityNames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceEntity)
                      .Select(x => x.Name).ToList();

            int i = 0;
            Dictionary<string, object> mainPropertiesEntity=null;
            Dictionary<string, object> mainPropertiesDTO = null;
            StringBuilder toDTO = null ;
            StringBuilder toEntity = null;

            string dtoName = null;
            string entityName = null;

            while (i!=classEntityNames.Count)
            {
                toDTO = new StringBuilder();
                toEntity = new StringBuilder();

                mainPropertiesEntity = new Dictionary<string, object>();
                mainPropertiesDTO = new Dictionary<string, object>();
                for (int k = 0; k < classDTONames.Count; k++)
                {
                    if (($"{classEntityNames[i]}dto").Equals(classDTONames[k], StringComparison.InvariantCultureIgnoreCase))
                    {
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
                        dtoName = classDTONames[k];
                        entityName = classEntityNames[i];

                    }
                }

                int j = 0;
                while (j != mainPropertiesEntity.Count)
                {
                    for (int h = 0; h < mainPropertiesDTO.Count; h++)
                    {
                        if ((mainPropertiesEntity.ElementAt(j).Key.Equals(mainPropertiesDTO.ElementAt(h).Key)
                            || $"{entityName}{mainPropertiesEntity.ElementAt(j).Key}".Equals(mainPropertiesDTO.ElementAt(h).Key, StringComparison.InvariantCultureIgnoreCase)
                            || mainPropertiesEntity.ElementAt(j).Key.Equals($"{entityName}{mainPropertiesDTO.ElementAt(h).Key}", StringComparison.InvariantCultureIgnoreCase)
                            ) && mainPropertiesEntity.ElementAt(j).Value.Equals(mainPropertiesDTO.ElementAt(h).Value))
                        {
                            toDTO.AppendLine($"\t\t\t{FirstCharacterToLow(entityName)}.{mainPropertiesEntity.ElementAt(j).Key} = {FirstCharacterToLow(dtoName)}.{mainPropertiesDTO.ElementAt(h).Key};");
                            toEntity.AppendLine($"\t\t\t{FirstCharacterToLow(dtoName)}.{mainPropertiesDTO.ElementAt(h).Key} = {FirstCharacterToLow(entityName)}.{mainPropertiesEntity.ElementAt(j).Key};");

                        }
                    }

                    j++;
                }
                CreateFile(entityName, GenerateScriptForFile(dtoName,entityName, toDTO,toEntity));

                i++;
            }

        }
        static void Main(string[] args)
        {

            CreateFolder(folderLocation);

            Map();
            Console.WriteLine();




        }
    }
}
