using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class PostProcessBuildUtils
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        AddImagesXcAssetsToBuildPhases(path);
    }

    private static void AddImagesXcAssetsToBuildPhases(string path)
    {
        string projectPath = PBXProject.GetPBXProjectPath(path);
        var project = new PBXProject();
        project.ReadFromFile(projectPath);

        string mainGuid = project.GetUnityMainTargetGuid();
        project.AddFileToBuild(mainGuid, project.AddFile("Unity-iPhone/Images.xcassets", "Images.xcassets"));
        project.WriteToFile(projectPath);
    }
}
