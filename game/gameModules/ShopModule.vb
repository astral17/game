Public Module ShopModule
    <Serializable()>
    Public Structure shopItem
        Public id, count, cost As Integer
        Public Sub New(ByVal id As Integer, count As Integer, cost As Integer)
            Me.id = id
            Me.count = count
            Me.cost = cost
        End Sub
    End Structure
    <Serializable()>
    Public Class ShopClass
        Public count As Integer = -1
        Public p(99) As Point
        Public tex(99) As Integer
        Public items(99, 99) As shopItem
        Public world(99) As String
        Public itemsCount(99) As Integer
        Public selectedItem As Integer = -1
        Public selectedShop As Integer = -1
        Public desc As String = "LBM - buy" + vbNewLine + "RBM - sell"
        Public page As Integer = 0
        Public Function create(ByVal x As Integer, y As Integer, tex1 As Integer)
            count += 1
            p(count).X = x
            p(count).Y = y
            tex(count) = tex1
            world(count) = Player.curWorld
            itemsCount(count) = -1
            Return count
        End Function
        Public Sub addItem(ByVal num As Integer, item As shopItem)
            itemsCount(num) += 1
            items(num, itemsCount(num)) = item
        End Sub
        Public Sub buyItem(ByVal shopNum As Integer, itemID As Integer, itemCount As Integer)
            With items(shopNum, itemID)
                If ((.count <> -1 And .count < itemCount) Or Player.money < .cost * itemCount) Then
                    Return
                End If
                If .count <> -1 Then
                    .count -= itemCount
                End If
                Player.money -= .cost * itemCount
                Inv.additem(.id, itemCount)
            End With
        End Sub
        Public Sub sellItem(ByVal shopNum As Integer, itemID As Integer, itemCount As Integer)
            With items(shopNum, itemID)
                If Inv.searchItem(.id) < itemCount Then
                    Return
                End If
                If .count <> -1 Then
                    .count += itemCount
                End If
                Player.money += .cost * itemCount
                Inv.subItem(.id, itemCount)
            End With
        End Sub
    End Class
End Module
