Public Class Form1
    Public x, y, sX, sY, wX, wY As Integer
    Public tileset(99) As Bitmap
    Public itemtex(99) As Bitmap
    Dim map(99, 99) As BlockType
    Public mapStr(99) As String
    Public items As New item
    Public enem As New enemy
    Public inv As New inventory
    'Public enemie As New enemy
    Public encount As Integer
    Public doors As New Door
    Enum BlockType
        Grass = 0
        Block = 1
        DeepWater = 2
        Bridge = 3
        Water = 4
        Lava = 5
    End Enum
    Public Structure itemobj
        Public id, count, x, y As Integer
    End Structure
    Public Class Door
        Public dX(99), dY(99), id(99), tex(99) As Integer
        Public count As Integer = -1
        Public Function CanStep(ByVal dir As Integer) As Boolean
            Dim x1, y1 As Integer
            x1 = Form1.x
            y1 = Form1.y
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
                If (dX(i) = x1 And dY(i) = y1) Then
                    If Form1.inv.searchitem(id(i)) = -1 Then
                        Return False
                    Else
                        dX(i) = -1
                        dY(i) = -1
                        tex(i) = 10
                        Form1.inv.slot(Form1.inv.searchitem(id(i))).count = 0
                        Return True
                    End If
                End If
            Next
            Return True
        End Function
        Public Sub init()
            Dim fReader As System.IO.StreamReader = New IO.StreamReader("doors.txt")
            Dim n As Integer
            count = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To count
                dX(i) = Int(fReader.ReadLine())
                dY(i) = Int(fReader.ReadLine())
                id(i) = Int(fReader.ReadLine())
                tex(i) = Int(fReader.ReadLine())
            Next
            fReader.Close()
        End Sub
    End Class
    Public Class item
        Public name(99), type(99) As String
        Public item(99) As itemobj
        Public count As Integer = -1
        Public Sub init()
            Dim fReader As System.IO.StreamReader = New IO.StreamReader("items.txt")
            Dim n As Integer
            n = Int(fReader.ReadLine()) - 1
            For i As Integer = 0 To n
                name(i) = fReader.ReadLine()
                type(i) = fReader.ReadLine()
            Next
            fReader.Close()
        End Sub
        Public Sub spawnitem(ByVal id As Integer, counti As Integer, x As Integer, y As Integer)
            count += 1
            item(count).id = id
            item(count).x = x
            item(count).y = y
            item(count).count = counti
        End Sub
    End Class
    Public Structure slots
        Public id, count As Integer
    End Structure
    Public Class inventory
        Public slot(11) As slots
        Public count As Integer = -1
        Public open As Boolean = False
        Public Sub additem(ByVal id As Integer, countitem As Integer)
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
            End If
        End Sub
        Public Function getslot(ByVal n As Integer) As slots
            Return slot(n)
        End Function
        Public Function searchitem(ByVal id As Integer)
            For i As Integer = 0 To count
                If slot(i).id = id Then
                    Return i
                End If
            Next
            Return -1
        End Function
    End Class
    Public Class enemy
        Dim p(100) As Point
        Dim state(100) As Integer
        Dim mcount As Integer = -1
        Public Property count As Integer
            Get
                Return mcount
            End Get
            Set(value As Integer)
                mcount = value
            End Set
        End Property

        Public Sub spawn(ByVal x As Integer, y As Integer)
            mcount += 1
            p(mcount).X = x
            p(mcount).Y = y
            state(mcount) = 1
        End Sub
        Public Function getX(ByVal n As Integer) As Integer
            Return p(n).X
        End Function
        Public Function getY(ByVal n As Integer) As Integer
            Return p(n).Y
        End Function
        Public Function getSt(ByVal n As Integer) As Integer
            Return state(n)
        End Function
        Public Sub test(ByVal x As Integer, y As Integer)
            For i As Integer = 0 To mcount
                If (p(i).X = x And p(i).Y = y And state(i) = 1) Then
                    battle(i)
                End If
            Next

        End Sub
        Public Sub battle(ByVal n As Integer)
            'p(n).X = -1
            'p(n).Y = -1
            state(n) = 0
            'MessageBox.Show("battle")
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
        If (n = BlockType.Block Or n = BlockType.DeepWater Or doors.CanStep(dir) = False) Then
            Return False
        Else
            Return True
        End If

    End Function
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim ups As Boolean = False
        inv.open = False
        Select Case e.KeyCode
            Case Keys.Up
                If (y > 0 And canStep(map(x, y - 1), 1)) Then
                    y -= 1
                    ups = True
                End If
            Case Keys.Down
                If (y < sY - 1 And canStep(map(x, y + 1), 2)) Then
                    y += 1
                    ups = True
                End If
            Case Keys.Left
                If (x > 0 And canStep(map(x - 1, y), 3)) Then
                    x -= 1
                    ups = True
                End If
            Case Keys.Right
                If (x < sX - 1 And canStep(map(x + 1, y), 4)) Then
                    x += 1
                    ups = True
                End If
            Case Keys.E
                ups = True
                inv.open = True
        End Select
        If ups Then
            PictureBox1.Refresh()
        End If
    End Sub
    Public Function scroolX(ByVal x1 As Integer) As Integer
        Return (x1 - x) * 32 + wX
    End Function
    Public Function scroolY(ByVal y1 As Integer) As Integer
        Return (y1 - y) * 32 + wY
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadTextures(32)
        items.init()
        doors.init()
        Dim fReader As System.IO.StreamReader = New IO.StreamReader("map.txt")
        Dim n, tmp1, tmp2, tmp3, tmp4 As Integer
        Dim s As String
        wX = (5) * 32 + 100
        wY = (5) * 32 + 50
        sX = Int(fReader.ReadLine())
        sY = Int(fReader.ReadLine())
        x = Int(fReader.ReadLine()) - 1
        y = Int(fReader.ReadLine()) - 1
        'wX = x * 32 - 50
        'wY = y * 32 - 50
        For i As Integer = 0 To sY - 1
            mapStr(i) = fReader.ReadLine()
        Next
        n = Int(fReader.ReadLine()) - 1
        For i As Integer = 0 To n
            tmp1 = Int(fReader.ReadLine())
            tmp2 = Int(fReader.ReadLine())
            enem.spawn(tmp1, tmp2)
        Next
        s = fReader.ReadLine()
        n = Int(fReader.ReadLine()) - 1
        For i As Integer = 0 To n
            tmp1 = Int(fReader.ReadLine())
            tmp2 = Int(fReader.ReadLine())
            tmp3 = Int(fReader.ReadLine())
            tmp4 = Int(fReader.ReadLine())
            items.spawnitem(tmp1, tmp2, tmp3, tmp4)
        Next
        fReader.Close()
        For i As Integer = 0 To sX - 1
            For j As Integer = 0 To sY - 1
                If mapStr(j)(i) = "1" Then
                    map(i, j) = BlockType.Block
                End If
                If mapStr(j)(i) = "2" Then
                    map(i, j) = BlockType.DeepWater
                End If
                If mapStr(j)(i) = "3" Then
                    map(i, j) = BlockType.Bridge
                End If
                If mapStr(j)(i) = "4" Then
                    map(i, j) = BlockType.Water
                End If
                If mapStr(j)(i) = "5" Then
                    map(i, j) = BlockType.Lava
                End If
            Next
        Next
        'tmp

        'tmp
    End Sub
    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        System.Threading.Monitor.Enter(e.Graphics)
        'события
        enem.test(x, y)
        If (map(x, y) = BlockType.Lava) Then
            MessageBox.Show("game over")
            x = -1
            y = -1
        End If
        For i As Integer = 0 To items.count
            If (x = items.item(i).x And y = items.item(i).y And items.item(i).count > 0) Then
                inv.additem(items.item(i).id, items.item(i).count)
                items.item(i).count = 0
            End If
        Next
        ''''For i As Integer = 0 To encount
        ''''If (x = enemies(i).pos.X And y = enemies(i).pos.Y) Then
        ''''enemies(i).battle()
        ''''End If
        ''''Next
        'EсобытияE
        'e.Graphics.FillRectangle(Brushes.Aqua, x * 32, y * 32, 32, 32)
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
                End If
            Next
        Next
        'отрисовка игрока
        e.Graphics.DrawImage(tileset(0), wX, wY)
        'отрисовка предметов
        For i As Integer = 0 To items.count
            If items.item(i).count > 0 Then
                e.Graphics.DrawImage(itemtex(items.item(i).id), scroolX(items.item(i).x), scroolY(items.item(i).y))
            End If
        Next
        'отрисовка монстров
        For i As Integer = 0 To enem.count
            If enem.getSt(i) = 1 Then
                e.Graphics.DrawImage(tileset(6), scroolX(enem.getX(i)), scroolY(enem.getY(i)), 32, 32)
            End If
        Next
        'отрисовка дверей
        For i As Integer = 0 To doors.count
            e.Graphics.DrawImage(tileset(doors.tex(i)), scroolX(doors.dX(i)), scroolY(doors.dY(i)), 32, 32)
        Next
        'e.Graphics.DrawImage(tileset(6), scroolX(4), scroolY(3), 32, 32)
        'отрисовка инвентаря
        If inv.open Then
            e.Graphics.FillRectangle(Brushes.LightGray, 100, 10, 400, 460)
            For i As Integer = 0 To inv.count
                e.Graphics.DrawString(items.name(inv.slot(i).id), New Font("Arial", 14), Brushes.Black, 145, 30 + 40 * i)
                e.Graphics.DrawString(inv.slot(i).count, New Font("Arial", 14), Brushes.Black, 310, 30 + 40 * i)
                e.Graphics.DrawString(items.type(inv.slot(i).id), New Font("Arial", 14), Brushes.Black, 370, 30 + 40 * i)
                e.Graphics.DrawImage(itemtex(inv.slot(i).id), 105, 30 + 40 * i)
            Next
        End If
        System.Threading.Monitor.Exit(e.Graphics)
    End Sub
End Class
