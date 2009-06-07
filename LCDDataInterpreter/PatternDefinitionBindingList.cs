using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LCDDataInterpreter {

    [Serializable]
    class PatternDefinition {
        public Bitmap Pattern { get; set; }
        public String Value { get; set; }

        public PatternDefinition(Bitmap pattern, String value) {
            Pattern = pattern;
            Value = value;
        }
    }

    class PatternDefinitionBindingList: BindingList<PatternDefinition> {

        public void Save(Stream target) {
            var bformatter = new BinaryFormatter();
            bformatter.Serialize(target, Items);
            target.Close();
        }

        public void Load(Stream source) {
            var bformatter = new BinaryFormatter();
            var dict = bformatter.Deserialize(source) as IList<PatternDefinition>;
            source.Close();

            Clear();
            if (dict != null) {
                foreach (PatternDefinition pd in dict) {
                    Add(pd);
                }
            }
        }
    }
}
