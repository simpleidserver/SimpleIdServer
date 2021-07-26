using Medallion.Threading.FileSystem;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Nuget.Transform.MSBuild.Task
{
    public class TransformTask : Microsoft.Build.Utilities.Task
    {
        private const string LOCK_FILE_NAME = "Nuget.Transform.MSBuild.Task.lock";
        private static string MANAGED_DLL_DIRECTORY = Path.GetDirectoryName(new Uri(typeof(TransformTask).GetTypeInfo().Assembly.CodeBase).LocalPath);

        public override bool Execute()
        {
            var projectInstance = this.GetProjectInstance();
            var rootNamespace = projectInstance.Properties.FirstOrDefault(p => p.Name == "RootNamespace");
            if (rootNamespace == null)
            {
                throw new Exception("Rootnamespace cannot be found");
            }

            var lck = new FileDistributedLock(new DirectoryInfo(projectInstance.Directory), "MyLockName");
            using (var handle = lck.TryAcquire())
            {
                if (handle == null)
                {
                    return Execute();
                }

                var lockFilePath = Path.Combine(projectInstance.Directory, LOCK_FILE_NAME);
                var lockFiles = DeserializeLockFile(projectInstance.Directory);
                foreach (var filePath in Directory.GetFiles(projectInstance.Directory, "*.pp", SearchOption.AllDirectories))
                {
                    var text = File.ReadAllText(filePath);
                    using (var sha256 = SHA256.Create())
                    {
                        var hashPayload = sha256.ComputeHash(Encoding.UTF8.GetBytes((text)));
                        var hashContent = Convert.ToBase64String(hashPayload.Take(100).ToArray());
                        var kvp = lockFiles.FirstOrDefault(k => k.Key.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));
                        if (!kvp.Equals(default(KeyValuePair<string, string>)) && kvp.Value.Equals(hashContent, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        if (!kvp.Equals(default(KeyValuePair<string, string>)))
                        {
                            lockFiles.Remove(kvp.Key);
                        }

                        lockFiles.Add(filePath, hashContent);
                        text = text.Replace("$rootnamespace$", rootNamespace.EvaluatedValue);
                        var extension = Path.GetExtension(filePath).Replace(".pp", "");
                        var newFilePath = Path.ChangeExtension(filePath, extension);
                        File.WriteAllText(newFilePath, text);
                    }
                }

                SerializeLockFile(projectInstance.Directory, lockFiles);
                return true;
            }
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

        private static Dictionary<string, string> DeserializeLockFile(string directory)
        {
            var result = new Dictionary<string, string>();
            var filePath = Path.Combine(directory, LOCK_FILE_NAME);
            if (File.Exists(filePath))
            {
                var fileText = File.ReadAllText(filePath);
                foreach(var line in fileText.Split(';'))
                {
                    var splittedLine = line.Split('$');
                    result.Add(splittedLine.First(), splittedLine.Last());
                }
            }

            return result;
        }

        private static void SerializeLockFile(string directory, Dictionary<string, string> content)
        {
            var filePath = Path.Combine(directory, LOCK_FILE_NAME);
            var lst = new List<string>();
            foreach(var kvp in content)
            {
                lst.Add($"{kvp.Key}${kvp.Value}");
            }

            var serialized = string.Join(";", lst);
            File.WriteAllText(filePath, serialized);
        }
    }
}
