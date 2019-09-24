using Microsoft.Build.Execution;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nuget.Transform.MSBuild.Task
{
    public class TransformTask : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            var projectInstance = this.GetProjectInstance();
            var rootNamespace = projectInstance.Properties.FirstOrDefault(p => p.Name == "RootNamespace");
            if (rootNamespace == null)
            {
                throw new Exception("Rootnamespace cannot be found");
            }

            var fileItems = projectInstance.Items.Where(i => i.EvaluatedInclude.EndsWith(".pp")).ToList();
            foreach (var fileItem in fileItems)
            {
                var filePath = Path.Combine(projectInstance.Directory, fileItem.EvaluatedInclude);
                var text = File.ReadAllText(filePath);
                text = text.Replace("$rootnamespace$", rootNamespace.EvaluatedValue);
                var extension = Path.GetExtension(filePath).Replace(".pp", "");
                File.WriteAllText(Path.ChangeExtension(filePath, extension), text);
                File.Delete(filePath);
                projectInstance.RemoveItem(fileItem);
            }

            return true;
        }

        private ProjectInstance GetProjectInstance()
        {
            var type1 = BuildEngine.GetType();
            var field1 = type1.GetField("targetBuilderCallback", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if ((object)field1 == null)
                field1 = type1.GetField("_targetBuilderCallback", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var fieldInfo1 = field1;
            if (fieldInfo1 == null)
                throw new Exception("Could not extract targetBuilderCallback from " + type1.FullName);
            object obj = fieldInfo1.GetValue((object)this.BuildEngine);
            Type type2 = obj.GetType();
            FieldInfo field2 = type2.GetField("projectInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if ((object)field2 == null)
                field2 = type2.GetField("_projectInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            FieldInfo fieldInfo2 = field2;
            if (fieldInfo2 == null)
                throw new Exception("Could not extract projectInstance from " + type2.FullName);
            return (ProjectInstance)fieldInfo2.GetValue(obj);
        }
    }
}
