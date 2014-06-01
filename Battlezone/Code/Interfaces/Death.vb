Namespace Interfaces
    Public Class Death
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Public Sub New()
            MyBase.New("Death", 4)

            REM Display the Main Texture
            Dim BackGround As New HellionEngine.Game.Sprite("Textures/Interface/Death.png")
            BackGround.Dimensions = New HellionEngine.Support.Vector(1024, 768)
            Me.RenderLayerSystem.Append(1, BackGround)

            Dim ExitGameButton As New HellionEngine.Game.Button("Textures/Buttons/Exit_Up.png", "Textures/Buttons/Exit_Moused.png", "Textures/Buttons/Exit_Moused.png")
            ExitGameButton.Position = New HellionEngine.Support.Vector(25, 720)
            ExitGameButton.Scale = New HellionEngine.Support.Vector(0.5, 0.65)
            AddHandler ExitGameButton.OnClicked, AddressOf Me.ButtonExit_Clicked
            Me.RenderLayerSystem.Append(0, ExitGameButton)
            Me.UpdateObjectList.Append(ExitGameButton)
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            If (HellionEngine.Sound.Singleton.CurrentMusicFile <> "Music/Death.wav") Then
                HellionEngine.Sound.Singleton.PlayMusic("Music/Death.wav", False)
            End If
        End Sub

        Private Sub ButtonExit_Clicked()
            PlayUI.RenderLayerSystem.Empty()
            PlayUI.Game.Dispose()
            PlayUI.Game = Nothing

            Battlezone.InterfaceSystem.ActiveInterfaceName = "Main"
        End Sub
    End Class
End Namespace