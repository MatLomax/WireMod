using System.Collections.Generic;

namespace WireMod
{
    public static class Constants
    {
        public static readonly List<string> ToolCategories = new List<string>
        {
            "Tools",
            "Logic Gates",
            "Maths",
            "Comparison",
            "Basic Inputs",
            "Sensor Inputs",
            "Text",
            "Outputs",
            "Experimental",
        };

        public static readonly List<List<string>> Tools = new List<List<string>>
        {
            new List<string>
            {
                "None",
                "Wiring",
                "Delete",
                "Debug",
            },
            new List<string>
            {
                "AndGate",
                "OrGate",
                "NotGate",
            },
            new List<string>
            {
                "Add",
                "Subtract",
                "Multiply",
                "Divide",
                "Modulo",
            },
            new List<string>
            {
                "Equals",
                "LessThan",
                "GreaterThan",
            },
            new List<string>
            {
                "BooleanConstant",
                "IntegerConstant",
                "StringConstant",
                "TeamColorConstant",
                "RandomInt",
            },
            new List<string>
            {
                "PlayerDistanceSensor",
                "NPCDistanceSensor",
                "TimeSensor",
                "NPCCounter",
            },
            new List<string>
            {
                "Concat",
                "TextTileController",
            },
            new List<string>
            {
                "OutputLamp",
                //"OutputSign",
                "FlipFlop",
                "Trigger",
            },
            new List<string>
            {
                "Repulsor",
                "Spawner",
                "Buffer",
            },
        };

        public static readonly Dictionary<string, string> ToolNames = new Dictionary<string, string>
        {
            { "None", "None" },
            { "Wiring", "Wiring" },
            { "Delete", "Remove Device" },
            { "Debug", "Debug Device" },
            { "AndGate", "AND Logic Gate" },
            { "OrGate", "OR Logic Gate" },
            { "NotGate", "NOT Logic Gate" },
            { "Add", "Add Operation" },
            { "Subtract", "Subtract Operation" },
            { "Multiply", "Multiply Operation" },
            { "Divide", "Divide Operation" },
            { "Modulo", "Modulo Operation" },
            { "Equals", "Equals (=) Comparer" },
            { "LessThan", "Less Than (<) Comparer" },
            { "GreaterThan", "Greater Than (>) Comparer" },
            { "BooleanConstant", "Constant Boolean Value" },
            { "IntegerConstant", "Constant Number Value" },
            { "StringConstant", "Constant String Value" },
            { "TeamColorConstant", "Constant Team Color Value" },
            { "RandomInt", "Random Integer Value" },
            { "Concat", "Concatenate Strings" },
            { "TextTileController", "Text Tile Controller" },
            { "FlipFlop", "Flip/Flop" },
            { "OutputLamp", "Output Lamp" },
            //{ "OutputSign", "Output Sign" },
            { "Trigger", "Trigger Output" },
            { "PlayerDistanceSensor", "Nearest Player Distance Sensor" },
            { "NPCDistanceSensor", "Nearest NPC Distance Sensor" },
            { "TimeSensor", "Time Sensor - basically a glorified clock" },
            { "NPCCounter", "Count nearby NPCs" },
            { "Repulsor", "Repulsor (USE WITH CAUTION)" },
            { "Spawner", "NPC Spawner (USE WITH CAUTION)" },
            { "Buffer", "Buffer (USE WITH CAUTION)" },
        };
    }
}