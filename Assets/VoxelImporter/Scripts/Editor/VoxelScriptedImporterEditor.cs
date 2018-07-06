﻿#if UNITY_2017_1_OR_NEWER

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System;

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelScriptedImporter)), CanEditMultipleObjects]
    public class VoxelScriptedImporterEditor : ScriptedImporterEditor
    {
        private static bool advancedMode;

        private SerializedProperty legacyVoxImportProp;
        private SerializedProperty importModeProp;
        private SerializedProperty importScaleProp;
        private SerializedProperty importOffsetProp;
        private SerializedProperty combineFacesProp;
        private SerializedProperty ignoreCavityProp;
        private SerializedProperty outputStructureProp;
        private SerializedProperty generateLightmapUVsProp;
        private SerializedProperty meshFaceVertexOffsetProp;
        private SerializedProperty retainExistingProp;
        private SerializedProperty loadFromVoxelFileProp;
        private SerializedProperty generateMipMapsProp;
        private SerializedProperty colliderTypeProp;

        private readonly GUIContent[] ImportModeStrings =
        {
            new GUIContent(VoxelBase.ImportMode.LowTexture.ToString()),
            new GUIContent(VoxelBase.ImportMode.LowPoly.ToString()),
        };
        private readonly int[] ImportModeValues =
        {
            (int)VoxelBase.ImportMode.LowTexture,
            (int)VoxelBase.ImportMode.LowPoly,
        };
        private readonly GUIContent[] ColliderTypeStrings =
        {
            new GUIContent(VoxelScriptedImporter.ColliderType.None.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Box.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Sphere.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Capsule.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Mesh.ToString()),
        };
        private readonly int[] ColliderTypeValues =
        {
            (int)VoxelScriptedImporter.ColliderType.None,
            (int)VoxelScriptedImporter.ColliderType.Box,
            (int)VoxelScriptedImporter.ColliderType.Sphere,
            (int)VoxelScriptedImporter.ColliderType.Capsule,
            (int)VoxelScriptedImporter.ColliderType.Mesh,
        };

        public override void OnEnable()
        {
            base.OnEnable();

            legacyVoxImportProp = serializedObject.FindProperty("legacyVoxImport");
            importModeProp = serializedObject.FindProperty("importMode");
            importScaleProp = serializedObject.FindProperty("importScale");
            importOffsetProp = serializedObject.FindProperty("importOffset");
            combineFacesProp = serializedObject.FindProperty("combineFaces");
            ignoreCavityProp = serializedObject.FindProperty("ignoreCavity");
            outputStructureProp = serializedObject.FindProperty("outputStructure");
            generateLightmapUVsProp = serializedObject.FindProperty("generateLightmapUVs");
            meshFaceVertexOffsetProp = serializedObject.FindProperty("meshFaceVertexOffset");
            retainExistingProp = serializedObject.FindProperty("retainExisting");
            loadFromVoxelFileProp = serializedObject.FindProperty("loadFromVoxelFile");
            generateMipMapsProp = serializedObject.FindProperty("generateMipMaps");
            colliderTypeProp = serializedObject.FindProperty("colliderType");
        }

        public override void OnInspectorGUI()
        {
            var vtarget = target as VoxelScriptedImporter;
            if (vtarget == null)
            {
                base.OnInspectorGUI();
                return;
            }

            #region Simple
            {
                EditorGUI.BeginChangeCheck();
                var mode = GUILayout.Toolbar(advancedMode ? 1 : 0, VoxelBaseEditor.Edit_AdvancedModeStrings);
                if (EditorGUI.EndChangeCheck())
                {
                    advancedMode = mode != 0 ? true : false;
                }
            }
            #endregion

            #region Settings
            {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    if (advancedMode)
                    {
                        if (vtarget.fileType == VoxelBase.FileType.vox)
                        {
                            EditorGUILayout.PropertyField(legacyVoxImportProp, new GUIContent("Legacy Vox Import", "Import with legacy behavior up to Version 1.1.2.\nMultiple objects do not correspond.\nIt is deprecated for future use.\nThis is left for compatibility."));
                        }
                        EditorGUILayout.IntPopup(importModeProp, ImportModeStrings, ImportModeValues, new GUIContent("Import Mode"));
                    }
                    EditorGUILayout.PropertyField(importScaleProp);
                    EditorGUILayout.PropertyField(importOffsetProp);
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Optimize
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Optimize", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(combineFacesProp, new GUIContent("Combine Voxel Faces"));
                    EditorGUILayout.PropertyField(ignoreCavityProp, new GUIContent("Ignore Cavity"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Output
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(outputStructureProp, new GUIContent("Voxel Structure", "Save the structure information."));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Mesh
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(generateLightmapUVsProp, new GUIContent("Generate Lightmap UVs", "Generate lightmap UVs into UV2."));
                    EditorGUILayout.Slider(meshFaceVertexOffsetProp, 0f, 0.01f, new GUIContent("Vertex Offset", "Increase this value if flickering of polygon gaps occurs at low resolution."));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Material
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(retainExistingProp, new GUIContent("Retain Existing Materials", "When disabled, existing settings are always discarded."));
                    EditorGUILayout.PropertyField(loadFromVoxelFileProp, new GUIContent("Import Materials"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Texture
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(generateMipMapsProp, new GUIContent("Generate Mip Maps"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Collider
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Collider", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.IntPopup(colliderTypeProp, ColliderTypeStrings, ColliderTypeValues, new GUIContent("Generate Colliders"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            ApplyRevertGUI();
        }
    }
}
#endif