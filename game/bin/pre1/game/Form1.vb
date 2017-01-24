Public Class Form1
    Public x, y, sX, sY, wX, wY As Integer
    Public tileset(99) As Bitmap
    Dim map(99, 99) As BlockType
    Public mapStr(99) As String
    Enum BlockType
        Grass = 0
        Block = 1
        Water = 2
        Bridge = 3
    End Enum
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
    Public Function tsolid(ByVal n As BlockType) As Boolean
        If (n = BlockType.Block Or n = BlockType.Water) Then
            Return False
        Else
            Return True
        End If

    End Function
    Public Function max(ByVal a As Integer, b As Integer) As Integer
        If a > b Then
            Return a
        Else
            Return b
        End If
    End Function
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If (y > 0 And tsolid(map(x, y - 1))) Then
                    y -= 1
                End If
            Case Keys.Down
                If (y < sY - 1 And tsolid(map(x, y + 1))) Then
                    y += 1
                End If
            Case Keys.Left
                If (x > 0 And tsolid(map(x - 1, y))) Then
                    x -= 1
                End If
            Case Keys.Right
                If (x < sX - 1 And tsolid(map(x + 1, y))) Then
                    x += 1
                End If
        End Select
        PictureBox1.Refresh()
    End Sub
    

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadTextures(32)
        Dim fReader As System.IO.StreamReader = New IO.StreamReader("map.txt")
        Dim n, m As Integer
        sX = Int(fReader.ReadLine())
        sY = Int(fReader.ReadLine())
        x = Int(fReader.ReadLine()) - 1
        y = Int(fReader.ReadLine()) - 1
        wX = x * 32 - 50
        wY = y * 32 - 50
        For i As Integer = 0 To sX - 1
            mapStr(i) = fReader.ReadLine()
        Next
        fReader.Close()
        For i As Integer = 0 To sX - 1
            For j As Integer = 0 To sY - 1
                If mapStr(j)(i) = "1" Then
                    map(i, j) = BlockType.Block
                End If
                
                If mapStr(j)(i) = "2" Then
                    map(i, j) = BlockType.Water
                End If
                If mapStr(j)(i) = "3" Then
                    map(i, j) = BlockType.Bridge
                End If
            Next
        Next
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        System.Threading.Monitor.Enter(e.Graphics)
        'e.Graphics.FillRectangle(Brushes.Aqua, x * 32, y * 32, 32, 32)
        wX = (5) * 32 + 100
        wY = (5) * 32 + 50
        For i As Integer = 0 To sX - 1
            For j As Integer = 0 To sY - 1
                Dim nX As Integer = (i - x) * 32 + wX
                Dim nY As Integer = (j - y) * 32 + wY
                If map(i, j) = BlockType.Block Then
                    e.Graphics.DrawImage(tileset(2), nX, nY, 32, 32)
                ElseIf map(i, j) = BlockType.Water Then
                    e.Graphics.DrawImage(tileset(3), nX, nY, 32, 32)
                ElseIf map(i, j) = BlockType.Grass Then
                    e.Graphics.DrawImage(tileset(1), nX, nY, 32, 32)
                ElseIf map(i, j) = BlockType.Bridge Then
                    e.Graphics.DrawImage(tileset(4), nX, nY, 32, 32)
                End If
            Next
        Next
        e.Graphics.DrawImage(tileset(0), wX, wY)
        System.Threading.Monitor.Exit(e.Graphics)
    End Sub
End Class
