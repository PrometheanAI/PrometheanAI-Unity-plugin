using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class used to by Handles to interact with Unities Undo system
    /// </summary>
    public static class UndoUtility
    {
        static int s_UndoIndex;
        static string s_LastToken;

        /// <summary>
        /// Records Undo for Objects state change or Objects creation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        /// <param name="isObjectCreation"></param>
        public static void RecordUndo(string token, Object obj, bool isObjectCreation) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
                s_UndoIndex = Undo.GetCurrentGroup();
            }

            if (isObjectCreation) {
                s_UndoIndex = Undo.GetCurrentGroup();
                Undo.RegisterCreatedObjectUndo(obj, $"Promethean Command {token}");
            }
            else {
                Undo.RegisterCompleteObjectUndo(obj, $"Promethean Command {token}");
            }

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Overload method for RecordUndo to handle IEnumerable<Object> instead of single Object
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objects"></param>
        /// <param name="isObjectCreation"></param>
        public static void RecordUndo(string token, IEnumerable<Object> objects, bool isObjectCreation) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
                s_UndoIndex = Undo.GetCurrentGroup();
            }

            if (isObjectCreation) {
                foreach (var obj in objects) {
                    Undo.RegisterCreatedObjectUndo(obj, $"Promethean Command {token}");
                }
            }
            else {
                foreach (var obj in objects) {
                    Undo.RegisterCompleteObjectUndo(obj, $"Promethean Command {token}");
                }
            }

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Used to Destroy collection of Objects and record corresponding Undo action
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objects"></param>
        public static void RegisterObjectDestruction(string token, IEnumerable<Object> objects) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
            }

            foreach (var obj in objects) {
                s_UndoIndex = Undo.GetCurrentGroup();
                Undo.RegisterCompleteObjectUndo(obj, $"Promethean Command {token}");
                Undo.DestroyObjectImmediate(obj);
            }

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Used to Destroy Object and record corresponding Undo action
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        public static void RegisterObjectDestruction(string token, Object obj) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
            }

            s_UndoIndex = Undo.GetCurrentGroup();
            Undo.RegisterCompleteObjectUndo(obj, $"Promethean Command {token}");
            Undo.DestroyObjectImmediate(obj);
            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Sets a parent of the Object and record corresponding Undo action
        /// </summary>
        /// <param name="token"></param>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        public static void RecordParenting(string token, GameObject target, GameObject parent) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
                s_UndoIndex = Undo.GetCurrentGroup();
            }

            Undo.SetTransformParent(target.transform, parent != null
                ? parent.transform
                : null, $"Promethean Command {token}");

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Used to record Undo action for setting Object enabled/disabled
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        public static void RecordUndoForHierarchy(string token, Object obj) {
            if (!string.Equals(token, s_LastToken, StringComparison.Ordinal)) {
                s_LastToken = token;
                s_UndoIndex = Undo.GetCurrentGroup();
            }

            Undo.RegisterFullObjectHierarchyUndo(obj, $"Promethean Command {token}");

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Collapses all current Undo actions to corresponding index
        /// </summary>
        public static void CollapseUndo() {
            Undo.CollapseUndoOperations(s_UndoIndex);
        }

        public static string GetLastToken() => s_LastToken;
        public static int GetUndoIndex() => s_UndoIndex;
    }
}