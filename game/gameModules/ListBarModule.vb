Public Module ListBarModule
    Public Structure listStruct
        Public tex As Integer
        Public text As String
        Public isActive As Boolean
        Public Sub New(ByVal tex As Integer, text As String, isActive As Boolean)
            Me.tex = tex
            Me.text = text
            Me.isActive = isActive
        End Sub
    End Structure
    Public Class listBarClass
        Public selected As Integer = ListsEnum.inventory
        Public activeCount As Integer = 0
        Public desc As String = ""
        Public open As Boolean = False
        Public lists As New List(Of listStruct)
        Public Sub init()
            lists.Add(New listStruct(1, "inventory", True))
            lists.Add(New listStruct(1, "equipment", True))
            lists.Add(New listStruct(1, "shop", False))
            lists.Add(New listStruct(1, "chest", False))
            activeCount = 2
        End Sub
        Public Sub activate(ByVal num As ListsEnum)
            Dim x As listStruct
            If Not lists(num).isActive Then
                activeCount += 1
            End If
            x = lists(num)
            x.isActive = True
            lists(num) = x
        End Sub
        Public Sub deactivate(ByVal num As ListsEnum)
            Dim x As listStruct
            If lists(num).isActive Then
                activeCount -= 1
            End If
            x = lists(num)
            x.isActive = False
            lists(num) = x
        End Sub
    End Class
End Module
