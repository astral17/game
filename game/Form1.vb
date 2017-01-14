Public Class Form1
    Public version = "0.9.0beta"
    Public sX, sY, wX, wY, wordscount As Integer
    Public kUp, kDown, kLeft, kRight As Boolean
    Public godMode As Boolean = False
    Public str As String
    Public tileset(99) As Bitmap
    Public itemtex(99) As Bitmap
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
    Public Class Door
        Public dX(99), dY(99), id(99), tex(99) As Integer
        Public world(99) As String
        Public count As Integer = -1
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
            For i As Integer = 0 To count
                If (dX(i) = x1 And dY(i) = y1 And world(i) = Form1.player.curWorld) Then
                    If (Form1.inv.searchItem(8) <> -1) Then 'masterkey
                        dX(i) = -1
                        dY(i) = -1
                        tex(i) = -1
                        Return True
                    End If
                    If (Form1.inv.searchItem(id(i)) = -1) Then
                        Return False
                    Else
                        dX(i) = -1
                        dY(i) = -1
                        tex(i) = -1
                        Form1.inv.subitem(id(i), 1)
                        Return True
                    End If
                End If
            Next
            Return True
        End Function
        Public Sub create(ByVal x As Integer, y As Integer, vid As Integer, vtex As Integer)
            count += 1
            dX(count) = x
            dY(count) = y
            id(count) = vid
            tex(count) = vtex
            world(count) = Form1.player.curWorld
        End Sub
    End Class
    <Serializable()>
    Public Structure itemobj
        Public id, count, x, y, tex As Integer
        Public world As String
    End Structure
    <Serializable()>
    Public Class item
        Public name(99), type(99), desc(99) As String
        Public tex(99), eff(99), dureff(99), poweff(99) As Integer
        Public isUsable(99) As Boolean
        Public world(99) As String
        Public item(99) As itemobj
        Public count As Integer = -1
        Public Sub init()
            Dim fReader As System.IO.StreamReader = New IO.StreamReader("items.txt")
            Dim n As Integer
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                name(i) = fReader.ReadLine()
                type(i) = fReader.ReadLine()
                desc(i) = fReader.ReadLine()
                tex(i) = Form1.readstreamint(fReader)
                eff(i) = Form1.readstreamint(fReader)
                If eff(i) > 0 Then
                    dureff(i) = Form1.readstreamint(fReader)
                    poweff(i) = Form1.readstreamint(fReader)
                    Dim t As Integer
                    t = Form1.readstreamint(fReader)
                    If t = 0 Then
                        isUsable(i) = False
                    Else
                        isUsable(i) = True
                    End If
                End If
            Next
            fReader.Close()
        End Sub
        Public Sub spawnitem(ByVal x As Integer, y As Integer, id As Integer, counti As Integer)
            count += 1
            item(count).id = id
            item(count).x = x
            item(count).y = y
            item(count).count = counti
            item(count).tex = tex(id)
            item(count).world = Form1.player.curWorld
        End Sub
    End Class
    <Serializable()>
    Public Structure slots
        Public id, count, tex As Integer
    End Structure
    <Serializable()>
    Public Class inventory
        Public slot(99) As slots
        Public count As Integer = -1
        Public open As Boolean = False
        Public desc As String = ""
        Public page As Integer = 0
        Public Sub additem(ByVal id As Integer, countitem As Integer)
            If id = 2 Then
                Form1.player.money += countitem
                Return
            End If
            Dim t As Boolean = True
            For i As Integer = 0 To count
                If slot(i).id = id Then
                    t = False
                    slot(i).count += countitem
                End If
            Next
            If t Then
                count += 1
                slot(count).id = id
                slot(count).count = countitem
                Dim x As Boolean = Form1.items.isUsable(count)
                'MessageBox.Show(Form1.items.isUsable(slot(count).id).ToString)
                If Not (Form1.items.isUsable(slot(count).id)) Then
                    useItem(count)
                End If
            End If
        End Sub
        Public Sub subitem(ByVal id As Integer, countitem As Integer)
            Dim t As Boolean = False
            For i As Integer = 0 To count
                If t Then
                    slot(i - 1) = slot(i)
                ElseIf slot(i).id = id Then
                    If Not Form1.items.isUsable(slot(i).id) Then
                        Dim idEff = Form1.items.eff(slot(i).id)
                        Dim poweff = Form1.items.poweff(slot(i).id)
                        Form1.effect.remove(idEff, poweff)
                    End If
                    slot(i).count -= 1
                    If slot(i).count = 0 Then
                        t = True
                    End If
                End If
            Next
            If t Then
                slot(count).id = 0
                slot(count).count = 0
                count -= 1
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
            Form1.effect.add(idEff, dureff, poweff, slot(slotNum).id, isUsable)
            Return True
        End Function
        Public Function getSlot(ByVal n As Integer) As slots
            Return slot(n)
        End Function
        Public Function searchItem(ByVal id As Integer)
            For i As Integer = 0 To count
                If slot(i).id = id Then
                    Return i
                End If
            Next
            Return -1
        End Function
    End Class
    <Serializable()>
    Public Class Effects  'id dur pow temp
        Public id(99) As Integer, dur(99) As Integer, pow(99) As Integer, isTemp(99) As Boolean, tex(99) As Integer, showDesc(99) As Boolean
        Public count = -1
        Public Sub add(ByVal id1 As Integer, dur1 As Integer, pow1 As Integer, itemID As Integer, Optional ByVal isTemp1 As Boolean = True)
            count += 1
            id(count) = id1
            dur(count) = dur1
            pow(count) = pow1
            tex(count) = Form1.items.tex(itemID)
            showDesc(count) = False
            isTemp(count) = isTemp1
        End Sub
        Public Sub remove(ByVal id1 As Integer, Optional ByVal pow1 As Integer = -1)
            Dim t As Boolean = False
            For i As Integer = 0 To count
                If (t) And (i > 0) Then
                    id(i - 1) = id(i)
                    dur(i - 1) = dur(i)
                    pow(i - 1) = pow(i)
                    tex(i - 1) = tex(i)
                    showDesc(i - 1) = showDesc(i)
                    isTemp(i - 1) = isTemp(i)
                End If
                If (id(i) = id1) And ((pow1 = -1) Or (pow(i) = pow1)) Then
                    t = True
                End If
            Next
            If t Then
                count -= 1
            End If
        End Sub
        Public Sub removeByNum(ByVal num As Integer)
            For i As Integer = num To count
                If i > num Then
                    id(i - 1) = id(i)
                    dur(i - 1) = dur(i)
                    pow(i - 1) = pow(i)
                    tex(i - 1) = tex(i)
                    showDesc(i - 1) = showDesc(i)
                    isTemp(i - 1) = isTemp(i)
                End If
            Next
            count -= 1
        End Sub
        Public Function search(ByVal id1 As Integer) As Integer
            Dim max As Integer = -1
            For i As Integer = 0 To count
                If id(i) = id1 Then
                    max = Math.Max(pow(i), max)
                End If
            Next
            Return max
        End Function
        Public Sub upTime()
            For i As Integer = 0 To count
                If isTemp(i) Then
                    dur(i) -= 1
                End If
                'Case 1 undead обработка там где лава
                'Case 2 incAttack
                'Case 3 reincarnation
                If id(i) = 3 Then
                    Form1.player.stepCD = 0
                    add(1, 300, 1, 0, True)
                End If
                'Case 4 waterbreath
            Next
            Dim offset As Integer = 0
            Dim ocount = count
            For i As Integer = 0 To ocount
                If dur(i - offset) < 1 Then
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
    End Structure
    <Serializable()>
    Public Class plates
        Dim p(99) As Point, p2(99) As Point
        Dim ac(99) As action
        Public world(99) As String
        Public count As Integer = -1
        Public update As Boolean = True
        Public Sub runPlate(ByVal n As Integer)
            If (ac(n).id = 0) And (ac(n).val = 0) Then
                MessageBox.Show("testbox")
                Form1.unpressarrows()
            End If
            Select Case ac(n).id
                Case 0 'remove messageBox with val id
                    If ac(n).val = 1 Then
                        ac(n).id = -1
                        ac(n - 1).id = -1
                    End If
                Case 1 'set step chance
                    Form1.enem.stepChance = ac(n).val
                Case 2 'set step difficult
                    Form1.enem.stepDif = ac(n).val
                Case 3 'teleport to val world
                    Dim tmp1, tmp2 As Integer
                    tmp1 = ac(n).val
                    tmp2 = ac(n).val2
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
                    Form1.loadmap(ac(n).valS, tmp1, tmp2)
            End Select
        End Sub
        Public Sub create(ByVal x As Integer, y As Integer, x2 As Integer, y2 As Integer, id As Integer, val As Integer, Optional ByVal val2 As Integer = 0, Optional ByVal valS As String = "")
            count += 1
            p(count).X = x
            p(count).Y = y
            p2(count).X = x2
            p2(count).Y = y2
            ac(count).id = id
            ac(count).val = val
            ac(count).val2 = val2
            ac(count).valS = valS
            world(count) = Form1.player.curWorld
        End Sub
        Public Sub test(ByVal x As Integer, y As Integer)
            If update Then
                update = False
                For i As Integer = 0 To count
                    If (form1.inRect(x, y, p(i).X, p(i).Y, p2(i).X, p2(i).Y)) And (world(i) = Form1.player.curWorld) Then
                        runPlate(i)
                    End If
                Next
            End If
        End Sub
    End Class
    <Serializable()>
    Public Class enemy

        Dim p(100) As Point
        Public dif(100) As Integer
        Public world(100) As String
        Dim mcount As Integer = -1
        Public stepChance As Integer = 0
        Public stepDif As Integer = 2
        Public curBattle As Integer = -1
        Public update As Boolean = False
        Public Property count As Integer
            Get
                Return mcount
            End Get
            Set(value As Integer)
                mcount = value
            End Set
        End Property

        Public Sub spawn(ByVal x As Integer, y As Integer, diff As Integer)
            mcount += 1
            p(mcount).X = x
            p(mcount).Y = y
            dif(mcount) = diff
            world(mcount) = Form1.player.curWorld

        End Sub
        Public Sub kill(ByVal n As Integer)
            For i As Integer = n To mcount - 1
                p(i) = p(i + 1)
                dif(i) = dif(i + 1)
            Next
            mcount -= 1
        End Sub
        Public Function getX(ByVal n As Integer) As Integer
            Return p(n).X
        End Function
        Public Function getY(ByVal n As Integer) As Integer
            Return p(n).Y
        End Function
        'Public Function getSt(ByVal n As Integer) As Integer
        '    Return state(n)
        'End Function
        Public Sub test(ByVal x As Integer, y As Integer)
            For i As Integer = 0 To mcount
                If (p(i).X = x And p(i).Y = y And world(i) = Form1.player.curWorld) Then
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
                    battle(mcount)
                End If
            End If
        End Sub
        Public Sub battle(ByVal n As Integer)
            If Form1.godMode Then '!g
                Return '!g
            End If '!g
            Dim tmp As Integer = 0
            If Form1.effect.search(2) > -1 Then
                tmp -= Form1.effect.search(2)
            End If
            If dif(n) + tmp < 1 Then
                kill(n)
                Form1.player.money += 1
            Else
                dif(n) += tmp
                curBattle = n
                game.Form1.player.gamestate = "battleInit"
            End If
            '
        End Sub
    End Class
    <Serializable()>
    Public Class Loading
        Public loaded(100) As String
        Public count As Integer = -1
        Public Function isLoaded(ByVal name As String) As Boolean
            For i = 0 To count
                If name = loaded(i) Then
                    Return True
                End If
            Next
            Return False
        End Function
        Public Sub load(ByVal name As String)
            count += 1
            loaded(count) = name
        End Sub
    End Class
    Public Sub LoadTextures(ByVal TileSetSize As Integer)
        Dim tex As Bitmap = Bitmap.FromFile("texture.png")
        For i As Integer = 0 To 5
            For j As Integer = 0 To 5
                tileset(j * 6 + i) = New Bitmap(TileSetSize, TileSetSize)
                Dim g As Graphics = Graphics.FromImage(tileset(j * 6 + i))
                g.DrawImage(tex, New Rectangle(0, 0, TileSetSize, TileSetSize), New Rectangle(i * TileSetSize, j * TileSetSize, TileSetSize, TileSetSize), GraphicsUnit.Pixel)
            Next
        Next
        Dim tex2 As Bitmap = Bitmap.FromFile("items.png")
        For i As Integer = 0 To 5
            For j As Integer = 0 To 5
                itemtex(j * 6 + i) = New Bitmap(TileSetSize, TileSetSize)
                Dim g As Graphics = Graphics.FromImage(itemtex(j * 6 + i))
                g.DrawImage(tex2, New Rectangle(0, 0, TileSetSize, TileSetSize), New Rectangle(i * TileSetSize, j * TileSetSize, TileSetSize, TileSetSize), GraphicsUnit.Pixel)
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
        Dim tmp = inv.open
        inv.open = False
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
                    inv.open = False
                Else
                    inv.desc = ""
                    inv.open = True
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
                loadmap("cave")
            Case Keys.Y
                loadmap("main")
            Case Keys.C
                MessageBox.Show(player.x.ToString + " " + player.y.ToString)
            Case Keys.Z
                inv.subitem(0, 1)
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
    Public Function readstreamint(ByVal fstream As System.IO.StreamReader) As Integer
        Dim n As Integer
        Dim bool As Boolean = False
        Dim c As Integer
        While (c <> 32) And (c <> 13) And (fstream.EndOfStream = False)
            c = fstream.Read()
            If c = 45 Then
                bool = Not bool
            ElseIf (c <> 32) And (c <> 13) Then
                n = n * 10 + (c - 48)
            End If
        End While
        If (c = 13) Then
            c = fstream.Read()
        End If
        If bool Then
            n *= -1
        End If
        Return n
    End Function
    Public Function readstreamword(ByVal fstream As System.IO.StreamReader) As String
        Dim s As String = ""
        Dim c As Integer
        While (c <> 32) And (c <> 13) And (fstream.EndOfStream = False)
            c = fstream.Read()
            If (c <> 32) And (c <> 13) Then
                s += Convert.ToChar(c)
            End If
        End While
        If (c = 13) Then
            c = fstream.Read()
        End If
        Return s
    End Function
    Public Sub gamesave(Optional ByVal dir As String = "save")
        dir = "saves/" + dir
        IO.Directory.CreateDirectory(dir)

        Dim TestFileStream As IO.Stream = IO.File.Create(dir + "/enemy.dat")
        Dim serializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        serializer.Serialize(TestFileStream, enem)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/player.dat")
        serializer.Serialize(TestFileStream, player)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/doors.dat")
        serializer.Serialize(TestFileStream, doors)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/items.dat")
        serializer.Serialize(TestFileStream, items)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/inv.dat")
        serializer.Serialize(TestFileStream, inv)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/effect.dat")
        serializer.Serialize(TestFileStream, effect)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/plates.dat")
        serializer.Serialize(TestFileStream, plate)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/loader.dat")
        serializer.Serialize(TestFileStream, loader)
        TestFileStream.Close()
    End Sub
    Public Sub gameload(Optional ByVal dir As String = "save")
        unpressarrows()
        '
        Dim filename As String
        'Dim ocWorld = player.curWorld
        Dim deserializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        dir = "saves/" + dir
        Dim tmpPlayer = New Players
        filename = "player.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            tmpPlayer = CType(deserializer.Deserialize(TestFileStream), Players)
            TestFileStream.Close()
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
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            enem = CType(deserializer.Deserialize(TestFileStream), enemy)
            TestFileStream.Close()
        End If
        filename = "doors.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            doors = CType(deserializer.Deserialize(TestFileStream), Door)
            TestFileStream.Close()
        End If
        filename = "items.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            items = CType(deserializer.Deserialize(TestFileStream), item)
            TestFileStream.Close()
        End If
        filename = "inv.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            inv = CType(deserializer.Deserialize(TestFileStream), inventory)
            TestFileStream.Close()
        End If
        filename = "effect.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            effect = CType(deserializer.Deserialize(TestFileStream), Effects)
            TestFileStream.Close()
        End If
        filename = "plates.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            plate = CType(deserializer.Deserialize(TestFileStream), plates)
            TestFileStream.Close()
        End If
        filename = "loader.dat"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim TestFileStream As IO.Stream = IO.File.OpenRead(dir + "/" + filename)
            loader = CType(deserializer.Deserialize(TestFileStream), Loading)
            TestFileStream.Close()
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
            n = readstreamint(fReader)
            For i As Integer = 1 To n
                tmp = fReader.Read()
                chrset(tmp) = readstreamint(fReader)
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
            loader.load(dir)
            dir = "worlds/" + dir + "/"
            'doors.init(dir)
            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            Dim n, tmp1, tmp2, tmp3, tmp4, tmp5, tmp7, tmp8 As Integer
            Dim tmp6 As String
            sX = readstreamint(fReader)
            sY = readstreamint(fReader)
            player.x = readstreamint(fReader) - 1
            player.y = readstreamint(fReader) - 1
            If (x <> -1) Then
                player.x = x
            End If
            If (y <> -1) Then
                player.y = y
            End If
            For i As Integer = 0 To sY - 1
                mapStr(i) = fReader.ReadLine()
            Next
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                tmp1 = readstreamint(fReader)
                tmp2 = readstreamint(fReader)
                tmp3 = readstreamint(fReader)
                enem.spawn(tmp1, tmp2, tmp3)
            Next
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                tmp1 = readstreamint(fReader)
                tmp2 = readstreamint(fReader)
                tmp3 = readstreamint(fReader)
                tmp4 = readstreamint(fReader)
                items.spawnitem(tmp1, tmp2, tmp3, tmp4)
            Next
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                tmp1 = readstreamint(fReader)
                tmp2 = readstreamint(fReader)
                tmp3 = readstreamint(fReader)
                tmp4 = readstreamint(fReader)
                doors.create(tmp1, tmp2, tmp3, tmp4)
            Next
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                tmp1 = readstreamint(fReader)
                tmp2 = readstreamint(fReader)
                tmp7 = readstreamint(fReader)
                tmp8 = readstreamint(fReader)
                tmp3 = readstreamint(fReader)
                If tmp3 = 3 Then
                    'tmp6 = fReader.ReadLine
                    tmp6 = readstreamword(fReader)
                    tmp4 = readstreamint(fReader)
                    tmp5 = readstreamint(fReader)
                Else
                    tmp4 = readstreamint(fReader)
                    tmp5 = 0
                    tmp6 = ""
                End If
                plate.create(tmp1, tmp2, tmp7, tmp8, tmp3, tmp4, tmp5, tmp6)
            Next
            fReader.Close()
            initmap(sX, sY, dir)
        Else
            dir = "worlds/" + dir + "/"
            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            sX = readstreamint(fReader)
            sY = readstreamint(fReader)
            player.x = readstreamint(fReader) - 1
            player.y = readstreamint(fReader) - 1
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
        loadmap("main")
        wX = (5) * 32 + 100
        wY = (5) * 32 + 50

        Dim fReader As IO.StreamReader = New IO.StreamReader("words.txt", System.Text.Encoding.Default)
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
                    'map(player.x, player.y) = BlockType.Block
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
                For i As Integer = 0 To items.count
                    If (player.x = items.item(i).x And player.y = items.item(i).y And items.item(i).count > 0 And items.item(i).world = player.curWorld) Then
                        inv.additem(items.item(i).id, items.item(i).count)
                        items.item(i).count = 0
                    End If
                Next
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
                For i As Integer = 0 To items.count
                    If (items.item(i).count > 0 And items.item(i).world = player.curWorld) Then
                        e.Graphics.DrawImage(itemtex(items.item(i).tex), scroolX(items.item(i).x), scroolY(items.item(i).y))
                    End If
                Next
                'отрисовка монстров
                For i As Integer = 0 To enem.count
                    If enem.world(i) = player.curWorld Then
                        e.Graphics.DrawImage(tileset(6), scroolX(enem.getX(i)), scroolY(enem.getY(i)), 32, 32)
                    End If
                Next
                'отрисовка дверей
                For i As Integer = 0 To doors.count
                    If (doors.tex(i) <> -1 And doors.world(i) = player.curWorld) Then
                        e.Graphics.DrawImage(tileset(doors.tex(i)), scroolX(doors.dX(i)), scroolY(doors.dY(i)), 32, 32)
                    End If
                Next
                'отрисовка эффектов
                For i As Integer = 0 To effect.count
                    e.Graphics.DrawImage(itemtex(effect.tex(i)), 1, 30 + i * 40)
                    If effect.isTemp(i) Then
                        e.Graphics.DrawString(effect.dur(i) / 100, New Font("Arial", 9), Brushes.Black, 50, 50 + 40 * i)
                    End If
                Next
                'отрисовка инвентаря
                If inv.open Then
                    e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
                    For i As Integer = 0 To 9
                        If i + inv.page * 10 <= inv.count Then
                            Dim it As Integer = i + inv.page * 10
                            e.Graphics.DrawString(items.name(inv.slot(it).id), New Font("Arial", 14), Brushes.Black, 145, 30 + 40 * i)
                            e.Graphics.DrawString(inv.slot(it).count, New Font("Arial", 14), Brushes.Black, 310, 30 + 40 * i)
                            e.Graphics.DrawString(items.type(inv.slot(it).id), New Font("Arial", 14), Brushes.Black, 370, 30 + 40 * i)
                            e.Graphics.DrawImage(itemtex(items.tex(inv.slot(it).id)), 105, 30 + 40 * i)
                        End If
                    Next '160 435
                    e.Graphics.FillRectangle(Brushes.Gray, 105, 30 + 40 * 10, 95, 32)
                    e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 435)
                    e.Graphics.DrawLine(Pens.Black, 140, 446, 160, 457)
                    e.Graphics.FillRectangle(Brushes.Gray, 400, 30 + 40 * 10, 95, 32)
                    e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 435)
                    e.Graphics.DrawLine(Pens.Black, 460, 446, 440, 457)
                    e.Graphics.DrawString((inv.page + 1).ToString + "/" + (Math.Max(Math.Floor(inv.count / 10) + 1, 1)).ToString, New Font("Arial", 14), Brushes.Black, 284 + 5 * (Math.Floor(inv.count / 10) > 8) + 5 * (inv.page > 8), 446 - 10)
                    If inv.desc <> "" Then
                        e.Graphics.FillRectangle(Brushes.LightGray, 500, 10, 130, 460)
                        e.Graphics.DrawString(inv.desc, New Font("Arial", 12), Brushes.Black, New RectangleF(501, 20, 130, 200))
                    End If
                End If
                e.Graphics.DrawString("money " + player.money.ToString, New Font("Arial", 11), Brushes.Black, 5, 5)
            Case "battleInit"
                unpressarrows()
                Label1.Show()
                'text load
                str = words(rand(1, wordscount))
                For i As Integer = 1 To enem.dif(enem.curBattle) - 1
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
        If inv.open Then
            'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
            If e.Button = Windows.Forms.MouseButtons.Right Then
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 30 + 40 * i, 495, 30 + 40 * i + 32) Then
                        Dim it As Integer = i + inv.page * 10
                        If (items.isUsable(inv.slot(it).id)) Then
                            If inv.useItem(it) Then
                                inv.subitem(inv.slot(it).id, 1)
                            End If
                        End If
                    End If
                Next
            End If
            If inRect(e.X, e.Y, 105, 30 + 40 * 10, 200, 30 + 40 * 10 + 32) Then
                'MessageBox.Show("left")
                If inv.page > 0 Then
                    inv.page -= 1
                End If
            End If
            If inRect(e.X, e.Y, 400, 30 + 40 * 10, 495, 30 + 40 * 10 + 32) Then
                'MessageBox.Show("right")
                If inv.page < Math.Floor(inv.count / 10) Then
                    inv.page += 1
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If inv.open Then
            inv.desc = ""
            'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
            For i As Integer = 0 To 9 '105, 30 + 40 * i
                If inRect(e.X, e.Y, 105, 30 + 40 * i, 495, 30 + 40 * i + 32) Then
                    Dim it As Integer = i + inv.page * 10
                    'If (inv.useItem(it)) And (items.isUsable(inv.slot(it).id)) Then
                    '   inv.subitem(inv.slot(it).id, 1)
                    'End If
                    'MessageBox.Show(it)
                    'MessageBox.Show(items.desc(inv.slot(it).id))
                    If inv.count >= it Then
                        inv.desc = items.desc(inv.slot(it).id)
                    End If
                End If
            Next
        End If
        For i As Integer = 1 To effect.count

        Next
    End Sub
End Class
