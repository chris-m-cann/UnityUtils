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
        private const string EVENT_FILEPATH_KEY = "Util.Events.GameEventBuilderWindow.EventClassFilepath";
        private const string EDITOR_FILEPATH_KEY = "Util.Events.GameEventBuilderWindow.EditorClassFilepath";

        public string EventTypeName;
        public string EventClassFilepath;
        public string EditorClassFilepath;

        private SerializedObject _so;

        private SerializedProperty _propEventTypeName;
        private SerializedProperty _propEventClassFilepath;
        private SerializedProperty _propEditorClassFilepath;


        [MenuItem("Tools/GameEventBuilder")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameEventBuilderWindow>();
            window.titleContent = new GUIContent("Game Event Builder");
            window.Show();
        }

        private void OnEnable()
        {
            _so = new SerializedObject(this);
            _propEventTypeName = _so.FindProperty("EventTypeName");
            _propEventClassFilepath = _so.FindProperty("EventClassFilepath");
            _propEditorClassFilepath = _so.FindProperty("EditorClassFilepath");

            EventTypeName = EditorPrefs.GetString(EVENT_TYPENAME_KEY, EventTypeName);
            EventClassFilepath = EditorPrefs.GetString(EVENT_FILEPATH_KEY, EventClassFilepath);
            EditorClassFilepath = EditorPrefs.GetString(EDITOR_FILEPATH_KEY, EditorClassFilepath);
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EVENT_TYPENAME_KEY, EventTypeName);
            EditorPrefs.SetString(EVENT_FILEPATH_KEY, EventClassFilepath);
            EditorPrefs.SetString(EDITOR_FILEPATH_KEY, EditorClassFilepath);
        }

        private void OnGUI()
        {
            using (_so.UpdateScope())
            {
                EditorGUILayout.PropertyField(_propEventTypeName);

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
                    GenerateClasses();
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
                result =
                    EditorUtility.OpenFolderPanel("", openAt, "");

                GUI.FocusControl(null);
                Repaint();
            }

            if (String.IsNullOrEmpty(result)) return;

            prop.stringValue = result;


        }


        private void GenerateClasses()
        {
            var prefix = TakeLastWhile(EventTypeName, c => char.IsLetterOrDigit(c) || c == '_');

            if (String.IsNullOrEmpty(prefix) || !System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(prefix))
            {
                Debug.LogError($"Cannot generate classes with prefx = {prefix}");
                return;
            }

            prefix = char.ToUpper(prefix.First()) + prefix.Substring(1);

            Debug.Log($"Generate classes with prefix = {prefix}");

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

            var path = EventClassFilepath + Path.DirectorySeparatorChar + prefix + "GameEvent" + ".cs";

            var eventClass = new CodeTypeDeclaration(prefix + "GameEvent")
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
                Name = "Util.Events",
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

            var path = EditorClassFilepath + Path.DirectorySeparatorChar + prefix + "GameEventEditor" + ".cs";

            var editorClass = new CodeTypeDeclaration(prefix + "GameEventEditor")
            {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };


            var customEditor = new CodeAttributeDeclaration("CustomEditor");
            var canEditMultipleObjects = new CodeAttributeDeclaration("CanEditMultipleObjects");
            customEditor.Arguments.Add(
                new CodeAttributeArgument(new CodeSnippetExpression($"typeof({prefix + "GameEvent"})")));

            var baseClass = new CodeTypeReference($"GameEventEditor<{EventTypeName}>");

            editorClass.BaseTypes.Add(baseClass);
            editorClass.CustomAttributes.Add(customEditor);
            editorClass.CustomAttributes.Add(canEditMultipleObjects);

            var @namespace = new CodeNamespace
            {
                Name = "Util.Events",
                Types = {editorClass},
                Imports = {new CodeNamespaceImport("UnityEditor")}
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

            var path = EventClassFilepath + Path.DirectorySeparatorChar + prefix + "GameEventListenerBehaviour" + ".cs";

            var editorClass = new CodeTypeDeclaration(prefix + "GameEventListenerBehaviour")
            {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            var baseClass = new CodeTypeReference($"GameEventListenerBehaviour<{EventTypeName}, {prefix + "GameEvent"}>");

            editorClass.BaseTypes.Add(baseClass);

            var @namespace = new CodeNamespace
            {
                Name = "Util.Events",
                Types = {editorClass}
            };


            var compileUnit = new CodeCompileUnit
            {
                Namespaces = {@namespace}
            };


            WriteCsFile(path, compileUnit);
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




        private static void WriteCsFile(string path, CodeCompileUnit compileUnit)
        {
            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                System.IO.Directory.CreateDirectory(directory);
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
    }
}