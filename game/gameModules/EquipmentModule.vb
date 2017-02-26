Public Module EquipmentModule
    Public Structure EquipmentSlot

    End Structure
    Public Class EquipmentClass
        Public equipment As New List(Of InventorySlot)
        Public Sub init()
            For i As Integer = 0 To EquipmentSlots.Last
                equipment.Add(New InventorySlot(-1, 0, -1))
            Next
        End Sub
        Public Sub Equip(ByVal slot As EquipmentSlots, item As InventorySlot)
            If equipment(slot).count <> 0 Then
                Inv.additem(equipment(slot).id, equipment(slot).count)
            End If
            equipment(slot) = item
        End Sub
        Public Sub UnEquip(ByVal slot As EquipmentSlots)
            If equipment(slot).count <> 0 Then
                Inv.additem(equipment(slot).id, equipment(slot).count)
            End If
        End Sub
    End Class
End Module
