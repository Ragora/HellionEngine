Namespace Interfaces
    Public Class Main
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Private BattlezoneAnimation As HellionEngine.Game.AnimatedSprite

        Public Sub New()
            MyBase.New("Main", 4)

            REM Display the Main Texture
            Dim BackGround As New HellionEngine.Game.Sprite("Textures/Interface/Main.png")
            BackGround.Dimensions = New HellionEngine.Support.Vector(1024, 768)
            Me.RenderLayerSystem.Append(1, BackGround)

            Dim ArcadeButton As New HellionEngine.Game.Button("Textures/Buttons/Arcade_Up.png", "Textures/Buttons/Arcade_Moused.png", "Textures/Buttons/Arcade_Moused.png")
            ArcadeButton.Position = New HellionEngine.Support.Vector(100, 100)
            ArcadeButton.Scale = New HellionEngine.Support.Vector(1.3, 1.2)
            AddHandler ArcadeButton.OnClicked, AddressOf Me.ButtonArcade_Clicked
            Me.RenderLayerSystem.Append(0, ArcadeButton)
            Me.UpdateObjectList.Append(ArcadeButton)

            Dim ExitGameButton As New HellionEngine.Game.Button("Textures/Buttons/Exit_Up.png", "Textures/Buttons/Exit_Moused.png", "Textures/Buttons/Exit_Moused.png")
            ExitGameButton.Position = New HellionEngine.Support.Vector(25, 3)
            ExitGameButton.Scale = New HellionEngine.Support.Vector(0.5, 0.65)
            AddHandler ExitGameButton.OnClicked, AddressOf Me.ButtonExitGame_Clicked
            Me.RenderLayerSystem.Append(0, ExitGameButton)
            Me.UpdateObjectList.Append(ExitGameButton)

            Me.BattlezoneAnimation = New HellionEngine.Game.AnimatedSprite("Battlezone", True)
            Me.BattlezoneAnimation.Dimensions = New HellionEngine.Support.Vector(900, 120)
            Me.BattlezoneAnimation.FramesPerSecond = 20
            Me.BattlezoneAnimation.Looping = True
            Me.BattlezoneAnimation.Position = New HellionEngine.Support.Vector(67, 323)
            Me.RenderLayerSystem.Append(3, Me.BattlezoneAnimation)
            Me.UpdateObjectList.Append(Me.BattlezoneAnimation)
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            Me.BattlezoneAnimation.CurrentFrame = 0

            If (HellionEngine.Sound.Singleton.CurrentMusicFile <> "Music/Menu.wav") Then
                HellionEngine.Sound.Singleton.PlayMusic("Music/Menu.wav", True)
            End If
        End Sub

        Private Sub ButtonArcade_Clicked()
            Battlezone.InterfaceSystem.ActiveInterfaceName = "SelectFaction"
        End Sub

        Private Sub ButtonExitGame_Clicked()
            HellionEngine.EngineInstance.Singleton.Shutdown()
        End Sub
    End Class
End Namespace