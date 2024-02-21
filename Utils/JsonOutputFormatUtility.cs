using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class presents a set of methods to simplify workflow with strings and Json strings generation
    /// </summary>
    public static class JsonOutputFormatUtility
    {
        /// <summary>
        /// Used to replace empty spaces in string with "_"
        /// </summary>
        /// <param name="input"></param>
        /// <returns> edited string</returns>
        public static string ReplaceEmptySpaces(string input) {
            return input.Replace(" ", "_");
        }

        /// <summary>
        /// Used to generate name for GameObject with its InstanceId
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>generated name as string</returns>
        public static string GeneratePrometheanObjectName(GameObject obj) {
            var objName = obj.name;
            if (obj.name.Contains($"#{obj.GetInstanceID()}")) {
                return obj.name.Replace("(", "").Replace(")", "");
            }

            objName = ReplaceEmptySpaces(objName);
            objName = objName.Replace("(", "").Replace(")", "");
            return objName.ToLower() + $"#{obj.GetInstanceID()}";
        }

        /// <summary>
        /// cuts off unnecessary parts of materials name
        /// </summary>
        /// <param name="material"></param>
        /// <returns>generated name as string </returns>
        public static string CutMaterialName(Material material) {
            var materialName = material.name;
            materialName = materialName.Replace(" (Instance)", "");
            return materialName;
        }

        /// <summary>
        /// Used to remove Ange brackets from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns>edited string</returns>
        private static string RemoveAngleBrackets(string input) {
            var output = input.Replace(")", "");
            output = output.Replace("(", "");
            return output;
        }

        /// <summary>
        /// Used to remove empty spaces from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns>modified string</returns>
        private static string RemoveEmptySpaces(string input) => input.Replace(" ", "");

        /// <summary>
        /// Used to covert incoming data to json string
        /// </summary>
        /// <param name="data"></param>
        /// <returns> formated Json string</returns>
        public static string GenerateJsonString(object data) {
            var newOutput = JsonConvert.SerializeObject(data);
            newOutput = RemoveAngleBrackets(newOutput);
            return newOutput;
        }

        /// <summary>
        /// Used to extract needed parameters from incoming parameters list
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns> List of incoming parameters as strings </returns>
        public static List<string> ProduceParametersData(List<string> commandParameters) {
            var parameters = commandParameters[0].Split(',').ToList();
            parameters[parameters.Count - 1] = RemoveEmptySpaces(parameters[parameters.Count - 1]);
            return parameters;
        }

        /// <summary>
        /// Used to split first element of a list of strings on a new line
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns>list of strings created from first element of
        /// incoming list</returns>
        public static List<string> SplitStringOnNewLine(List<string> commandParameters) {
            return commandParameters[0].Split('\n').ToList();
        }
    }
}