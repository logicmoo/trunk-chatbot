using System;
using System.Runtime;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using AltAIMLbot;
using AltAIMLbot.Utils;
using AltAIMLParser;
using RTParser;
using RTParser.Utils;

namespace RTParser.AIMLTagHandlers
{
    public class setstate : RTParser.Utils.AIMLTagHandlerU
    {

        public setstate(RTParser.AltBot bot,
                User user,
                SubQuery query,
                Request request,
                Result result,
                XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }



        protected override Unifiable ProcessChangeU()
        {
            if (CheckNode("setstate"))
            {
                try
                {
                    var varMSM = this.botActionMSM;
                    string machine = GetAttribValue("machine", varMSM.lastDefMachine);
                    string name = GetAttribValue("name", varMSM.lastDefState);
                    string cur_prob_str = GetAttribValue("prob", "0.1");
                    double cur_prob = double.Parse(cur_prob_str);

                    MachineSideEffect(() => varMSM.setState(machine, name, cur_prob));
                }
                catch (Exception e)
                {
                    writeToLogWarn("MSMWARN: " + e);
                }

            }
            return Unifiable.Empty;

        }
    }
}