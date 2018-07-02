﻿using System;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal static class Extensions
    {
        public static bool CheckComponentDef(this MechComponentDef def, ComponentType type, string prefix)
        {
            if (def.ComponentType != type)
            {
                return false;
            }

            if (def.Description == null || def.Description.Id == null)
            {
                return false;
            }

            return def.Description.Id.StartsWith(prefix);
        }

        internal static bool IsSingle(this HeatSinkDef def)
        {
            return def.Description.Id == Control.settings.GearHeatSinkStandard;
        }

        internal static void PerformOperation(this StatCollection collection, Statistic statistic, StatisticEffectData data)
        {
            var type = Type.GetType(data.modType);
            var variant = new Variant(type);
            variant.SetValue(data.modValue);
            variant.statName = data.statName;
            collection.PerformOperation(statistic, data.operation, variant);
        }

        internal static float RoundStandard(this float @this)
        {
            if (Control.settings.FractionalAccounting)
            {
                return Mathf.Round(@this * 1000) / 1000;
            }

            return Mathf.Round(@this * 2) / 2;
        }

        internal static float RoundBy5(this float @this)
        {
            return Mathf.Round(@this / 5) * 5;
        }
    }
}