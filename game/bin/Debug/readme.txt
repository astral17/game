game.exe - ������ ����
-map.txt - �� ��� ��������� �� �����(������ �����, �������, ��������) -- ��������
items.txt - ��� �������� (id, name, type, stats)
items.png - �������� ���������
texture.png - �������� ������ � ��������
-�����������- monsters.png �������� �������� --������ ����� �� ������
saves - ����� � ������������
worlds - ����� � ������
{
    map.txt - �� ��� ��������� �� �����
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