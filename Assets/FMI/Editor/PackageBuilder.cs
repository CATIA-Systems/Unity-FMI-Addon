using UnityEngine;
using UnityEditor;

class PackageBuilder
{
    [MenuItem("Assets/Export FMI Add-on Package")]
    static void ExportPackage()
    {
        FileUtil.ReplaceFile(Application.dataPath + "/../CHANGELOG.md", Application.dataPath + "/FMI/CHANGELOG.md");
        FileUtil.ReplaceFile(Application.dataPath + "/../LICENSE.md", Application.dataPath + "/FMI/LICENSE.md");
        FileUtil.ReplaceFile(Application.dataPath + "/../README.md", Application.dataPath + "/FMI/README.md");

        AssetDatabase.Refresh();

        AssetDatabase.ExportPackage("Assets", "FMI-Addon-0.0.2.unitypackage", ExportPackageOptions.Recurse);
    }

}
