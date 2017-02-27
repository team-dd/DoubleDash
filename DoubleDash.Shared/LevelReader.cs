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
            using (var streamReader = new FileStream(filename, FileMode.Open))
            {
                int levelSize = (int) streamReader.Length;
                Byte[] levelDataRaw = new Byte[levelSize];
                streamReader.Read(levelDataRaw, 0, (int)levelSize);
                GameSave save = JsonConvert.DeserializeObject<GameSave>(System.Text.Encoding.Default.GetString(levelDataRaw));
                Level level = new Level();
                foreach (BlockDescription block in save.blocks)
                {
                    level.blocksDescription.Add(block);
                }
                level.start = save.start * 4;
                level.end = save.end * 4;
                return level;
            }
        }
    }
}
