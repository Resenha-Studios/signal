/* ==============================================================================
 * CLASS: SignalAudioDashboard
 * DESCRIPTION: Main Editor Window for Signal Audio Architecture. Handles 1-Click
 * initial setup, audio library management, import tools, batch renaming, 
 * audio previews, and code generation for AudioKeys.
 * Standardized for Resenha Studios with UI Toolkit & Multi-language localization.
 * ==============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Resenha.Signal.Core;
using Resenha.Signal.Data;
using Resenha.Signal.Editor.Localization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace Resenha.Signal.Editor.UI
{
    public class SignalAudioDashboard : EditorWindow
    {
        private VisualElement _dashboardContainer;
        private VisualElement _welcomeScreen;
        private VisualElement _mainContent;
        private UQueryState<Button> _sidebarButtons;
        private Button _activeTabButton;
        private SerializedObject _serializedConfig;
        
        private AudioSource _previewSource;
        private GameObject _previewObject;

        [MenuItem("Tools/Resenha/Signal %#s", priority = 101)]
        public static void ShowWindow()
        {
            SignalAudioDashboard wnd = GetWindow<SignalAudioDashboard>();
            wnd.titleContent = new GUIContent("Signal", EditorGUIUtility.IconContent("d_AudioSource Icon").image);
            wnd.minSize = new Vector2(760, 480);
            wnd.Show();
        }

        private void OnEnable()
        {
            LoadSerializedConfiguration();
            CreatePreviewAudioSource();
        }

        private void OnDisable()
        {
            if (_previewObject != null)
            {
                DestroyImmediate(_previewObject);
            }
        }

        private void CreatePreviewAudioSource()
        {
            if (_previewObject == null)
            {
                _previewObject = new GameObject("SignalAudio_Preview_Hidden");
                _previewObject.hideFlags = HideFlags.HideAndDontSave;
                _previewSource = _previewObject.AddComponent<AudioSource>();
                _previewSource.playOnAwake = false;
            }
        }

        private void LoadSerializedConfiguration()
        {
            string[] guids = AssetDatabase.FindAssets("t:SoundManagerSO");
            if (guids.Length > 0)
            {
                var configAsset = AssetDatabase.LoadAssetAtPath<SoundManagerSO>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (configAsset != null)
                {
                    _serializedConfig = new SerializedObject(configAsset);
                }
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            string[] uxmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset SignalAudioDashboard");
            if (uxmlGuids.Length == 0) return;
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(uxmlGuids[0]));
            visualTree.CloneTree(root);

            string[] ussGuids = AssetDatabase.FindAssets("t:StyleSheet SignalStyle");
            if (ussGuids.Length > 0)
            {
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(ussGuids[0]));
                root.styleSheets.Add(styleSheet);
            }

            _dashboardContainer = root.Q<VisualElement>("dashboard-container");
            _welcomeScreen = root.Q<VisualElement>("welcome-screen");
            
            root.schedule.Execute(() => {
                CheckResponsivity(root.resolvedStyle.width);
            }).StartingIn(100);

            Button setupBtn = root.Q<Button>("btn-initial-setup");
            if (setupBtn != null) setupBtn.clicked += RunInitialSetupWithFolderPicker;

            Button headerSetupBtn = root.Q<Button>("btn-header-setup");
            if (headerSetupBtn != null) headerSetupBtn.clicked += RunInitialSetupWithFolderPicker;

            SetupLanguageSelector(root);
            InitializeTabs(root);
            ApplyLocalization(root);
            UpdateUIState();
            root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void SetupLanguageSelector(VisualElement root)
        {
            var dropdown = root.Q<DropdownField>("language-dropdown");
            if (dropdown == null) return;

            dropdown.choices = new List<string> { "English", "Português", "Español", "日本語", "简体中文" };
            dropdown.index = (int)SignalStrings.CurrentLanguage;

            dropdown.RegisterValueChangedCallback(evt =>
            {
                int idx = dropdown.choices.IndexOf(evt.newValue);
                if (idx >= 0)
                {
                    SignalStrings.CurrentLanguage = (SignalLanguage)idx;
                    ApplyLocalization(root);
                    if (_activeTabButton != null)
                    {
                        LoadTabContent(_activeTabButton.name);
                    }
                }
            });
        }

        private void ApplyLocalization(VisualElement root)
        {
            SetLabelText(root.Q<Label>("header-title"), SignalStrings.Get("window_title"));
            SetLabelText(root.Q<Label>("header-subtitle"), SignalStrings.Get("window_subtitle"));
            
            SetLabelText(root.Q<Label>("welcome-title"), SignalStrings.Get("welcome_title"));
            SetLabelText(root.Q<Label>("welcome-subtitle"), SignalStrings.Get("welcome_subtitle"));
            SetLabelText(root.Q<Label>("welcome-desc"), SignalStrings.Get("welcome_desc"));
            
            var welcomeBtn = root.Q<Button>("btn-initial-setup");
            if (welcomeBtn != null) welcomeBtn.text = SignalStrings.Get("welcome_btn");

            SetLabelText(root.Q<Label>("lbl-tab-global"), SignalStrings.Get("tab_global"));
            SetLabelText(root.Q<Label>("lbl-tab-music"), SignalStrings.Get("tab_music"));
            SetLabelText(root.Q<Label>("lbl-tab-sfx"), SignalStrings.Get("tab_sfx"));
            SetLabelText(root.Q<Label>("lbl-tab-tools"), SignalStrings.Get("tab_tools"));
            SetLabelText(root.Q<Label>("lbl-tab-help"), SignalStrings.Get("tab_help"));
        }

        private void SetLabelText(Label label, string text)
        {
            if (label != null) label.text = text;
        }

        private void RunInitialSetupWithFolderPicker()
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Setup Folder", "Assets", "");
    
            if (string.IsNullOrEmpty(selectedPath)) return;
            
            if (selectedPath.StartsWith(Application.dataPath)) {
                selectedPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            } else {
                EditorUtility.DisplayDialog("Error", "Please select a folder inside your Assets directory.", "OK");
                return;
            }

            RunInitialSetup(selectedPath);
        }
        
        private void CheckResponsivity(float width)
        {
            if (_dashboardContainer == null) return;
            if (width < 600f) _dashboardContainer.AddToClassList("collapsed-mode");
            else _dashboardContainer.RemoveFromClassList("collapsed-mode");
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (_dashboardContainer == null) return;
            
            if (evt.newRect.width < 600f)
            {
                _dashboardContainer.AddToClassList("collapsed-mode");
            }
            else
            {
                _dashboardContainer.RemoveFromClassList("collapsed-mode");
            }
        }

        private void UpdateUIState()
        {
            if (_serializedConfig == null)
            {
                _dashboardContainer.style.display = DisplayStyle.None;
                _welcomeScreen.style.display = DisplayStyle.Flex;
            }
            else
            {
                _dashboardContainer.style.display = DisplayStyle.Flex;
                _welcomeScreen.style.display = DisplayStyle.None;
                
                var firstBtn = _dashboardContainer.Q<Button>("tab-global");
                if (firstBtn != null) 
                {
                    OnTabClicked(firstBtn);
                }
            }
        }

        private void InitializeTabs(VisualElement root)
        {
            _mainContent = root.Q<VisualElement>("main-content");
            _sidebarButtons = root.Query<Button>(className: "sidebar-btn").Build();
            
            SetIcon(root.Q<VisualElement>("icon-global"), "icon_settings");
            SetIcon(root.Q<VisualElement>("icon-music"), "icon_music");
            SetIcon(root.Q<VisualElement>("icon-sfx"), "icon_sfx");
            SetIcon(root.Q<VisualElement>("icon-tools"), "icon_import");
            SetIcon(root.Q<VisualElement>("icon-help"), "icon_help");

            foreach (var button in _sidebarButtons)
            {
                button.clicked += () => OnTabClicked(button);
            }
        }

        private void SetIcon(VisualElement element, string iconName)
        {
            if (element == null) return;
            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");
            if (guids.Length > 0)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
                element.style.backgroundImage = new StyleBackground(tex);
            }
        }

        private void OnTabClicked(Button clickedButton)
        {
            foreach (var button in _sidebarButtons)
            {
                button.RemoveFromClassList("sidebar-btn-active");
            }

            clickedButton.AddToClassList("sidebar-btn-active");
            _activeTabButton = clickedButton;
            LoadTabContent(clickedButton.name);
        }

        private void LoadTabContent(string tabName)
        {
            if (_mainContent == null || _serializedConfig == null) return;
            
            _mainContent.Clear();
            _serializedConfig.Update();

            switch (tabName)
            {
                case "tab-global":
                    LoadGlobalSettingsTab();
                    break;
                case "tab-music":
                    LoadLibraryTab(SignalStrings.Get("music_title"), "musicDatabase");
                    break;
                case "tab-sfx":
                    LoadLibraryTab(SignalStrings.Get("sfx_title"), "sfxDatabase");
                    break;
                case "tab-tools":
                    LoadImportToolsTab();
                    break;
                case "tab-help":
                    LoadHelpTab();
                    break;
            }
        }

        private void LoadGlobalSettingsTab()
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset SignalGlobalSettings");
            if (guids.Length == 0) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var content = visualTree.Instantiate();
            content.Bind(_serializedConfig);
            
            // Apply localization to Global Settings
            SetLabelText(content.Q<Label>("lbl-mixer-section"), SignalStrings.Get("mixer_section"));
            SetLabelText(content.Q<Label>("lbl-assets-section"), SignalStrings.Get("assets_section"));
            SetLabelText(content.Q<Label>("lbl-general-section"), SignalStrings.Get("general_section"));
            SetLabelText(content.Q<Label>("lbl-codegen-section"), SignalStrings.Get("codegen_section"));
            SetLabelText(content.Q<Label>("lbl-codegen-desc"), SignalStrings.Get("codegen_desc"));
            SetLabelText(content.Q<Label>("lbl-codegen-example"), SignalStrings.Get("codegen_example"));

            var btnChangePath = content.Q<Button>("btn-change-keys-path");
            if (btnChangePath != null) btnChangePath.text = SignalStrings.Get("btn_change_path");

            var btnGenKeys = content.Q<Button>("btn-generate-keys");
            if (btnGenKeys != null) btnGenKeys.text = SignalStrings.Get("btn_generate_keys");

            TextField pathField = content.Q<TextField>("keys-path-field");
            if (pathField != null)
            {
                pathField.value = EditorPrefs.GetString("SignalAudio_KeysPath", "Default (Assets or Manager Location)");
            }

            if (btnChangePath != null)
            {
                btnChangePath.clicked += () =>
                {
                    string currentPath = EditorPrefs.GetString("SignalAudio_KeysPath", "Assets");
                    string defaultDir = Directory.Exists(Path.GetDirectoryName(currentPath)) ? Path.GetDirectoryName(currentPath) : "Assets";
                    
                    string newPath = EditorUtility.SaveFilePanelInProject(
                        "Select AudioKeys Export Location", 
                        "AudioKeys", 
                        "cs", 
                        "Choose where to save the generated AudioKeys script.", 
                        defaultDir);
                    
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        EditorPrefs.SetString("SignalAudio_KeysPath", newPath);
                        if (pathField != null) pathField.value = newPath;
                    }
                };
            }

            if (btnGenKeys != null)
            {
                btnGenKeys.clicked += GenerateKeysCode;
            }

            _mainContent.Add(content);
        }

        private void GenerateKeysCode()
        {
            if (_serializedConfig == null) return;
            _serializedConfig.Update();
            SoundManagerSO data = _serializedConfig.targetObject as SoundManagerSO;
            if (data == null) return;

            string filePath = EditorPrefs.GetString("SignalAudio_KeysPath", "");

            if (string.IsNullOrEmpty(filePath))
            {
                string configPath = AssetDatabase.GetAssetPath(data);
                string configDir = !string.IsNullOrEmpty(configPath) ? Path.GetDirectoryName(configPath) : "Assets";
                filePath = EditorUtility.SaveFilePanelInProject(
                    "Save AudioKeys Script",
                    "AudioKeys",
                    "cs",
                    "Choose where to save the generated AudioKeys script.",
                    configDir
                );

                if (string.IsNullOrEmpty(filePath)) return;

                EditorPrefs.SetString("SignalAudio_KeysPath", filePath);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ==============================================================================");
            sb.AppendLine(" * AUTO-GENERATED BY SIGNAL (Resenha Studios)");
            sb.AppendLine(" * Strongly-typed audio IDs to prevent typos and enable IDE autocomplete.");
            sb.AppendLine(" * Do not modify this file manually. Regenerate from Tools > Resenha > Signal.");
            sb.AppendLine(" * ==============================================================================*/");
            sb.AppendLine();
            sb.AppendLine("namespace Resenha.Signal");
            sb.AppendLine("{");
            sb.AppendLine("    public static class AudioKeys");
            sb.AppendLine("    {");

            // SFX
            sb.AppendLine("        public static class SFX");
            sb.AppendLine("        {");
            foreach (var entry in data.sfxDatabase)
            {
                if (string.IsNullOrWhiteSpace(entry.audioID)) continue;
                string safeName = SanitizeVariableName(entry.audioID);
                sb.AppendLine($"            public const string {safeName} = \"{entry.audioID}\";");
            }
            sb.AppendLine("        }");
            sb.AppendLine();

            // MUSIC
            sb.AppendLine("        public static class Music");
            sb.AppendLine("        {");
            foreach (var entry in data.musicDatabase)
            {
                if (string.IsNullOrWhiteSpace(entry.audioID)) continue;
                string safeName = SanitizeVariableName(entry.audioID);
                sb.AppendLine($"            public const string {safeName} = \"{entry.audioID}\";");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();

            // Legacy backward compatibility alias
            sb.AppendLine("namespace SignalAudioManagerUnity");
            sb.AppendLine("{");
            sb.AppendLine("    [System.Obsolete(\"SignalAudioManagerUnity.AudioKeys is deprecated. Please use Resenha.Signal.AudioKeys instead.\")]");
            sb.AppendLine("    public static class AudioKeys");
            sb.AppendLine("    {");
            sb.AppendLine("        public static class SFX");
            sb.AppendLine("        {");
            foreach (var entry in data.sfxDatabase)
            {
                if (string.IsNullOrWhiteSpace(entry.audioID)) continue;
                string safeName = SanitizeVariableName(entry.audioID);
                sb.AppendLine($"            public const string {safeName} = Resenha.Signal.AudioKeys.SFX.{safeName};");
            }
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static class Music");
            sb.AppendLine("        {");
            foreach (var entry in data.musicDatabase)
            {
                if (string.IsNullOrWhiteSpace(entry.audioID)) continue;
                string safeName = SanitizeVariableName(entry.audioID);
                sb.AppendLine($"            public const string {safeName} = Resenha.Signal.AudioKeys.Music.{safeName};");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            try
            {
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("AudioKeys Generated", SignalStrings.Get("keys_success"), "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Code Generation Error", $"Failed to generate script:\n{ex.Message}", "OK");
            }
        }

        private string SanitizeVariableName(string rawName)
        {
            if (string.IsNullOrEmpty(rawName)) return "_empty";
            string clean = Regex.Replace(rawName, @"[^a-zA-Z0-9_]", "_");
            if (string.IsNullOrEmpty(clean)) return "_audio";
            
            if (char.IsDigit(clean[0]))
            {
                clean = "_" + clean;
            }
            return clean;
        }

        private void LoadLibraryTab(string title, string propertyName)
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset SignalAudioLibrary");
            if (guids.Length == 0) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var content = visualTree.Instantiate();
            content.style.flexGrow = 1;

            content.Q<Label>("library-title").text = title;

            // Apply localization
            var btnCollapse = content.Q<Button>("btn-collapse-all");
            if (btnCollapse != null) btnCollapse.text = SignalStrings.Get("btn_collapse");

            var btnExpand = content.Q<Button>("btn-expand-all");
            if (btnExpand != null) btnExpand.text = SignalStrings.Get("btn_expand");

            var btnDeleteAll = content.Q<Button>("btn-delete-all");
            if (btnDeleteAll != null) btnDeleteAll.text = SignalStrings.Get("btn_delete_all");

            var foldoutStates = new Dictionary<int, bool>();
            string currentSearchText = "";
            var searchField = content.Q<ToolbarSearchField>("audio-search-field");

            int currentPage = 0;
            int itemsPerPage = 10;
            List<int> filteredIndices = new List<int>();

            ListView listView = content.Q<ListView>("audio-list-view");
            Label lblPageInfo = content.Q<Label>("lbl-page-info");

            void UpdatePagination()
            {
                _serializedConfig.Update();
                SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                int totalItems = listProperty.arraySize;

                filteredIndices.Clear();

                for (int i = 0; i < totalItems; i++)
                {
                    if (string.IsNullOrEmpty(currentSearchText))
                    {
                        filteredIndices.Add(i);
                    }
                    else
                    {
                        SerializedProperty itemProp = listProperty.GetArrayElementAtIndex(i);
                        SerializedProperty idProp = itemProp.FindPropertyRelative("audioID");
                        string id = idProp != null ? idProp.stringValue : "";
                        if (id.ToLowerInvariant().Contains(currentSearchText))
                        {
                            filteredIndices.Add(i);
                        }
                    }
                }

                int totalFiltered = filteredIndices.Count;
                int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)totalFiltered / itemsPerPage));

                if (currentPage >= totalPages) currentPage = totalPages - 1;
                if (currentPage < 0) currentPage = 0;

                if (lblPageInfo != null)
                {
                    lblPageInfo.text = SignalStrings.Get("page_info", currentPage + 1, totalPages) + $" (Total: {totalItems})";
                }

                var pageIndices = filteredIndices.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();
                listView.itemsSource = pageIndices;
                listView.RefreshItems();
            }

            if (searchField != null)
            {
                searchField.RegisterValueChangedCallback(evt =>
                {
                    currentSearchText = evt.newValue?.ToLowerInvariant() ?? "";
                    currentPage = 0;
                    UpdatePagination();
                });
            }

            var btnPrev = content.Q<Button>("btn-prev-page");
            var btnNext = content.Q<Button>("btn-next-page");
            var btnAdd = content.Q<Button>("btn-add-item");
            var btnRemove = content.Q<Button>("btn-remove-item");

            if (btnPrev != null)
            {
                btnPrev.clicked += () =>
                {
                    if (currentPage > 0)
                    {
                        currentPage--;
                        UpdatePagination();
                    }
                };
            }

            if (btnNext != null)
            {
                btnNext.clicked += () =>
                {
                    int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)filteredIndices.Count / itemsPerPage));
                    if (currentPage < totalPages - 1)
                    {
                        currentPage++;
                        UpdatePagination();
                    }
                };
            }

            if (btnAdd != null)
            {
                btnAdd.clicked += () =>
                {
                    SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                    listProperty.arraySize++;
                    _serializedConfig.ApplyModifiedProperties();
                    
                    currentSearchText = "";
                    if (searchField != null) searchField.value = "";
                    
                    int newTotal = listProperty.arraySize;
                    currentPage = Mathf.Max(0, Mathf.CeilToInt((float)newTotal / itemsPerPage) - 1);
                    
                    UpdatePagination();
                    listView.ScrollToItem(-1);
                };
            }

            if (btnRemove != null)
            {
                btnRemove.clicked += () =>
                {
                    if (listView.selectedIndex >= 0 && listView.selectedIndex < listView.itemsSource.Count)
                    {
                        int realIndex = (int)listView.itemsSource[listView.selectedIndex];
                        SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                        listProperty.DeleteArrayElementAtIndex(realIndex);
                        _serializedConfig.ApplyModifiedProperties();
                        
                        if (foldoutStates.ContainsKey(realIndex)) foldoutStates.Remove(realIndex);
                        
                        UpdatePagination();
                    }
                };
            }

            BindListView(listView, propertyName, foldoutStates, (realIndex) => 
            {
                SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                listProperty.DeleteArrayElementAtIndex(realIndex);
                _serializedConfig.ApplyModifiedProperties();
                if (foldoutStates.ContainsKey(realIndex)) foldoutStates.Remove(realIndex);
                UpdatePagination();
            });

            if (btnCollapse != null)
            {
                btnCollapse.clicked += () =>
                {
                    SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                    for (int i = 0; i < listProperty.arraySize; i++) foldoutStates[i] = false;
                    listView.Query<Foldout>().ForEach(f => f.value = false);
                };
            }

            if (btnExpand != null)
            {
                btnExpand.clicked += () =>
                {
                    SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                    for (int i = 0; i < listProperty.arraySize; i++) foldoutStates[i] = true;
                    listView.Query<Foldout>().ForEach(f => f.value = true);
                };
            }

            if (btnDeleteAll != null)
            {
                btnDeleteAll.clicked += () =>
                {
                    if (EditorUtility.DisplayDialog("Clear Audio Database", $"Are you sure you want to delete all entries in {title}?", "Delete All", "Cancel"))
                    {
                        SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
                        listProperty.ClearArray();
                        _serializedConfig.ApplyModifiedProperties();
                        foldoutStates.Clear();
                        currentPage = 0;
                        UpdatePagination();
                    }
                };
            }

            UpdatePagination();
            _mainContent.Add(content);
        }

        private void LoadHelpTab()
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset SignalHelpTab");
            if (guids.Length == 0) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var content = visualTree.Instantiate();
            _mainContent.Add(content);
        }

        private void LoadImportToolsTab()
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset SignalImportTools");
            if (guids.Length == 0) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var content = visualTree.Instantiate();

            // Apply localization
            SetLabelText(content.Q<Label>("lbl-rules-title"), SignalStrings.Get("rules_title"));
            SetLabelText(content.Q<Label>("lbl-rules-desc"), SignalStrings.Get("rules_desc"));
            SetLabelText(content.Q<Label>("lbl-music-importer"), SignalStrings.Get("music_importer"));
            SetLabelText(content.Q<Label>("lbl-sfx-importer"), SignalStrings.Get("sfx_importer"));
            SetLabelText(content.Q<Label>("lbl-batch-renamer"), SignalStrings.Get("batch_renamer"));

            var btnMusic = content.Q<Button>("btn-run-import-music");
            if (btnMusic != null) btnMusic.text = SignalStrings.Get("btn_import_music");

            var btnSfx = content.Q<Button>("btn-run-import-sfx");
            if (btnSfx != null) btnSfx.text = SignalStrings.Get("btn_import_sfx");

            var btnRename = content.Q<Button>("btn-run-rename");
            if (btnRename != null) btnRename.text = SignalStrings.Get("btn_rename_files");

            // MUSIC CARD
            ObjectField folderMusic = content.Q<ObjectField>("folder-picker-music");
            folderMusic.objectType = typeof(DefaultAsset);
            TextField prefixMusic = content.Q<TextField>("prefix-input-music");

            string savedPathMusic = EditorPrefs.GetString("SignalAudio_ImportPath_Music", "");
            if (!string.IsNullOrEmpty(savedPathMusic)) folderMusic.value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(savedPathMusic);
            prefixMusic.value = EditorPrefs.GetString("SignalAudio_ImportPrefix_Music", "mus_");

            if (btnMusic != null) btnMusic.clicked += () => RunBatchImport("Music", folderMusic.value as DefaultAsset, prefixMusic.value);

            // SFX CARD
            ObjectField folderSfx = content.Q<ObjectField>("folder-picker-sfx");
            folderSfx.objectType = typeof(DefaultAsset);
            TextField prefixSfx = content.Q<TextField>("prefix-input-sfx");

            string savedPathSfx = EditorPrefs.GetString("SignalAudio_ImportPath_SFX", "");
            if (!string.IsNullOrEmpty(savedPathSfx)) folderSfx.value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(savedPathSfx);
            prefixSfx.value = EditorPrefs.GetString("SignalAudio_ImportPrefix_SFX", "sfx_");

            if (btnSfx != null) btnSfx.clicked += () => RunBatchImport("SFX", folderSfx.value as DefaultAsset, prefixSfx.value);
            
            // BATCH RENAMER CARD
            ObjectField folderRename = content.Q<ObjectField>("folder-picker-rename");
            folderRename.objectType = typeof(DefaultAsset);
            TextField prefixRename = content.Q<TextField>("prefix-input-rename");

            string savedPathRename = EditorPrefs.GetString("SignalAudio_RenamePath", "");
            if (!string.IsNullOrEmpty(savedPathRename)) folderRename.value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(savedPathRename);
            prefixRename.value = EditorPrefs.GetString("SignalAudio_RenamePrefix", "sfx_");

            if (btnRename != null) btnRename.clicked += () => RunBatchRename(folderRename.value as DefaultAsset, prefixRename.value);

            _mainContent.Add(content);
        }
        
        private void RunBatchRename(DefaultAsset targetFolder, string prefix)
        {
            if (targetFolder == null)
            {
                EditorUtility.DisplayDialog("Rename Error", "Please assign a target folder first.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                EditorUtility.DisplayDialog("Rename Error", "The Prefix cannot be empty.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(targetFolder);
            EditorPrefs.SetString("SignalAudio_RenamePath", path);
            EditorPrefs.SetString("SignalAudio_RenamePrefix", prefix);

            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { path });
            int renamedCount = 0;

            foreach (string guid in audioGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                
                if (!filename.StartsWith(prefix))
                {
                    string newName = prefix + filename;
                    AssetDatabase.RenameAsset(assetPath, newName);
                    renamedCount++;
                }
            }

            if (renamedCount > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Rename Successful", $"Successfully renamed {renamedCount} audio files.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No Files Renamed", $"All audio files in this folder already start with '{prefix}', or no audio files were found.", "OK");
            }
        }

        private void RunBatchImport(string targetDB, DefaultAsset importFolder, string prefix)
        {
            if (importFolder == null)
            {
                EditorUtility.DisplayDialog("Import Error", "Please assign a source folder first.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                EditorUtility.DisplayDialog("Import Error", "The File Prefix is required to identify and group the audio files.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(importFolder);
            EditorPrefs.SetString($"SignalAudio_ImportPath_{targetDB}", path);
            EditorPrefs.SetString($"SignalAudio_ImportPrefix_{targetDB}", prefix);

            string propertyName = targetDB == "Music" ? "musicDatabase" : "sfxDatabase";

            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { path });
            _serializedConfig.Update();
            SoundManagerSO data = _serializedConfig.targetObject as SoundManagerSO;
            
            var targetList = propertyName == "musicDatabase" ? data.musicDatabase : data.sfxDatabase;
            int clipsAdded = 0;
            int clipsUpdated = 0;

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
                
                if (clip != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (!filename.StartsWith(prefix)) continue;

                    string audioID = filename.Substring(prefix.Length);

                    AudioEntry existingEntry = targetList.Find(entry => entry.audioID == audioID);
                    if (existingEntry != null)
                    {
                        if (!existingEntry.audioClips.Contains(clip))
                        {
                            existingEntry.audioClips.Add(clip);
                            clipsUpdated++;
                        }
                    }
                    else
                    {
                        AudioEntry newEntry = new AudioEntry { audioID = audioID };
                        newEntry.audioClips.Add(clip);
                        targetList.Add(newEntry);
                        clipsAdded++;
                    }
                }
            }

            if (clipsAdded > 0 || clipsUpdated > 0)
            {
                EditorUtility.SetDirty(data);
                _serializedConfig.Update();
                EditorUtility.DisplayDialog("Import Successful", $"Added {clipsAdded} new entries\nUpdated {clipsUpdated} existing entries", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No New Clips Found", $"No new audio clips starting with '{prefix}' were found in the selected folder.", "OK");
            }
        }

        private void BindListView(ListView listView, string propertyName, Dictionary<int, bool> foldoutStates = null, Action<int> onDelete = null)
        {
            SerializedProperty listProperty = _serializedConfig.FindProperty(propertyName);
            
            string[] itemGuids = AssetDatabase.FindAssets("t:VisualTreeAsset AudioEntryItem");
            if (itemGuids.Length == 0) return;

            var itemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(itemGuids[0]));

            listView.makeItem = () =>
            {
                var element = itemTemplate.Instantiate();
                Button playBtn = element.Q<Button>("btn-play");
                Button stopBtn = element.Q<Button>("btn-stop");

                if (playBtn != null)
                {
                    playBtn.clicked += () =>
                    {
                        if (playBtn.userData is int realIdx)
                        {
                            _serializedConfig.ApplyModifiedProperties();
                            SoundManagerSO data = _serializedConfig.targetObject as SoundManagerSO;
                            var targetList = propertyName == "musicDatabase" ? data.musicDatabase : data.sfxDatabase;
                            
                            if (realIdx >= 0 && realIdx < targetList.Count)
                            {
                                PlayPreview(targetList[realIdx]);
                            }
                        }
                    };
                }

                if (stopBtn != null)
                {
                    stopBtn.clicked += () =>
                    {
                        if (_previewSource != null) _previewSource.Stop();
                    };
                }
                Button deleteBtn = element.Q<Button>("btn-delete");
                if (deleteBtn != null)
                {
                    deleteBtn.clicked += () =>
                    {
                        if (deleteBtn.userData is int realIdx)
                        {
                            if (EditorUtility.DisplayDialog("Delete Audio", "Are you sure you want to delete this audio entry?", "Yes", "No"))
                            {
                                onDelete?.Invoke(realIdx);
                            }
                        }
                    };
                }
                return element;
            };

            listView.bindItem = (element, pageIndex) =>
            {
                if (pageIndex < 0 || pageIndex >= listView.itemsSource.Count) return;
                
                int realIndex = (int)listView.itemsSource[pageIndex];

                if (realIndex < 0 || realIndex >= listProperty.arraySize) return;

                SerializedProperty itemProperty = listProperty.GetArrayElementAtIndex(realIndex);

                var idField = element.Q<TextField>("audio-id-field");
                idField?.BindProperty(itemProperty.FindPropertyRelative("audioID"));

                var clipsField = element.Q<PropertyField>("audio-clips-field");
                clipsField?.BindProperty(itemProperty.FindPropertyRelative("audioClips"));

                var minVolField = element.Q<PropertyField>("min-vol-field");
                minVolField?.BindProperty(itemProperty.FindPropertyRelative("minVolume"));

                var maxVolField = element.Q<PropertyField>("max-vol-field");
                maxVolField?.BindProperty(itemProperty.FindPropertyRelative("maxVolume"));

                var minPitchField = element.Q<PropertyField>("min-pitch-field");
                minPitchField?.BindProperty(itemProperty.FindPropertyRelative("minPitch"));

                var maxPitchField = element.Q<PropertyField>("max-pitch-field");
                maxPitchField?.BindProperty(itemProperty.FindPropertyRelative("maxPitch"));

                var spatialBlendField = element.Q<PropertyField>("spatial-blend-field");
                spatialBlendField?.BindProperty(itemProperty.FindPropertyRelative("spatialBlend"));

                var dopplerLevelField = element.Q<PropertyField>("doppler-level-field");
                dopplerLevelField?.BindProperty(itemProperty.FindPropertyRelative("dopplerLevel"));

                var spreadField = element.Q<PropertyField>("spread-field");
                spreadField?.BindProperty(itemProperty.FindPropertyRelative("spread"));

                var minDistanceField = element.Q<PropertyField>("min-distance-field");
                minDistanceField?.BindProperty(itemProperty.FindPropertyRelative("minDistance"));

                var maxDistanceField = element.Q<PropertyField>("max-distance-field");
                maxDistanceField?.BindProperty(itemProperty.FindPropertyRelative("maxDistance"));

                var playBtn = element.Q<Button>("btn-play");
                if (playBtn != null) playBtn.userData = realIndex;
                var deleteBtn = element.Q<Button>("btn-delete");
                if (deleteBtn != null) deleteBtn.userData = realIndex;

                var foldout = element.Q<Foldout>();
                if (foldout != null)
                {
                    if (foldout.userData is EventCallback<ChangeEvent<bool>> prevCb)
                        foldout.UnregisterValueChangedCallback(prevCb);

                    bool isExpanded = foldoutStates != null && foldoutStates.TryGetValue(realIndex, out bool saved) ? saved : false;
                    foldout.SetValueWithoutNotify(isExpanded);

                    EventCallback<ChangeEvent<bool>> cb = evt =>
                    {
                        if (foldoutStates != null) foldoutStates[realIndex] = evt.newValue;
                    };
                    foldout.userData = cb;
                    foldout.RegisterValueChangedCallback(cb);
                }

                var errorContainer = element.Q<VisualElement>("error-container");
                Label errorLabel = null;

                if (errorContainer == null)
                {
                    errorContainer = new VisualElement { name = "error-container" };
                    errorContainer.style.flexDirection = FlexDirection.Row;
                    errorContainer.style.alignItems = Align.Center;
                    errorContainer.style.backgroundColor = new Color(0.25f, 0.08f, 0.08f, 0.9f);
                    errorContainer.style.paddingTop = 6;
                    errorContainer.style.paddingBottom = 6;
                    errorContainer.style.paddingLeft = 10;
                    errorContainer.style.borderTopLeftRadius = 4;
                    errorContainer.style.borderTopRightRadius = 4;
                    errorContainer.style.borderBottomLeftRadius = 4;
                    errorContainer.style.borderBottomRightRadius = 4;
                    errorContainer.style.borderLeftColor = new Color(1f, 0.3f, 0.3f);
                    errorContainer.style.display = DisplayStyle.None;

                    var icon = new VisualElement { name = "error-icon" };
                    icon.style.width = 16;
                    icon.style.height = 16;
                    icon.style.marginRight = 8;
                    icon.style.unityBackgroundImageTintColor = new Color(1f, 0.4f, 0.4f);
                    SetIcon(icon, "icon_error");

                    errorLabel = new Label { name = "error-label" };
                    errorLabel.style.color = new Color(1f, 0.7f, 0.7f);
                    errorLabel.style.fontSize = 11;
                    errorLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

                    errorContainer.Add(icon);
                    errorContainer.Add(errorLabel);

                    element.Insert(0, errorContainer);
                }
                else
                {
                    errorLabel = errorContainer.Q<Label>("error-label");
                }

                void ValidateEntry()
                {
                    try
                    {
                        if (itemProperty == null || itemProperty.serializedObject == null) return;
                        if (realIndex >= listProperty.arraySize) return;

                        itemProperty.serializedObject.Update();
                        
                        var idProp = itemProperty.FindPropertyRelative("audioID");
                        if (idProp == null) return;

                        string id = idProp.stringValue;
                        SerializedProperty clipsProp = itemProperty.FindPropertyRelative("audioClips");
                        
                        bool hasValidClip = false;
                        if (clipsProp != null && clipsProp.isArray)
                        {
                            for (int i = 0; i < clipsProp.arraySize; i++)
                            {
                                if (clipsProp.GetArrayElementAtIndex(i).objectReferenceValue != null)
                                {
                                    hasValidClip = true;
                                    break;
                                }
                            }
                        }

                        int occurrences = 0;
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            for (int i = 0; i < listProperty.arraySize; i++)
                            {
                                var otherProp = listProperty.GetArrayElementAtIndex(i);
                                var otherIdProp = otherProp.FindPropertyRelative("audioID");
                                if (otherIdProp != null && otherIdProp.stringValue == id)
                                {
                                    occurrences++;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(id))
                        {
                            errorContainer.style.display = DisplayStyle.Flex;
                            errorLabel.text = SignalStrings.Get("error_empty_id");
                        }
                        else if (!hasValidClip)
                        {
                            errorContainer.style.display = DisplayStyle.Flex;
                            errorLabel.text = SignalStrings.Get("error_no_clips");
                        }
                        else if (occurrences > 1)
                        {
                            errorContainer.style.display = DisplayStyle.Flex;
                            errorLabel.text = SignalStrings.Get("error_duplicate_id", id);
                        }
                        else
                        {
                            errorContainer.style.display = DisplayStyle.None;
                        }
                    }
                    catch
                    {
                        // Safe ignore in teardown
                    }
                }

                IVisualElementScheduledItem scheduledValidation = element.schedule.Execute(ValidateEntry).Every(250);
                element.userData = scheduledValidation;
                
                element.RegisterCallback<DetachFromPanelEvent>(_ => 
                {
                    if (element.userData is IVisualElementScheduledItem sched)
                        sched.Pause();
                });
            };
        }

        private void PlayPreview(AudioEntry entry)
        {
            if (_previewSource == null || entry == null) return;
            _previewSource.Stop();

            AudioClip clip = entry.GetRandomClip();
            if (clip == null) return;

            _previewSource.clip = clip;
            _previewSource.pitch = entry.GetRandomPitch();
            _previewSource.volume = entry.GetRandomVolume();
            _previewSource.spatialBlend = entry.spatialBlend;
            
            _previewSource.Play();
        }

        private void RunInitialSetup(string selectedPath)
        {
            string targetBaseFolder = !string.IsNullOrEmpty(selectedPath) ? selectedPath : "Assets/Resenha Studio";
            string targetFolder = $"{targetBaseFolder}/SignalAudio_Setup";
            string resourcesFolder = $"{targetFolder}/Resources";
            
            string absoluteBasePath = Application.dataPath + targetBaseFolder.Substring(6);
            string absoluteTargetFolder = Application.dataPath + targetFolder.Substring(6);
            string absoluteResourcesFolder = Application.dataPath + resourcesFolder.Substring(6);
            
            if (!Directory.Exists(absoluteBasePath)) Directory.CreateDirectory(absoluteBasePath);
            if (!Directory.Exists(absoluteTargetFolder)) Directory.CreateDirectory(absoluteTargetFolder);
            if (!Directory.Exists(absoluteResourcesFolder)) Directory.CreateDirectory(absoluteResourcesFolder);
            
            AssetDatabase.Refresh();

            string mixerGuid = FindTemplateGuid("SignalAudioMixer", "t:AudioMixerController");
            string sfxPrefabGuid = FindTemplateGuid("SFX_Prefab", "t:Prefab");
            string managerPrefabGuid = FindTemplateGuid("Audio_Managers", "t:Prefab");

            if (string.IsNullOrEmpty(mixerGuid) || string.IsNullOrEmpty(sfxPrefabGuid) || string.IsNullOrEmpty(managerPrefabGuid))
            {
                EditorUtility.DisplayDialog("Setup Error", "Templates not found in the package.", "OK");
                return;
            }

            string newMixerPath = $"{targetFolder}/SignalAudioMixer.mixer";
            string newSfxPath = $"{targetFolder}/SFX_Prefab.prefab";
            string newManagerPath = $"{resourcesFolder}/Audio_Managers.prefab";

            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(mixerGuid), newMixerPath);
            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(sfxPrefabGuid), newSfxPath);
            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(managerPrefabGuid), newManagerPath);

            SoundManagerSO newSettings = ScriptableObject.CreateInstance<SoundManagerSO>();
            
            AudioMixer loadedMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(newMixerPath);
            GameObject loadedSfxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newSfxPath);

            if (loadedMixer != null) newSettings.audioMixer = loadedMixer;
            if (loadedSfxPrefab != null) newSettings.sfxPrefab = loadedSfxPrefab;

            newSettings.masterVolumeParam = "MasterVolume";
            newSettings.musicVolumeParam = "MusicVolume";
            newSettings.sfxVolumeParam = "SFXVolume";
            newSettings.uiVolumeParam = "UIVolume";
            newSettings.maxSimultaneousMusic = 3;
            newSettings.sfxPoolSize = 20;

            string settingsPath = $"{targetFolder}/Project_AudioSettings.asset";
            AssetDatabase.CreateAsset(newSettings, settingsPath);

            GameObject managerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newManagerPath);
            if (managerPrefab != null)
            {
                var soundManager = managerPrefab.GetComponent<SoundManager>();
                if (soundManager != null)
                {
                    using (var so = new SerializedObject(soundManager))
                    {
                        so.FindProperty("_config").objectReferenceValue = newSettings;
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                    PrefabUtility.SavePrefabAsset(managerPrefab);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LoadSerializedConfiguration();
            UpdateUIState();
        }

        private string FindTemplateGuid(string fileName, string filter)
        {
            string[] guids = AssetDatabase.FindAssets($"{fileName} {filter}");
            return guids.Length > 0 ? guids[0] : null;
        }
    }
}

namespace SignalAudioManagerUnity.Editor.UI
{
    [Obsolete("SignalAudioManagerUnity.Editor.UI.SignalAudioDashboard is deprecated. Please use Resenha.Signal.Editor.UI.SignalAudioDashboard instead.")]
    public class SignalAudioDashboard : Resenha.Signal.Editor.UI.SignalAudioDashboard
    {
    }
}