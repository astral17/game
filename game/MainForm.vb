Public Class MainForm
    Public version = "1.0.0pre-3"
    Public sX, sY, wX, wY, wordscount As Integer
    Public kUp, kDown, kLeft, kRight As Boolean
    Public godMode As Boolean = False
    Public str As String
    Public tileset(99) As Bitmap
    Public itemtex(99) As Bitmap
    Public effecttex(99) As Bitmap
    Public words(99) As String
    Public mapStr(99) As String
    Public encount As Integer

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
        If (n = BlockType.Block Or n = BlockType.StoneWall Or (n = BlockType.DeepWater And Effect.search(4) = -1) Or Door.CanStep(dir) = False) Then
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
                    Inv.desc = ""
                    If (listBar.selected < 0) Or (listBar.selected >= listBar.activeCount) Then
                        listBar.selected = ListsEnum.inventory
                    End If
                    listBar.open = True
                End If
            Case Keys.Q
                If tmp Then
                    Dim cfd = 1
                    If (e.Shift And (Inv.selected >= 0 And Inv.selected <= Inv.slot.Count)) Then
                        cfd = Inv.slot(Inv.selected).count
                    End If
                    Inv.subItemFromSlot(Inv.selected, cfd)
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
                loadmap(Player.curWorld, Player.x, Player.y)
            Case Keys.Y
                loadmap("cave2")
            Case Keys.O
                Player.stepCD = 0
            Case Keys.C
                MessageBox.Show(Player.x.ToString + " " + Player.y.ToString)
            Case Keys.Z
                Inv.subItem(0, 1)
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
        Return (x1 - Player.x) * 32 + wX
    End Function
    Public Function scroolY(ByVal y1 As Integer) As Integer
        Return (y1 - Player.y) * 32 + wY
    End Function
    Public Sub gamesave(Optional ByVal dir As String = "save")
        dir = "saves/" + dir
        IO.Directory.CreateDirectory(dir)

        Dim TestFileStream As IO.Stream = IO.File.Create(dir + "/enemy.xml")
        Dim serializer As New Xml.Serialization.XmlSerializer(Enemy.GetType)
        serializer.Serialize(TestFileStream, Enemy)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/player.xml")
        serializer = New Xml.Serialization.XmlSerializer(Player.GetType)
        serializer.Serialize(TestFileStream, Player)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/doors.xml")
        serializer = New Xml.Serialization.XmlSerializer(Door.GetType)
        serializer.Serialize(TestFileStream, Door)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/items.xml")
        serializer = New Xml.Serialization.XmlSerializer(Item.GetType)
        serializer.Serialize(TestFileStream, Item)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/inv.xml")
        serializer = New Xml.Serialization.XmlSerializer(Inv.GetType)
        serializer.Serialize(TestFileStream, Inv)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/effect.xml")
        serializer = New Xml.Serialization.XmlSerializer(Effect.GetType)
        serializer.Serialize(TestFileStream, Effect)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/triggers.xml")
        serializer = New Xml.Serialization.XmlSerializer(Trigger.GetType)
        serializer.Serialize(TestFileStream, Trigger)
        TestFileStream.Close()

        TestFileStream = IO.File.Create(dir + "/loader.xml")
        serializer = New Xml.Serialization.XmlSerializer(Loader.GetType)
        serializer.Serialize(TestFileStream, Loader)
        TestFileStream.Close()
    End Sub
    Public Sub gameload(Optional ByVal dir As String = "save")
        unpressarrows()
        Dim filename As String
        Dim deserializer As New Runtime.Serialization.Formatters.Binary.BinaryFormatter

        dir = "saves/" + dir
        Dim tmpPlayer = New PlayerClass
        filename = "player.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(PlayerClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            tmpPlayer = CType( _
            mySerializer.Deserialize(myFileStream), PlayerClass)
            myFileStream.Close()
        End If
        If Not tmpPlayer.testVersion() Then
            If tmpPlayer.version = "" Then
                tmpPlayer.version = "before0.8.x beta"
            End If
            MessageBox.Show("this save for version: """ + tmpPlayer.version + """ not for """ + version + """")
            Return
        Else
            Player = tmpPlayer
        End If
        filename = "enemy.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(EnemyClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Enemy = CType( _
            mySerializer.Deserialize(myFileStream), EnemyClass)
            myFileStream.Close()
        End If
        filename = "doors.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(DoorClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Door = CType( _
            mySerializer.Deserialize(myFileStream), DoorClass)
            myFileStream.Close()
        End If
        filename = "items.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(ItemClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Item = CType( _
            mySerializer.Deserialize(myFileStream), ItemClass)
            myFileStream.Close()
        End If
        filename = "inv.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(InventoryClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Inv = CType( _
            mySerializer.Deserialize(myFileStream), InventoryClass)
            myFileStream.Close()
        End If
        filename = "effect.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(EffectClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Effect = CType( _
            mySerializer.Deserialize(myFileStream), EffectClass)
            myFileStream.Close()
        End If
        filename = "triggers.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(TriggerClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Trigger = CType( _
            mySerializer.Deserialize(myFileStream), TriggerClass)
            myFileStream.Close()
        End If
        filename = "loader.xml"
        If IO.File.Exists(dir + "/" + filename) Then
            Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(LoaderClass))
            Dim myFileStream As IO.FileStream = _
            New IO.FileStream(dir + "/" + filename, IO.FileMode.Open)
            Loader = CType( _
            mySerializer.Deserialize(myFileStream), LoaderClass)
            myFileStream.Close()
        End If
        loadmap(Player.curWorld, Player.x, Player.y)
    End Sub
    Public Sub initmap(ByVal sX As Integer, sY As Integer, dir As String)
        ReDim map(99, 99)
        Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
        Dim tempStr As String
        Dim x() As String
        For i As Integer = 0 To sY - 1
            tempStr = fReader.ReadLine
            x = tempStr.Split(" ")
            For j As Integer = 0 To sX - 1
                map(j, i) = x(j)
            Next
        Next
        fReader.Close()
        'If IO.File.Exists(dir + "charset.txt") Then
    End Sub
    Public Sub loadmap(ByVal dir As String, Optional ByVal x As Integer = -1, Optional ByVal y As Integer = -1)
        Player.curWorld = dir
        If Not Loader.isLoaded(dir) Then
            Dim numLoad As Integer = Loader.load(dir)
            dir = "worlds/" + dir + "/"
            Dim Xd As XDocument = XDocument.Load(dir + "data.xml")
            'MessageBox.Show(Xd.Element("map").Element("test").Elements.Count)
            For Each xe As XElement In Xd.Element("map").Element("enemies").Elements
                Enemy.spawn(xe.Element("x"), xe.Element("y"), xe.Element("difficulty"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("items").Elements
                Item.spawnitem(xe.Element("x"), xe.Element("y"), xe.Element("id"), xe.Element("count"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("doors").Elements
                Door.create(xe.Element("x"), xe.Element("y"), xe.Element("keyID"), xe.Element("texture"))
            Next

            For Each xe As XElement In Xd.Element("map").Element("plates").Elements
                If xe.Element("id") = "3" Then
                    Trigger.create(xe.Element("x"), xe.Element("y"), xe.Element("x2"), xe.Element("y2"), xe.Element("id"), xe.Element("posX"), xe.Element("posY"), xe.Element("value"))
                Else
                    Trigger.create(xe.Element("x"), xe.Element("y"), xe.Element("x2"), xe.Element("y2"), xe.Element("id"), xe.Element("value"))
                End If
            Next
            For Each xe As XElement In Xd.Element("map").Element("shops").Elements
                Dim num As Integer = Shop.create(xe.Element("x"), xe.Element("y"), xe.Element("texture"))
                For Each xe2 As XElement In xe.Elements("items").Elements
                    Shop.addItem(num, New shopItem(xe2.Element("id"), xe2.Element("count"), xe2.Element("cost")))
                Next
            Next

            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            sX = Xd.Element("map").Element("info").Element("width")
            sY = Xd.Element("map").Element("info").Element("height")
            Player.x = Xd.Element("map").Element("info").Element("playerX")
            Player.y = Xd.Element("map").Element("info").Element("playerY")

            Loader.addInfo(sX, sY, Player.x, Player.y)

            If (x <> -1) Then
                Player.x = x
            End If
            If (y <> -1) Then
                Player.y = y
            End If
            'For i As Integer = 0 To sY - 1
            'mapStr(i) = fReader.ReadLine()
            'Next
            fReader.Close()
            initmap(sX, sY, dir)
        Else
            Dim numLoad As Integer = Loader.getNumLoaded(dir)
            dir = "worlds/" + dir + "/"
            Dim fReader As System.IO.StreamReader = New IO.StreamReader(dir + "map.txt")
            sX = Loader.bW(numLoad)
            sY = Loader.bH(numLoad)
            Player.x = Loader.bX(numLoad)
            Player.y = Loader.bY(numLoad)

            If (x <> -1) Then
                Player.x = x
            End If
            If (y <> -1) Then
                Player.y = y
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
        Player.version = version
        unpressarrows()
        LoadTextures(32)
        Item.init()
        Effect.init()
        listBar.init()
        Equipment.init()
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
        Select Case Player.gamestate
            Case "noAction"
            Case "normal"
                'события
                If Player.stepCD > 0 Then
                    Player.stepCD -= 1
                End If
                If Player.stepCD = 0 Then
                    Dim goodstep As Boolean = False
                    If kUp Then
                        If (Player.y > 0) Then
                            If canStep(map(Player.x, Player.y - 1), Directions.Up) Then
                                Trigger.update = True
                                Enemy.update = True
                                goodstep = True
                                Player.y -= 1
                            ElseIf godMode Then '!g
                                Trigger.update = True '!g
                                Enemy.update = True '!g
                                goodstep = True '!g
                                Player.y -= 1 '!g
                            End If
                        End If
                    End If
                    If (Not goodstep) And (kDown) Then
                        If (Player.y < sY - 1 And canStep(map(Player.x, Player.y + 1), Directions.Down)) Then
                            Trigger.update = True
                            Enemy.update = True
                            goodstep = True
                            Player.y += 1
                        ElseIf godMode Then '!g
                            Trigger.update = True '!g
                            Enemy.update = True '!g
                            goodstep = True '!g
                            Player.y += 1 '!g
                        End If
                    End If
                    If (Not goodstep) And (kLeft) Then
                        If Player.x > 0 Then
                            If canStep(map(Player.x - 1, Player.y), Directions.Left) Then
                                Trigger.update = True
                                Enemy.update = True
                                goodstep = True
                                Player.x -= 1
                            ElseIf godMode Then '!g
                                Trigger.update = True '!g
                                Enemy.update = True '!g
                                goodstep = True '!g
                                Player.x -= 1 '!g
                            End If
                        End If
                    End If
                    If (Not goodstep) And (kRight) Then
                        If (Player.x < sX - 1 And canStep(map(Player.x + 1, Player.y), Directions.Right)) Then
                            Trigger.update = True
                            Enemy.update = True
                            goodstep = True
                            Player.x += 1
                        ElseIf godMode Then '!g
                            Trigger.update = True '!g
                            Enemy.update = True '!g
                            goodstep = True '!g
                            Player.x += 1 '!g
                        End If
                    End If
                    If goodstep Then
                        Player.stepCD = 5
                    End If
                End If
                Effect.upTime()
                Enemy.test(Player.x, Player.y)
                If (map(Player.x, Player.y) = BlockType.Lava) And (Effect.search(1) > -1) Then
                ElseIf (map(Player.x, Player.y) = BlockType.Lava) And (Player.stepCD <> -1) And (Not godMode) Then '!g
                    unpressarrows()
                    Player.stepCD = -1
                    MessageBox.Show("game over(burn in lava)")
                End If
                If (map(Player.x, Player.y) = BlockType.DeepWater) And (Effect.search(4) = -1) And (Player.stepCD <> -1) And (Not godMode) And (Effect.search(1) = -1) Then
                    unpressarrows()
                    Player.stepCD = -1
                    MessageBox.Show("game over(death underwater)")
                End If
                Trigger.test(Player.x, Player.y)
                Enemy.chance(Player.x, Player.y)

                Dim tlist = Item.items.FindAll(Function(x) (x.x = Player.x) And (x.y = Player.y) And (x.world = Player.curWorld))
                If tlist.Count <> 0 Then
                    For Each x As ItemObject In tlist
                        Inv.additem(x.id, x.count)
                        Item.items.Remove(x)
                    Next
                End If
                '
                Shop.selectedShop = -1
                Dim bool As Boolean = True
                For i As Integer = 0 To Shop.count
                    If (Player.x = Shop.p(i).X And Player.y = Shop.p(i).Y And Shop.world(i) = Player.curWorld) Then
                        Shop.selectedShop = i
                        listBar.activate(ListsEnum.shop)
                        bool = False
                        'shop.open = True
                    End If
                Next
                If bool Then
                    listBar.deactivate(ListsEnum.shop)
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
                For Each x As ItemObject In Item.items
                    If (x.count > 0 And x.world = Player.curWorld) Then
                        'Console.WriteLine(x.x.ToString + " " + x.y.ToString)
                        e.Graphics.DrawImage(itemtex(x.tex), scroolX(x.x), scroolY(x.y))
                    End If
                Next
                'отрисовка монстров
                For Each x As enemyStruct In Enemy.enemies
                    If x.world = Player.curWorld Then
                        e.Graphics.DrawImage(tileset(6), scroolX(x.x), scroolY(x.y), 32, 32)
                    End If
                Next
                'отрисовка магазинов
                For i As Integer = 0 To Shop.count
                    If Shop.world(i) = Player.curWorld Then
                        e.Graphics.DrawImage(tileset(Shop.tex(i)), scroolX(Shop.p(i).X), scroolY(Shop.p(i).Y), 32, 32)
                    End If
                Next
                'отрисовка дверей
                For i As Integer = 0 To Door.doors.Count - 1
                    If (Door.doors(i).tex <> -1 And Door.doors(i).world = Player.curWorld) Then
                        e.Graphics.DrawImage(tileset(Door.doors(i).tex), scroolX(Door.doors(i).x), scroolY(Door.doors(i).y), 32, 32)
                    End If
                Next
                'отрисовка эффектов
                For i As Integer = 0 To Effect.effects.Count - 1
                    e.Graphics.DrawImage(effecttex(Effect.effects(i).tex), 1, 30 + i * 40)
                    If Effect.effects(i).isTemp Then
                        e.Graphics.DrawString(Effect.effects(i).dur / 100, New Font("Arial", 9), Brushes.Black, 50, 50 + 40 * i)
                    End If
                Next
                If Effect.selected <> -1 Then
                    'MessageBox.Show(effect.desc(effect.e(effect.selected).id))                    
                    e.Graphics.DrawString(String.Format(Effect.desc(Effect.effects(Effect.selected).id - 1), Effect.effects(Effect.selected).pow), New Font("Arial", 9), Brushes.Black, 50, 30 + Effect.selected * 40)
                End If
                'money
                e.Graphics.DrawString("money " + Player.money.ToString, New Font("Arial", 11), Brushes.Black, 5, 5)
                'отрисовка ListBar
                If listBar.open Then
                    e.Graphics.FillRectangle(Brushes.Gray, 50, 10, 50, 460)
                    'e.Graphics.FillRectangle(Brushes.LightGray, 50, 10 + (1 - 1) * 50, 50, 50)
                    'e.Graphics.FillRectangle(Brushes.DarkGray, 50, 10 + (2 - 1) * 50, 50, 50)
                    'e.Graphics.FillRectangle(Brushes.DarkGray, 50, 10 + (3 - 1) * 50, 50, 50)
                    Dim offset As Integer = 0
                    For i As Integer = 0 To listBar.lists.Count - 1
                        If listBar.lists(i).isActive Then
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
                    If listBar.selected = ListsEnum.inventory Then
                        e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
                        For i As Integer = 0 To 9
                            If i + Inv.page * 10 < Inv.slot.Count Then
                                Dim it As Integer = i + Inv.page * 10
                                e.Graphics.DrawString(Item.itemsInfo(Inv.slot(it).id).name, New Font("Arial", 14), Brushes.Black, 145, 50 + 38 * i)
                                e.Graphics.DrawString(Inv.slot(it).count, New Font("Arial", 14), Brushes.Black, 310, 50 + 38 * i)
                                e.Graphics.DrawString(Item.itemsInfo(Inv.slot(it).id).type, New Font("Arial", 14), Brushes.Black, 370, 50 + 38 * i)
                                e.Graphics.DrawImage(itemtex(Item.itemsInfo(Inv.slot(it).id).tex), 105, 50 + 38 * i)
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
                        e.Graphics.DrawString((Inv.page + 1).ToString + "/" + (Math.Max(Math.Floor(Inv.slot.Count / 10) + 1, 1)).ToString, New Font("Arial", 14), Brushes.Black, 284 + 5 * (Math.Floor(Inv.slot.Count / 10) > 8) + 5 * (Inv.page > 8), 446 - 10)
                        If Inv.desc <> "" Then
                            e.Graphics.FillRectangle(Brushes.LightGray, 500, 10, 130, 460)
                            e.Graphics.DrawString(Inv.desc, New Font("Arial", 12), Brushes.Black, New RectangleF(501, 20, 130, 200))
                        End If
                    End If
                    'отрисовка отрытого магазина
                    If (listBar.selected = ListsEnum.shop) And (Shop.selectedShop <> -1) Then
                        e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
                        For i As Integer = 0 To 9
                            If i + Shop.page * 10 <= Shop.itemsCount(Shop.selectedShop) Then
                                Dim it As Integer = i + Shop.page * 10
                                With Shop.items(Shop.selectedShop, it)
                                    e.Graphics.DrawString(Item.itemsInfo(.id).name, New Font("Arial", 14), Brushes.Black, 145, 50 + 38 * i)
                                    e.Graphics.DrawString(.cost, New Font("Arial", 14), Brushes.Black, 310, 50 + 38 * i)
                                    e.Graphics.DrawString(Item.itemsInfo(.id).type, New Font("Arial", 14), Brushes.Black, 370, 50 + 38 * i)
                                    e.Graphics.DrawImage(itemtex(Item.itemsInfo(.id).tex), 105, 50 + 38 * i)
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
                        e.Graphics.DrawString((Shop.page + 1).ToString + "/" + (Math.Max(Math.Floor(Inv.slot.Count / 10) + 1, 1)).ToString, New Font("Arial", 14), Brushes.Black, 284 + 5 * (Math.Floor(Inv.slot.Count / 10) > 8) + 5 * (Inv.page > 8), 446 - 10)
                        If Shop.desc <> "" Then
                            e.Graphics.FillRectangle(Brushes.LightGray, 500, 10, 130, 460)
                            e.Graphics.DrawString(Shop.desc, New Font("Arial", 12), Brushes.Black, New RectangleF(501, 20, 130, 200))
                        End If
                    End If
                End If
            Case "battleInit"
                unpressarrows()
                Label1.Show()
                'text load
                str = words(rand(1, wordscount))
                For i As Integer = 1 To Enemy.curDif - 1 'enemy.dif(enemy.curBattle) - 1
                    str += " " + words(rand(1, wordscount))
                Next
                Label1.Text = str
                typeText.Show()
                typeText.Enabled = True
                typeText.Focus()
                Player.gamestate = "battle"
            Case "battle"
                'battle mode on
                e.Graphics.DrawString("test mode no texture", New Font("Arial", 14), Brushes.Black, 20, 20)
                If typeText.Text = Label1.Text Then
                    Label1.Hide()
                    typeText.Hide()
                    typeText.Clear()
                    typeText.Enabled = False
                    Player.money += 10
                    Enemy.kill(Enemy.curBattle)
                    Enemy.curBattle = -1
                    Player.gamestate = "normal"
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
                    For j As Integer = 0 To listBar.lists.Count
                        If listBar.lists(j).isActive Then
                            offset += 1
                        End If
                        If offset = i Then
                            listBar.selected = j
                            Exit For
                        End If
                    Next
                End If
            Next
            If (listBar.selected = ListsEnum.shop) And (Shop.selectedShop <> -1) Then
                'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + Shop.page * 10
                        If e.Button = MouseButtons.Left Then
                            Shop.buyItem(Shop.selectedShop, Shop.selectedItem, 1)
                        End If
                        If e.Button = MouseButtons.Right Then
                            Shop.sellItem(Shop.selectedShop, Shop.selectedItem, 1)
                        End If
                    End If
                Next
                If inRect(e.X, e.Y, 105, 30 + 40 * 10, 200, 30 + 40 * 10 + 32) Then
                    If Shop.page > 0 Then
                        Shop.page -= 1
                    End If
                End If
                If inRect(e.X, e.Y, 400, 30 + 40 * 10, 495, 30 + 40 * 10 + 32) Then
                    If Shop.page < Math.Floor(Shop.itemsCount(Shop.selectedShop) / 10) Then
                        Shop.page += 1
                    End If
                End If
            ElseIf listBar.selected = ListsEnum.inventory Then
                'MessageBox.Show(e.X.ToString + " " + e.Y.ToString)
                If e.Button = Windows.Forms.MouseButtons.Right Then
                    For i As Integer = 0 To 9 '105, 30 + 40 * i
                        If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                            Dim it As Integer = i + Inv.page * 10
                            If (Item.itemsInfo(Inv.slot(it).id).isUsable) Then
                                If Inv.useItem(it) Then
                                    Inv.subItem(Inv.slot(it).id, 1)
                                End If
                            End If
                        End If
                    Next
                End If
                If inRect(e.X, e.Y, 105, 30 + 40 * 10, 200, 30 + 40 * 10 + 32) Then
                    If Inv.page > 0 Then
                        Inv.page -= 1
                    End If
                End If
                If inRect(e.X, e.Y, 400, 30 + 40 * 10, 495, 30 + 40 * 10 + 32) Then
                    If Inv.page < Math.Floor(Inv.slot.Count / 10) Then
                        Inv.page += 1
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If listBar.open Then
            If (listBar.selected = ListsEnum.shop) And (Shop.selectedShop <> -1) Then
                Shop.desc = "LBM - buy" + vbNewLine + "RBM - sell"
                Shop.selectedItem = -1
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + Shop.page * 10
                        If Inv.slot.Count >= it Then
                            Shop.desc = Item.itemsInfo(Shop.items(Shop.selectedShop, it).id).desc + vbNewLine + "LBM - buy" + vbNewLine + "RBM - sell"
                            Shop.selectedItem = it
                        End If
                    End If
                Next
            ElseIf listBar.selected = ListsEnum.inventory Then
                Inv.desc = ""
                Inv.selected = -1
                For i As Integer = 0 To 9 '105, 30 + 40 * i
                    If inRect(e.X, e.Y, 105, 50 + 38 * i, 495, 50 + 38 * i + 32) Then
                        Dim it As Integer = i + Inv.page * 10
                        If Inv.slot.Count > it Then
                            Inv.desc = Item.itemsInfo(Inv.slot(it).id).desc
                            Inv.selected = it
                        End If
                    End If
                Next
            End If
        End If
        Effect.selected = -1
        For i As Integer = 0 To Effect.effects.Count - 1
            If inRect(e.X, e.Y, 1, 30 + i * 40, 1 + 32, 30 + i * 40 + 32) Then
                Effect.selected = i
            End If
        Next
    End Sub
End Class
