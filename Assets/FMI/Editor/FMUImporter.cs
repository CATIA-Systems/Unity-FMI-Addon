using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Xml.Linq;


public static class FMUImporter
{

    [MenuItem("Assets/Import FMU...")]
    public static void ImportFMU()
    {

        string[] filters = { "FMUs", "fmu", "All files", "*" };

        string fmuPath = EditorUtility.OpenFilePanelWithFilters("Import FMU", "", filters);

        if (string.IsNullOrEmpty(fmuPath)) return;

        var fmuName = Path.GetFileNameWithoutExtension(fmuPath);

        var unzipdir = Path.Combine(Application.streamingAssetsPath, fmuName);

        var zip = ZipFile.Read(fmuPath);
        zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
        zip.ExtractAll(unzipdir);

        var modelDescription = ScriptableObject.CreateInstance<ModelDescription>();

        XDocument doc = XDocument.Load(unzipdir + "/modelDescription.xml");

        var root = doc.Root;

        modelDescription.fmiVersion = (string)root.Attribute("fmiVersion");
        modelDescription.guid = ((string)root.Attribute("guid")).ToCharArray();
        modelDescription.modelName = (string)root.Attribute("modelName");

        foreach (var e in root.Elements("CoSimulation"))
        {
            modelDescription.coSimulation = new Implementation();
            modelDescription.coSimulation.modelIdentifier = (string)e.Attribute("modelIdentifier");
        }

        var variables = new List<ScalarVariable>();

        foreach (var e in root.Element("ModelVariables").Elements("ScalarVariable"))
        {
            var v = new ScalarVariable
            {
                name = (string)e.Attribute("name"),
                description = (string)e.Attribute("description"),
                causality = (string)e.Attribute("causality"),
                variability = (string)e.Attribute("variability"),
                initial = (string)e.Attribute("initial"),
                valueReference = (uint)e.Attribute("valueReference"),
            };

            variables.Add(v);

            foreach (var el in e.Elements("Real"))
            {
                v.type = VariableType.Real;
                v.start = (string)el.Attribute("start");
            }

            foreach (var el in e.Elements("Integer"))
            {
                v.type = VariableType.Integer;
                v.start = (string)el.Attribute("start");
            }

            foreach (var el in e.Elements("Boolean"))
            {
                v.type = VariableType.Boolean;
                v.start = (string)el.Attribute("start");
            }

            foreach (var el in e.Elements("String"))
            {
                v.type = VariableType.String;
                v.start = (string)el.Attribute("start");
            }

        }

        modelDescription.modelVariables = variables.ToArray();

        AssetDatabase.CreateAsset(modelDescription, "Assets/Resources/" + fmuName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh ();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = modelDescription;
    }

}
