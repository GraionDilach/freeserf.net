﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Freeserf.Render
{
    public class TextureAtlasManager
    {
        static TextureAtlasManager instance = null;
        static ITextureAtlasBuilderFactory factory = null;
        readonly Dictionary<int, ITextureAtlasBuilder> atlasBuilders = new Dictionary<int, ITextureAtlasBuilder>();
        readonly Dictionary<int, ITextureAtlas> atlas = new Dictionary<int, ITextureAtlas>();

        public static TextureAtlasManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextureAtlasManager();

                return instance;
            }
        }

        TextureAtlasManager()
        {

        }

        public static void RegisterFactory(ITextureAtlasBuilderFactory factory)
        {
            TextureAtlasManager.factory = factory;
        }

        public void AddSprite(int atlasIndex, uint spriteIndex, Sprite sprite)
        {
            if (factory == null)
                throw new ExceptionFreeserf("No TextureAtlasBuilderFactory was registered.");

            if (atlas.ContainsKey(atlasIndex))
                throw new ExceptionFreeserf("Texture atlas already created.");

            if (!atlasBuilders.ContainsKey(atlasIndex))
                atlasBuilders.Add(atlasIndex, factory.Create());

            atlasBuilders[atlasIndex].AddSprite(spriteIndex, sprite);
        }

        public ITextureAtlas GetOrCreate(int index)
        {
            if (!atlas.ContainsKey(index))
                atlas.Add(index, atlasBuilders[index].Create());

            return atlas[index];
        }

        public void AddAll(DataSource data)
        {
            uint i;

            // use black color (TODO: correct for all?)
            var color = new Sprite.Color() { Red = 0, Green = 0, Blue = 0, Alpha = 255 };

            // Note:
            // We enlarge all tile sprites to the maximum height of 41 (max mask height) with repeated texture data.
            // The masks are also enlarged to this height but with cleared data (full transparency).
            // This way the masked tiles will show up correctly and we don't need to change sizes when tiles change.

            // add all normal landscape tile sprites
            for (i = 0; i < 33u; ++i) // 33 map tile sprites
                AddSprite((int)Layer.Landscape, i, data.GetSprite(Data.Resource.MapGround, i, color).RepeatTo(RenderMap.TILE_RENDER_MAX_HEIGHT));

            // add all tile up mask sprites
            uint numUpMasks = 61u; // 61 tile up mask sprites
            i = 0;
            uint maskIndex = 0;

            while (maskIndex < numUpMasks)
            {
                var mask = data.GetSprite(Data.Resource.MapMaskUp, i, color);

                if (mask != null)
                {
                    AddSprite((int)Layer.Landscape, maskIndex + 33u, mask.ClearTo(RenderMap.TILE_RENDER_MAX_HEIGHT));
                    ++maskIndex;
                }

                ++i;
            }

            // add all tile down mask sprites
            uint numDownMasks = 61u; // 61 tile down mask sprites
            i = 0;
            maskIndex = 0;

            while (maskIndex < numDownMasks)
            {
                var mask = data.GetSprite(Data.Resource.MapMaskDown, i, color);

                if (mask != null)
                {
                    AddSprite((int)Layer.Landscape, maskIndex + 33u + 61u, mask.ClearTo(RenderMap.TILE_RENDER_MAX_HEIGHT));
                    ++maskIndex;
                }

                ++i;
            }

            // TODO
        }
    }
}
