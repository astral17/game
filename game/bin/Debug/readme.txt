game.exe - ������ ����
data
{
    items.xml - ��� �������� (id, name, type, stats)
    effects.xml - �������� ��������
    words.txt - ��� ����� ������� ������ � ���
    items.png - �������� ���������
    texture.png - �������� ������ � ��������
    effects.png - �������� ��������
}
saves - ����� � ������������
worlds - ����� � ������
{
    map.txt - ����� ����� ����
    data.xml - ��� ��������, �����, �������� �������� �����
    charset.txt - ���� �������� ��� ������ ������ �� �����, ��� ���������� ������������ ����������� �����
}
����������:
{
            Case Keys.Up
                kUp = True
            Case Keys.Down
                kDown = True
            Case Keys.Left
                kLeft = True
            Case Keys.Right
                kRight = True
            Case Keys.E
                inv.open = True
            Case Keys.S
                'save data
                gamesave()
            Case Keys.R
                gameload("start")
            Case Keys.L
                'load data
                gameload()
            Case Keys.T
                loadmap("tmp")
            Case Keys.Y
                loadmap("main")
            Case Keys.C
                MessageBox.Show(player.x.ToString + " " + player.y.ToString)
            Case Keys.G
                If godMode Then
                    godMode = False
                Else
                    godMode = True
                End If
}