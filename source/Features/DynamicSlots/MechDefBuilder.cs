﻿using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer
{
    internal class MechDefBuilder
    {
        internal readonly DataManager DataManager;
        internal readonly MechDefSlots Slots;
        internal readonly List<MechComponentRef> Inventory;
        internal readonly Dictionary<ChassisLocations, int> LocationUsage = new Dictionary<ChassisLocations, int>();

        internal MechDefBuilder(ChassisDef chassisDef, List<MechComponentRef> inventory)
        {
            Slots = new MechDefSlots(chassisDef, inventory);

            Inventory = inventory;

            DataManager = UnityGameInstance.BattleTechGame.DataManager;
            //Control.mod.Logger.LogDebug("");
            //Control.mod.Logger.LogDebug($"chassisDef={chassisDef.Description.Id}");

            foreach (var group in inventory.GroupBy(r => r.MountedLocation))
            {
                var location = group.Key;
                var sum = group.Sum(r => r.Def.InventorySize);
                LocationUsage[location] = sum;
                //Control.mod.Logger.LogDebug($"location={location} sum={sum}");
            }
        }

        internal bool Add(MechComponentDef def, ChassisLocations location = ChassisLocations.None)
        {
            // find location
            if (location == ChassisLocations.None || LocationCount(location) > 1)
            {
                location = AddSlots(def.InventorySize, def.AllowedLocations);
                if (location == ChassisLocations.None)
                {
                    return false;
                }
            }
            else
            {
                AddSlotsToLocation(def.InventorySize, location, true);
            }
            var componentRef = new MechComponentRef(def.Description.Id, null, def.ComponentType, location);
            componentRef.DataManager = DataManager;
            componentRef.RefreshComponentDef();
            Inventory.Add(componentRef);
            return true;
        }

        private ChassisLocations AddSlots(int slotCount, ChassisLocations allowedLocations)
        {
            return GetLocations()
                .Where(location => (location & allowedLocations) != 0)
                .FirstOrDefault(location => AddSlotsToLocation(slotCount, location));
        }

        private IEnumerable<ChassisLocations> GetLocations()
        {
            yield return ChassisLocations.CenterTorso;

            if (GetFreeSlots(ChassisLocations.LeftTorso) >= GetFreeSlots(ChassisLocations.RightTorso))
            {
                yield return ChassisLocations.LeftTorso;
                yield return ChassisLocations.RightTorso;
            }
            else
            {
                yield return ChassisLocations.RightTorso;
                yield return ChassisLocations.LeftTorso;
            }

            if (GetFreeSlots(ChassisLocations.LeftLeg) >= GetFreeSlots(ChassisLocations.RightLeg))
            {
                yield return ChassisLocations.LeftLeg;
                yield return ChassisLocations.RightLeg;
            }
            else
            {
                yield return ChassisLocations.RightLeg;
                yield return ChassisLocations.LeftLeg;
            }

            yield return ChassisLocations.Head;

            if (GetFreeSlots(ChassisLocations.LeftArm) >= GetFreeSlots(ChassisLocations.RightArm))
            {
                yield return ChassisLocations.LeftArm;
                yield return ChassisLocations.RightArm;
            }
            else
            {
                yield return ChassisLocations.RightArm;
                yield return ChassisLocations.LeftArm;
            }
        }

        private bool AddSlotsToLocation(int slotCount, ChassisLocations location, bool force = false)
        {
            var used = GetUsedSlots(location);
            var max = GetMaxSlots(location);
            if (force || max - used >= slotCount)
            {
                LocationUsage[location] = used + slotCount;
                return true;
            }
            return false;
        }

        internal void Remove(MechComponentRef item)
        {
            Inventory.Remove(item);
            LocationUsage[item.MountedLocation] -= item.Def.InventorySize;
        }

        internal static int LocationCount(ChassisLocations container)
        {
            if (container == ChassisLocations.All)
            {
                return MechDefSlots.Locations.Length;
            }
            else
            {
                return MechDefSlots.Locations.Count(location => (container & location) != ChassisLocations.None);
            }
        }

        internal int GetUsedSlots(ChassisLocations location)
        {
            return LocationUsage.TryGetValue(location, out var count) ? count : 0;
        }

        internal int GetMaxSlots(ChassisLocations location)
        {
            return Slots.Chassis.GetLocationDef(location).InventorySlots;
        }

        internal int GetFreeSlots(ChassisLocations location)
        {
            return GetMaxSlots(location) - GetUsedSlots(location);
        }

        internal bool HasOveruse()
        {
            return (from location in MechDefSlots.Locations
                let max = GetMaxSlots(location)
                let used = GetUsedSlots(location)
                where used > max
                select max).Any();
        }
    }
}