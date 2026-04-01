using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class BackgroundRecompiler
{
    static FileSystemWatcher fileWatcher;
    static bool compilationRequested;

    static BackgroundRecompiler()
    {
        InitializeWatcher();
        EditorApplication.update += OnEditorUpdate;
    }

    static void InitializeWatcher()
    {
        string projectPath = Path.GetFullPath(Application.dataPath);

        fileWatcher = new FileSystemWatcher
        {
            Path = projectPath,
            Filter = "*.cs",
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
        };

        fileWatcher.Changed += OnFilesChanged;
        fileWatcher.Created += OnFilesChanged;
        fileWatcher.Deleted += OnFilesChanged;
        fileWatcher.Renamed += OnFilesChanged;

        fileWatcher.EnableRaisingEvents = true;
    }

    static void OnFilesChanged(object sender, FileSystemEventArgs e)
    {
        // Flag the main thread that a file modification occurred
        compilationRequested = true;
    }

    static void OnEditorUpdate()
    {
        // Execute the refresh operation when the editor is idle and not currently in play mode
        if (compilationRequested && !EditorApplication.isCompiling && !EditorApplication.isPlaying)
        {
            compilationRequested = false;
            AssetDatabase.Refresh();
        }
    }
}
