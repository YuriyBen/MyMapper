using MyMapper.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MyMapper
{
    public static class StringExtension
    {
        public static string FirstCharacterToLower(this string text)
        {
            return text.Replace(text[0], char.ToLower(text[0]));
        }
    }
    class ClassProperties
    {
        public string ClassName { get; set; }
        public Dictionary<string, object> mainProperties { get; set; } = new Dictionary<string, object>();
    }

    static class FilesManipulation 
    {
        public static void CreateFile(string fullPathToFileWhichWillBeCreated, StringBuilder mainLogicInFile)
        {
            using (StreamWriter sw = File.CreateText(fullPathToFileWhichWillBeCreated))
            {
                sw.WriteLine(mainLogicInFile);
            }
            string fileName = fullPathToFileWhichWillBeCreated.Split(@"\")[^1];
            Console.WriteLine($"Updating a file {fileName}...");
        }
        public static void CreateFolder(string folderLocation)
        {
            bool exists = Directory.Exists(folderLocation);
            string folderName = folderLocation.Split(@"\")[^1];

            if (!exists)
            {
                Directory.CreateDirectory(folderLocation);

                Console.WriteLine($"Creating a folder {folderName}...");
            }
            else
            {
                Console.WriteLine($"Folder {folderName} is currently exist..");
            }
        }
        public static StringBuilder GenerateScriptForFile(string dtoName, string entityName, StringBuilder toDTO, StringBuilder toEntity, string namespaceDTO = "MyMapper.Models", string namespaceEntity = "MyMapper.Entities")
        {
            StringBuilder mainLogic = new StringBuilder();
            mainLogic.AppendLine($"using {namespaceDTO};\nusing {namespaceEntity};");
            string namespaceForCreation = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
            mainLogic.AppendLine($"namespace {namespaceForCreation}.Extensions");
            mainLogic.AppendLine("{");
            mainLogic.AppendLine($"\tpublic static class {entityName}MapperExtensions");
            mainLogic.AppendLine("\t{");
            mainLogic.AppendLine($"\t\tpublic static {entityName} To{entityName}(this {dtoName} {dtoName.FirstCharacterToLower()})");
            mainLogic.AppendLine("\t\t{");
            mainLogic.AppendLine($"\t\t\t{entityName} {entityName.FirstCharacterToLower()} = new {entityName}(); ");

            mainLogic.Append(toDTO);

            mainLogic.AppendLine($"\t\t\treturn {entityName.FirstCharacterToLower()};");
            mainLogic.AppendLine("\t\t}");

            mainLogic.AppendLine($"\t\tpublic static {dtoName} To{dtoName}(this {entityName} {entityName.FirstCharacterToLower()})");
            mainLogic.AppendLine("\t\t{");
            mainLogic.AppendLine($"\t\t\t{dtoName} {dtoName.FirstCharacterToLower()} = new {dtoName}(); ");

            mainLogic.Append(toEntity);

            mainLogic.AppendLine($"\t\t\treturn {dtoName.FirstCharacterToLower()};");
            mainLogic.AppendLine("\t\t}");
            mainLogic.AppendLine("\t}");
            mainLogic.AppendLine("}");
            return mainLogic;
        }

        
    }
    class Program
    {
#warning Relocate the variables
        static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString();
        static string folderLocation = $@"{projectDirectory}\Extensions";
        
        
        static void Map(string namespaceDTO = "MyMapper.Models", string namespaceEntity = "MyMapper.Entities")
        {
            var classDTONames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceDTO)
                      .Select(x => x.Name).ToList();


            var classEntityNames = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.Namespace == namespaceEntity)
                      .Select(x => x.Name).ToList();


            #region Create a dto class if not exist
            var entityClassNamesWithoutDTO = classEntityNames.Except(classDTONames.Select(l => l.Replace("DTO", ""))).ToList();
            foreach (string dtoNameToCreate in entityClassNamesWithoutDTO)
            {
                string[] lines = File.ReadAllLines(@$"{projectDirectory}\Entities\{dtoNameToCreate}.cs");
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Replace($"{namespaceEntity}", $"{namespaceDTO}").Replace($"class {dtoNameToCreate}", $"class {dtoNameToCreate}DTO");
                }
                StringBuilder allLines = new StringBuilder();
                foreach (var item in lines)
                {
                    allLines.AppendLine(item);
                }
                FilesManipulation.CreateFile(@$"{projectDirectory}\Models\{dtoNameToCreate}DTO.cs", allLines);
            }
            #endregion
            //
            List<ClassProperties> mainPropertiesEntity = GetClassProperties(classEntityNames,namespaceEntity);
            List<ClassProperties> mainPropertiesDTO = GetClassProperties(classDTONames, namespaceDTO);

            int j = 0;
            string entityName;
            string dtoName;

            while (j != mainPropertiesEntity.Count)
            {
                for (int h = 0; h < mainPropertiesDTO.Count; h++)
                {
                    entityName = mainPropertiesEntity[j].ClassName;
                    dtoName = mainPropertiesDTO[h].ClassName;
                    if ($"{entityName}dto".Equals($"{dtoName}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        (StringBuilder toEntity, StringBuilder toDTO) GenerateScripts = 
                            SortOutThePropertiesAndGenerateScripts(mainPropertiesEntity[j].mainProperties, mainPropertiesDTO[h].mainProperties,entityName,dtoName);

                        StringBuilder logicData = FilesManipulation.GenerateScriptForFile(dtoName, entityName, GenerateScripts.toDTO, GenerateScripts.toEntity);
                        FilesManipulation.CreateFile($@"{folderLocation}\{entityName}Extensions.cs", logicData);
                    }
                }
                j++;
            }
        }

        private static (StringBuilder, StringBuilder) SortOutThePropertiesAndGenerateScripts(Dictionary<string, object> mainPropertiesEntity, Dictionary<string, object> mainPropertiesDTO,string entityName, string dtoName)
        {
            StringBuilder toDTO = new StringBuilder();
            StringBuilder toEntity = new StringBuilder();
            for (int a = 0; a < mainPropertiesEntity.Count; a++)
            {
                for (int s = 0; s < mainPropertiesDTO.Count; s++)
                {
                    if ((mainPropertiesEntity.ElementAt(a).Key.Equals(mainPropertiesDTO.ElementAt(s).Key)
                    || $"{entityName}{mainPropertiesEntity.ElementAt(a).Key}".Equals(mainPropertiesDTO.ElementAt(s).Key, StringComparison.InvariantCultureIgnoreCase)
                    || mainPropertiesEntity.ElementAt(a).Key.Equals($"{entityName}{mainPropertiesDTO.ElementAt(s).Key}", StringComparison.InvariantCultureIgnoreCase)
                    ) && mainPropertiesEntity.ElementAt(a).Value.Equals(mainPropertiesDTO.ElementAt(s).Value))
                    {
                        toDTO.AppendLine($"\t\t\t{entityName.FirstCharacterToLower()}.{mainPropertiesEntity.ElementAt(a).Key} = {dtoName.FirstCharacterToLower()}.{mainPropertiesDTO.ElementAt(s).Key};");
                        toEntity.AppendLine($"\t\t\t{dtoName.FirstCharacterToLower()}.{mainPropertiesDTO.ElementAt(s).Key} = {entityName.FirstCharacterToLower()}.{mainPropertiesEntity.ElementAt(a).Key};");
                    }
                }
            }
            return (toEntity, toDTO);
        }
        private static List<ClassProperties> GetClassProperties(List<string> classNames,string namespaceToClass)
        {
            List<ClassProperties> mainPropertiesToReturn = new List<ClassProperties>();
            Dictionary<string, object> mainProperties = default;
            for (int x = 0; x < classNames.Count; x++)
            {
                mainProperties = new Dictionary<string, object>();
                var propertiesEntity = Type.GetType($"{namespaceToClass}.{classNames[x]}").GetProperties();
                foreach (var item in propertiesEntity)
                {
                    mainProperties.Add(item.Name, item.PropertyType);
                }
                mainPropertiesToReturn.Add(new ClassProperties { ClassName = classNames[x], mainProperties = mainProperties });
            }
            return mainPropertiesToReturn;
        }

        static void Main(string[] args)
        {

            FilesManipulation.CreateFolder(folderLocation);

            Map();


            Console.WriteLine();



        }
    }
}
