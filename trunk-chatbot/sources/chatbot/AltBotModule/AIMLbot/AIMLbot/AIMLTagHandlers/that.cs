using System;
using System.Text;
using System.Xml;
using AltAIMLbot.Utils;
using AltAIMLParser;
using MushDLR223.ScriptEngines;
using RTParser;
using AIMLTagHandler=AltAIMLbot.Utils.AIMLTagHandler;
using Unifiable = System.String;

namespace AltAIMLbot.AIMLTagHandlers
{
    /// <summary>
    /// The template-side that element indicates that an AIML interpreter should substitute the 
    /// contents of a previous bot output. 
    /// 
    /// The template-side that has an optional index attribute that may contain either a single 
    /// integer or a comma-separated pair of integers. The minimum value for either of the integers 
    /// in the index is "1". The index tells the AIML interpreter which previous bot output should be 
    /// returned (first dimension), and optionally which "sentence" (see [8.3.2.]) of the previous bot
    /// output (second dimension). 
    /// 
    /// The AIML interpreter should raise an error if either of the specified index dimensions is 
    /// invalid at run-time. 
    /// 
    /// An unspecified index is the equivalent of "1,1". An unspecified second dimension of the index 
    /// is the equivalent of specifying a "1" for the second dimension. 
    /// 
    /// The template-side that element does not have any content. 
    /// </summary>
    public class that : AIMLConstraintTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public that(AltBot bot,
                        User user,
                        SubQuery query,
                        Request request,
                        Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode, 1)
        {
        }

        protected override string ProcessChange()
        {
            if (CheckNode("that,justbeforethat,response"))
            {
                var at1 = GetAttribValue("index", null);
                var talker = this.user;
                var responder = request.Responder;
                return GetIndexes(at1, responder, (a, b, u) => talker.getThat(a, b, u),
                                   (str, args) => localError(at1, str));
            }
            return string.Empty;
        }
    }

    abstract public class AIMLConstraintTagHandler : AIMLTagHandler
    {
        protected int offetFrom;
        public AIMLConstraintTagHandler(
                AltBot bot,
                User user,
                SubQuery query,
                Request request,
                Result result,
                XmlNode templateNode,
            int offset)
            : base(bot, user, query, request, result, templateNode)
        {
            offetFrom = offset;
        }

        protected void localError(string s, string at1)
        {
            writeToLogError("An input tag with a bady formed index (" + at1 + ") was encountered processing the input: " +
                this.request.rawInput + s);
        }

        public Unifiable GetIndexes(string at1, User responder, Func<int, int, User, Unifiable> getThat, OutputDelegate debug)
        {
            try
            {
                if (string.IsNullOrEmpty(at1))
                {
                    return getThat(offetFrom - 1, 0, responder);
                }
                if (at1.EndsWith(",*"))
                {
                    at1 = at1.Substring(0, at1.Length - 2);
                }
                if (at1 == "1")
                {

                    return getThat(0, 0, responder);
                }
                else if (at1 == "1,1")
                {

                    return getThat(0, 0, responder);
                }
                // see if there is a split
                string[] dimensions = at1.Split(",".ToCharArray());
                if (dimensions.Length == 2)
                {
                    int result = Convert.ToInt32(dimensions[0].Trim());
                    int sentence = Convert.ToInt32(dimensions[1].Trim());
                    if ((result > 0) & (sentence > 0))
                    {
                        return getThat(result - 1 + offetFrom - 1, sentence - 1, responder);
                    }
                    else
                    {
                        debug("");
                        return Unifiable.Empty;
                    }
                }
                else
                {
                    int result = Convert.ToInt32(at1.Trim());
                    if (result > 0)
                    {
                        return getThat(result - 1 + offetFrom - 1, 0, responder);
                    }
                    else
                    {
                        debug("");
                        return Unifiable.Empty;
                    }
                }
            }
            catch (Exception exception)
            {

                debug(" " + exception);
                return Unifiable.Empty;
            }
        }
    }

}
