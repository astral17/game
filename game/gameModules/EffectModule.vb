Public Module EffectModule
    <Serializable()>
    Public Structure effectStruct
        Public id, dur, pow, tex As Integer
        Public isTemp As Boolean
        Public Sub New(ByVal id As Integer, dur As Integer, pow As Integer, tex As Integer, isTemp As Boolean)
            Me.id = id
            Me.dur = dur
            Me.pow = pow
            Me.tex = tex
            Me.isTemp = isTemp
        End Sub
    End Structure
    <Serializable()>
    Public Class EffectClass  'id dur pow temp
        Public effects As New List(Of effectStruct)
        Public desc As New List(Of String)
        Public selected As Integer = -1
        Public Sub init()
            Dim Xd As XDocument = XDocument.Load("data/effects.xml")
            For Each xe As XElement In Xd.Elements("effects").Elements
                desc.Add(xe.Value)
            Next
        End Sub
        Public Sub add(ByVal id As Integer, dur As Integer, pow As Integer, tex As Integer, Optional ByVal isTemp As Boolean = True)
            effects.Add(New effectStruct(id, dur, pow, tex, isTemp))
        End Sub
        Public Sub remove(ByVal id As Integer, Optional ByVal pow As Integer = -1)
            Dim i As Integer = effects.FindIndex(Function(x) (x.id = id) And ((x.pow = pow) Or (pow = -1)))
            If selected >= i Then
                selected = -1
            End If
            If i <> -1 Then
                effects.RemoveAt(i)
            End If
        End Sub
        Public Sub removeByNum(ByVal num As Integer)
            If selected >= num Then
                selected = -1
            End If
            If num <> -1 Then
                effects.RemoveAt(num)
            End If
        End Sub
        Public Function search(ByVal id As Integer) As Integer
            Dim max As Integer = -1
            For i As Integer = 0 To effects.Count - 1
                If effects(i).id = id Then
                    max = Math.Max(effects(i).pow, max)
                End If
            Next
            Return max
        End Function
        Public Sub upTime()
            If effects.Count <> 0 Then
                'For Each x As effectStruct In e
                'remove KOSTIL!!
                For i As Integer = 0 To effects.Count - 1
                    If effects(i).isTemp Then
                        'Dim i = e.FindIndex(Function(zn) (zn.id = x.id) And (zn.dur = x.dur) And (zn.pow = x.pow) And (zn.isTemp = x.isTemp) And (zn.tex = x.tex))
                        Dim x As effectStruct = effects(i)
                        x.dur -= 1
                        effects(i) = x
                    End If
                    'Case 1 undead обработка там где лава
                    'Case 2 incAttack
                    'Case 3 reincarnation
                    If effects(i).id = 3 Then
                        Player.stepCD = 0
                        add(1, 300, 1, 3, True)
                    End If
                    'Case 4 waterbreath
                Next
            End If
            Dim offset As Integer = 0
            Dim ocount = effects.Count
            For i As Integer = 0 To ocount - 1
                If effects(i - offset).dur < 1 Then
                    removeByNum(i - offset)
                    offset += 1
                End If
            Next
        End Sub
    End Class
End Module
