Public Module EnumModule
    Public Enum Directions
        Up = 1
        Down = 2
        Left = 3
        Right = 4
    End Enum
    Public Enum BlockType
        Grass = 0
        Block = 1
        DeepWater = 2
        Bridge = 3
        Water = 4
        Lava = 5
        DarkGrass = 6
        CobbleStone = 7
        StoneWall = 8
        NetherRock = 9
    End Enum
    Public Enum ListsEnum
        inventory = 0
        equipment = 1
        shop = 2
        chest = 3
    End Enum
    Public Enum EquipmentSlots
        MainHand = 0
        OffHand = 1
        Head = 2
        Chest = 3
        Legs = 4
        Feet = 5
        Last = 5
    End Enum
End Module
