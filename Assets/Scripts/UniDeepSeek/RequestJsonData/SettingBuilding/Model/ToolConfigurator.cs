using System.Collections.Generic;
using Xiyu.UniDeepSeek.Tools;

namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public class ToolConfigurator : ITool, IToolInstances, IToolChoice
    {
        public ToolConfigurator(IRequestParameter requestParameter, IList<ToolInstance> instances, ToolChoice choice)
        {
            Base = requestParameter;
            _instances = instances;
            _choice = choice;
        }

        private readonly IList<ToolInstance> _instances;
        private readonly ToolChoice _choice;


        public IToolInstances ToolInstances => this;
        public IToolChoice ToolChoice => this;


        public IRequestParameter Base { get; }


        public ITool FunctionDefine(FunctionDefine functionDefine)
        {
            _instances.Add(new ToolInstance { FunctionDefine = functionDefine });
            return this;
        }

        public ITool SetNone()
        {
            _choice.FunctionCallModel = FunctionCallModel.None;
            return this;
        }

        public ITool SetAuto()
        {
            _choice.FunctionCallModel = FunctionCallModel.Auto;
            return this;
        }

        public IToolInstances SetRequired()
        {
            _choice.FunctionCallModel = FunctionCallModel.Required;
            return this;
        }

        public IToolInstances SetFunction()
        {
            _choice.FunctionCallModel = FunctionCallModel.Function;
            return this;
        }
    }
}