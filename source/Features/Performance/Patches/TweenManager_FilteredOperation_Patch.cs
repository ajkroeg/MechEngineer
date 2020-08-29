﻿using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace MechEngineer.Features.Performance.Patches
{
    [HarmonyPatch]
    public static class TweenManager_FilteredOperation_Patch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("DG.Tweening.Core.TweenManager:FilteredOperation");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(object), nameof(object.Equals), new []{typeof(object)}),
                AccessTools.Method(typeof(object), nameof(object.ReferenceEquals))
            ).MethodReplacer(
                AccessTools.Method(typeof(object), nameof(object.Equals), new []{typeof(object),typeof(object)}),
                AccessTools.Method(typeof(object), nameof(object.ReferenceEquals))
            );
        }
    }
}