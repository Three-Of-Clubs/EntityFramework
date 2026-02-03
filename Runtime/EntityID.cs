using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EntityFramework
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct EntityID
    {
        [SerializeField] uint idEntity;
        [SerializeField] ushort idScene;
        [SerializeField] ushort flags;

        public EntityID(uint idEntity, ushort idScene, ushort flags)
        {
            this.idEntity = idEntity;
            this.idScene = idScene;
            this.flags = flags;
        }

        public static EntityID InvalidID { get; } = new EntityID { flags = (ushort)EntityFlags.Invalid };
        public bool IsValid => (flags & (ushort)EntityFlags.Invalid) == 0;

        public uint IDEntity => idEntity;
        public ushort IDScene => idScene;
        public EntityFlags Flags => (EntityFlags)flags;

		public EntityID(ushort sceneID, uint entityID, EntityFlags flags)
		{
            idScene = sceneID;
            idEntity = entityID;
            this.flags = (ushort)flags;
        }
	}

    [Flags]
    public enum EntityFlags : ushort
	{
        Dynamic = 1 << 0,

        IsGameObject = 1 << 1,
        IsPrefab = 1 << 2,

        Unused3 = 1 << 3,
        Unused4 = 1 << 4,
        Unused5 = 1 << 5,
        Unused6 = 1 << 6,
        Unused7 = 1 << 7,
        Unused8 = 1 << 8,
        Unused9 = 1 << 9,
        UnusedA = 1 << 10,
        UnusedB = 1 << 11,
        UnusedC = 1 << 12,
        UnusedD = 1 << 13,
        UnusedE = 1 << 14,
        
        Invalid = 1 << 15,
    }
}
