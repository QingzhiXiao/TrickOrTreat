using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlmUnity
{
    public class GlmFunctionTool : GlmTool
    {
        public override string type { get => "function"; }
        public GlmFunction function;

        public class GlmFunction
        {
            /// <summary>
            /// 函数名称
            /// </summary>
            public string name;

            /// <summary>
            /// 函数描述
            /// </summary>
            public string description;

            /// <summary>
            /// 函数的输入参数
            /// </summary>
            public GlmFunctionParameters parameters;

            /// <summary>
            /// 函数的返回值，string格式
            /// </summary>
            public string arguments;

            public GlmFunction(string name, string description)
            {
                this.name = name;
                this.description = description;
                parameters = new GlmFunctionParameters();
            }

            public class GlmFunctionParameters
            {
                public string type => "object";
                public Dictionary<string, GlmFunctionProperty> properties;

                /// <summary>
                /// 哪些Property必须被包含
                /// </summary>
                public List<string> required;

                public GlmFunctionParameters()
                {
                    properties = new Dictionary<string, GlmFunctionProperty>();
                    required = new List<string>();
                }

                public class GlmFunctionProperty
                {
                    public string type;
                    public string description;
                    [JsonProperty("enum")]
                    public List<string> Enum;

                    public GlmFunctionProperty(string type, string description, List<string> Enum)
                    {
                        this.type = type;
                        this.description = description;
                        this.Enum = Enum;
                    }
                }
            }
        }

    }

}