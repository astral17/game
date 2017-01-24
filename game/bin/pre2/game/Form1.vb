Public Class Form1
    Public x, y, sX, sY, wX, wY As Integer
    Public tileset(99) As Bitmap
    Dim map(99, 99) As BlockType
    Public mapStr(99) As String

    Public enem As New enemy
    'Public enemie As New enemy
    Public encount As Integer
    Enum BlockType
        Grass = 0
        Block = 1
        DeepWater = 2
        Bridge = 3
        Water = 4
        Lava = 5
    End Enum
    Public Structure item
        Public id, name As Integer
    End Structure
    Public Structure slots
        Public id, count As Integer
    End Structure
    Public Class inventory
        Dim slot(1000) As slots
        Public Sub additem(ByVal it As item)

        End Sub
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
            MessageBox.Show("battle")
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
    End Sub
    Public Function canStep(ByVal n As BlockType) As Boolean
        If (n = BlockType.Block Or n = BlockType.DeepWater) Then
            Return False
        Else
            Return True
        End If

    End Function

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If (y > 0 And canStep(map(x, y - 1))) Then
                    y -= 1
                End If
            Case Keys.Down
                If (y < sY - 1 And canStep(map(x, y + 1))) Then
                    y += 1
                End If
            Case Keys.Left
                If (x > 0 And canStep(map(x - 1, y))) Then
                    x -= 1
                End If
            Case Keys.Right
                If (x < sX - 1 And canStep(map(x + 1, y))) Then
                    x += 1
                End If
        End Select
        PictureBox1.Refresh()
    End Sub
    Public Function scroolX(ByVal x1 As Integer) As Integer
        Return (x1 - x) * 32 + wX
    End Function
    Public Function scroolY(ByVal y1 As Integer) As Integer
        Return (y1 - y) * 32 + wY
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadTextures(32)
        Dim fReader As System.IO.StreamReader = New IO.StreamReader("map.txt")
        Dim n, m, mn, tmp1, tmp2 As Integer
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
        mn = Int(fReader.ReadLine())
        For i As Integer = 0 To mn
            tmp1 = Int(fReader.ReadLine())
            tmp2 = Int(fReader.ReadLine())
            enem.spawn(tmp1, tmp2)
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
        'enem.spawn(3, 3)
        'enem.spawn(10, 10)
        'enem.spawn(18, 3)
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
        'отрисовка монстров
        For i As Integer = 0 To enem.count
            If enem.getSt(i) = 1 Then
                e.Graphics.DrawImage(tileset(6), scroolX(enem.getX(i)), scroolY(enem.getY(i)), 32, 32)
            End If
        Next
        'e.Graphics.DrawImage(tileset(6), scroolX(4), scroolY(3), 32, 32)
        System.Threading.Monitor.Exit(e.Graphics)
    End Sub
End Class
