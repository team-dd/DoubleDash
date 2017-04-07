using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DoubleDash
{
    [JsonObject]
    class GameSave
    {
        [JsonProperty("rectangles")]
        public List<BlockDescription> blocks;

        [JsonProperty("start")]
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 start;

        [JsonProperty("end")]
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 end;
    }
}
