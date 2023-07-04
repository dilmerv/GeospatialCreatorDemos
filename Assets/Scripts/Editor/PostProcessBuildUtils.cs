using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Editor
{
#if UNITY_IOS

    public static class PostProcessBuildUtils
    {
        public static bool enableBitcode = false;
        private const string ARCoreSwiftPackageName = "ARCoreGeospatial";
        private const string ARCoreSwiftPackageURL = "https://github.com/google-ar/arcore-ios-sdk";
        private const string ARCoreSwiftPackageVersion = "1.38.0";

        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            AddImagesXcAssetsToBuildPhases(path);
            SetupBitcode(path);
            AddRemotePackage(path, ARCoreSwiftPackageName, ARCoreSwiftPackageURL, ARCoreSwiftPackageVersion);
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

        private static void SetupBitcode(string pathToBuiltProject)
        {
            var project = new PBXProject();
            var pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            project.ReadFromFile(pbxPath);
            SetupBitcodeFramework(project);
            SetupBitcodeMain(project);
            project.WriteToFile(pbxPath);
        }

        private static void SetupBitcodeFramework(PBXProject project)
        {
            SetupBitcode(project, project.GetUnityFrameworkTargetGuid());
        }

        private static void SetupBitcodeMain(PBXProject project)
        {
            SetupBitcode(project, project.GetUnityMainTargetGuid());
        }

        private static void SetupBitcode(PBXProject project, string targetGUID)
        {
            project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
        }

        private static void AddRemotePackage(string pathToBuildProject, string packageName, string packageUrl, string version)
        {
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

            string packageGuid =
                project.AddRemotePackageReferenceAtVersionUpToNextMajor(url: packageUrl, version: version);
            project.AddRemotePackageFrameworkToProject(targetGuid: project.GetUnityMainTargetGuid(), name: packageName,
                packageGuid: packageGuid, weak: false);

            project.WriteToFile(projectPath);
        }
    }
#endif
}