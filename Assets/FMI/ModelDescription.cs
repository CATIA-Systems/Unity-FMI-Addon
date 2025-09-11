using System;
using UnityEngine;


[Serializable]
public class ModelDescription : ScriptableObject
{

    public string fmiVersion;

    public string instantiationToken;

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
    Float32,
    Float64,
    Int32,
    UInt32,
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
