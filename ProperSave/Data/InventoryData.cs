using RoR2;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ProperSave.Data
{
    public class InventoryData
    {
        [DataMember(Name = "ib")]
        public uint infusionBonus;
        [DataMember(Name = "ed")]
        public bool equipmentDisabled;
        [DataMember(Name = "bah")]
        public float beadAppliedHealth;
        [DataMember(Name = "bas")]
        public float beadAppliedShield;
        [DataMember(Name = "bar")]
        public float beadAppliedRegen;
        [DataMember(Name = "bad")]
        public float beadAppliedDamage;
        [DataMember(Name = "i")]
        public List<ItemData> items;

        [DataMember(Name = "e")]
        public EquipmentData[] equipments;
        [DataMember(Name = "aes")]
        public byte activeEquipmentSlot;

        public InventoryData(Inventory inventory)
        {
            infusionBonus = inventory.infusionBonus;
            equipmentDisabled = inventory.equipmentDisabled;
            beadAppliedDamage = inventory.beadAppliedDamage;
            beadAppliedHealth = inventory.beadAppliedHealth;
            beadAppliedRegen = inventory.beadAppliedRegen;
            beadAppliedShield = inventory.beadAppliedShield;

            items = new List<ItemData>();
            foreach (var item in inventory.itemAcquisitionOrder)
            {
                items.Add(new ItemData { itemIndex = (int)item, count = inventory.GetItemCount(item) });
            }

            equipments = new EquipmentData[inventory.GetEquipmentSlotCount()];
            for (var i = 0; i < equipments.Length; i++)
            {
                equipments[i] = new EquipmentData(inventory.GetEquipment((uint)i));
            }
            activeEquipmentSlot = inventory.activeEquipmentSlot;
        }

        public void LoadInventory(Inventory inventory)
        {
            inventory.beadAppliedShield = beadAppliedShield;
            inventory.beadAppliedRegen = beadAppliedRegen;
            inventory.beadAppliedHealth = beadAppliedHealth;
            inventory.beadAppliedDamage = beadAppliedDamage;
            inventory.equipmentDisabled = equipmentDisabled;

            inventory.itemAcquisitionOrder.Clear();
            foreach (var item in items)
            {
                inventory.itemStacks[item.itemIndex] = item.count;
                inventory.itemAcquisitionOrder.Add((ItemIndex)item.itemIndex);
            }

            inventory.HandleInventoryChanged();

            for (byte i = 0; i < equipments.Length; i++)
            {
                equipments[i].LoadEquipment(inventory, i);
            }
            inventory.SetActiveEquipmentSlot(activeEquipmentSlot);

            inventory.AddInfusionBonus(infusionBonus);
        }
    }
}
