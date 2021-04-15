using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace Util.Events
{
    public class GameEventBuilderWindow : EditorWindow
    {
        private const string EVENT_TYPENAME_KEY = "Util.Events.GameEventBuilderWindow.EventTypeName";
        private const string EVENT_NAMESPACE_KEY = "Util.Events.GameEventBuilderWindow.EventNamespace";
        private const string EVENT_FILEPATH_KEY = "Util.Events.GameEventBuilderWindow.EventClassFilepath";
        private const string EDITOR_FILEPATH_KEY = "Util.Events.GameEventBuilderWindow.EditorClassFilepath";

        private const string GAME_EVENT_CLASS_SUFFIX = "GameEvent";
        private const string EVENT_EDITOR_CLASS_SUFFIX = "GameEventEditor";
        private const string EVENT_LISTENER_CLASS_SUFFIX = "GameEventListenerBehaviour";

        public string EventTypeName;
        public string EventNamespace = "Util.Events";
        public string EventClassFilepath;
        public string EditorClassFilepath;

        private SerializedObject _so;

        private SerializedProperty _propEventTypeName;
        private SerializedProperty _propEventNamespace;
        private SerializedProperty _propEventClassFilepath;
        private SerializedProperty _propEditorClassFilepath;


        [MenuItem("Tools/GameEventBuilder")]
        [MenuItem("Assets/Create/Custom/Events/new event...")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameEventBuilderWindow>();
            window.titleContent = new GUIContent("Game Event Builder");
            window.Show();
        }

        private string GetEventPath(string prefix) => EventClassFilepath + Path.DirectorySeparatorChar + prefix +
                                                      GAME_EVENT_CLASS_SUFFIX + ".cs";

        private string GetEditorPath(string prefix) => EditorClassFilepath + Path.DirectorySeparatorChar + prefix +
                                                       EVENT_EDITOR_CLASS_SUFFIX + ".cs";

        private string GetListenerPath(string prefix) => EventClassFilepath + Path.DirectorySeparatorChar + prefix +
                                                         EVENT_LISTENER_CLASS_SUFFIX + "GameEventListenerBehaviour.cs";

        private void OnEnable()
        {
            _so = new SerializedObject(this);
            _propEventTypeName = _so.FindProperty("EventTypeName");
            _propEventNamespace = _so.FindProperty("EventNamespace");
            _propEventClassFilepath = _so.FindProperty("EventClassFilepath");
            _propEditorClassFilepath = _so.FindProperty("EditorClassFilepath");

            EventTypeName = EditorPrefs.GetString(EVENT_TYPENAME_KEY, EventTypeName);
            EventNamespace = EditorPrefs.GetString(EVENT_NAMESPACE_KEY, EventNamespace);
            EventClassFilepath = EditorPrefs.GetString(EVENT_FILEPATH_KEY, EventClassFilepath);
            EditorClassFilepath = EditorPrefs.GetString(EDITOR_FILEPATH_KEY, EditorClassFilepath);
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EVENT_TYPENAME_KEY, EventTypeName);
            EditorPrefs.SetString(EVENT_NAMESPACE_KEY, EventNamespace);
            EditorPrefs.SetString(EVENT_FILEPATH_KEY, EventClassFilepath);
            EditorPrefs.SetString(EDITOR_FILEPATH_KEY, EditorClassFilepath);
        }

        private void OnGUI()
        {
            using (_so.UpdateScope())
            {
                EditorGUILayout.PropertyField(_propEventTypeName);
                EditorGUILayout.PropertyField(_propEventNamespace);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(_propEventClassFilepath);
                    LayoutBrowseButton(_propEventClassFilepath);
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(_propEditorClassFilepath);
                    LayoutBrowseButton(_propEditorClassFilepath);
                }

                if (GUILayout.Button("Generate"))
                {
                    var prefix = ParsePrefix();
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        GenerateClasses(prefix);
                    }
                }

                if (GUILayout.Button("Delete"))
                {
                    var prefix = ParsePrefix();
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        DeleteClassFiles(prefix);
                    }
                }
            }


            // if left mouse clicked then remove focus from our properties
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Repaint();
            }
        }

        private void LayoutBrowseButton(SerializedProperty prop)
        {
            string result = null;

            var openAt = prop.stringValue;
            if (String.IsNullOrEmpty(openAt))
            {
                openAt = Application.dataPath;
            }

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                result = EditorUtility.OpenFolderPanel("", openAt, "");

                GUI.FocusControl(null);
                Repaint();
            }

            if (String.IsNullOrEmpty(result)) return;

            prop.stringValue = result;
        }

        private string ParsePrefix()
        {
            var prefix = TakeLastWhile(EventTypeName, c => char.IsLetterOrDigit(c) || c == '_');

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
            GenerateEventClass(prefix);
            GenerateListenerClass(prefix);
            GenerateEditorClass(prefix);

            AssetDatabase.Refresh();
        }

        private void GenerateEventClass(string prefix)
        {
            if (string.IsNullOrEmpty(EventClassFilepath) || !Directory.Exists(EventClassFilepath))
            {
                Debug.LogError($"Directory '{EventClassFilepath}' does not exist");
                return;
            }

            var path = GetEventPath(prefix);

            var eventClass = new CodeTypeDeclaration(prefix + GAME_EVENT_CLASS_SUFFIX)
            {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };


            var createAssetMenu = new CodeAttributeDeclaration("CreateAssetMenu");
            createAssetMenu.Arguments.Add(
                new CodeAttributeArgument(new CodeSnippetExpression($"menuName = \"Custom/Events/{EventTypeName}\"")));

            var baseClass = new CodeTypeReference($"GameEvent<{EventTypeName}>");

            eventClass.BaseTypes.Add(baseClass);
            eventClass.CustomAttributes.Add(createAssetMenu);

            var @namespace = new CodeNamespace
            {
                Name = _propEventNamespace.stringValue,
                Types = {eventClass},
                Imports = {new CodeNamespaceImport("UnityEngine")}
            };

            var compileUnit = new CodeCompileUnit
            {
                Namespaces = {@namespace}
            };


            WriteCsFile(path, compileUnit);
        }

        private void GenerateEditorClass(string prefix)
        {
            if (string.IsNullOrEmpty(EditorClassFilepath) || !Directory.Exists(EditorClassFilepath))
            {
                Debug.LogError($"Directory '{EditorClassFilepath}' does not exist");
                return;
            }

            var path = GetEditorPath(prefix);

            var editorClass = new CodeTypeDeclaration(prefix + EVENT_EDITOR_CLASS_SUFFIX)
            {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };


            var customEditor = new CodeAttributeDeclaration("CustomEditor");
            var canEditMultipleObjects = new CodeAttributeDeclaration("CanEditMultipleObjects");
            customEditor.Arguments.Add(
                new CodeAttributeArgument(new CodeSnippetExpression($"typeof({prefix + GAME_EVENT_CLASS_SUFFIX})")));

            var baseClass = new CodeTypeReference($"GameEventEditor<{EventTypeName}>");

            editorClass.BaseTypes.Add(baseClass);
            editorClass.CustomAttributes.Add(customEditor);
            editorClass.CustomAttributes.Add(canEditMultipleObjects);

            var @namespace = new CodeNamespace
            {
                Name = _propEventNamespace.stringValue,
                Types = {editorClass},
                Imports = {new CodeNamespaceImport("UnityEditor"), new CodeNamespaceImport("UnityEngine")}
            };

            var compileUnit = new CodeCompileUnit
            {
                Namespaces = {@namespace}
            };


            WriteCsFile(path, compileUnit);
        }

        private void GenerateListenerClass(string prefix)
        {
            if (string.IsNullOrEmpty(EventClassFilepath) || !Directory.Exists(EventClassFilepath))
            {
                Debug.LogError($"Directory '{EventClassFilepath}' does not exist");
                return;
            }

            var path = GetListenerPath(prefix);

            var editorClass = new CodeTypeDeclaration(prefix + EVENT_LISTENER_CLASS_SUFFIX)
            {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            var baseClass =
                new CodeTypeReference($"GameEventListenerBehaviour<{EventTypeName}, {prefix + GAME_EVENT_CLASS_SUFFIX}>");

            editorClass.BaseTypes.Add(baseClass);

            var @namespace = new CodeNamespace
            {
                Name = _propEventNamespace.stringValue,
                Types = {editorClass},
                Imports = {new CodeNamespaceImport("UnityEngine")}
            };

            var compileUnit = new CodeCompileUnit
            {
                Namespaces = {@namespace}
            };

            WriteCsFile(path, compileUnit);
        }

        private static void WriteCsFile(string path, CodeCompileUnit compileUnit)
        {
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
            var eventPath = GetEventPath(prefix);
            var editorPath = GetEditorPath(prefix);
            var listenerPath = GetListenerPath(prefix);

            File.Delete(eventPath);
            File.Delete(eventPath + ".meta");
            File.Delete(editorPath);
            File.Delete(editorPath + ".meta");
            File.Delete(listenerPath);
            File.Delete(listenerPath + ".meta");

            AssetDatabase.Refresh();
        }
    }
}