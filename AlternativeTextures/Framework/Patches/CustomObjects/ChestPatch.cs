﻿using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.CustomObjects
{
    internal class ChestPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Chest);

        internal ChestPatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Chest.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Chest.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawRecolorPrefix)));
        }

        private static bool DrawPrefix(Chest __instance, int ____shippingBinFrameCounter, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1)
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                float draw_x = x;
                float draw_y = y;
                if (__instance.localKickStartTile.HasValue)
                {
                    draw_x = Utility.Lerp(__instance.localKickStartTile.Value.X, draw_x, __instance.kickProgress);
                    draw_y = Utility.Lerp(__instance.localKickStartTile.Value.Y, draw_y, __instance.kickProgress);
                }

                float base_sort_order = Math.Max(0f, ((draw_y + 1f) * 64f - 24f) / 10000f) + draw_x * 1E-05f;
                if (__instance.localKickStartTile.HasValue)
                {
                    spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((draw_x + 0.5f) * 64f, (draw_y + 0.5f) * 64f)), Game1.shadowTexture.Bounds, Color.Black * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
                    draw_y -= (float)Math.Sin((double)__instance.kickProgress * Math.PI) * 0.5f;
                }

                spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f + (float)((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (draw_y - 1f) * 64f)), new Rectangle(0, textureOffset, 16, 32), __instance.tint.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, base_sort_order);
                spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f + (float)((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (draw_y - 1f) * 64f)), new Rectangle((1 + ____shippingBinFrameCounter) * textureModel.TextureWidth, textureOffset, 16, 32), __instance.tint.Value * alpha * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, base_sort_order + 1E-05f);
                return false;
            }
            return true;
        }

        private static bool DrawRecolorPrefix(Chest __instance, int ___currentLidFrame, SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool local = false)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1)
                {
                    return true;
                }

                if ((bool)__instance.playerChest)
                {
                    var textureOffset = textureVariation * textureModel.TextureHeight;
                    spriteBatch.Draw(textureModel.Texture, local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), new Rectangle(0, textureOffset, 16, 32), __instance.playerChoiceColor.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : ((float)(y * 64 + 4) / 10000f));
                }

                return false;
            }
            return true;
        }
    }
}
