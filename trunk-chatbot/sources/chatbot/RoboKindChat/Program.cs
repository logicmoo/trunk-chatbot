using System;
using System.IO;
using AltAIMLbot;
using LAIR.ResourceAPIs.WordNet;
using RoboKindAvroQPID;
using RTParser;

namespace RoboKindChat
{
    public class Program
    {
        private static RoboKindEventModule _theRoboKindEventModule;
        private static ChatProgram _theChatProg;


        static void Main(string[] args)
        {
            DeletePreArtifacts();
            _theRoboKindEventModule = new RoboKindEventModule();
            _theChatProg = new AltAIMLbot.ChatProgram();
            //_theRoboKindEventModule.Spy();
            //_theRoboKindEventModule.Block();
         
            _theChatProg.SetForegrounded(true);
            //_theChatProg.LoadDataset("justine_degurl");
            //_theChatProg.LoadDataset("kotoko_irata");
            //_theChatProg.LoadDataset("test_suite/ProgramD/AIML.aiml");
            //_theChatProg.LoadDataset("special/blackjack.aiml");
            // _theChatProg.LoadDataset("special/lesson_template.aiml"); 
            
            // for now lets use the old interactor for texting
            try
            {
                AltBot.Main(args);
            } finally
            {
                _theChatProg.Terminate();
                
            }
            return;
            _theChatProg.RunMain("Nephrael Rae","consoleUser",(s) => Console.Write(s), () =>
                                                              {
                                                                  Console.Write("You: ");
                                                                  return Console.ReadLine();
                                                              }, true);
        }

        private static void DeletePreArtifacts()
        {
            DeleteArtifact("./aiml/servitorgraphmap.aiml");
            DeleteArtifact("./rapstore/");
        }

        private static void DeleteArtifact(string sgm)
        {
            if (File.Exists(sgm)) File.Delete(sgm);
            if (Directory.Exists(sgm)) Directory.Delete(sgm, true);
        }
    }
}
