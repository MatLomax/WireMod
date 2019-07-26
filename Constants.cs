using System.Collections.Generic;

namespace WireMod
{
    public static class Constants
    {
        public static readonly List<string> ToolCategories = new List<string>
        {
            "Wiring",
            "Logic Gates",
            "Maths",
            "Comparison",
            "Constants",
            "Outputs",
            "Sensors",
        };

        public static readonly List<List<string>> Tools = new List<List<string>>
        {
            new List<string>
            {
                "None",
                "Wiring",
                "Delete"
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
                "LessThan",
                "GreaterThan",
            },
            new List<string>
            {
                "BooleanConstant",
                "NumberConstant",
                "StringConstant",
                "TeamColorConstant",
            },
            new List<string>
            {
                "OutputLamp",
                "OutputSign",
                "FlipFlop",
            },
            new List<string>
            {
                "PlayerDistanceSensor",
                "NPCDistanceSensor",
                "TimeSensor",
            },
        };

        public static readonly Dictionary<string, string> ToolNames = new Dictionary<string, string>
        {
            { "None", "None" },
            { "Wiring", "Wiring" },
            { "Delete", "Remove Device" },
            { "AndGate", "AND Logic Gate" },
            { "OrGate", "OR Logic Gate" },
            { "NotGate", "NOT Logic Gate" },
            { "Add", "Add Operation" },
            { "Subtract", "Subtract Operation" },
            { "Multiply", "Multiply Operation" },
            { "Divide", "Divide Operation" },
            { "Modulo", "Modulo Operation" },
            { "LessThan", "Less Than (<) Comparer" },
            { "GreaterThan", "Greater Than (>) Comparer" },
            { "BooleanConstant", "Constant Boolean Value" },
            { "NumberConstant", "Constant Number Value" },
            { "StringConstant", "Constant String Value" },
            { "TeamColorConstant", "Constant Team Color Value" },
            { "FlipFlop", "Flip/Flop" },
            { "OutputLamp", "Output Lamp" },
            { "OutputSign", "Output Sign" },
            { "PlayerDistanceSensor", "Nearest Player Distance Sensor" },
            { "NPCDistanceSensor", "Nearest NPC Distance Sensor" },
            { "TimeSensor", "Time Sensor - basically a glorified clock" },
        };
    }
}