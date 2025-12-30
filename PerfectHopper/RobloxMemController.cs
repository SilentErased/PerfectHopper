using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SilentExternal
{
    class RobloxMemController
    {

        public static UIntPtr GetDataModel()
        {
            Memory.Process Roblox = new Memory.Process("RobloxPlayerBeta");
            UIntPtr FakeDataModel = Roblox.ReadAddress<UIntPtr>((long)Roblox.GetBaseAddress() + Offsets.FakeDataModelPointer);
            UIntPtr RealDataModel = Roblox.ReadAddress<UIntPtr>((long)FakeDataModel + Offsets.FakeDataModelToDataModel);
            return RealDataModel;
        }

        public static int PlaceId()
        {
            Memory.Process Roblox = new Memory.Process("RobloxPlayerBeta");
            return Roblox.ReadAddress<int>((long)RobloxMemController.GetDataModel() + Offsets.PlaceId);
        }

        public static UIntPtr GetLocalPlayerByPlayers(UIntPtr Address)
        {
            Memory.Process Roblox = new Memory.Process("RobloxPlayerBeta");
            return Roblox.ReadAddress<UIntPtr>((long)Address + Offsets.LocalPlayer);
        }
    }

    class RobloxObject
    {
        public Memory.Process Roblox;
        public UIntPtr Address;

        public RobloxObject(UIntPtr AddressInput)
        {
            Roblox = new Memory.Process("RobloxPlayerBeta");
            Address = AddressInput;
        }

        public string ClassName
        {
            get
            {
                Memory.Process Roblox = new Memory.Process("RobloxPlayerBeta");
                string ClassNameStr;
                UIntPtr ClassDescription = Roblox.ReadAddress<UIntPtr>((long)Address + Offsets.ClassDescriptor);
                UIntPtr ClassNamePtr = Roblox.ReadAddress<UIntPtr>((long)ClassDescription + Offsets.ClassDescriptorToClassName);
                ClassNameStr = Roblox.ReadString((long)ClassNamePtr, 16);
                return ClassNameStr;
            }
        }
        public List<RobloxObject> GetChildren()
        {
            List<RobloxObject> ChildrenList = new List<RobloxObject>();

            if (Address == UIntPtr.Zero || Address.ToUInt64() < 0x10000)
            {
                return ChildrenList;
            }

            ulong ChildrenStart = Roblox.ReadAddress<ulong>((long)Address + Offsets.Children);

            if (ChildrenStart == 0 || ChildrenStart < 0x10000)
            {
                return ChildrenList;
            }

            ulong ChildrenEnd = Roblox.ReadAddress<ulong>((long)ChildrenStart + Offsets.ChildrenEnd);

            for (ulong i = Roblox.ReadAddress<ulong>((long)ChildrenStart); i != ChildrenEnd; i += 0x10)
            {
                ulong ChildAddress = Roblox.ReadAddress<ulong>((long)i);

                if (ChildAddress != 0 && ChildAddress > 0x10000)
                {
                    ChildrenList.Add(new RobloxObject((UIntPtr)ChildAddress));
                }
            }

            return ChildrenList;
        }

        public string Name
        {
            get
            {
                long NamePtr = Roblox.ReadAddress<long>((long)Address + Offsets.Name);
                string NameStr;
                if (Roblox.ReadAddress<int>(NamePtr + 0x10) >= 16)
                {
                    long NamePtr2 = Roblox.ReadAddress<long>(NamePtr);
                    var Sb = new StringBuilder();
                    while (Roblox.ReadAddress<byte>(NamePtr2) != 0)
                    {
                        Sb.Append((char)Roblox.ReadAddress<byte>(NamePtr2));
                        NamePtr2++;
                    }
                    NameStr = Sb.ToString();
                }
                else
                {
                    NameStr = Roblox.ReadString(NamePtr, 256);
                }
                return NameStr;
            }
        }

        public UIntPtr FindFirstChild(string ChildName)
        {
            foreach (RobloxObject ChildObj in GetChildren())
            {
                try
                {
                    if (ChildObj.Name.Contains(ChildName))
                    {
                        return ChildObj.Address;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return UIntPtr.Zero;
        }
    }
}