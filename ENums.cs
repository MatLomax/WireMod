using System.Collections.Generic;

namespace WireMod
{
    public enum TeamColor
    {
        White = 0, // None
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Pink = 5
    }

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
            },
            new List<string>
            {
                "LessThan",
                "GreaterThan",
            },
            new List<string>
            {
                "TrueConstant",
                "FalseConstant",
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
            },
        };

        //public static readonly Dictionary<string, string> ToolNames = new Dictionary<string, string>
        //{
        //    { "Wiring", "Wiring" },
        //    { "AndGate", "AND Logic Gate" },
        //    { "OrGate", "OR Logic Gate" },
        //    { "NotGate", "NOT Logic Gate" },
        //    { "LessThan", "Less Than (<) Comparer" },
        //    { "GreaterThan", "Greater Than (>) Comparer" },
        //    { "TrueConstant", "Constant True Value" },
        //    { "FalseConstant", "Constant False Value" },
        //    { "NumberConstant", "Constant Number Value" },
        //    { "TeamColorConstant", "Constant Team Color Value" },
        //    { "OutputLamp", "Output Lamp" },
        //    { "OutputSign", "Output Sign" },
        //};
    }
}