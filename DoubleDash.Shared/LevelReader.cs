using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DoubleDash;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoubleDash
{
    public class LevelReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            using (var streamReader = new StreamReader(input.BaseStream))
            {
                JArray a = JArray.Load(new JsonTextReader(streamReader));
                Level level = new Level();
                foreach (JObject o in a)
                {
                    level.blocksDescription.Add(new BlockDescription(
                        (int)o["X"],
                        (int)o["Y"],
                        (int)o["Width"],
                        (int)o["Height"]));
                }
                return level;
            }
        }

        public static Level Load(string filename)
        {
            using (var streamReader = new StreamReader(filename))
            {
                JArray a = JArray.Load(new JsonTextReader(streamReader));
                Level level = new Level();
                foreach (JObject o in a)
                {
                    level.blocksDescription.Add(new BlockDescription(
                        (int)o["X"],
                        (int)o["Y"],
                        (int)o["Width"],
                        (int)o["Height"]));
                }
                return level;
            }
        }
    }
}
