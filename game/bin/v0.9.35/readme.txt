game.exe - запуск игры
-map.txt - всЄ что находитс€ на карте(€чейки карты, монстры, предметы) -- устарело
items.txt - все предметы (id, name, type, stats)
items.png - текстуры предметов
texture.png - текстуры блоков и монстров
-планируетс€- monsters.png текстуры монстров --скорее всего не буедет
saves - папка с сохранени€ми
worlds - папка с мирами
{
    map.txt - всЄ что находитс€ на карте
    charset.txt - файл задающий что символ значит на карте, при отсутствии используетс€ стандартный набор
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