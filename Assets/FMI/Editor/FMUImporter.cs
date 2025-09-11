using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Xml.Linq;
using System.Xml.Schema;
using System;
using System.Xml.XPath;


[Serializable]
public class FMUImportException : Exception
{
    public FMUImportException (string message) : base(message) {}   
}


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

        XDocument doc = XDocument.Load(unzipdir + "/modelDescription.xml");
        var root = doc.Root;

        string fmiVersion = (string)root.Attribute("fmiVersion");

        ModelDescription modelDescription;

        if (fmiVersion == "2.0")
        {
            modelDescription = ReadModelDescriptionFMI2(doc);
        }
        else if (fmiVersion.StartsWith("3."))
        {
            modelDescription = ReadModelDescriptionFMI3(doc);
        }
        else
        {
            throw new FMUImportException("Only FMI 2 and 3 are supported.");
        }

        AssetDatabase.CreateAsset(modelDescription, "Assets/Resources/" + fmuName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = modelDescription;
    }

    static ModelDescription ReadModelDescriptionFMI2(XDocument doc)
    {
        XElement root = doc.Root;

        string xsdPath = Path.Combine(Application.dataPath, "FMI", "Editor", "schema", "fmi2", "fmi2ModelDescription.xsd");

        XmlSchemaSet schemas = new XmlSchemaSet();
        schemas.Add("", xsdPath);

        bool isValid = true;
        string validationErrors = "";

        doc.Validate(schemas, (o, e) =>
        {
            isValid = false;
            validationErrors += e.Message + "\n";
        });

        if (!isValid)
        {
            throw new FMUImportException("Failed to validate model description. " + validationErrors);
        }

        var modelDescription = ScriptableObject.CreateInstance<ModelDescription>();

        modelDescription.fmiVersion = (string)root.Attribute("fmiVersion");
        modelDescription.instantiationToken = (string)root.Attribute("guid");
        modelDescription.modelName = (string)root.Attribute("modelName");

        var coSimulation = root.Element("CoSimulation") ?? throw new FMUImportException("The FMU does not support Co-Simulation.");

        modelDescription.coSimulation = new()
        {
            modelIdentifier = (string)coSimulation.Attribute("modelIdentifier")
        };

        var variables = new List<ScalarVariable>();

        string typeNames = string.Join("|", Enum.GetNames(typeof(VariableType)));

        foreach (var e in root.Element("ModelVariables").Elements("ScalarVariable"))
        {
            var typeElement = e.XPathSelectElement(typeNames);

            ScalarVariable v = new()
            {
                name = (string)e.Attribute("name"),
                description = (string)e.Attribute("description"),
                causality = (string)e.Attribute("causality"),
                variability = (string)e.Attribute("variability"),
                initial = (string)e.Attribute("initial"),
                valueReference = (uint)e.Attribute("valueReference"),
                type = (VariableType)Enum.Parse(typeof(VariableType), typeElement.Name.ToString(), true),
                start = (string)typeElement.Attribute("start")
            };

            variables.Add(v);
        }

        modelDescription.modelVariables = variables.ToArray();

        return modelDescription;
    }

    static ModelDescription ReadModelDescriptionFMI3(XDocument doc)
    {
        XElement root = doc.Root;

        string xsdPath = Path.Combine(Application.dataPath, "FMI", "Editor", "schema", "fmi3", "fmi3ModelDescription.xsd");

        XmlSchemaSet schemas = new();
        schemas.Add("", xsdPath);

        bool isValid = true;
        string validationErrors = "";

        doc.Validate(schemas, (o, e) =>
        {
            isValid = false;
            validationErrors += e.Message + "\n";
        });

        if (!isValid)
        {
            throw new FMUImportException("Failed to validate model description. " + validationErrors);
        }

        var modelDescription = ScriptableObject.CreateInstance<ModelDescription>();

        modelDescription.fmiVersion = (string)root.Attribute("fmiVersion");
        modelDescription.instantiationToken = (string)root.Attribute("instantiationToken");
        modelDescription.modelName = (string)root.Attribute("modelName");

        var coSimulation = root.Element("CoSimulation") ?? throw new FMUImportException("The FMU does not support Co-Simulation.");

        modelDescription.coSimulation = new()
        {
            modelIdentifier = (string)coSimulation.Attribute("modelIdentifier")
        };

        var variables = new List<ScalarVariable>();

        string typeNames = string.Join("|", Enum.GetNames(typeof(VariableType)));

        foreach (var e in root.Element("ModelVariables").XPathSelectElements(typeNames))
        {
            ScalarVariable v = new()
            {
                type = (VariableType)Enum.Parse(typeof(VariableType), e.Name.ToString(), true),
                name = (string)e.Attribute("name"),
                description = (string)e.Attribute("description"),
                causality = (string)e.Attribute("causality"),
                variability = (string)e.Attribute("variability"),
                initial = (string)e.Attribute("initial"),
                valueReference = (uint)e.Attribute("valueReference"),
            };

            variables.Add(v);
        }

        modelDescription.modelVariables = variables.ToArray();

        return modelDescription;
    }

}
