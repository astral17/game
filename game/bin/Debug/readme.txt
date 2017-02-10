game.exe - запуск игры
data
{
    items.xml - все предметы (id, name, type, stats)
    effects.xml - описания эффектов
    words.txt - все слова которые даются в бою
    items.png - текстуры предметов
    texture.png - текстуры блоков и монстров
    effects.png - текстуры эффектов
}
saves - папка с сохранениями
worlds - папка с мирами
{
    map.txt - карта этого мира
    data.xml - все существа, двери, предметы хранятся здесь
    charset.txt - файл задающий что символ значит на карте, при отсутствии используется стандартный набор
}
управление:
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