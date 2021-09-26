using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Util.Var
{
    public class VariableTypeBuilderWindow : OdinEditorWindow
    {
        
        [MenuItem("Tools/VariableTypeBuilder")]
        [MenuItem("Assets/Create/Custom/new variable...")]
        private static void ShowWindow()
        {
            var window = GetWindow<VariableTypeBuilderWindow>();
            window.titleContent = new GUIContent("Variable Type Builder");
            window.Show();
        }

        [Tooltip("namespace qualified type that you are looking to generate the variable/event for e.g. UnityEngine.Vector2")]
        [SerializeField] private string targetTypeName;
        [Tooltip("Root namespace for generate types. Variable<T> goes in root")]
        [SerializeField] private string rootNamespace = "Util.Var";
        [Tooltip("Namespace of event types will be relative to root. eg YourEvent will be added to {rootNamespace}/{eventNamespace}")]
        [SerializeField] private string eventNamespace = "Events";
        [Tooltip("Namespace of observable types will be relative to root. eg YourObservable will be added to {rootNamespace}/{observableNamespace}")]
        [SerializeField] private string observableNamespace = "Observe";
        [Tooltip("The folder in which to put the Variable type. Event and Observable will be children of this folder")]
        [FolderPath, OnValueChanged("UpdateFullClassPath")]
        [SerializeField] private string rootClassFolder;

        private string _rootClassFilepath;

        [Button, HorizontalGroup("Buttons")]
        private void Generate()
        {
            var prefix = ParsePrefix();
            if (!string.IsNullOrEmpty(prefix))
            {
                GenerateClasses(prefix);
            }
        }

        [Button, HorizontalGroup("Buttons")]
        private void Delete()
        {
            var prefix = ParsePrefix();
            if (!string.IsNullOrEmpty(prefix))
            {
                DeleteClassFiles(prefix);
            }
        }

        private void UpdateFullClassPath()
        {
            var projectDir =
                Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length, "Assets".Length);
            _rootClassFilepath = projectDir + rootClassFolder;
        }
        
      
        private string GetVariableClassName(string prefix) => $"{prefix}Variable";
        private string GetObservableVariableClassName(string prefix) => $"Observable{prefix}Variable";
        
        private string GetVariableReferenceClassName(string prefix) => $"{prefix}Reference";
        private string GetEventClassName(string prefix) => $"{prefix}GameEvent";
        
        private string GetEventListenerClassName(string prefix) => $"{prefix}GameEventListenerBehaviour";
        
        private string GetEventReferenceClassName(string prefix) => $"{prefix}EventReference";
       
        private string ParsePrefix()
        {
            var prefix = TakeLastWhile(targetTypeName, c => char.IsLetterOrDigit(c) || c == '_');

            if (String.IsNullOrEmpty(prefix) || !CodeGenerator.IsValidLanguageIndependentIdentifier(prefix))
            {
                Debug.LogError($"Cannot generate classes with prefx = {prefix}");
                return "";
            }

            prefix = char.ToUpper(prefix.First()) + prefix.Substring(1);


            Debug.Log($"GameEventBuilder: Parsed Prefix = {prefix}");

            return prefix;
        }

        private string TakeLastWhile(string str, Func<char, bool> predicate)
        {
            if (!predicate(str[str.Length - 1])) return "";

            var idx = 0;
            for (int i = str.Length - 1; i > -1; i--)
            {
                if (!predicate(str[i]))
                {
                    idx = i + 1;
                    break;
                }
            }

            return str.Substring(idx);
        }

        private void GenerateClasses(string prefix)
        {
            Debug.Log($"Generating classes of prefix {prefix} in {_rootClassFilepath}");

            GenerateFile(GetVariableFileDefinition(prefix));
            GenerateFile(GetObservableVariableFileDefinition(prefix));
            GenerateFile(GetEventFileDefinition(prefix));
            GenerateFile(GetEventListenerFileDefinition(prefix));

            AssetDatabase.Refresh();
        }

        public class FileDefinition
        {
            public string Dir;
            public string Name;
            public string Namespace;
            public string[] Imports;
            public ClassDefinition[] Classes;

            public string FilePath => Dir + Path.DirectorySeparatorChar + Name + ".cs";
        }

        public struct ClassDefinition
        {
            public string ClassName;
            public AttributeDefinition[] Attributes;
            public string BaseClass;
        }

        public struct AttributeDefinition
        {
            public string Name;
            public string[] Arguments;
        }

        private FileDefinition GetVariableFileDefinition(string prefix)
        {
            return new FileDefinition
            {
                Dir = _rootClassFilepath,
                Name = GetVariableClassName(prefix),
                Namespace = rootNamespace,
                Imports = new[] {"UnityEngine", $"{rootNamespace}.{observableNamespace}"},
                Classes = new[]
                {
                    new ClassDefinition
                    {
                        ClassName = GetVariableClassName(prefix),
                        BaseClass = $"Variable<{targetTypeName}>",
                        Attributes = new []
                        {
                            new AttributeDefinition
                            {
                                Name = "CreateAssetMenu",
                                Arguments = new []{ $"menuName = \"Custom/Variable/{targetTypeName}\"" }
                            }
                        }
                    },
                    new ClassDefinition
                    {
                        ClassName = GetVariableReferenceClassName(prefix),
                        BaseClass = $"VariableReference<{GetVariableClassName(prefix)}, {GetObservableVariableClassName(prefix)}, {targetTypeName}>",
                        Attributes = new []
                        {
                            new AttributeDefinition
                            {
                                Name = "System.Serializable"
                            }
                        }
                    }
                }
            };
        }

        private FileDefinition GetObservableVariableFileDefinition(string prefix)
        {
            return new FileDefinition
            {
                Dir = _rootClassFilepath + Path.DirectorySeparatorChar + observableNamespace,
                Name = GetObservableVariableClassName(prefix),
                Namespace = $"{rootNamespace}.{observableNamespace}",
                Imports = new[] {"UnityEngine"},
                Classes = new[]
                {
                    new ClassDefinition
                    {
                        ClassName = GetObservableVariableClassName(prefix),
                        BaseClass = $"ObservableVariable<{targetTypeName}>",
                        Attributes = new []
                        {
                            new AttributeDefinition
                            {
                                Name = "CreateAssetMenu",
                                Arguments = new []{ $"menuName = \"Custom/Observable/{targetTypeName}\"" }
                            }
                        }
                    }
                }
            };
        }

        private FileDefinition GetEventFileDefinition(string prefix)
        {
            return new FileDefinition
            {
                Dir = _rootClassFilepath + Path.DirectorySeparatorChar + eventNamespace,
                Name = GetEventClassName(prefix),
                Namespace = $"{rootNamespace}.{eventNamespace}",
                Imports = new[] {"UnityEngine", $"{rootNamespace}.{observableNamespace}"},
                Classes = new[]
                {
                    new ClassDefinition
                    {
                        ClassName = GetEventClassName(prefix),
                        BaseClass = $"GameEvent<{targetTypeName}>",
                        Attributes = new []
                        {
                            new AttributeDefinition
                            {
                                Name = "CreateAssetMenu",
                                Arguments = new []{ $"menuName = \"Custom/Event/{targetTypeName}\"" }
                            }
                        }
                    },
                    new ClassDefinition
                    {
                        ClassName = GetEventReferenceClassName(prefix),
                        BaseClass = $"EventReference<{GetEventClassName(prefix)}, {GetObservableVariableClassName(prefix)}, {targetTypeName}>",
                        Attributes = new []
                        {
                            new AttributeDefinition
                            {
                                Name = "System.Serializable"
                            }
                        }
                    }
                }
            };
        }

        private FileDefinition GetEventListenerFileDefinition(string prefix)
        {
            return new FileDefinition
            {
                Dir = _rootClassFilepath + Path.DirectorySeparatorChar + eventNamespace,
                Name = GetEventListenerClassName(prefix),
                Namespace = $"{rootNamespace}.{eventNamespace}",
                Imports = new[] {"UnityEngine"},
                Classes = new[]
                {
                    new ClassDefinition
                    {
                        ClassName = GetEventListenerClassName(prefix),
                        BaseClass = $"GameEventListenerBehaviour<{targetTypeName}, {GetEventReferenceClassName(prefix)}>"
                    }
                }
            };
        }

        private void GenerateFile(FileDefinition file)
        {
            if (string.IsNullOrEmpty(file.Dir) || !Directory.Exists(file.Dir))
            {
                Debug.LogError($"Directory '{file.Dir}' does not exist");
                return;
            }

            var classes = file.Classes.Select(clss =>
            {
                var classDecl = new CodeTypeDeclaration(clss.ClassName)
                {
                    TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
                };

                if (clss.Attributes != null)
                {
                    foreach (var attribute in clss.Attributes)
                    {
                        var attr = new CodeAttributeDeclaration(attribute.Name);
                        if (attribute.Arguments != null)
                        {
                            foreach (var argument in attribute.Arguments)
                            {
                                attr.Arguments.Add(
                                    new CodeAttributeArgument(new CodeSnippetExpression(argument))
                                );
                            }
                        }

                        classDecl.CustomAttributes.Add(attr);
                    }
                }

                var baseClass = new CodeTypeReference(clss.BaseClass);

                classDecl.BaseTypes.Add(baseClass);

                return classDecl;
            });



            var @namespace = new CodeNamespace
            {
                Name = file.Namespace
            };

            foreach (var clss in classes)
            {
                @namespace.Types.Add(clss);
            }

            foreach (var import in file.Imports)
            {
                @namespace.Imports.Add(new CodeNamespaceImport(import));
            }


            var compileUnit = new CodeCompileUnit
            {
                Namespaces = {@namespace}
            };


            WriteCsFile(file.FilePath, compileUnit);
        }

        private static void WriteCsFile(string path, CodeCompileUnit compileUnit)
        {
            Debug.Log($"Writing file {path}");
            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            using var provider = new CSharpCodeProvider();
            using var sw = new StreamWriter(path, false);
            var opt = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = false,
                ElseOnClosing = false,
                VerbatimOrder = false,
                BracingStyle = "C",
                IndentString = "    "
            };
            provider.GenerateCodeFromCompileUnit(compileUnit, sw, opt);
        }

        private void DeleteClassFiles(string prefix)
        {
            var paths = new string[]
            {
                GetVariableFileDefinition(prefix).FilePath,
                GetObservableVariableFileDefinition(prefix).FilePath,
                GetEventFileDefinition(prefix).FilePath,
                GetEventListenerFileDefinition(prefix).FilePath,
            };

            foreach (var path in paths)
            {
                Debug.LogWarning($"Removing {path} + meta file");
                File.Delete(path);
                File.Delete(path + ".meta");
            }

            AssetDatabase.Refresh();
        }
    }
}