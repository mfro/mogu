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
    private static string[] scenes => EditorBuildSettings.scenes.Select(s => s.path).ToArray();

    private static string BuildWindows()
    {
        var output = "build/windows";

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = $"{output}/mogu.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
        });

        return output;
    }
    [MenuItem("mushroom/build - windows")]
    private static string BuildWindowsZip()
    {
        var output = "build/mogu windows.zip";
        var input = BuildWindows();

        File.Delete(output);
        using (var zip = ZipFile.Open(output, ZipArchiveMode.Create))
        {
            AddToZip(zip, input, "mogu");
        }

        return output;
    }

    private static string BuildMac()
    {
        var output = "build/mac.app";

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneOSX,
        });

        return output;
    }
    [MenuItem("mushroom/build - mac")]
    private static string BuildMacZip()
    {
        var output = "build/mogu mac.zip";
        var input = BuildMac();

        File.Delete(output);
        using (var zip = ZipFile.Open(output, ZipArchiveMode.Create))
        {
            AddToZip(zip, input, "mogu.app");
        }

        return output;
    }

    [MenuItem("mushroom/build - web")]
    private static string WebBuild()
    {
        var output = "build/web";

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            targetGroup = BuildTargetGroup.WebGL,
            target = BuildTarget.WebGL,
        });

        return output;
    }

    [MenuItem("mushroom/build - canvas")]
    private static void BuildCanvas()
    {
        var output = "lbitzer mfroehli miczhang rkiv ssscrazy.zip";
        var windows = BuildWindows();
        var mac = BuildMac();

        File.Delete(output);
        using (var zip = ZipFile.Open(output, ZipArchiveMode.Create))
        {
            AddToZip(zip, windows, "windows");
            AddToZip(zip, mac, "mac/mogu.app");
            AddToZip(zip, "Assets", "Assets");
            AddToZip(zip, "ProjectSettings", "ProjectSettings");

            foreach (var item in Directory.EnumerateFiles(".", "*.txt"))
            {
                var name = Path.GetFileName(item);
                AddToZip(zip, item, name);
            }
        }
    }

    [MenuItem("mushroom/build - playtesting")]
    private static void BuildPlaytest()
    {
        BuildWindowsZip();
        BuildMacZip();
    }


    private static void AddToZip(ZipArchive zip, string src, string dst)
    {
        if (Directory.Exists(src))
        {
            foreach (var child in Directory.EnumerateFileSystemEntries(src))
            {
                var name = Path.GetFileName(child);
                AddToZip(zip, child, Path.Combine(dst, name));
            }
        }
        else
        {
            zip.CreateEntryFromFile(src, dst.Replace("\\", "/"));
        }
    }
}
