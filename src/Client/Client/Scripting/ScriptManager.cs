using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Scripting
{
    public static class ScriptManager
    {
        public static readonly string ScriptPath = Program.StartupPath + "scripts\\";
        private static Dictionary<string, CompiledCode> _scriptLibrary = new Dictionary<string, CompiledCode>();
        private static ScriptEngine _engine;
        private static ScriptScope _scope;

        public static void Initialize() {

            // Create a new python interpreter, and a scope to run scripts with.
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();

            // Insert custom functions for scripting use.
            InitializeCommands();

            // Loop through every python file in the script folder.
            foreach (string file in Directory.GetFiles(ScriptPath, "*.py", SearchOption.AllDirectories)) {
                var fi = new FileInfo(file);
                string scriptFile = fi.Name.Replace(".py", string.Empty);
                var source = _engine.CreateScriptSourceFromFile(file).Compile();

                // Add a new dictionary entry with the raw filename as the key, and the
                // compiled source as the value.
                _scriptLibrary.Add(scriptFile, source);
            }
        }

        public static dynamic RunFunction(string key, string functionName, params dynamic[] arguments) {
            // Make sure the collection has a script for the specified key.
            if (_scriptLibrary.ContainsKey(key)) {
                // Return the result of the function if the function exists.
                if (_scope.ContainsVariable(functionName)) {
                    var function = _scope.GetVariable(functionName);
                    return function(arguments);
                }
            }

            // Return null if the function could not be run.
            return null;
        }

        public static void RunFile(string key) {
            // Run the script file if it exists.
            if (_scriptLibrary.ContainsKey(key)) {
                _scriptLibrary[key].Execute(_scope);
            }
        }

        private static void InitializeCommands() {
            _scope.SetVariable("SendDataToPlayer", new Action<int, int>((x, y) => {

            }));
            _scope.SetVariable("SendPlayerToData", new Action<int, int>((x, y) => {

            }));
        }
    }
}
