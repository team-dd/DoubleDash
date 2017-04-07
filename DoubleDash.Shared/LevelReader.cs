using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DoubleDash;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace DoubleDash
{
    public class LevelReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            using (var streamReader = new StreamReader(input.BaseStream))
            {
                JArray a = JArray.Load(new JsonTextReader(streamReader));
                Level level = new Level(0);
                foreach (JObject o in a)
                {
                    level.blocksDescription.Add(new BlockDescription(
                        (int)o["X"],
                        (int)o["Y"],
                        (int)o["Width"],
                        (int)o["Height"],
                        (bool)o["IsMoving"]));
                }
                return level;
            }
        }

        public static Level Load(string filename)
        {
            Stream streamReader = null;

#if WINDOWS_UWP
            StorageFile file = null;
            Task.Run(async () =>
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{filename}"));
                streamReader = await file.OpenStreamForReadAsync();
            }).Wait();
#else
            streamReader = new FileStream(filename, FileMode.Open);
#endif
            int levelSize = (int)streamReader.Length;
            Byte[] levelDataRaw = new Byte[levelSize];
            streamReader.Read(levelDataRaw, 0, (int)levelSize);
            GameSave save = JsonConvert.DeserializeObject<GameSave>(Encoding.UTF8.GetString(levelDataRaw));
            Level level = new Level(0);
            foreach (BlockDescription block in save.blocks)
            {
                level.blocksDescription.Add(block);
            }
            level.start = save.start * 4;
            level.end = save.end * 4;
            streamReader.Dispose();
            return level;
        }
    }
}
