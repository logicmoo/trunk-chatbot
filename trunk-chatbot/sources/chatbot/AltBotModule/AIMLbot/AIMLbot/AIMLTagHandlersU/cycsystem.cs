using System;
using System.Xml;
using AltAIMLbot;
using AltAIMLbot.Utils;
using AltAIMLParser;

namespace RTParser.AIMLTagHandlers
{
    /// <summary>
    /// &lt;cycsystem&gt; executes a CycL statement and returns the result 
    /// </summary>
    public class cycsystem : RTParser.Database.CycTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be Processed</param>
        public cycsystem(RTParser.AltBot bot,
                        User user,
                        SubQuery query,
                        Request request,
                        Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }
        public override Unifiable CompleteProcessU()
        {
            return ProcessAimlChange();
        }

        protected override Unifiable ProcessChangeU()
        {
            if (this.templateNode.Name.ToLower() == "cycsystem")
            {
                Unifiable filter = base.GetAttribValue("filter", null);
                if (!IsNullOrEmpty(templateNodeInnerText))
                {
                    if (WhenTrue(this.TheCyc.EvalSubL(Recurse(), filter)))
                    {
                        base.Succeed();
                        return templateNodeInnerText;
                    }
                }
            }
            return Unifiable.Empty;
        }
    }
}
