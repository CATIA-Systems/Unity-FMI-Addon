using System;
using UnityEngine;


[Serializable]
public class ModelDescription : ScriptableObject
{

    public string fmiVersion;

    // store the GUID as a char array to work around serialization bug in Unity (curly braces)
    public char[] guid;

    public string modelName;

    public Implementation coSimulation;

    public ScalarVariable[] modelVariables;

}

[Serializable]
public class Implementation
{

    public string modelIdentifier;

}

public enum VariableType
{
    Real, Integer, Enumeration, Boolean, String
}

[Serializable]
public class ScalarVariable {

	public string name;

	public uint valueReference;

	public string description;

    public string causality;

    public string variability;

    public string initial;

    public VariableType type;

    public string start;

}
