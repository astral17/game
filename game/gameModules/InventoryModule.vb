Public Module InventoryModule
    <Serializable()>
    Public Structure InventorySlot
        Public id, count, tex As Integer
        Public Sub New(ByVal id As Integer, count As Integer, tex As Integer)
            Me.id = id
            Me.count = count
            Me.tex = tex
        End Sub
    End Structure
    <Serializable()>
    Public Class InventoryClass
        Public slot As New List(Of InventorySlot)
        Public selected As Integer = -1
        'Public desc As String = ""
        Public page As Integer = 0
        Public Sub additem(ByVal id As Integer, countitem As Integer)
            If id = 2 Then
                Player.money += countitem
                Return
            End If

            Dim i As Integer = slot.FindIndex(Function(x) x.id = id)
            If i <> -1 Then
                Dim x = slot(i)
                x.count += countitem
                slot(i) = x
            Else
                slot.Add(New InventorySlot(id, countitem, 1))
                'If Not (Item.itemsInfo(slot(slot.Count - 1).id).isUsable) Then
                '    useItem(slot.Count - 1)
                'End If
            End If
        End Sub
        Public Sub subItem(ByVal id As Integer, itemCount As Integer)
            Dim i As Integer = slot.FindIndex(Function(x) x.id = id)
            If i <> -1 Then
                If slot(i).count > itemCount Then
                    Dim x As InventorySlot = slot(i)
                    x.count -= itemCount
                    slot(i) = x
                Else
                    'If Not Item.itemsInfo(slot(i).id).isUsable Then
                    '    Dim idEff = Item.itemsInfo(slot(i).id).effectID
                    '    Dim poweff = Item.itemsInfo(slot(i).id).poweff
                    '    Effect.remove(idEff, poweff)
                    'End If
                    slot.RemoveAt(i)
                End If
            End If
        End Sub
        Public Sub subItemFromSlot(ByVal num As Integer, countitem As Integer)
            If (num <> -1) And (num < slot.Count) Then
                If slot(num).count > countitem Then
                    Dim x As InventorySlot = slot(num)
                    x.count -= countitem
                    slot(num) = x
                Else
                    slot.RemoveAt(num)
                End If
            End If
        End Sub
        Public Function useItem(ByVal slotNum As Integer) As Boolean
            If slotNum > slot.Count Then
                Return False
            End If
            Return Item.useItem(slot(slotNum))
            'If slot(slotNum).count < 1 Then
            '    Return False
            'End If
            'Dim idEff = Item.itemsInfo(slot(slotNum).id).effectID
            'If idEff = 0 Then
            '    Return False
            'End If
            'Dim dureff = Item.itemsInfo(slot(slotNum).id).dureff
            'Dim poweff = Item.itemsInfo(slot(slotNum).id).poweff
            'Dim isUsable = Item.itemsInfo(slot(slotNum).id).isUsable
            'Effect.add(idEff, dureff, poweff, idEff, isUsable)
            'Return True
        End Function
        Public Function getSlot(ByVal n As Integer) As InventorySlot
            Return slot(n)
        End Function
        Public Function searchItem(ByVal id As Integer)
            Return slot.FindIndex(Function(x) x.id = id)
        End Function
    End Class
End Module
