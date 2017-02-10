Public Class Form1
    Public version = "1.0.0pre-2"
    Public sX, sY, wX, wY, wordscount As Integer
    Public kUp, kDown, kLeft, kRight As Boolean
    Public godMode As Boolean = False
    Public str As String
    Public tileset(99) As Bitmap
    Public itemtex(99) As Bitmap
    Public effecttex(99) As Bitmap
    Public words(99) As String
    Dim map(99, 99) As BlockType
    Public mapStr(99) As String
    Public items As New item
    Public enem As New enemy
    Public inv As New inventory
    Public encount As Integer
    Public doors As New Door
    Public plate As New plates
    Public loader As New Loading
    Public player As New Players
    Public effect As New Effects
    Public shops As New Shop
    Public listBar As New listBarClass

    Enum BlockType
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
    Enum lists
        inventory = 0
        equipment = 1
        shop = 2
        chest = 3
    End Enum

    <Serializable()>
    Public Class Players
        Public version As String = "noInited"
        Public x, y As Integer
        Public stepCD As Integer
        Public money As Integer = 0
        Public gamestate As String = "normal"
        Public curWorld As String = "main"
        Public Function testVersion() As Boolean
            If Form1.version = version Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    <Serializable()>
    Public Structure doorStruct
        Public x, y, id, tex As Integer
        Public world As String
        Public Sub New(ByVal x1 As Integer, y1 As Integer, id1 As Integer, tex1 As Integer, world1 As String)
            x = x1
            y = y1
            id = id1
            tex = tex1
            world = world1
        End Sub
    End Structure
    <Serializable()>
    Public Class Door
        Public d As New List(Of doorStruct)
        Public Function CanStep(ByVal dir As Integer) As Boolean
            Dim x1, y1 As Integer
            x1 = Form1.player.x
            y1 = Form1.player.y
            If dir = 1 Then
                y1 -= 1
            End If
            If dir = 2 Then
                y1 += 1
            End If
            If dir = 3 Then
                x1 -= 1
            End If
            If dir = 4 Then
                x1 += 1
            End If
            'System.Console.WriteLine(d.Contains(New doorStruct(x1, y1, vbNull, vbNull, "main")))
            'System.Console.WriteLine(d.FindIndex(Function(x) (x.x = x1) And (x.y = y1) And (x.world = Form1.player.curWorld)))

            Dim tlist = d.FindAll(Function(x) (x.x = x1) And (x.y = y1) And (x.world = Form1.player.curWorld))
            For Each x As doorStruct In tlist
                If (Form1.inv.searchItem(8) <> -1) Then 'masterkey
                    d.Remove(x)
                    Return True
                End If
                If (Form1.inv.searchItem(x.id) = -1) Then
                    Return False
                Else
                    d.Remove(x)
                    Form1.inv.subItem(x.id, 1)
                    Return True
                End If
            Next
            'For i As Integer = 0 To d.Count - 1
            '    If (d(i).x = x1 And d(i).y = y1 And d(i).world = Form1.player.curWorld) Then
            '        If (Form1.inv.searchItem(8) <> -1) Then 'masterkey
            '            d.RemoveAt(i)
            '            Return True
            '        End If
            '        If (Form1.inv.searchItem(d(i).id) = -1) Then
            '            Return False
            '        Else
            '            d.RemoveAt(i)
            '            Form1.inv.subItem(d(i).id, 1)
            '            Return True
            '        End If
            '    End If
            'Next
            Return True
        End Function
        Public Sub create(ByVal x As Integer, y As Integer, vid As Integer, vtex As Integer)
            d.Add(New doorStruct(x, y, vid, vtex, Form1.player.curWorld))
        End Sub
    End Class
    <Serializable()>
    Public Structure itemobj
        Public id, count, x, y, tex As Integer
        Public world As String
        Public Sub New(id1 As Integer, x1 As Integer, y1 As Integer, count1 As Integer, tex1 As Integer, world1 As String)
            id = id1
            count = count1
            x = x1
            y = y1
            tex = tex1
            world = world1
        End Sub
    End Structure
    <Serializable()>
    Public Class item
        Public name As New List(Of String)
        Public type As New List(Of String)
        Public desc As New List(Of String)
        Public tex As New List(Of Integer)
        Public eff As New List(Of Integer)
        Public dureff As New List(Of Integer)
        Public poweff As New List(Of Integer)
        Public isUsable As New List(Of Boolean)
        Public world As New List(Of String)
        Public item As New List(Of itemobj)
        'Public count As Integer = -1
        Public Sub init()
            Dim Xd As XDocument = XDocument.Load("data/items.xml")
            'MessageBox.Show(Xd.Element("map").Element("test").Elements.Count)
            'item[name,type,desc,tex,eff[dureff,poweff,usable]]
            'Dim i As Integer = -1
            For Each xe As XElement In Xd.Elements("items").Elements
                'i = i + 1
                name.Add(xe.Element("name"))
                type.Add(xe.Element("type"))
                desc.Add(xe.Element("description"))
                tex.Add(xe.Element("texture"))
                eff.Add(xe.Element("effectID"))
                If xe.Element("effectID") <> "0" Then
                    dureff.Add(xe.Element("duration"))
                    poweff.Add(xe.Element("power"))
                    Dim t As Integer
                    t = xe.Element("usable")
                    If t = 0 Then
                        isUsable.Add(False)
                    Else
                        isUsable.Add(True)
                    End If
                Else
                    dureff.Add(0)
                    poweff.Add(0)
                    isUsable.Add(True)
                End If
            Next
        End Sub
        Public Sub spawnitem(ByVal x As Integer, y As Integer, id As Integer, counti As Integer)
            item.Add(New itemobj(id, x, y, counti, tex(id), Form1.player.curWorld))
        End Sub
    End Class
    Public Structure listStruct
        Public tex As Integer
        Public text As String
        Public isActive As Boolean
        Public Sub New(ByVal tex1 As Integer, text1 As String, isActive1 As Boolean)
            tex = tex1
            text = text1
            isActive = isActive1
        End Sub
    End Structure
    Public Class listBarClass
        Public selected As Integer = lists.inventory
        Public activeCount As Integer = 0
        Public open As Boolean = False
        Public e As New List(Of listStruct)
        Public Sub init()
            e.Add(New listStruct(1, "inventory", True))
            e.Add(New listStruct(1, "equipment", True))
            e.Add(New listStruct(1, "shop", False))
            e.Add(New listStruct(1, "chest", False))
            activeCount = 2
        End Sub
        Public Sub activate(ByVal num As lists)
            Dim x As listStruct
            If Not e(num).isActive Then
                activeCount += 1
            End If
            x = e(num)
            x.isActive = True
            e(num) = x
        End Sub
        Public Sub deactivate(ByVal num As lists)
            Dim x As listStruct
            If e(num).isActive Then
                activeCount -= 1
            End If
            x = e(num)
            x.isActive = False
            e(num) = x
        End Sub
    End Class
    <Serializable()>
    Public Structure slots
        Public id, count, tex As Integer
        Public Sub New(ByVal id1 As Integer, count1 As Integer, tex1 As Integer)
            id = id1
            count = count1
            tex = tex1
        End Sub
    End Structure
    <Serializable()>
    Public Class inventory
        Public slot As New List(Of slots)
        Public selected As Integer = -1
        Public desc As String = ""
        Public page As Integer = 0
        Public Sub additem(ByVal id As Integer, countitem As Integer)
            If id = 2 Then
                Form1.player.money += countitem
                Return
            End If

            Dim i As Integer = slot.FindIndex(Function(x) x.id = id)
            If i <> -1 Then
                Dim x = slot(i)
                x.count += countitem
                slot(i) = x
            Else
                slot.Add(New slots(id, countitem, 1))
                If Not (Form1.items.isUsable(slot(slot.Count - 1).id)) Then
                    useItem(slot.Count - 1)
                End If
            End If
        End Sub
        Public Sub subItem(ByVal id As Integer, itemCount As Integer)
            Dim i As Integer = slot.FindIndex(Function(x) x.id = id)
            If i <> -1 Then
                If slot(i).count > itemCount Then
                    Dim x As slots = slot(i)
                    x.count -= itemCount
                    slot(i) = x
                Else
                    If Not Form1.items.isUsable(slot(i).id) Then
                        Dim idEff = Form1.items.eff(slot(i).id)
                        Dim poweff = Form1.items.poweff(slot(i).id)
                        Form1.effect.remove(idEff, poweff)
                    End If
                    slot.RemoveAt(i)
                End If
            End If
        End Sub
        Public Sub subItemFromSlot(ByVal num As Integer, countitem As Integer)
            If num <> -1 Then
                If slot(num).count > countitem Then
                    Dim x As slots = slot(num)
                    x.count -= countitem
                    slot(num) = x
                Else
                    slot.RemoveAt(num)
                End If
            End If
        End Sub
        Public Function useItem(ByVal slotNum As Integer) As Boolean
            If slot(slotNum).count < 1 Then
                Return False
            End If
            Dim idEff = Form1.items.eff(slot(slotNum).id)
            If idEff = 0 Then
                Return False
            End If
            Dim dureff = Form1.items.dureff(slot(slotNum).id)
            Dim poweff = Form1.items.poweff(slot(slotNum).id)
            Dim isUsable = Form1.items.isUsable(slot(slotNum).id)
            Form1.effect.add(idEff, dureff, poweff, idEff, isUsable)
            Return True
        End Function
        Public Function getSlot(ByVal n As Integer) As slots
            Return slot(n)
        End Function
        Public Function searchItem(ByVal id As Integer)
            Dim i As Integer = slot.FindIndex(Function(x) x.id = id)

            Return i
        End Function
    End Class
    <Serializable()>
    Public Structure shopItem
        Public id, count, cost As Integer
        Public Sub New(ByVal id1 As Integer, count1 As Integer, cost1 As Integer)
            id = id1
            count = count1
            cost = cost1
        End Sub
    End Structure
    <Serializable()>
    Public Class Shop
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
            world(count) = Form1.player.curWorld
            itemsCount(count) = -1
            Return count
        End Function
        Public Sub addItem(ByVal num As Integer, item As shopItem)
            itemsCount(num) += 1
            items(num, itemsCount(num)) = item
        End Sub
        Public Sub buyItem(ByVal shopNum As Integer, itemID As Integer, itemCount As Integer)
            With items(shopNum, itemID)
                If ((.count <> -1 And .count < itemCount) Or Form1.player.money < .cost * itemCount) Then
                    Return
                End If
                If .count <> -1 Then
                    .count -= itemCount
                End If
                Form1.player.money -= .cost * itemCount
                Form1.inv.additem(.id, itemCount)
            End With
        End Sub
        Public Sub sellItem(ByVal shopNum As Integer, itemID As Integer, itemCount As Integer)
            With items(shopNum, itemID)
                If Form1.inv.searchItem(.id) < itemCount Then
                    Return
                End If
                If .count <> -1 Then
                    .count += itemCount
                End If
                Form1.player.money += .cost * itemCount
                Form1.inv.subItem(.id, itemCount)
            End With
        End Sub
    End Class
    <Serializable()>
    Public Structure effectStruct
        Public id, dur, pow, tex As Integer
        Public isTemp As Boolean
        Public Sub New(ByVal id1 As Integer, dur1 As Integer, pow1 As Integer, tex1 As Integer, isTemp1 As Boolean)
            id = id1
            dur = dur1
            pow = pow1
            tex = tex1
            isTemp = isTemp1
        End Sub
    End Structure
    <Serializable()>
    Public Class Effects  'id dur pow temp
        Public e As New List(Of effectStruct)
        Public desc As New List(Of String)
        Public selected As Integer = -1
        Public Sub init()
            Dim Xd As XDocument = XDocument.Load("data/effects.xml")
            For Each xe As XElement In Xd.Elements("effects").Elements
                desc.Add(xe.Value)
            Next
        End Sub
        Public Sub add(ByVal id1 As Integer, dur1 As Integer, pow1 As Integer, tex1 As Integer, Optional ByVal isTemp1 As Boolean = True)
            e.Add(New effectStruct(id1, dur1, pow1, tex1, isTemp1))
        End Sub
        Public Sub remove(ByVal id1 As Integer, Optional ByVal pow1 As Integer = -1)
            Dim i As Integer = e.FindIndex(Function(x) (x.id = id1) And ((x.pow = pow1) Or (pow1 = -1)))
            If selected >= i Then
                selected = -1
            End If
            If i <> -1 Then
                e.RemoveAt(i)
            End If
        End Sub
        Public Sub removeByNum(ByVal num As Integer)
            If selected >= num Then
                selected = -1
            End If
            If num <> -1 Then
                e.RemoveAt(num)
            End If
        End Sub
        Public Function search(ByVal id1 As Integer) As Integer
            Dim max As Integer = -1
            For i As Integer = 0 To e.Count - 1
                If e(i).id = id1 Then
                    max = Math.Max(e(i).pow, max)
                End If
            Next
            Return max
        End Function
        Public Sub upTime()
            If e.Count <> 0 Then
                'For Each x As effectStruct In e
                'remove KOSTIL!!
                For i As Integer = 0 To e.Count - 1
                    If e(i).isTemp Then
                        'Dim i = e.FindIndex(Function(zn) (zn.id = x.id) And (zn.dur = x.dur) And (zn.pow = x.pow) And (zn.isTemp = x.isTemp) And (zn.tex = x.tex))
                        Dim x As effectStruct = e(i)
                        x.dur -= 1
                        e(i) = x
                        'e(i).dur -= 1
                        'e(i) = x
                    End If
                    'Case 1 undead обработка там где лава
                    'Case 2 incAttack
                    'Case 3 reincarnation
                    If e(i).id = 3 Then
                        Form1.player.stepCD = 0
                        add(1, 300, 1, 3, True)
                    End If
                    'Case 4 waterbreath
                Next
            End If
            Dim offset As Integer = 0
            Dim ocount = e.Count
            For i As Integer = 0 To ocount - 1
                If e(i - offset).dur < 1 Then
                    removeByNum(i - offset)
                    offset += 1
                End If
            Next
        End Sub
    End Class
    <Serializable()>
    Public Structure action
        Public id, val, val2 As Integer
        Public valS As String
        Public Sub New(ByVal id1 As Integer, val1 As Integer, val21 As Integer, valS1 As String)
            id = id1
            val = val1
            val2 = val21
            valS = valS1
        End Sub
    End Structure
    <Serializable()>
    Public Structure plateStruct
        Public x, y, x2, y2 As Integer
        Public ac As action
        Public world As String
        Public Sub New(ByVal x1 As Integer, y1 As Integer, x21 As Integer, y21 As Integer, world1 As String, ac1 As action)
            x = x1
            y = y1
            x2 = x21
            y2 = y21
            world = world1
            ac = ac1
        End Sub
    End Structure
    <Serializable()>
    Public Class plates
        Public e As New List(Of plateStruct)
        Public update As Boolean = True
        Public Sub runPlate(ByVal n As Integer)
            If (e(n).ac.id = 0) And (e(n).ac.val = 0) Then
                Form1.unpressarrows()
                MessageBox.Show("testbox")
            End If

            Select Case e(n).ac.id
                Case 0 'remove messageBox with val id
                    e.RemoveAt(n)
                Case 1 'set step chance
                    Form1.enem.stepChance = e(n).ac.val
                Case 2 'set step difficult
                    Form1.enem.stepDif = e(n).ac.val
                Case 3 'teleport to val world
                    Dim tmp1, tmp2 As Integer
                    tmp1 = e(n).ac.val
                    tmp2 = e(n).ac.val2
                    If tmp1 = -2 Then
                        tmp1 = Form1.player.x
                    End If
                    If tmp1 = -3 Then
                        tmp1 = Form1.player.y
                    End If
                    If tmp2 = -2 Then
                        tmp2 = Form1.player.x
                    End If
                    If tmp2 = -3 Then
                        tmp2 = Form1.player.y
                    End If
                    Form1.loadmap(e(n).ac.valS, tmp1, tmp2)
            End Select
        End Sub
        Public Sub create(ByVal x As Integer, y As Integer, x2 As Integer, y2 As Integer, id As Integer, val As Integer, Optional ByVal val2 As Integer = 0, Optional ByVal valS As String = "")
            e.Add(New plateStruct(x, y, x2, y2, Form1.player.curWorld, New action(id, val, val2, valS)))
        End Sub
        Public Sub test(ByVal x As Integer, y As Integer)
            If update Then
                update = False
                For i As Integer = e.Count - 1 To 0 Step -1
                    If (Form1.inRect(x, y, e(i).x, e(i).y, e(i).x2, e(i).y2)) And (e(i).world = Form1.player.curWorld) Then
                        runPlate(i)
                    End If
                Next
            End If
        End Sub
    End Class
    <Serializable()>
    Public Structure enemyStruct
        Public x, y, dif As Integer
        Public world As String
        Public Sub New(ByVal x1 As Integer, y1 As Integer, dif1 As Integer, world1 As String)
            x = x1
            y = y1
            dif = dif1
            world = world1
        End Sub
    End Structure
    <Serializable()>
    Public Class enemy
        Public e As New List(Of enemyStruct)
        Public stepChance As Integer = 0
        Public stepDif As Integer = 2
        Public curBattle As Integer = -1
        Public curDif As Integer = 0
        Public update As Boolean = False

        Public Sub spawn(ByVal x As Integer, y As Integer, diff As Integer)
            e.Add(New enemyStruct(x, y, diff, Form1.player.curWorld))

        End Sub
        Public Sub kill(ByVal n As Integer)
            e.RemoveAt(n)
        End Sub
        Public Sub test(ByVal x As Integer, y As Integer)
            'Dim tlist = e.FindAll(Function(x) (x.x = x) And (x.y = y))
            ''System.Console.WriteLine(tlist.Count)
            'If tlist.Count <> 0 Then
            '    For Each xe As itemobj In tlist
            '        battle(i)
            '    Next
            'End If
            For i As Integer = e.Count - 1 To 0 Step -1
                If (e(i).x = x And e(i).y = y And e(i).world = Form1.player.curWorld) Then
                    battle(i)
                End If
            Next

        End Sub
        Public Sub chance(ByVal x As Integer, y As Integer)
            If Form1.godMode Then '!g
                Return '!g
            End If '!g
            If update Then
                update = False
                If Form1.rand(1, 100) <= stepChance Then
                    spawn(x, y, stepDif)
                    battle(e.Count - 1)
                End If
            End If
        End Sub
        Public Sub battle(ByRef n As Integer)
            If Form1.godMode Then '!g
                Return '!g
            End If '!g
            Dim tmp As Integer = 0
            If Form1.effect.search(2) > -1 Then
                tmp -= Form1.effect.search(2)
            End If
            If e(n).dif + tmp < 1 Then
                kill(n)
                Form1.player.money += 1
            Else
                'e(n).dif += tmp
                curDif = e(n).dif + tmp
                curBattle = n
                game.Form1.player.gamestate = "battleInit"
            End If
            '
        End Sub
    End Class
    <Serializable()>
    Public Class Loading
        Public loaded As New List(Of String)
        Public bW As New List(Of Integer)
        Public bH As New List(Of Integer)
        Public bX As New List(Of Integer)
        Public bY As New List(Of Integer)
        Public Function isLoaded(ByVal name As String) As Boolean
            For i As Integer = 0 To loaded.Count - 1
                If name = loaded(i) Then
                    Return True
                End If
            Next
            Return False
        End Function
        Public Function getNumLoaded(ByVal name As String) As Integer
            For i As Integer = 0 To loaded.Count - 1
                If name = loaded(i) Then
                    Return i
                End If
            Next
            Return -1
        End Function
        Public Function load(ByVal name As String) As Integer
            loaded.Add(name)
            Return loaded.Count
        End Function
        Public Sub addInfo(ByVal sx, sy, px, py)
            bW.Add(sx)
            bH.Add(sy)
            bX.Add(px)
            bY.Add(py)
        End Sub
    End Class
    Public Sub LoadTextures(ByVal TileSetSize As Integer)
        Dim tex As Bitmap = Bitmap.FromFile("data/texture.png")
        For i As Integer = 0 To 5
            For j As Integer = 0 To 5
                tileset(j * 6 + i) = New Bitmap(TileSetSize, TileSetSize)
                Dim g As Graphics = Graphics.FromImage(tileset(j * 6 + i))
                g.DrawImage(tex, New Rectangle(0, 0, TileSetSize, TileSetSize), New Rectangle(i * TileSetSize, j * TileSetSize, TileSetSize, TileSetSize), GraphicsUnit.Pixel)
            Next
        Next
        Dim tex2 As Bitmap = Bitmap.FromFile("data/items.png")
        For i As Integer = 0 To 5
            For j As Integer = 0 To 5
                itemtex(j * 6 + i) = New Bitmap(TileSetSize, TileSetSize)
                Dim g As Graphics = Graphics.FromImage(itemtex(j * 6 + i))
                g.DrawImage(tex2, New Rectangle(0, 0, TileSetSize, TileSetSize), New Rectangle(i * TileSetSize, j * TileSetSize, TileSetSize, TileSetSize), GraphicsUnit.Pixel)
            Next
        Next
        Dim tex3 As Bitmap = Bitmap.FromFile("data/effects.png")
        For i As Integer = 0 To 5
            For j As Integer = 0 To 5
                effecttex(j * 6 + i + 1) = New Bitmap(TileSetSize, TileSetSize)
                Dim g As Graphics = Graphics.FromImage(effecttex(j * 6 + i + 1))
                g.DrawImage(tex3, New Rectangle(0, 0, TileSetSize, TileSetSize), New Rectangle(i * TileSetSize, j * TileSetSize, TileSetSize, TileSetSize), GraphicsUnit.Pixel)
            Next
        Next
    End Sub
    Public Function canStep(ByVal n As BlockType, Optional ByVal dir As Integer = -1) As Boolean
        If (n = BlockType.Block Or n = BlockType.StoneWall Or (n = BlockType.DeepWater And effect.search(4) = -1) Or doors.CanStep(dir) = False) Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.ShiftKey Then
            Return
        End If
        Dim tmp = listBar.open
        listBar.open = False
        e.SuppressKeyPress = True
        Select Case e.KeyCode
            Case Keys.Up
                kUp = True
            Case Keys.Down
                kDown = True
            Case Keys.Left
                kLeft = True
            Case Keys.Right
                kRight = True
            Case Keys.E
                If tmp Then
                    listBar.open = False
                Else
                    inv.desc = ""
                    If (listBar.selected < 0) Or (listBar.selected >= listBar.activeCount) Then
                        listBar.selected = lists.inventory
                    End If
                    listBar.open = True
                End If
            Case Keys.Q
                If tmp Then
                    Dim cfd = 1
                    If (e.Shift And (inv.selected >= 0 And inv.selected <= inv.slot.Count)) Then
                        cfd = inv.slot(inv.selected).count
                    End If
                    inv.subItemFromSlot(inv.selected, cfd)
                    listBar.open = True
                End If
            Case Keys.S
                'save data
                gamesave()
            Case Keys.R
                gameload("start")
            Case Keys.L
                'load data
                gameload()
            Case Keys.T
                loadmap(player.curWorld, player.x, player.y)
            Case Keys.Y
                listBar.selected += 1
            Case Keys.U
                listBar.selected -= 1
            Case Keys.O
                player.stepCD = 0
            Case Keys.C
                MessageBox.Show(player.x.ToString + " " + player.y.ToString)
            Case Keys.Z
                inv.subItem(0, 1)
            Case Keys.G
                If godMode Then
                    godMode = False
                Else
                    godMode = True
                End If
        End Select
    End Sub
    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Select Case e.KeyCode
            Case Keys.Up
                kUp = False
            Case Keys.Down
                kDown = False
            Case Keys.Left
                kLeft = False
            Case Keys.Right
                kRight = False
        End Select
    End Sub
    Public Function scroolX(ByVal x1 As Integer) As Integer
        Return (x1 - player.x) * 32 + wX
    End Function
    Public Function scroolY(ByVal y1 As Integer) As Integer
        Return (y1 - player.y) * 32 + wY
    End Function
    Public Sub gamesave(Optional ByVal dir As String = "save")
        dir = "saves/" + dir
        IO.Directory.CreateDirectory(dir)

        Dim TestFileStream As IO.Stream = IO.File.Create(dir + "/enemy.dat")
        Dim serializer As New Xml.Serialization.XmlSerializer(enem.GetType)
        serializer.Serialize(TestFileStream, enem)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/player.dat")
        serializer = New Xml.Serialization.XmlSerializer(player.GetType)
        serializer.Serialize(TestFileStream, player)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/doors.dat")
        serializer = New Xml.Serialization.XmlSerializer(doors.GetType)
        serializer.Serialize(TestFileStream, doors)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/items.dat")
        serializer = New Xml.Serialization.XmlSerializer(items.GetType)
        serializer.Serialize(TestFileStream, items)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/inv.dat")
        serializer = New Xml.Serialization.XmlSerializer(inv.GetType)
        serializer.Serialize(TestFileStream, inv)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/effect.dat")
        serializer = New Xml.Serialization.XmlSerializer(effect.GetType)
        serializer.Serialize(TestFileStream, effect)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/plates.dat")
        serializer = New Xml.Serialization.XmlSerializer(plate.GetType)
        serializer.Serialize(TestFileStream, plate)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/loader.dat")
        serializer = New Xml.Serialization.XmlSerializer(loader.GetType)
        serializer.Serialize(TestFileStream, loader)
        TestFileStream.Close()
    End Sub
    Public Sub gameload(Optional ByVal dir As String = "save")
        unpressarrows()
        Dim filename As String
        Dim deserializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter

        dir = "saves/" + dir
        Dim tmpPlayer = New Players
        filename = "player.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(Players))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            tmpPlayer = CType( _
            mySerializer.Deserialize(myFileStream), Players)
            myFileStream.Close()
        End If
        If Not tmpPlayer.testVersion() Then
            If tmpPlayer.version = "" Then
                tmpPlayer.version = "before0.8.x beta"
            End If
            MessageBox.Show("this save for version: """ + tmpPlayer.version + """ not for """ + version + """")
            Return
        Else
            player = tmpPlayer
        End If
        filename = "enemy.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(enemy))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            enem = CType( _
            mySerializer.Deserialize(myFileStream), enemy)
            myFileStream.Close()
        End If
        filename = "doors.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(Door))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            doors = CType( _
            mySerializer.Deserialize(myFileStream), Door)
            myFileStream.Close()
        End If
        filename = "items.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(item))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            items = CType( _
            mySerializer.Deserialize(myFileStream), item)
            myFileStream.Close()
        End If
        filename = "inv.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(inventory))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            inv = CType( _
            mySerializer.Deserialize(myFileStream), inventory)
            myFileStream.Close()
        End If
        filename = "effect.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(Effects))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            effect = CType( _
            mySerializer.Deserialize(myFileStream), Effects)
            myFileStream.Close()
        End If
        filename = "plates.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(plates))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            plate = CType( _
            mySerializer.Deserialize(myFileStream), plates)
            myFileStream.Close()
        End If
        filename = "loader.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(Loading))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            loader = CType( _
            mySerializer.Deserialize(myFileStream), Loading)
            myFileStream.Close()
        End If
        loadmap(player.curWorld, player.x, player.y)
    End Sub
    Public Function getblocktype(ByVal x As Char) As BlockType
        If x = "1" Then
            Return BlockType.Block
        ElseIf x = "2" Then
            Return BlockType.DeepWater
        ElseIf x = "3" Then
            Return BlockType.Bridge
        ElseIf x = "4" Then
            Return BlockType.Water
        ElseIf x = "5" Then
            Return BlockType.Lava
        ElseIf x = "6" Then
            Return BlockType.DarkGrass
        ElseIf x = "7" Then
            Return BlockType.CobbleStone
        ElseIf x = "8" Then
            Return BlockType.StoneWall
        ElseIf x = "9" Then
            Return BlockType.NetherRock
        Else
            Return BlockType.Grass
        End If
    End Function
    Public Sub initmap(ByVal sX As Integer, sY As Integer, dir As String)
        ReDim map(99, 99)
        If IO.File.Exists(dir + "charset.txt") Then
            Dim chrset(256) As Integer
            Dim tmp, n As Integer
            For i As Integer = 0 To 255
                chrset(i) = -1
            Next
            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "charset.txt")
            n = fReader.ReadLine
            For i As Integer = 1 To n
                tmp = fReader.Read()                
                chrset(tmp) = fReader.ReadLine
            Next
            For i As Integer = 0 To sX - 1
                For j As Integer = 0 To sY - 1
                    tmp = chrset(Asc(mapStr(j)(i)))
                    If tmp = -1 Then
                        map(i, j) = getblocktype(mapStr(j)(i))
                    Else
                        map(i, j) = tmp
                    End If
                Next
            Next
            fReader.Close()
        Else
            For i As Integer = 0 To sX - 1
                For j As Integer = 0 To sY - 1
                    map(i, j) = getblocktype(mapStr(j)(i))
                Next
            Next
        End If
    End Sub
    Public Sub loadmap(ByVal dir As String, Optional ByVal x As Integer = -1, Optional ByVal y As Integer = -1)
        player.curWorld = dir
        If Not loader.isLoaded(dir) Then
            Dim numLoad As Integer = loader.load(dir)
            dir = "worlds/" + dir + "/"
            Dim Xd As XDocument = XDocument.Load(dir + "data.xml")
            'MessageBox.Show(Xd.Element("map").Element("test").Elements.Count)
            For Each xe As XElement In Xd.Element("map").Element("enemies").Elements
                enem.spawn(xe.Element("x"), xe.Element("y"), xe.Element("difficulty"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("items").Elements
                items.spawnitem(xe.Element("x"), xe.Element("y"), xe.Element("id"), xe.Element("count"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("doors").Elements
                doors.create(xe.Element("x"), xe.Element("y"), xe.Element("keyID"), xe.Element("texture"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("plates").Elements
                If xe.Element("id") = "3" Then
                    plate.create(xe.Element("x"), xe.Element("y"), xe.Element("x2"), xe.Element("y2"), xe.Element("id"), xe.Element("posX"), xe.Element("posY"), xe.Element("value"))
                Else
                    plate.create(xe.Element("x"), xe.Element("y"), xe.Element("x2"), xe.Element("y2"), xe.Element("id"), xe.Element("value"))
                End If
            Next
            For Each xe As XElement In Xd.Element("map").Element("shops").Elements
                Dim num As Integer = shops.create(xe.Element("x"), xe.Element("y"), xe.Element("texture"))
                For Each xe2 As XElement In xe.Elements("items").Elements
                    shops.addItem(num, New shopItem(xe2.Element("id"), xe2.Element("count"), xe2.Element("cost")))
                Next
            Next

            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            sX = Xd.Element("map").Element("info").Element("width")
            sY = Xd.Element("map").Element("info").Element("height")
            player.x = Xd.Element("map").Element("info").Element("playerX")
            player.y = Xd.Element("map").Element("info").Element("playerY")

            loader.addInfo(sX, sY, player.x, player.y)

            If (x <> -1) Then
                player.x = x
            End If
            If (y <> -1) Then
                player.y = y
            End If
            For i As Integer = 0 To sY - 1
                mapStr(i) = fReader.ReadLine()
            Next
            fReader.Close()
            initmap(sX, sY, dir)
        Else
            Dim numLoad As Integer = loader.getNumLoaded(dir)
            dir = "worlds/" + dir + "/"
            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            sX = loader.bW(numLoad)
            sY = loader.bH(numLoad)
            player.x = loader.bX(numLoad)
            player.y = loader.bY(numLoad)

            If (x <> -1) Then
                player.x = x
            End If
            If (y <> -1) Then
                player.y = y
            End If
            For i As Integer = 0 To sY - 1
                mapStr(i) = fReader.ReadLine()
            Next
            fReader.Close()
            initmap(sX, sY, dir)
        End If
    End Sub
    Public Sub unpressarrows()
        kUp = False
        kDown = False
        kLeft = False
        kRight = False
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        player.version = version
        unpressarrows()
        LoadTextures(32)
        items.init()
        effect.init()
        listBar.init()
        loadmap("main")
        wX = (5) * 32 + 100
        wY = (5) * 32 + 50

        Dim fReader As IO.StreamReader = New IO.StreamReader("data/words.txt", System.Text.Encoding.Default)
        wordscount = Int(fReader.ReadLine)
        For i As Integer = 1 To wordscount
            words(i) = fReader.ReadLine()
        Next
        fReader.Close()
        Randomize()

        'tmp
        'For i As Integer = 0 To 91
        'inv.additem(i, 1)
        'Next
        'MessageBox.Show(String.Format("good {0} may", 18))
        'tmp
        Timer1.Interval = 10
        Timer1.Start()

        Label1.Hide()
        typeText.Hide()
        typeText.Enabled = False
    End Sub
    Public Function rand(ByVal lowerbound, upperbound) As Integer
        Return CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound
    End Function
    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        System.Threading.Monitor.Enter(e.Graphics)
        Select Case player.gamestate
            Case "noAction"
            Case "normal"
                'события
                If player.stepCD > 0 Then
                    player.stepCD -= 1
                End If
                If player.stepCD = 0 Then
                    Dim goodstep As Boolean = False
                    If kUp Then
                        If (player.y > 0) Then
                            If canStep(map(player.x, player.y - 1), 1) Then
                                plate.update = True
                                enem.update = True
                                goodstep = True
                                player.y -= 1
                            ElseIf godMode Then '!g
                                plate.update = True '!g
                                enem.update = True '!g
                                goodstep = True '!g
                                player.y -= 1 '!g
                            End If
                        End If
                    End If
                    If (Not goodstep) And (kDown) Then
                        If (player.y < sY - 1 And canStep(map(player.x, player.y + 1), 2)) Then
                            plate.update = True
                            enem.update = True
                            goodstep = True
                            player.y += 1
                        ElseIf godMode Then '!g
                            plate.update = True '!g
                            enem.update = True '!g
                            goodstep = True '!g
                            player.y += 1 '!g
                        End If
                    End If
                    If (Not goodstep) And (kLeft) Then
                        If player.x > 0 Then
                            If canStep(map(player.x - 1, player.y), 3) Then
                                plate.update = True
                                enem.update = True
                                goodstep = True
                                player.x -= 1
                            ElseIf godMode Then '!g
                                plate.update = True '!g
                                enem.update = True '!g
                                goodstep = True '!g
                                player.x -= 1 '!g
                            End If
                        End If
                    End If
                    If (Not goodstep) And (kRight) Then
                        If (player.x < sX - 1 And canStep(map(player.x + 1, player.y), 4)) Then
                            plate.update = True
                            enem.update = True
                            goodstep = True
                            player.x += 1
                        ElseIf godMode Then '!g
                            plate.update = True '!g
                            enem.update = True '!g
                            goodstep = True '!g
                            player.x += 1 '!g
                        End If
                    End If
                    If goodstep Then
                        player.stepCD = 5
                    End If
                End If
                effect.upTime()
                enem.test(player.x, player.y)
                If (map(player.x, player.y) = BlockType.Lava) And (effect.search(1) > -1) Then
                ElseIf (map(player.x, player.y) = BlockType.Lava) And (player.stepCD <> -1) And (Not godMode) Then '!g
                    unpressarrows()
                    player.stepCD = -1
                    MessageBox.Show("game over(burn in lava)")
                End If
                If (map(player.x, player.y) = BlockType.DeepWater) And (effect.search(4) = -1) And (player.stepCD <> -1) And (Not godMode) And (effect.search(1) = -1) Then
                    unpressarrows()
                    player.stepCD = -1
                    MessageBox.Show("game over(death underwater)")
                End If
                plate.test(player.x, player.y)
                enem.chance(player.x, player.y)

                Dim tlist = items.item.FindAll(Function(x) (x.x = player.x) And (x.y = player.y) And (x.world = player.curWorld))
                If tlist.Count <> 0 Then
                    For Each x As itemobj In tlist
                        inv.additem(x.id, x.count)
                        items.item.Remove(x)
                    Next
                End If
                '
                shops.selectedShop = -1
                Dim bool As Boolean = True
                For i As Integer = 0 To shops.count
                    If (player.x = shops.p(i).X And player.y = shops.p(i).Y And shops.world(i) = player.curWorld) Then
                        shops.selectedShop = i
                        listBar.activate(lists.shop)
                        bool = False
                        'shops.open = True
                    End If
                Next
                If bool Then
                    listBar.deactivate(lists.shop)
                End If
                'EсобытияE
                'отрисовка карты
                For i As Integer = 0 To sX - 1
                    For j As Integer = 0 To sY - 1
                        Dim nX As Integer = scroolX(i)
                        '(i - x) * 32 + wX
                        Dim nY As Integer = scroolY(j)
                        '(j - y) * 32 + wY
                        If map(i, j) = BlockType.Block Then
                            e.Graphics.DrawImage(tileset(2), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.DeepWater Then
                            e.Graphics.DrawImage(tileset(3), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.Water Then
                            e.Graphics.DrawImage(tileset(5), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.Grass Then
                            e.Graphics.DrawImage(tileset(1), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.Bridge Then
                            e.Graphics.DrawImage(tileset(4), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.Lava Then
                            e.Graphics.DrawImage(tileset(7), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.DarkGrass Then
                            e.Graphics.DrawImage(tileset(9), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.CobbleStone Then
                            e.Graphics.DrawImage(tileset(11), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.StoneWall Then
                            e.Graphics.DrawImage(tileset(12), nX, nY, 32, 32)
                        ElseIf map(i, j) = BlockType.NetherRock Then
                            e.Graphics.DrawImage(tileset(13), nX, nY, 32, 32)
                        End If
                    Next
                Next
                'отрисовка игрока
                e.Graphics.DrawImage(tileset(0), wX, wY)
                'отрисовка предметов
                For Each x As itemobj In items.item
                    If (x.count > 0 And x.world = player.curWorld) Then
                        'Console.WriteLine(x.x.ToString + " " + x.y.ToString)
                        e.Graphics.DrawImage(itemtex(x.tex), scroolX(x.x), scroolY(x.y))
                    End If
                Next
                'отрисовка монстров
                For Each x As enemyStruct In enem.e
                    If x.world = player.curWorld Then
                        e.Graphics.DrawImage(tileset(6), scroolX(x.x), scroolY(x.y), 32, 32)
                    End If
                Next
                'отрисовка магазинов
                For i As Integer = 0 To shops.count
                    If shops.world(i) = player.curWorld Then
                        e.Graphics.DrawImage(tileset(shops.tex(i)), scroolX(shops.p(i).X), scroolY(shops.p(i).Y), 32, 32)
                    End If
                Next
                'отрисовка дверей
                For i As Integer = 0 To doors.d.Count - 1
                    If (doors.d(i).tex <> -1 And doors.d(i).world = player.curWorld) Then
                        e.Graphics.DrawImage(tileset(doors.d(i).tex), scroolX(doors.d(i).x), scroolY(doors.d(i).y), 32, 32)
                    End If
                Next
                'отрисовка эффектов
                For i As Integer = 0 To effect.e.Count - 1
                    e.Graphics.DrawImage(effecttex(effect.e(i).tex), 1, 30 + i * 40)
                    If effect.e(i).isTemp Then
                        e.Graphics.DrawString(effect.e(i).dur / 100, New Font("Arial", 9), Brushes.Black, 50, 50 + 40 * i)
                    End If
                Next
                If effect.selected <> -1 Then
                    'MessageBox.Show(effect.desc(effect.e(effect.selected).id))                    
                    e.Graphics.DrawString(String.Format(effect.desc(effect.e(effect.selected).id - 1), effect.e(effect.selected).pow), New Font("Arial", 9), Brushes.Black, 50, 30 + effect.selected * 40)
                End If
                'money
                e.Graphics.DrawString("money " + player.money.ToString, New Font("Arial", 11), Brushes.Black, 5, 5)
                'отрисовка ListBar
                If listBar.open Then
                    e.Graphics.FillRectangle(Brushes.Gray, 50, 10, 50, 460)
                    'e.Graphics.FillRectangle(Brushes.LightGray, 50, 10 + (1 - 1) * 50, 50, 50)
                    'e.Graphics.FillRectangle(Brushes.DarkGray, 50, 10 + (2 - 1) * 50, 50, 50)
                    'e.Graphics.FillRectangle(Brushes.DarkGray, 50, 10 + (3 - 1) * 50, 50, 50)
                    Dim offset As Integer = 0
                    For i As Integer = 0 To listBar.e.Count - 1
                        If listBar.e(i).isActive Then
                            If i = listBar.selected Then
                                e.Graphics.FillRectangle(Brushes.LightGray, 50, 10 + (i - offset) * 50, 50, 50)
                            Else
                                e.Graphics.FillRectangle(Brushes.DarkGray, 50, 10 + (i - offset) * 50, 50, 50)
                            End If
                        Else
                            offset += 1
                        End If
                    Next

                    'отрисовка инвентаря
                    If listBar.selected = lists.inventory Then
                        e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
                        For i As Integer = 0 To 9
                            If i + inv.page * 10 < inv.slot.Count Then
                                Dim it As Integer = i + inv.page * 10
                                e.Graphics.DrawString(items.name(inv.slot(it).id), New Font("Arial", 14), Brushes.Black, 145, 50 + 38 * i)
                                e.Graphics.DrawString(inv.slot(it).count, New Font("Arial", 14), Brushes.Black, 310, 50 + 38 * i)
                                e.Graphics.DrawString(items.type(inv.slot(it).id), New Font("Arial", 14), Brushes.Black, 370, 50 + 38 * i)
                                e.Graphics.DrawImage(itemtex(items.tex(inv.slot(it).id)), 105, 50 + 38 * i)
                            End If
                        Next '160 435
                        e.Graphics.DrawString("TEX", New Font("Arial", 14), Brushes.Black, 100, 20)
                        e.Graphics.DrawString("NAME", New Font("Arial", 14), Brushes.Black, 175, 20)
                        e.Graphics.DrawString("COUNT", New Font("Arial", 14), Brushes.Black, 285, 20)
                        e.Graphics.DrawString("TYPE", New Font("Arial", 14), Brushes.Black, 385, 20)

                        e.Graphics.FillRectangle(Brushes.Gray, 105, 30 + 40 * 10, 95, 32)
                        e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 435)
                        e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 457)
                        e.Graphics.FillRectangle(Brushes.Gray, 400, 30 + 40 * 10, 95, 32)
                        e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 435)
                        e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 457)
                        e.Graphics.DrawString((inv.page + 1).ToString + "/" + (Math.Max(Math.Floor(inv.slot.Count / 10) + 1, 1)).ToString, New Font("Arial", 14), Brushes.Black, 284 + 5 * (Math.Floor(inv.slot.Count / 10) > 8) + 5 * (inv.page > 8), 446 - 10)
                        If inv.desc <> "" Then
                            e.Graphics.FillRectangle(Brushes.LightGray, 500, 10, 130, 460)
                            e.Graphics.DrawString(inv.desc, New Font("Arial", 12), Brushes.Black, New RectangleF(501, 20, 130, 200))
                        End If
                    End If
                    'отрисовка отрытого магазина
                    If (listBar.selected = lists.shop) And (shops.selectedShop <> -1) Then
                        e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
                        For i As Integer = 0 To 9
                            If i + shops.page * 10 <= shops.itemsCount(shops.selectedShop) Then
                                Dim it As Integer = i + shops.page * 10
                                With shops.items(shops.selectedShop, it)
                                    e.Graphics.DrawString(items.name(.id), New Font("Arial", 14), Brushes.Black, 145, 50 + 38 * i)
                                    e.Graphics.DrawString(.cost, New Font("Arial", 14), Brushes.Black, 310, 50 + 38 * i)
                                    e.Graphics.DrawString(items.type(.id), New Font("Arial", 14), Brushes.Black, 370, 50 + 38 * i)
                                    e.Graphics.DrawImage(itemtex(items.tex(.id)), 105, 50 + 38 * i)
                                End With
                            End If
                        Next '160 435
                        e.Graphics.DrawString("TEX", New Font("Arial", 14), Brushes.Black, 100, 20)
                        e.Graphics.DrawString("NAME", New Font("Arial", 14), Brushes.Black, 175, 20)
                        e.Graphics.DrawString("COST", New Font("Arial", 14), Brushes.Black, 295, 20)
                        e.Graphics.DrawString("TYPE", New Font("Arial", 14), Brushes.Black, 385, 20)

                        e.Graphics.FillRectangle(Brushes.Gray, 105, 30 + 40 * 10, 95, 32)
                        e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 435)
                        e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 457)
                        e.Graphics.FillRectangle(Brushes.Gray, 400, 30 + 40 * 10, 95, 32)
                        e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 435)
                        e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 457)
                        e.Graphics.DrawString((shops.page + 1).ToString + "/" + (Math.Max(Math.Floor(inv.slot.Count / 10) + 1, 1)).ToString, New Font("Arial", 14), Brushes.Black, 284 + 5 * (Math.Floor(inv.slot.Count / 10) > 8) + 5 * (inv.page > 8), 446 - 10)
                        If shops.desc <> "" Then
                            e.Graphics.FillRectangle(Brushes.LightGray, 500, 10, 130, 460)
                            e.Graphics.DrawString(shops.desc, New Font("Arial", 12), Brushes.Black, New RectangleF(501, 20, 130, 200))
                        End If
                    End If
                End If
            Case "battleInit"
                unpressarrows()
                Label1.Show()
                'text load
                str = words(rand(1, wordscount))
                For i As Integer = 1 To enem.curDif - 1 'enem.dif(enem.curBattle) - 1
                    str += " " + words(rand(1, wordscount))
                Next
                Label1.Text = str
                typeText.Show()
                typeText.Enabled = True
                typeText.Focus()
                player.gamestate = "battle"
            Case "battle"
                'battle mode on
                e.Graphics.DrawString("test mode no texture", New Font("Arial", 14), Brushes.Black, 20, 20)
                If typeText.Text = Label1.Text Then
                    Label1.Hide()
                    typeText.Hide()
                    typeText.Clear()
                    typeText.Enabled = False
                    player.money += 10
                    enem.kill(enem.curBattle)
                    enem.curBattle = -1
                    player.gamestate = "normal"
                End If
                '
        End Select
        System.Threading.Monitor.Exit(e.Graphics)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        PictureBox1.Refresh()
    End Sub
    Public Function inRect(ByVal x As Integer, y As Integer, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As Boolean
        If (x >= x1) And (x <= x2) And (y >= y1) And (y <= y2) Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        If listBar.open Then
            For i As Integer = 0 To listBar.activeCount - 1
                If inRect(e.X, e.Y, 50, 10 + i * 50, 100, 10 + (i + 1) * 50) Then
                    Dim offset As Integer = -1
                    For j As Integer = 0 To listBar.e.Count
                        If listBar.e(j).isActive Then
                            offset += 1
                        End If
                        If offset = i Then
                            listBar.selected = j
                            Exit For
                        End If
                    Next
                End If
            Next
            If (listBar.selected = lists.shop) And (shops.selectedShop <> -1) Then
                'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + shops.page * 10
                        If e.Button = MouseButtons.Left Then
                            shops.buyItem(shops.selectedShop, shops.selectedItem, 1)
                        End If
                        If e.Button = MouseButtons.Right Then
                            shops.sellItem(shops.selectedShop, shops.selectedItem, 1)
                        End If
                    End If
                Next
                If inRect(e.X, e.Y, 105, 30 + 40 * 10, 200, 30 + 40 * 10 + 32) Then
                    If shops.page > 0 Then
                        shops.page -= 1
                    End If
                End If
                If inRect(e.X, e.Y, 400, 30 + 40 * 10, 495, 30 + 40 * 10 + 32) Then
                    If shops.page < Math.Floor(shops.itemsCount(shops.selectedShop) / 10) Then
                        shops.page += 1
                    End If
                End If
            ElseIf listBar.selected = lists.inventory Then
                'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
                If e.Button = Windows.Forms.MouseButtons.Right Then
                    For i As Integer = 0 To 9 '105, 30 + 40 * i
                        If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                            Dim it As Integer = i + inv.page * 10
                            If (items.isUsable(inv.slot(it).id)) Then
                                If inv.useItem(it) Then
                                    inv.subItem(inv.slot(it).id, 1)
                                End If
                            End If
                        End If
                    Next
                End If
                If inRect(e.X, e.Y, 105, 30 + 40 * 10, 200, 30 + 40 * 10 + 32) Then
                    If inv.page > 0 Then
                        inv.page -= 1
                    End If
                End If
                If inRect(e.X, e.Y, 400, 30 + 40 * 10, 495, 30 + 40 * 10 + 32) Then
                    If inv.page < Math.Floor(inv.slot.Count / 10) Then
                        inv.page += 1
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If listBar.open Then
            If (listBar.selected = lists.shop) And (shops.selectedShop <> -1) Then
                shops.desc = "LBM - buy" + vbNewLine + "RBM - sell"
                shops.selectedItem = -1
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + shops.page * 10
                        If inv.slot.Count >= it Then
                            shops.desc = items.desc(shops.items(shops.selectedShop, it).id) + vbNewLine + "LBM - buy" + vbNewLine + "RBM - sell"
                            shops.selectedItem = it
                        End If
                    End If
                Next
            ElseIf listBar.selected = lists.inventory Then
                inv.desc = ""
                inv.selected = -1
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + inv.page * 10
                        If inv.slot.Count > it Then
                            inv.desc = items.desc(inv.slot(it).id)
                            inv.selected = it
                        End If
                    End If
                Next
            End If
        End If
        effect.selected = -1
        For i As Integer = 0 To effect.e.Count - 1
            If inRect(e.X, e.Y, 1, 30 + i * 40, 1 + 32, 30 + i * 40 + 32) Then
                effect.selected = i
            End If
        Next
    End Sub
End Class
