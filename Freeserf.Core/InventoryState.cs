﻿using System;

namespace Freeserf
{
    using Serialize;
    using word = UInt16;
    using dword = UInt32;
    using ResourceMap = Serialize.DirtyArrayWithEnumIndex<Resource.Type, UInt32>;
    using SerfMap = Serialize.DirtyArrayWithEnumIndex<Serf.Type, UInt32>;

    internal class InventoryState : State
    {
        private byte player = 0;
        private word flag = 0;
        private word building = 0;
        private dword genericCount = 0;
        private byte resourceDir = 0;

        public InventoryState()
        {
            Resources.GotDirty += (object sender, EventArgs args) => { MarkPropertyAsDirty(nameof(Resources)); };
            Serfs.GotDirty += (object sender, EventArgs args) => { MarkPropertyAsDirty(nameof(Serfs)); };
            OutQueue.GotDirty += (object sender, EventArgs args) => { MarkPropertyAsDirty(nameof(OutQueue)); };
        }

        public override void ResetDirtyFlag()
        {
            lock (dirtyLock)
            {
                Resources.ResetDirtyFlag();
                Serfs.ResetDirtyFlag();
                OutQueue.ResetDirtyFlag();

                ResetDirtyFlagUnlocked();
            }
        }

        /// <summary>
        /// Owner of this inventory
        /// </summary>
        [Data]
        public byte Player
        {
            get => player;
            set
            {
                if (player != value)
                {
                    player = value;
                    MarkPropertyAsDirty(nameof(Player));
                }
            }
        }

        /// <summary>
        /// Index of flag connected to this inventory
        /// </summary>
        [Data]
        public word Flag
        {
            get => flag;
            set
            {
                if (flag != value)
                {
                    flag = value;
                    MarkPropertyAsDirty(nameof(Flag));
                }
            }
        }

        /// <summary>
        /// Index of building containing this inventory
        /// </summary>
        [Data]
        public word Building
        {
            get => building;
            set
            {
                if (building != value)
                {
                    building = value;
                    MarkPropertyAsDirty(nameof(Building));
                }
            }
        }

        /// <summary>
        /// Count of generic serfs
        /// </summary>
        [Data]
        public dword GenericCount
        {
            get => genericCount;
            set
            {
                if (genericCount != value)
                {
                    genericCount = value;
                    MarkPropertyAsDirty(nameof(GenericCount));
                }
            }
        }

        /// <summary>
        /// Count of resources
        /// </summary>
        [Data]
        public ResourceMap Resources { get; } = new ResourceMap((int)Resource.Type.MaxValueWithoutFoodGroup + 1);
        /// <summary>
        /// Indices to serfs of each type
        /// </summary>
        [Data]
        public SerfMap Serfs { get; } = new SerfMap((int)Serf.Type.MaxValue + 1);
        /// <summary>
        /// Resources waiting to be moved out
        /// </summary>
        [Data]
        public DirtyArray<Inventory.OutQueue> OutQueue { get; } = new DirtyArray<Inventory.OutQueue> // TODO: stock changes have to make the array dirty
        (
            new Inventory.OutQueue(), new Inventory.OutQueue()
        );

        /// <summary>
        /// Directions for resources and serfs
        /// Bit 0-1: Resource direction
        /// Bit 2-3: Serf direction
        /// </summary>
        [Data]
        public byte ResourceDirection
        {
            get => resourceDir;
            set
            {
                if (resourceDir != value)
                {
                    resourceDir = value;
                    MarkPropertyAsDirty(nameof(ResourceDirection));
                }
            }
        }

        public Inventory.Mode ResourceMode
        {
            get => (Inventory.Mode)(ResourceDirection & 0x03);
            set => ResourceDirection = (byte)((ResourceDirection & 0xFC) | (byte)value);
        }

        public Inventory.Mode SerfMode
        {
            get => (Inventory.Mode)((ResourceDirection >> 2) & 0x03);
            set => ResourceDirection = (byte)((ResourceDirection & 0xF3) | ((byte)value << 2));
        }

        public bool HasAnyOutMode()
        {
            return (ResourceDirection & 0x0A) != 0;
        }
    }
}
