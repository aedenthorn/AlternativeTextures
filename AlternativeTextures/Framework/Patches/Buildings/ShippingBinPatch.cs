﻿using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Utilities.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Buildings
{
    internal class ShippingBinPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(ShippingBin);

        internal ShippingBinPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(ShippingBin.Update), new[] { typeof(GameTime) }), prefix: new HarmonyMethod(GetType(), nameof(UpdatePrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(ShippingBin.initLid), null), postfix: new HarmonyMethod(GetType(), nameof(InitLidPostfix)));
        }

        internal static bool UpdatePrefix(ShippingBin __instance, TemporaryAnimatedSprite ___shippingBinLid, Rectangle ___shippingBinLidOpenArea, Vector2 ____lidGenerationPosition, GameTime time)
        {
            if (___shippingBinLid != null && __instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                if (textureModel.GetTexture(textureVariation) != ___shippingBinLid.texture || ___shippingBinLid.sourceRectStartingPos != new Vector2(32, textureOffset))
                {
                    InitLidPostfix(__instance, ___shippingBinLid, ___shippingBinLidOpenArea, ____lidGenerationPosition);
                }

                return true;
            }

            return true;
        }

        internal static void InitLidPostfix(ShippingBin __instance, TemporaryAnimatedSprite ___shippingBinLid, Rectangle ___shippingBinLidOpenArea, Vector2 ____lidGenerationPosition)
        {
            if (___shippingBinLid != null && __instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                ___shippingBinLid.texture = textureModel.GetTexture(textureVariation);
                ___shippingBinLid.currentParentTileIndex = 0;
                ___shippingBinLid.sourceRect = new Rectangle(32, textureOffset, 30, 25);
                ___shippingBinLid.sourceRectStartingPos = new Vector2(32, textureOffset);
            }

            return;
        }
    }
}
