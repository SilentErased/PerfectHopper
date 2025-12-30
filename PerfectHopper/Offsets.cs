using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

public static class Offsets
{
    public static string RobloxVersion { get; }
    public static int FakeDataModelPointer { get; }
    public static int FakeDataModelToDataModel { get; }
    public static int LocalPlayer { get; }
    public static int Children { get; }
    public static int ChildrenEnd { get; }
    public static int Name { get; }
    public static int PlaceId { get; }
    public static int UserId { get; }
    public static int LocalScriptByteCode { get; }
    public static int LocalScriptBytecodePointer { get; }
    public static int ModelInstance { get; }
    public static int ClassName { get; }
    public static int StringLenght { get; }
    public static int ModuleScriptByteCode { get; }
    public static int ModuleScriptBytecodePointer { get; }
    public static int Value { get; }
    public static int ClassDescriptor { get; }
    public static int ClassDescriptorToClassName { get; }
    public static int Parent { get; }

    static Offsets()
    {
        string json = new WebClient().DownloadString("https://offsets.ntgetwritewatch.workers.dev/offsets.json");
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        RobloxVersion = dict["RobloxVersion"];
        FakeDataModelPointer = ParseOffset(dict["FakeDataModelPointer"]);
        FakeDataModelToDataModel = ParseOffset(dict["FakeDataModelToDataModel"]);
        LocalPlayer = ParseOffset(dict["LocalPlayer"]);
        Children = ParseOffset(dict["Children"]);
        ChildrenEnd = ParseOffset(dict["ChildrenEnd"]);
        Name = ParseOffset(dict["Name"]);
        PlaceId = ParseOffset(dict["PlaceId"]);
        UserId = ParseOffset(dict["UserId"]);
        ModelInstance = ParseOffset(dict["ModelInstance"]);
        LocalScriptByteCode = ParseOffset(dict["LocalScriptByteCode"]);
        LocalScriptBytecodePointer = ParseOffset(dict["LocalScriptBytecodePointer"]);
        ClassName = ParseOffset(dict["ClassDescriptor"]) + ParseOffset(dict["ClassDescriptorToClassName"]);
        ModuleScriptByteCode = ParseOffset(dict["ModuleScriptByteCode"]);
        ModuleScriptBytecodePointer = ParseOffset(dict["ModuleScriptBytecodePointer"]);
        StringLenght = ParseOffset(dict["StringLength"]);
        Value = ParseOffset(dict["Value"]);
        ClassDescriptor = ParseOffset(dict["ClassDescriptor"]);
        ClassDescriptorToClassName = ParseOffset(dict["ClassDescriptorToClassName"]);
        Parent = ParseOffset(dict["Parent"]);
    }

    private static int ParseOffset(string s)
    {
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToInt32(s.Substring(2), 16);
        return int.TryParse(s, out var v) ? v : 0;
    }
}