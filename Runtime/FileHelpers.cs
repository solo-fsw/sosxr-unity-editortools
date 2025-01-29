using System;
using System.IO;
using System.Linq;
using UnityEngine;


public static class FileHelpers
{
    /// <summary>
    ///     NO IDEA IF THIS WORKS.
    /// </summary>
    /// <returns></returns>
    public static string GetHomePath()
    {
        var path = "";

        #if UNITY_ANDROID && !UNITY_EDITOR
        using var envClass = new UnityEngine.AndroidJavaClass("android.os.Environment");
        using var moviesDir = envClass.CallStatic<UnityEngine.AndroidJavaObject>("getExternalStoragePublicDirectory");
        path = moviesDir.Call<string>("getAbsolutePath");
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        #elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Home));
        #else
        path = "Home directory path is not supported on this platform.";
        #endif

        return path;
    }


    public static string GetMoviesPath()
    {
        var path = "";

        #if UNITY_ANDROID && !UNITY_EDITOR
        using var envClass = new UnityEngine.AndroidJavaClass("android.os.Environment");
        using var moviesDir = envClass.CallStatic<UnityEngine.AndroidJavaObject>("getExternalStoragePublicDirectory", "Movies");
        path = moviesDir.Call<string>("getAbsolutePath");
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Movies");
        #elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos));
        #else
        path = "Movies directory path is not supported on this platform.";
        #endif

        return path;
    }


    public static string GetArborXRPath()
    {
        var path = "";
        var arborVideoFolder = "ArborXR/videos";

        #if UNITY_ANDROID && !UNITY_EDITOR
        using var envClass = new UnityEngine.AndroidJavaClass("android.os.Environment");
        using var moviesDir = envClass.CallStatic<UnityEngine.AndroidJavaObject>("getExternalStoragePublicDirectory", arborVideoFolder);
        path = moviesDir.Call<string>("getAbsolutePath");
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), arborVideoFolder);
        #else
        path = "ArborXR directory path is not supported on this platform.";
        #endif

        return path;
    }


    public static string GetDocumentsPath()
    {
        var path = "";

        #if UNITY_ANDROID && !UNITY_EDITOR
        using var envClass = new UnityEngine.AndroidJavaClass("android.os.Environment");
        using var moviesDir = envClass.CallStatic<UnityEngine.AndroidJavaObject>("getExternalStoragePublicDirectory", "Documents");
        path = moviesDir.Call<string>("getAbsolutePath");
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");
        #elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
        #else
        path = "Documents directory path is not supported on this platform.";
        #endif

        return path;
    }


    public static string[] GetFileNamesFromDirectory(string[] extensions = null, bool excludeExtensions = false, bool excludeHidden = false, string basePath = null, string subFolder = null)
    {
        var path = basePath + (string.IsNullOrEmpty(subFolder) ? "" : Path.DirectorySeparatorChar + subFolder);

        if (!Directory.Exists(path))
        {
            Debug.LogErrorFormat("Directory does not exist: " + path);

            return Array.Empty<string>(); // Return an empty array if the directory does not exist
        }

        var filenames = Directory.GetFiles(path);

        // Filter by extensions
        if (extensions is {Length: > 0})
        {
            filenames = filenames.Where(f =>
                excludeExtensions
                    ? !extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
                    : extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
            ).ToArray();
        }

        // Filter out hidden files
        if (excludeHidden)
        {
            filenames = filenames.Where(f => !Path.GetFileName(f).StartsWith(".")).ToArray();
        }

        // Optionally, just get the filenames without their paths
        for (var i = 0; i < filenames.Length; i++)
        {
            filenames[i] = Path.GetFileName(filenames[i]);
        }

        return filenames;
    }
}