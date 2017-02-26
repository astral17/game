Public Module EnemyModule
    <Serializable()>
    Public Structure enemyStruct
        Public x, y, dif As Integer
        Public world As String
        Public Sub New(ByVal x As Integer, y As Integer, dif As Integer, world As String)
            Me.x = x
            Me.y = y
            Me.dif = dif
            Me.world = world
        End Sub
    End Structure
    <Serializable()>
    Public Class EnemyClass
        Public enemies As New List(Of enemyStruct)
        Public stepChance As Integer = 0
        Public stepDif As Integer = 2
        Public curBattle As Integer = -1
        Public curDif As Integer = 0
        Public update As Boolean = False

        Public Sub spawn(ByVal x As Integer, y As Integer, diff As Integer)
            enemies.Add(New enemyStruct(x, y, diff, Player.curWorld))

        End Sub
        Public Sub kill(ByVal n As Integer)
            enemies.RemoveAt(n)
        End Sub
        Public Sub test(ByVal x As Integer, y As Integer)
            'Dim tlist = e.FindAll(Function(x) (x.x = x) And (x.y = y))
            ''System.Console.WriteLine(tlist.Count)
            'If tlist.Count <> 0 Then
            '    For Each xe As itemobj In tlist
            '        battle(i)
            '    Next
            'End If
            For i As Integer = enemies.Count - 1 To 0 Step -1
                If (enemies(i).x = x And enemies(i).y = y And enemies(i).world = Player.curWorld) Then
                    battle(i)
                End If
            Next

        End Sub
        Public Sub chance(ByVal x As Integer, y As Integer)
            If MainForm.godMode Then '!g
                Return '!g
            End If '!g
            If update Then
                update = False
                If MainForm.rand(1, 100) <= stepChance Then
                    spawn(x, y, stepDif)
                    battle(enemies.Count - 1)
                End If
            End If
        End Sub
        Public Sub battle(ByRef n As Integer)
            If MainForm.godMode Then '!g
                Return '!g
            End If '!g
            Dim tmp As Integer = 0
            If Effect.search(2) > -1 Then
                tmp -= Effect.search(2)
            End If
            If enemies(n).dif + tmp < 1 Then
                kill(n)
                Player.money += 1
            Else
                'e(n).dif += tmp
                curDif = enemies(n).dif + tmp
                curBattle = n
                Player.gamestate = "battleInit"
            End If
            '
        End Sub
    End Class
End Module
