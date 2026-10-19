using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Localization;

namespace Resenha.Signal.Editor.Localization
{
    public enum SignalLanguage
    {
        English = 0,
        Portuguese = 1,
        Spanish = 2,
        Japanese = 3,
        Chinese = 4
    }

    public static class SignalStrings
    {
        private const string PrefsKey = "Resenha_Signal_Language";
        private static SignalLanguage _currentLanguage = (SignalLanguage)EditorPrefs.GetInt(PrefsKey, 0);

        public static SignalLanguage CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                EditorPrefs.SetInt(PrefsKey, (int)value);
            }
        }

        private static readonly Dictionary<string, string[]> Translations = new Dictionary<string, string[]>
        {
            // Header & Brand
            { "window_title", new[] { "Signal", "Signal", "Signal", "Signal", "Signal" } },
            { "window_subtitle", new[] { "Audio Architecture & Sound Manager", "Arquitetura de Áudio e Gerenciador de Som", "Arquitectura de Audio y Administrador de Sonido", "オーディオ設計＆サウンドマネージャー", "音频架构与声音管理器" } },
            { "brand_name", new[] { "RESENHA STUDIOS", "RESENHA STUDIOS", "RESENHA STUDIOS", "RESENHA STUDIOS", "RESENHA STUDIOS" } },
            { "language_selector", new[] { "Language", "Idioma", "Idioma", "言語", "语言" } },

            // Sidebar Tabs
            { "tab_global", new[] { "Settings", "Configurações", "Configuraciones", "設定", "设置" } },
            { "tab_music", new[] { "Music", "Músicas", "Música", "音楽", "音乐" } },
            { "tab_sfx", new[] { "SFX & UI", "Efeitos & UI", "Efectos & UI", "効果音 & UI", "音效 & UI" } },
            { "tab_tools", new[] { "Import Tools", "Ferramentas", "Herramientas", "インポートツール", "导入工具" } },
            { "tab_help", new[] { "Docs", "Ajuda & Docs", "Ayuda & Docs", "ドキュメント", "帮助文档" } },

            // Welcome / Initial Setup
            { "welcome_title", new[] { "Signal Audio Architecture", "Arquitetura de Áudio Signal", "Arquitectura de Audio Signal", "Signal オーディオ設計", "Signal 音频架构" } },
            { "welcome_subtitle", new[] { "No audio configuration found in this project.", "Nenhuma configuração de áudio encontrada neste projeto.", "No se encontró configuración de audio en este proyecto.", "このプロジェクトにはオーディオ設定が見つかりません。", "当前项目中未找到音频配置。" } },
            { "welcome_desc", new[] { "Click below to initialize folders, audio mixers, prefabs, and configuration assets automatically.", "Clique abaixo para criar pastas, audio mixers, prefabs e configurações automaticamente.", "Haga clic a continuación para generar carpetas, mezcladores, prefabs y configuraciones automáticamente.", "以下をクリックして、フォルダー、ミキサー、プレハブ、設定アセットを自動生成します。", "点击下方按钮以自动生成文件夹、混音器、预制体和配置文件。" } },
            { "welcome_btn", new[] { "Initialize Signal", "Inicializar Signal", "Inicializar Signal", "Signalを初期化", "初始化 Signal" } },

            // Global Settings Tab
            { "mixer_section", new[] { "AUDIO MIXER", "AUDIO MIXER", "AUDIO MIXER", "オーディオミキサー", "混音器" } },
            { "mixer_label", new[] { "Main Audio Mixer", "Audio Mixer Principal", "Audio Mixer Principal", "メインオーディオミキサー", "主音频混音器" } },
            { "master_param", new[] { "Master Volume Param", "Parâmetro Master", "Parámetro Master", "マスター音量パラメータ", "主音量参数" } },
            { "music_param", new[] { "Music Volume Param", "Parâmetro Música", "Parámetro Música", "音楽音量パラメータ", "音乐音量参数" } },
            { "sfx_param", new[] { "SFX Volume Param", "Parâmetro SFX", "Parámetro SFX", "効果音パラメータ", "音效参数" } },
            { "ui_param", new[] { "UI Volume Param", "Parâmetro UI", "Parámetro UI", "UI音量パラメータ", "UI音量参数" } },
            { "assets_section", new[] { "ASSET REFERENCES", "REFERÊNCIAS DE ASSETS", "REFERENCIAS DE ASSETS", "アセット参照", "资源引用" } },
            { "sfx_prefab_label", new[] { "SFX AudioSource Prefab", "Prefab de AudioSource SFX", "Prefab de AudioSource SFX", "効果音オーディオソースプレハブ", "音效AudioSource预制体" } },
            { "general_section", new[] { "GENERAL CONFIGURATION", "CONFIGURAÇÕES GERAIS", "CONFIGURACIÓN GENERAL", "一般設定", "常规设置" } },
            { "max_music", new[] { "Max Simultaneous Music", "Músicas Simultâneas Máx.", "Música Simultánea Máx.", "最大同時再生音楽数", "最大同时播放音乐数" } },
            { "sfx_pool", new[] { "SFX Pool Size", "Tamanho do Pool SFX", "Tamaño del Pool SFX", "効果音プールサイズ", "音效对象池大小" } },
            
            // Code Generation
            { "codegen_section", new[] { "AUDIO KEYS GENERATION", "GERAÇÃO DE AUDIOKEYS", "GENERACIÓN DE AUDIOKEYS", "AUDIOKEYS 生成", "生成音频常量" } },
            { "codegen_desc", new[] { "Generates a type-safe C# script with audio IDs as constants to avoid typos and unlock IDE autocomplete.", "Gera um script C# fortemente tipado com IDs de áudio em constantes para autocompletar na IDE.", "Genera un script C# con los IDs de audio como constantes para evitar errores tipográficos.", "タイポを防ぎ、IDEの自動補完を可能にするC#定数スクリプトを生成します。", "生成包含强类型音频常量ID的C#脚本，防止拼写错误并支持IDE代码补全。" } },
            { "codegen_example", new[] { "Usage: Signal.PlaySFX(AudioKeys.SFX.jump);", "Exemplo: Signal.PlaySFX(AudioKeys.SFX.jump);", "Ejemplo: Signal.PlaySFX(AudioKeys.SFX.jump);", "使用例: Signal.PlaySFX(AudioKeys.SFX.jump);", "使用示例: Signal.PlaySFX(AudioKeys.SFX.jump);" } },
            { "btn_change_path", new[] { "Browse", "Procurar", "Examinar", "参照", "浏览" } },
            { "btn_generate_keys", new[] { "Generate AudioKeys", "Gerar AudioKeys", "Generar AudioKeys", "AudioKeysを生成", "生成 AudioKeys" } },
            { "keys_success", new[] { "AudioKeys script generated successfully!", "Script AudioKeys gerado com sucesso!", "¡Script AudioKeys generado con éxito!", "AudioKeysスクリプトが正常に生成されました！", "AudioKeys脚本生成成功！" } },

            // Library Toolbar & Items
            { "music_title", new[] { "Music Library", "Biblioteca de Músicas", "Biblioteca de Música", "音楽ライブラリ", "音乐库" } },
            { "sfx_title", new[] { "SFX & UI Library", "Biblioteca de Efeitos (SFX) & UI", "Biblioteca de Efectos (SFX) & UI", "効果音 & UI ライブラリ", "音效与UI库" } },
            { "search_placeholder", new[] { "Search audio by ID...", "Pesquisar por ID...", "Buscar por ID...", "IDで音声を検索...", "按ID搜索音频..." } },
            { "btn_collapse", new[] { "Collapse", "Recolher", "Colapsar", "折りたたむ", "折叠" } },
            { "btn_expand", new[] { "Expand", "Expandir", "Expandir", "展開", "展开" } },
            { "btn_delete_all", new[] { "Clear All", "Limpar Tudo", "Borrar Todo", "すべて削除", "清空全部" } },
            { "page_info", new[] { "Page {0} of {1}", "Página {0} de {1}", "Página {0} de {1}", "ページ {0} / {1}", "第 {0} / {1} 页" } },
            { "audio_id_label", new[] { "Audio ID", "ID do Áudio", "ID del Audio", "オーディオID", "音频ID" } },
            { "details_foldout", new[] { "Parameters & Audio Clips", "Parâmetros e Clipes de Áudio", "Parámetros y Clips de Audio", "パラメータとクリップ", "参数与音频片段" } },
            { "audio_clips", new[] { "Audio Clips", "Clipes de Áudio", "Clips de Audio", "オーディオクリップ", "音频片段" } },
            { "min_vol", new[] { "Min Volume", "Volume Mínimo", "Volumen Mínimo", "最小音量", "最小音量" } },
            { "max_vol", new[] { "Max Volume", "Volume Máximo", "Volumen Máximo", "最大音量", "最大音量" } },
            { "min_pitch", new[] { "Min Pitch", "Pitch Mínimo", "Pitch Mínimo", "最小ピッチ", "最小音高" } },
            { "max_pitch", new[] { "Max Pitch", "Pitch Máximo", "Pitch Máximo", "最大ピッチ", "最大音高" } },
            { "spatial_blend", new[] { "Spatial Blend (2D/3D)", "Blend Espacial (2D/3D)", "Mezcla Espacial (2D/3D)", "空間ブレンド (2D/3D)", "空间混合 (2D/3D)" } },
            { "doppler_level", new[] { "Doppler Level", "Nível Doppler", "Nivel Doppler", "ドップラー効果", "多普勒等级" } },
            { "spread", new[] { "Spread", "Dispersão", "Dispersión", "スプレッド", "扩散度" } },
            { "min_dist", new[] { "Min Distance", "Distância Mínima", "Distancia Mínima", "最小距離", "最小距离" } },
            { "max_dist", new[] { "Max Distance", "Distância Máxima", "Distancia Máxima", "最大距離", "最大距离" } },

            // Validation Alerts
            { "error_empty_id", new[] { "Audio ID cannot be empty", "O ID do áudio não pode ser vazio", "El ID del audio no puede estar vacío", "オーディオIDは空にできません", "音频ID不能为空" } },
            { "error_no_clips", new[] { "No audio clips assigned", "Nenhum clipe de áudio atribuído", "No hay clips de audio asignados", "クリップが割り当てられていません", "未分配音频片段" } },
            { "error_duplicate_id", new[] { "Duplicate ID: '{0}' already exists", "ID duplicado: '{0}' já existe", "ID duplicado: '{0}' ya existe", "重複ID: '{0}' は既に存在します", "重复ID: '{0}' 已存在" } },

            // Import Tools Tab
            { "rules_title", new[] { "IMPORTER AUTOMATION", "AUTOMAÇÃO DE IMPORTAÇÃO", "AUTOMATIZACIÓN DE IMPORTACIÓN", "インポート自動化", "自动化导入" } },
            { "rules_desc", new[] { "Drop your audio folders to automatically generate entries. The file prefix is stripped to create clean IDs.", "Arraste pastas de áudio para gerar entradas automaticamente. O prefixo é removido para gerar IDs limpos.", "Arrastre carpetas de audio para generar entradas automáticamente. El prefijo se elimina para crear IDs limpios.", "オーディオフォルダーをドロップして自動生成します。プレフィックスはクリーンなIDに自動変換されます。", "拖入音频文件夹以自动生成条目。系统会自动去除前缀以生成规范ID。" } },
            { "music_importer", new[] { "MUSIC IMPORTER", "IMPORTADOR DE MÚSICA", "IMPORTADOR DE MÚSICA", "音楽インポーター", "音乐导入器" } },
            { "sfx_importer", new[] { "SFX IMPORTER", "IMPORTADOR DE SFX", "IMPORTADOR DE SFX", "効果音インポーター", "音效导入器" } },
            { "batch_renamer", new[] { "BATCH RENAMER", "RENOMEADOR EM LOTE", "RENOMBRADOR POR LOTES", "一括リネーム", "批量重命名" } },
            { "source_folder", new[] { "Source Folder", "Pasta de Origem", "Carpeta de Origen", "ソースフォルダー", "源文件夹" } },
            { "file_prefix", new[] { "File Prefix", "Prefixo dos Arquivos", "Prefijo de Archivos", "ファイルプレフィックス", "文件前缀" } },
            { "btn_import_music", new[] { "Import Music", "Importar Músicas", "Importar Música", "音楽をインポート", "导入音乐" } },
            { "btn_import_sfx", new[] { "Import SFX", "Importar SFX", "Importar SFX", "効果音をインポート", "导入音效" } },
            { "btn_rename_files", new[] { "Batch Rename", "Renomear Arquivos", "Renombrar Archivos", "ファイルを一括リネーム", "批量重命名" } }
        };

        public static string Get(string key, params object[] args)
        {
            if (Translations.TryGetValue(key, out var list))
            {
                int langIndex = (int)_currentLanguage;
                if (langIndex < 0 || langIndex >= list.Length) langIndex = 0;
                string format = list[langIndex];
                return args != null && args.Length > 0 ? string.Format(format, args) : format;
            }
            return key;
        }

        public static LocalizedString CreateLocalizedString(string table, string entry)
        {
            return new LocalizedString(table, entry);
        }
    }
}
