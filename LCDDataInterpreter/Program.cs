using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using LCDDataInterpreter.Filter;

namespace LCDDataInterpreter {
    static class Program {

        public const string defaultFileName = "autosave.pdm";
        public static IList<PatternDefinition> PatternDefinitionList { get; private set; }
        public static FilterChain FilterChain { get; private set; }

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var patternDefinitionList = new PatternDefinitionBindingList();
            PatternDefinitionList = patternDefinitionList;

            // load default list
            var fi = new FileInfo(@".\" + defaultFileName);
            if(fi.Exists) {
                patternDefinitionList.Load(fi.OpenRead());
            }

            // set up filter chain
            FilterChain = new FilterChain(new Grayscale(), new Contrast(20), new AmbienceRemover(128, 3), new Inverter()); // , new Threshold(230), new Sharpen(9)

            Application.Run(new PatternMatcherForm());

            // save default list
            patternDefinitionList.Save(fi.OpenWrite());
        }
    }
}
