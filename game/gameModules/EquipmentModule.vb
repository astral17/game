Public Module EquipmentModule
    <Serializable()>
    Public Structure EquipmentSlot

    End Structure
    <Serializable()>
    Public Class EquipmentClass
        Public equipment As New List(Of InventorySlot)
        Public equipSlot As New List(Of Rectangle)
        Public selected As Integer = -1
        Public Sub init()
            For i As Integer = 0 To EquipmentSlots.Last
                equipment.Add(New InventorySlot(-1, 0, -1))
            Next
            equipSlot.Add(New Rectangle(330, 90, 50, 50))
            equipSlot.Add(New Rectangle(210, 90, 50, 50))
            equipSlot.Add(New Rectangle(270, 30, 50, 50))
            equipSlot.Add(New Rectangle(270, 90, 50, 50))
            equipSlot.Add(New Rectangle(270, 150, 50, 50))
            equipSlot.Add(New Rectangle(270, 210, 50, 50))
        End Sub
        Public Sub Equip(EquipItem As InventorySlot) 'ByVal slot As EquipmentSlots, 
            Dim slot As Integer = Item.itemsInfo(EquipItem.id).wearPlace
            If equipment(slot).count <> 0 Then
                'Inv.additem(equipment(slot).id, equipment(slot).count)
                UnEquip(slot)
            End If
            equipment(slot) = EquipItem
            If Not (Item.itemsInfo(EquipItem.id).isUsable) Then
                Item.useItem(EquipItem)
            End If
            Inv.subItem(EquipItem.id, EquipItem.count)
        End Sub
        Public Sub UnEquip(ByVal slot As EquipmentSlots)
            If equipment(slot).count <> 0 Then
                If Not Item.itemsInfo(equipment(slot).id).isUsable Then
                    Dim idEff = Item.itemsInfo(equipment(slot).id).effectID
                    Dim poweff = Item.itemsInfo(equipment(slot).id).poweff
                    Effect.remove(idEff, poweff)
                End If
                Inv.additem(equipment(slot).id, equipment(slot).count)
                equipment(slot) = New InventorySlot(-1, 0, -1)
            End If
        End Sub
    End Class
End Module
