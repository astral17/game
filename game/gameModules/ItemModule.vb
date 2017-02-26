Public Module ItemModule
    <Serializable()>
    Public Structure ItemObject
        Public id, count, x, y, tex As Integer
        Public world As String
        Public Sub New(id As Integer, x As Integer, y As Integer, count As Integer, tex As Integer, world As String)
            Me.id = id
            Me.count = count
            Me.x = x
            Me.y = y
            Me.tex = tex
            Me.world = world
        End Sub
    End Structure
    <Serializable()>
    Public Structure ItemInfo
        Public name, type, desc, world As String
        Public tex, effectID, dureff, poweff As Integer
        Public isUsable As Boolean
        Public Sub New(ByVal name As String, type As String, desc As String, tex As Integer, effectID As Integer, dureff As Integer, poweff As Integer, isUsable As Boolean)
            Me.name = name
            Me.type = type
            Me.desc = desc
            Me.tex = tex
            Me.effectID = effectID
            Me.dureff = dureff
            Me.poweff = poweff
            Me.isUsable = isUsable
        End Sub
    End Structure
    <Serializable()>
    Public Class ItemClass
        Public itemsInfo As New List(Of ItemInfo)
        Public items As New List(Of ItemObject)
        Public Sub init()
            Dim Xd As XDocument = XDocument.Load("data/items.xml")
            'MessageBox.Show(Xd.Element("map").Element("test").Elements.Count)
            'item[name,type,desc,tex,eff[dureff,poweff,usable]]
            'Dim i As Integer = -1
            For Each xe As XElement In Xd.Elements("items").Elements
                'i = i + 1
                Dim effID As Integer = xe.Element("effectID")
                Dim dureff As Integer = 0
                Dim poweff As Integer = 0
                Dim isUsable As Boolean
                effID = xe.Element("effectID")
                If effID <> "0" Then
                    dureff = xe.Element("duration")
                    poweff = xe.Element("power")
                    Dim t As Integer
                    t = xe.Element("usable")
                    If t = 0 Then
                        isUsable = False
                    Else
                        isUsable = True
                    End If
                Else
                    dureff = 0
                    poweff = 0
                    isUsable = True
                End If
                itemsInfo.Add(New ItemInfo(xe.Element("name"), xe.Element("type"), xe.Element("description"), xe.Element("texture"), effID, dureff, poweff, isUsable))
            Next
        End Sub
        Public Sub spawnitem(ByVal x As Integer, y As Integer, id As Integer, counti As Integer)
            items.Add(New ItemObject(id, x, y, counti, itemsInfo(id).tex, Player.curWorld))
        End Sub
    End Class
End Module
