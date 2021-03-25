using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;

public class BuildStuff
{
    [MenuItem("mushroom/web build")]
    private static void WebBuild()
    {
        var scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "build/web",
            targetGroup = BuildTargetGroup.WebGL,
            target = BuildTarget.WebGL,
        });
    }


    [MenuItem("mushroom/playtesting & canvas build")]
    private static void Build()
    {
        var scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "build/win/mushroom.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
        });

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "build/mac.app",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneOSX,
        });

        using (var zip = ZipFile.Open("mushroom windows.zip", ZipArchiveMode.Create))
        {
            MakeZip(zip, "build/win", "mushroom");
        }

        using (var zip = ZipFile.Open("mushroom mac.zip", ZipArchiveMode.Create))
        {
            MakeZip(zip, "build/mac.app", "mushroom.app");
        }

        using (var zip = ZipFile.Open("lbitzer mfroehli miczhang rkiv ssscrazy.zip", ZipArchiveMode.Create))
        {
            MakeZip(zip, "build/win", "windows");
            MakeZip(zip, "build/mac.app", "mac/mushroom.app");
            MakeZip(zip, "Assets", "Assets");
            MakeZip(zip, "ProjectSettings", "ProjectSettings");

            foreach (var item in Directory.EnumerateFiles(".", "*.txt"))
            {
                var name = Path.GetFileName(item);
                MakeZip(zip, item, name);
            }
        }
    }

    private static void MakeZip(ZipArchive zip, string src, string dst)
    {
        if (Directory.Exists(src))
        {
            foreach (var child in Directory.EnumerateFileSystemEntries(src))
            {
                var name = Path.GetFileName(child);
                MakeZip(zip, child, Path.Combine(dst, name));
            }
        }
        else
        {
            zip.CreateEntryFromFile(src, dst.Replace("\\", "/"));
        }
    }
}
