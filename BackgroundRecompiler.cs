using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class BackgroundRecompiler
{
    static FileSystemWatcher fileWatcher;
    static volatile bool compilationRequested;

    // Cooldown variables tracking time on the main thread
    static double cooldownEndTime;
    static bool isCooldownActive;

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
            InternalBufferSize = 64 * 1024,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
        };

        fileWatcher.Changed += OnFilesChanged;
        fileWatcher.Created += OnFilesChanged;
        fileWatcher.Deleted += OnFilesChanged;
        fileWatcher.Renamed += OnFilesChanged;
        fileWatcher.Error += OnWatcherError;

        fileWatcher.EnableRaisingEvents = true;
    }

    static void OnFilesChanged(object sender, FileSystemEventArgs e)
    {
        compilationRequested = true;
    }

    static void OnWatcherError(object sender, ErrorEventArgs e)
    {
        compilationRequested = true;
    }

    static void OnEditorUpdate()
    {
        // When a change is detected, start or reset the cooldown timer
        // This gives VS Code enough time to finish writing and release OS file locks
        if (compilationRequested)
        {
            compilationRequested = false;
            cooldownEndTime = EditorApplication.timeSinceStartup + 0.3;
            isCooldownActive = true;
        }

        // Only trigger the asset pipeline refresh after the cooldown expires safely
        if (isCooldownActive && EditorApplication.timeSinceStartup >= cooldownEndTime)
        {
            isCooldownActive = false;

            if (!EditorApplication.isCompiling && !EditorApplication.isPlaying)
            {
                // ForceUpdate ensures Unity explicitly checks for modified timestamps
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
