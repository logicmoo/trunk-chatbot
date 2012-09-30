using System;
using RTParser;
using RTParser.Utils;

namespace AltAIMLbot.Utils
{
    /// <summary>
    /// Encapsulates all the required methods and attributes for any text transformation.
    /// 
    /// An input string is provided and various methods and attributes can be used to grab
    /// a transformed string.
    /// 
    /// The protected ProcessChange() method is abstract and should be overridden to contain 
    /// the code for transforming the input text into the output text.
    /// </summary>
    abstract public class TextTransformer : StaticAIMLUtils
    {

        public override string ToString()
        {
            return GetType().Name + ": " + inputString;
        }

        #region Attributes
        /// <summary>
        /// Instance of the input string
        /// </summary>
        protected string inputString;

        /// <summary>
        /// The bot that this transformation is connected with
        /// </summary>
        public AltBot bot;

        public AltBot Proc
        {
            get { return bot; }
        }

        /// <summary>
        /// The input string to be transformed in some way
        /// </summary>
        public string InputString
        {
            get{return this.inputString;}
            set{this.inputString=value;}
        }

        /// <summary>
        /// The transformed string
        /// </summary>
        public string OutputString
        {
            get{return this.Transform();}
        }

        public virtual bool isFormatter
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bot">The bot this transformer is a part of</param>
        /// <param name="inputString">The input string to be transformed</param>
        public TextTransformer(AltBot bot, string inputString)
            : this(bot, inputString, null)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bot">The bot this transformer is a part of</param>
        public TextTransformer(AltBot bot)
            : this(bot, string.Empty)
        {
        }

        /// <summary>
        /// Default ctor for used as part of late binding mechanism
        /// </summary>
        public TextTransformer()
            : this(null, string.Empty)
        {
        }

        /// <summary>
        /// Do a transformation on the supplied input string
        /// </summary>
        /// <param name="input">The string to be transformed</param>
        /// <returns>The resulting output</returns>
        public string Transform(string input)
        {
            this.inputString = input;
            return this.Transform();
        }

        private string transformComplete = null;
        /// <summary>
        /// Do a transformation on the string found in the InputString attribute
        /// </summary>
        /// <returns>The resulting transformed string</returns>
        virtual public string Transform()
        {
            if (this.inputString.Length > 0)
            {
                if (transformComplete == null)
                {
                    transformComplete = this.ProcessChange();
                }
                return transformComplete;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        /// <returns>The resulting processed text</returns>
        protected abstract string ProcessChange();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bot">The bot this transformer is a part of</param>
        /// <param name="inputString">The input Unifiable to be transformed</param>
        public TextTransformer(AltBot bot, string instr, Unifiable inputString)
        {
            this.bot = bot;
            this.InputStringUU = inputString ?? instr;
            this.inputString = initialString = instr ?? InputStringUU.AsString();
        }

        public virtual float CallCanUnify(Unifiable with)
        {
            return InputStringU == with ? Unifiable.UNIFY_TRUE : Unifiable.UNIFY_FALSE;
        }

        /// <summary>
        /// Do a transformation on the supplied input Unifiable
        /// </summary>
        /// <param name="input">The Unifiable to be transformed</param>
        /// <returns>The resulting output</returns>
        public string TransformU(string input)
        {
            this.InputStringUU = Unifiable.MakeUnifiableFromString(input, false);
            this.inputString = input;
            return this.TransformU();
        }

        /// <summary>
        /// Do a transformation on the Unifiable found in the InputString attribute
        /// </summary>
        /// <returns>The resulting transformed Unifiable</returns>
        public virtual Unifiable TransformU()
        {
            if (!IsNullOrEmpty(this.InputStringUU))
            {
                return this.ProcessAimlChange();
            }
            else
            {
                return Unifiable.Empty;
            }
        }

        public virtual Unifiable ProcessAimlChange()
        {
            return ProcessChangeU();
        }

        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        /// <returns>The resulting processed text</returns>
        protected abstract Unifiable ProcessChangeU();

        public virtual Unifiable CompleteProcessU()
        {
            return InputStringUU;
        }

        #region Attributes

        public string initialString;

        /// <summary>
        /// Instance of the input Unifiable
        /// </summary>
        protected Unifiable InputStringUU;


        /// <summary>
        /// The input Unifiable to be transformed in some way
        /// </summary>
        public Unifiable InputStringU
        {
            get { return this.InputStringUU; }
            set { this.InputStringUU = value; }
        }

        /// <summary>
        /// The transformed Unifiable
        /// </summary>
        public Unifiable OutputStringU
        {
            get { return this.TransformU(); }
        }

        #endregion
    }
}
